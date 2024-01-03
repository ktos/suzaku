# Suzaku

Suzaku is my private chat with chatbots over MQTT. In the future, it will be the
overall system for chatting with different machines at home, but right now it's
mostly a prototype.

## Suzaku.Chat

Chat application itself is build with Blazor Server. The app listens on MQTT
topics:

* `suzaku/+/chat_response` on which the bot will send the responses,
* `suzaku/+/chat_system` on which the bots are publishing "busy" statuses,

And publishes the messages on `suzaku/chat` topic, as a JSON messages.

![image](docs/suzaku-chat-screenshot.png)

The goal is to make it in a Copilot-like window available with a hotkey on my
computers.

Why MQTT? Because many systems at my home are already running on MQTT, and it
allows me to freely add new components in different programming languages.

## Suza

Suza is one of the bots -- it is listening on `suzaku/chat` for my requests,
passing the requests to the local OpenAI-compatible endpoints, and passing
responses to `suzaku/suza/chat_response` topic for the Blazor app to display
them.

Suza is also listening to the `suzaku/suza/chat_system` topic, on which if there
will be published messages from other bots, it can pass it to the user changing
from JSON into more natural form.

The goal is to publish data as JSON, for example from calendar or sensors, and
get them analyzed and rephrased into natural language with LLM.

Suza needs a system prompt in a `system.txt` file in the `Prompts` subdirectory.
