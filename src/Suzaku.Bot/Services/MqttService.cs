﻿using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;
using Suzaku.Bot.Models;
using Suzaku.Shared;

namespace Suzaku.Bot.Services
{
    public class MqttService
    {
        private readonly MqttFactory mqttFactory;
        private readonly IMqttClient mqttClient;
        private readonly MqttClientOptions mqttClientOptions;
        private readonly ILogger<MqttService> _logger;
        private readonly IMessageResponder _responder;
        private readonly string _botName;
        private readonly string _chatTopic;
        private readonly string _oobTopic;
        private readonly string _oobPrivateTopic;
        private readonly string _privateTopic;

        public MqttService(
            ILogger<MqttService> logger,
            IMessageResponder responder,
            IOptions<MqttConfiguration> mqttOptions,
            IOptions<BotConfiguration> botOptions
        )
        {
            _logger = logger;
            _responder = responder;
            _botName = botOptions.Value.Name;
            _chatTopic = "suzaku/chat";
            _oobTopic = $"suzaku/chat_system";
            _oobPrivateTopic = $"suzaku/{_botName.ToNormalizedChannelName()}/chat_system";
            _privateTopic = $"suzaku/{_botName.ToNormalizedChannelName()}/chat";

            mqttFactory = new MqttFactory();
            mqttClient = mqttFactory.CreateMqttClient();
            mqttClientOptions = new MqttClientOptionsBuilder()
                .WithTcpServer(mqttOptions.Value.Host, mqttOptions.Value.Port)
                .WithKeepAlivePeriod(TimeSpan.FromSeconds(60))
                .Build();

            mqttClient.ApplicationMessageReceivedAsync += async e =>
            {
                var content = e.ApplicationMessage.ConvertPayloadToString();
                _logger.LogDebug(
                    "Received message for topic {0}: {1}",
                    e.ApplicationMessage.Topic,
                    content
                );

                try
                {
                    if (e.ApplicationMessage.Topic == _oobTopic)
                    {
                        // message generated by the system
                        // for example marking the agent is generating response

                        //var sysmsg = JsonSerializer.Deserialize<SystemJsonMessage>(content);
                        //if (sysmsg != null && !sysmsg.BusyMarker && sysmsg.Sender != null)
                        //{
                        //    var result = await _responder.SummarizeBotMessageAsync(
                        //        sysmsg.Sender,
                        //        sysmsg.Content
                        //    );
                        //    if (result != null)
                        //        await PublishResponseMessage(msg, result);
                        //}
                    }
                    else if (
                        e.ApplicationMessage.Topic == _chatTopic
                        || e.ApplicationMessage.Topic == _privateTopic
                    )
                    {
                        bool isPrivate = e.ApplicationMessage.Topic == _privateTopic;

                        var msg = JsonSerializer.Deserialize<ChatJsonMessage>(content);

                        // message from human or another agent
                        // ignore messages from self, but if to ignore messages from another bots
                        // is left to IMessageResponder implementation
                        if (msg != null && msg.Sender != _botName)
                        {
                            // check if the message is an attachment
                            if (msg.Content.StartsWith("file:"))
                            {
                                await PublishBusyMessage(true, isPrivate);
                                var result = await _responder.HandleFileUploadedAsync(
                                    msg.Sender,
                                    msg.Content.Replace("file:", ""),
                                    msg.ConversationId.Value,
                                    isPrivate
                                );
                                if (result != null)
                                    await PublishResponseMessage(
                                        result,
                                        msg.ConversationId.Value,
                                        isPrivate
                                    );

                                await PublishBusyMessage(false, isPrivate);
                            }
                            else
                            {
                                await PublishBusyMessage(true, isPrivate);
                                var result = await _responder.RespondAsync(
                                    msg.Sender,
                                    msg.Content,
                                    msg.ConversationId.Value,
                                    isPrivate
                                );
                                if (result != null)
                                    await PublishResponseMessage(
                                        result,
                                        msg.ConversationId.Value,
                                        isPrivate
                                    );

                                await PublishBusyMessage(false, isPrivate);
                            }
                        }
                    }
                }
                catch (JsonException)
                {
                    _logger.LogError("Got a malformed message?");
                }

                return;
            };

            mqttClient.DisconnectedAsync += (
                async e =>
                {
                    _logger.LogError("MQTT disconnected! Trying to reconnect!");
                    await Task.Delay(TimeSpan.FromSeconds(5));

                    try
                    {
                        await InitializeAsync();
                    }
                    catch
                    {
                        _logger.LogCritical("Reconnect failed!");
                        Environment.Exit(127);
                    }
                }
            );
        }

        public async Task InitializeAsync()
        {
            await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

            var mqttSubscribeOptions = mqttFactory
                .CreateSubscribeOptionsBuilder()
                .WithTopicFilter(f => f.WithTopic(_oobTopic))
                .WithTopicFilter(f => f.WithTopic(_chatTopic))
                .WithTopicFilter(f => f.WithTopic(_privateTopic))
                .Build();

            await mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);
        }

        public async Task PublishResponseMessage(
            string content,
            Guid conversationId,
            bool isPrivate
        )
        {
            var chatMessage = new ChatJsonMessage
            {
                Sender = _botName,
                Content = content,
                ConversationId = conversationId,
            };

            var mqttMessage = new MqttApplicationMessageBuilder()
                .WithTopic(isPrivate ? _privateTopic : _chatTopic)
                .WithPayload(JsonSerializer.Serialize(chatMessage))
                .Build();

            await mqttClient.PublishAsync(mqttMessage);
        }

        public async Task PublishMessage(string content, bool isPrivate)
        {
            await PublishResponseMessage(content, Guid.NewGuid(), isPrivate);
        }

        public async Task PublishBusyMessage(bool isBusy, bool isPrivate)
        {
            var chatMessage = new SystemJsonMessage
            {
                Sender = _botName,
                Content = isBusy ? SystemJsonMessage.BUSY : SystemJsonMessage.NOT_BUSY,
            };

            var mqttMessage = new MqttApplicationMessageBuilder()
                .WithTopic(isPrivate ? _oobPrivateTopic : _oobTopic)
                .WithPayload(JsonSerializer.Serialize(chatMessage))
                .Build();

            await mqttClient.PublishAsync(mqttMessage);
        }

        public async Task DisconnectAsync()
        {
            await mqttClient.DisconnectAsync();
        }
    }
}
