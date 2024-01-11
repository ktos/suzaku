﻿using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;
using Suzaku.Chat.Models;
using Suzaku.Shared;
using System.Reflection;
using System.Text.Json;

namespace Suzaku.Chat.Services
{
    public class MqttService
    {
        private readonly MqttFactory mqttFactory;
        private readonly IMqttClient mqttClient;
        private readonly MqttClientOptions mqttClientOptions;
        private readonly ChatHistory _repository;
        private readonly ILogger<MqttService> _logger;

        private const string PUBLIC_TOPIC = "suzaku/chat";
        private const string PUBLIC_OOB_TOPIC = "suzaku/chat_system";

        private string? TopicToChannelName(string topic)
        {
            if (topic == PUBLIC_TOPIC || topic == PUBLIC_OOB_TOPIC)
                return null;

            return topic.Replace("suzaku/", "").Replace("/chat", "").Replace("_system", "");
        }

        public MqttService(
            ChatHistory repo,
            ILogger<MqttService> logger,
            IOptions<MqttConfiguration> options
        )
        {
            _repository = repo;
            _logger = logger;

            mqttFactory = new MqttFactory();
            mqttClient = mqttFactory.CreateMqttClient();
            mqttClientOptions = new MqttClientOptionsBuilder()
                .WithTcpServer(options.Value.Host, options.Value.Port)
                .Build();

            mqttClient.ApplicationMessageReceivedAsync += e =>
            {
                var content = e.ApplicationMessage.ConvertPayloadToString();
                _logger.LogDebug(
                    "Received message for topic {0}: {1}",
                    e.ApplicationMessage.Topic,
                    content
                );

                try
                {
                    if (e.ApplicationMessage.Topic.EndsWith("system"))
                    {
                        // message generated by the system, OOB communication
                        // for example marking the agent is generating response

                        var channelName = TopicToChannelName(e.ApplicationMessage.Topic);

                        var msg = JsonSerializer.Deserialize<SystemJsonMessage>(content);

                        if (msg != null)
                        {
                            if (msg.Content == SystemJsonMessage.BUSY)
                            {
                                var chat = new Busy
                                {
                                    Sender = msg.Sender,
                                    Id = new Guid(),
                                    Timestamp = DateTime.UtcNow
                                };

                                _repository.AddBusyMessage(chat, channelName);
                            }

                            if (msg.Content == SystemJsonMessage.NOT_BUSY)
                            {
                                _repository.RemoveBusyMessagesForSender(msg.Sender, channelName);
                            }
                        }
                    }
                    else
                    {
                        // so it must be message sent by the user or agent

                        var channelName = TopicToChannelName(e.ApplicationMessage.Topic);

                        var msg = JsonSerializer.Deserialize<ChatJsonMessage>(content);
                        if (msg != null)
                        {
                            if (msg.Content.StartsWith("file:"))
                            {
                                var attachment = new Attachment
                                {
                                    Sender = msg.Sender,
                                    Id = Guid.NewGuid(),
                                    ConversationId = msg.ConversationId,
                                    Content = msg.Content.Replace("file:", ""),
                                    Timestamp = DateTime.UtcNow
                                };

                                _repository.AddElement(attachment, channelName);
                            }
                            else
                            {
                                var chat = new Message
                                {
                                    Sender = msg.Sender,
                                    Id = Guid.NewGuid(),
                                    ConversationId = msg.ConversationId,
                                    Content = msg.Content,
                                    Timestamp = DateTime.UtcNow
                                };

                                _repository.AddMessage(chat, channelName);
                            }
                        }
                    }
                }
                catch (JsonException)
                {
                    _logger.LogError("Got a malformed message?");
                }

                return Task.CompletedTask;
            };
        }

        public async Task InitializeAsync()
        {
            await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

            var mqttSubscribeOptions = mqttFactory
                .CreateSubscribeOptionsBuilder()
                .WithTopicFilter(f => f.WithTopic("suzaku/+/chat_system"))
                .WithTopicFilter(f => f.WithTopic(PUBLIC_OOB_TOPIC))
                .WithTopicFilter(f => f.WithTopic("suzaku/+/chat"))
                .WithTopicFilter(f => f.WithTopic(PUBLIC_TOPIC))
                .Build();

            await mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);
        }

        public async Task PublishUserMessage(string content, Guid conversationId, string? chatName)
        {
            var chatMessage = new ChatJsonMessage
            {
                Content = content,
                ConversationId = conversationId,
                Sender = "User"
            };

            var topic = PUBLIC_TOPIC;
            if (chatName != null)
                topic = $"suzaku/{chatName.ToLower()}/chat";

            var mqttMessage = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(JsonSerializer.Serialize(chatMessage))
                .Build();

            await mqttClient.PublishAsync(mqttMessage);
        }

        public async Task PublishUserAttachmentAsync(
            string fileName,
            Guid conversationId,
            string? chatName
        )
        {
            var chatMessage = new ChatJsonMessage
            {
                Content = "file:" + fileName,
                ConversationId = conversationId,
                Sender = "User"
            };

            var topic = PUBLIC_TOPIC;
            if (chatName != null)
                topic = $"suzaku/{chatName.ToLower()}/chat";

            var mqttMessage = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(JsonSerializer.Serialize(chatMessage))
                .Build();

            await mqttClient.PublishAsync(mqttMessage);
        }
    }

    public static class StringExtensions
    {
        public static string FirstCharToUpper(this string input) =>
            input switch
            {
                null => throw new ArgumentNullException(nameof(input)),
                ""
                    => throw new ArgumentException(
                        $"{nameof(input)} cannot be empty",
                        nameof(input)
                    ),
                _ => string.Concat(input[0].ToString().ToUpper(), input.AsSpan(1))
            };
    }
}
