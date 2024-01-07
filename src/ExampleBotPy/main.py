# Example bot in Python

import random
import json
import time
import os
from paho.mqtt import client as mqtt_client
from suzaku_bot.bot import SuzakuBot

with open("./config.json", "r") as file:
    config = json.load(file)

broker = config['broker']
port = config['brokerPort']
bot_name = config['bot_name']

class CustomBot(SuzakuBot):
    def on_message(self, client, userdata, msg):        
        sender, message, conversation = self.extract_message(msg)

        # if sender != self.user_name:
        #     return

        if message.startswith(f"{bot_name}") or f"@{bot_name}" in message:
            #if message.startswith("Hey") or message.startswith("Hi"):
            self.respond_with_busy(f"Hey, {sender}! **smiles**", conversation)
            
if __name__ == '__main__':
    chat_bot = CustomBot(broker, port, bot_name)
    chat_bot.run()
