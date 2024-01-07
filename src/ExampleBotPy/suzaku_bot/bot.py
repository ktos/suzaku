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
        conversation = val["conversation_id"]
        
        return sender, message, conversation


    def on_message(self, client, userdata, msg):
        sender, message, conversation = self.extract_message(msg)

        if sender == self.bot_name:
            return        

        self.publish_busy(True)
        time.sleep(1)
        self.publish_chat(f"Hey, {sender}!", conversation)
        self.publish_busy(False)

    def respond_with_busy(self, message, conversation):
        self.publish_busy()
        time.sleep(1)
        self.publish_chat(message, conversation)
        self.publish_busy(False)

    def publish_chat(self, message: str, conversation: str):
        val = json.dumps({ "sender": self.bot_name, "content": message, "conversation_id": conversation})
        self.client.publish(self.CHAT_TOPIC, val)
        self.client.loop()

    def publish_busy(self, is_busy=True):
        self.client.publish(self.BUSY_TOPIC, json.dumps({ "sender": self.bot_name, "content": "BUSY" if is_busy else "NOT_BUSY"}))
        self.client.loop()

    def run(self):
        self.subscribe(self.CHAT_TOPIC, self.on_message)
        self.client.loop_forever()