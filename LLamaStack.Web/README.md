## LLamaStack.Web - Basic ASP.NET Core example
LLamaStack.Web has no heavy dependencies and no extra frameworks over bootstrap and jquery to keep the examples clean and easy to copy over to your own project

## Websockets
LLamaStack.Web uses Signalr websockets this simplifys the streaming of responses and model per connection management


## Setup
You can setup Models, Prompts and Inference parameters in the appsettings.json

**Models**
You can add multiple models to the options for quick selection in the UI, options are based on LLamaSharp ModelParams so its fully configurable


## Interactive UI
Manage and interact with all your models in one sime UI interface
![demo-web1](https://i.imgur.com/FG0YEzw.png)


## Live Parameters
Update inference parameters between each question/instruction
![demo-web2](https://i.imgur.com/fZEQTQ5.png)