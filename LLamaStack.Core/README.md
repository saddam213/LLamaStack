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

The `ModelService` is a powerful service that handles the loading, unloading, and caching of LLama models. It offers support for both single and multiple models in memory and includes model preloading capabilities.
```cs
// Load a model and create a context
var (model, context) = await _modelService.GetOrCreateModelAndContext("ModelName", "sessionId");
```

### ModelSessionService

The `ModelSessionService` is responsible for creating and managing model context sessions. It supports both streaming inference and completed inference results, making it versatile for various use cases.
```cs
// Create session
var sessionId = "sessionId";
var sessionConfig = new SessionConfig { Prompt = "Initial prompt", ExecutorType = Common.ExecutorType.Instruct };
var inferenceConfig = new InferenceConfig { Temperature = 0.6f, Mirostat = LLama.Common.MirostatType.Mirostat2 };
await _modelSessionService.CreateAsync(sessionId, sessionConfig, inferenceConfig);

// Run Inference
var sessionText = "What is .NET Core?";
await foreach (var token in _modelSessionService.InferAsync("sessionId, sessionText, inferenceConfig))
{
   Console.Write(tokem.Content);
}

// End the session
await _modelSessionService.CloseAsync(sessionId);
```


### ModelSessionStateService

The `ModelSessionStateService` plays a crucial role in loading and saving a model's session state. This includes context state, executor state, and session/inference parameters. It provides robust state management for your LLama model sessions.

### ModelLoaderService

The `ModelLoaderService` is a hosted service designed to efficiently manage the preloading of models for applications. When configured, this service will preload models during application startup, ensuring optimal performance.