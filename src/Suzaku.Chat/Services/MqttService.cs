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
                    if (e.ApplicationMessage.Topic.EndsWith("response"))
                    {
                        // message generated by the agent as the response for a chat
                        // or as a response for a command from another agent

                        var msg = JsonSerializer.Deserialize<ChatJsonMessage>(content);

                        if (msg != null)
                        {
                            var chat = new Message
                            {
                                Sender = FromTopic(e.ApplicationMessage.Topic),
                                Id = new Guid(),
                                ConversationId = msg.ConversationId,
                                Content = msg.Content,
                                Timestamp = DateTime.UtcNow
                            };

                            _repository.AddMessage(chat);
                        }
                    }
                    else if (e.ApplicationMessage.Topic.EndsWith("system"))
                    {
                        // message generated by the system
                        // for example marking the agent is generating response

                        var msg = JsonSerializer.Deserialize<SystemJsonMessage>(content);

                        if (msg != null)
                        {
                            if (msg.Content == SystemJsonMessage.BUSY)
                            {
                                var chat = new Busy
                                {
                                    Sender = msg.Sender ?? FromTopic(e.ApplicationMessage.Topic),
                                    Id = new Guid(),
                                    Timestamp = DateTime.UtcNow
                                };

                                _repository.AddBusyMessage(chat);
                            }

                            if (msg.Content == SystemJsonMessage.NOT_BUSY)
                            {
                                _repository.RemoveBusyMessagesForSender(
                                    msg.Sender ?? FromTopic(e.ApplicationMessage.Topic)
                                );
                            }
                        }
                    }
                    else
                    {
                        // so it must be message sent by the user

                        var msg = JsonSerializer.Deserialize<ChatJsonMessage>(content);
                        if (msg != null)
                        {
                            var chat = new Message
                            {
                                Sender = "User",
                                Id = Guid.NewGuid(),
                                ConversationId = msg.ConversationId,
                                Content = msg.Content,
                                Timestamp = DateTime.UtcNow
                            };

                            _repository.AddMessage(chat);
                        }
                    }
                }
                catch (JsonException ex)
                {
                    _logger.LogError("Got a malformed message?");
                }

                return Task.CompletedTask;
            };
        }

        private string FromTopic(string topic)
        {
            return topic.Replace("suzaku/", "").Replace("/chat_response", "").FirstCharToUpper();
        }

        public async Task InitializeAsync()
        {
            await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

            var mqttSubscribeOptions = mqttFactory
                .CreateSubscribeOptionsBuilder()
                .WithTopicFilter(f => f.WithTopic("suzaku/+/chat_system"))
                .WithTopicFilter(f => f.WithTopic("suzaku/+/chat_response"))
                .WithTopicFilter(f => f.WithTopic("suzaku/chat"))
                .Build();

            await mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);
        }

        public async Task PublishUserMessage(string content, Guid conversationId)
        {
            var chatMessage = new ChatJsonMessage
            {
                Content = content,
                ConversationId = conversationId,
                Sender = "User"
            };

            var mqttMessage = new MqttApplicationMessageBuilder()
                .WithTopic("suzaku/chat")
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
