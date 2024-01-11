from abc import abstractmethod
import random
import json
import time
import os
from paho.mqtt import client as mqtt_client

class SuzakuBot:
    CHAT_TOPIC = "suzaku/chat"
    BUSY_TOPIC = "suzaku/chat_system"

    def __init__(self, broker, port, bot_name):
        self.broker = broker
        self.port = port
        self.bot_name = bot_name
        self.user_name = "User"
        self.client_id = f'{bot_name}-{random.randint(0, 1000)}'
        self.client = self.connect_mqtt()

    def get_topic(self, channel:str | None):
        if channel is None:
            return self.CHAT_TOPIC
        else:
            return f"suzaku/{channel.lower()}/chat"
        
    def get_oob_topic(self, channel:str | None):
        if channel is None:
            return self.BUSY_TOPIC
        else:
            return f"suzaku/{channel.lower()}/chat_system"
        
    def topic_to_channel(self, topic:str) -> str | None:
        if topic == self.CHAT_TOPIC or topic == self.BUSY_TOPIC:
            return None
        
        return topic.replace("suzaku/", "").replace("/chat", "").replace("_system", "")

    def connect_mqtt(self):
        def on_connect(client, userdata, flags, rc):
            if rc == 0:
                print("Connected to MQTT Broker!")
            else:
                print(f"Failed to connect, return code {rc}")

        client = mqtt_client.Client(self.client_id)

        client.on_connect = on_connect
        client.connect(self.broker, self.port)

        return client

    def subscribe(self, topic, callback):
        self.client.subscribe(topic)
        self.client.on_message = callback

    def extract_message(self, msg) -> (str, str, str):
        val = json.loads(msg.payload.decode())

        sender = val["sender"]
        message = val["content"]
        conversation_id = val["conversation_id"]
        
        return sender, message, conversation_id


    def _on_message(self, client, userdata, msg):
        sender, message, conversation_id = self.extract_message(msg)
        channel = self.topic_to_channel(msg.topic)

        if sender == self.bot_name:
            return
        
        self.on_message(sender, message, conversation_id, channel)

    @abstractmethod
    def on_message(self, sender, message, conversation, channel):
        pass

    def respond_with_busy(self, message, conversation, channel = None):
        self.publish_busy(True, channel)
        time.sleep(1)
        self.publish_chat(message, conversation, channel)
        self.publish_busy(False, channel)

    def publish_chat(self, message: str, conversation: str, channel: str = None):
        val = json.dumps({ "sender": self.bot_name, "content": message, "conversation_id": conversation})
        self.client.publish(self.get_topic(channel), val)
        self.client.loop()

    def publish_busy(self, is_busy=True, channel: str = None):
        self.client.publish(self.get_oob_topic(channel), json.dumps({ "sender": self.bot_name, "content": "BUSY" if is_busy else "NOT_BUSY"}))
        self.client.loop()

    def run(self):
        self.subscribe(self.CHAT_TOPIC, self._on_message)
        self.subscribe("suzaku/+/chat", self._on_message)
        self.client.loop_forever()