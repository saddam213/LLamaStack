# LLamaStack.Core - High-Level Services for .NET Applications

LLamaStack.Core is a library that provides higher-level services for use in .NET applications. It offers extensive support for features such as dependency injection, .NET configuration implementations, ASP.NET Core integration, and IHostedService support.

## Getting Started

### .NET Core Registration

You can easily integrate LLamaStack.Core into your application services layer. This registration process sets up the necessary services and loads the `appsettings.json` configuration.

Example: Registering LLamaStack
```csharp
builder.Services.AddLLamaStack();
```

By default, LLamaStack uses GUIDs for session identification. However, you have the flexibility to change this behavior by specifying a different identifier type during registration.

Example: Registering LLamaStack with an Integer Identifier
```cs
builder.Services.AddLLamaStack<int>();
```

Example: Registering LLamaStack with a String Identifier
```cs
builder.Services.AddLLamaStack<string>();
```

Alternatively, if you're injecting your own derived services, you can register each LLamaStack component separately, providing full customization options.
```cs
var llamaStackConfiguration = ConfigManager.LoadConfiguration();
builder.Services.AddSingleton(llamaStackConfiguration);
builder.Services.AddHostedService<ModelLoaderService>();
builder.Services.AddSingleton<IModelService, ModelService>();
builder.Services.AddSingleton<IModelSessionService<T>, ModelSessionService<T>>();
builder.Services.AddSingleton<IModelSessionStateService<T>, ModelSessionStateService<T>>();
```

## Features

### ModelService

The `ModelService` is a service that handles the loading, unloading, and caching of models. It offers support for both single and multiple models in memory and includes model preloading capabilities.
```cs
//Load a model
var modelConfig = new ModelConfig { Name = "MyModel", ModelPath = "Path to model" };
var model = await _modelService.LoadModel(modelConfig)

//Unload a model
await _modelService.UnloadModel("MyModel");

//Load all models in the appsettings.json
await _modelService.LoadModels();

//Unload all models
await _modelService.UnloadModels();

//Get a model loaded model
var model = await _modelService.GetModel("ModelName");

//Create a context
var context = await _modelService.CreateContext("ModelName", "MyContext");

//Remove a context
await _modelService.RemoveContext("ModelName", "MyContext");

//Load a model and create a context
var (model, context) = await _modelService.GetOrCreateModelAndContext("ModelName", "sessionId");
```

### ModelSessionService

The `ModelSessionService` is responsible for creating and managing model context sessions. It supports both streaming inference and completed inference results, making it versatile for various use cases.
```cs
// Session identifier
var sessionId = "sessionId";

// SessionConfig, model, initial prompt, anti-prompts etc.
var sessionConfig = new SessionConfig 
{
   Model = "MyModel",
   Prompt = "Initial prompt",
   InferenceType = InferenceType.Instruct
};

// InferenceConfig
var inferenceConfig = new InferenceConfig
{ 
   Temperature = 0.6f, 
   Mirostat = MirostatType.Mirostat2 
};


// CreateAsync, create a new session
var session = await _modelSessionService.CreateAsync(sessionId, sessionConfig, inferenceConfig);


// CloseAsync, End the session
await _modelSessionService.CloseAsync(sessionId);


//Inference Examples:
var questionText = "What is .NET Core?";

// InferAsync, returns IAsyncEnumerable<InferTokenModel> for streaming output of tokens
IAsyncEnumerable<InferTokenModel> inferTokens = _modelSessionService.InferAsync(sessionId, questionText);
await foreach (var token in inferTokens)
{
   Console.Write(token.Id);
   Console.Write(token.Logit);
   Console.Write(token.Probability);
   Console.Write(token.Content);
   Console.Write(token.IsChild);
}
//Note: If a character has more than one token the first will contain the entire content
//the others are marked as IsChild and will only contain the Id, Logit and Probability


// InferTextAsync, returns IAsyncEnumerable<string> for streaming output of tokens
IAsyncEnumerable<string> inferTokens = _modelSessionService.InferTextAsync(sessionId, questionText);
await foreach (var token in inferTokens)
{
   Console.Write(token);
}


// InferTextCompleteAsync, returns the full inference response as a string 
string inferResponse = await _modelSessionService.InferTextCompleteAsync(sessionId, questionText);


// InferTextCompleteQueuedAsync, queues the request for inference, the task will wait until all other queue items
// have processed and returns the full inference response as a string, be sure to set appropriate timeouts 
// as inference can take time and the quue coule be long
string inferResponse = await _modelSessionService.InferTextCompleteQueuedAsync(sessionId, questionText);


// CancelAsync, cancel the currently running inference
await _modelSessionService.CancelAsync(sessionId);
```


### ModelSessionStateService

The `ModelSessionStateService` plays a crucial role in loading and saving a model's session state. This includes context state, inference state, and session/inference parameters. It provides robust state management for your LLama model sessions.

### ModelLoaderService

The `ModelLoaderService` is a hosted service designed to efficiently manage the preloading of models for applications. When configured, this service will preload models during application startup, ensuring optimal performance.