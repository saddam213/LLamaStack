## LLamaStack.WebApi - Basic ASP.NET Core WebAPI example
LLamaStack.WebApi has no heavy dependencies and no extra frameworks except `SwaggerUI` to keep the examples clean and easy to copy over to your own project

## Swagger Documentation
[WebAPI Swagger Documentation + Demo](https://api.llama-stack.com/swagger/index.html)

## Setup
You can setup Models, Prompts and Inference parameters in the appsettings.json


# ModelController

The `ModelController` is a controller that handles `Model` related HTTP requests.


## Endpoints

### GetModel
```
    HTTP Method: GET
    Route: /Get
    Description: Retrieves a model with the specified name.

    Example 
    Request: /Get?name=WizardLM-7B
    Response: 
    {
        "model": {
                "name": "WizardLM-7B",
                "contextSize": 2048,
                "batchSize": 512,
                "maxInstances": 10,
                "encoding": "UTF-8"
            }
    } 
```

### GetModels
```
    HTTP Method: GET
    Route: /GetAll
    Description: Retrieves all available models.

    Example 
    Request: /GetAll
    Response: 
    {
        "models": [{
                "name": "WizardLM-7B",
                "contextSize": 2048,
                "batchSize": 512,
                "maxInstances": 10,
                "encoding": "UTF-8"
            },{
                "name": "WizardLM-13B",
                "contextSize": 2048,
                "batchSize": 1028,
                "maxInstances": 5,
                "encoding": "UTF-8"
            }
        ]
    } 
```
These endpoints allow you to interact with model-related operations using HTTP requests. The `ModelController` relies on the `IApiModelService` for handling these operations.



# ModelSessionController

The `ModelSessionController` is a controller designed to handle various `ModelSession` related HTTP requests.

## Endpoints

### Create

    HTTP Method: POST
    Route: /Create
    Description: Creates a new model session.

    Example:

    Request:
    {
        // Required
        "model": "WizardLM-7B", 
        "executorType": "Instruct",
        "prompt": "Below is an instruction that describes a task. Write a response that appropriately completes the request.",
        
        // Optional
        "samplerType": "Default",
        "frequencyPenalty": 0,
        "maxTokens": -1,
        "mirostatEta": 0.1,
        "mirostatTau": 5,
        "penalizeNL": true,
        "presencePenalty": 0,
        "repeatLastTokensCount": 64,
        "repeatPenalty": 1.1,
        "temperature": 0.8,
        "tfsZ": -1,
        "tokensKeep": 0,
        "topK": 40,
        "topP": 0.95,
        "typicalP": 1,
        "inputPrefix": "\n\n### Instruction:\n\n",
        "inputSuffix": "\n\n### Response:\n\n",
        "antiPrompt": "",
        "antiPrompts": [],
        "outputFilter": "",
        "outputFilters": [],
        "logitBias": []
    }

    Response: 
    {
        "sessionId": "20c7f7a4-e5f9-4bec-9ac3-55f1a379169a"
    }

### Close

    HTTP Method: POST
    Route: /Close
    Description: Closes an existing model session.

    Example:
    Request:
    {
        "sessionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    }

    Response: 
    {
        "success": true
    }


### Cancel

    HTTP Method: POST
    Route: /Cancel
    Description: Cancels an ongoing action in the specified model session.
    
    Example:
    Request:
    {
        "sessionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    }

    Response: 
    {
        "success": true
    } 

### Infer

    HTTP Method: POST
    Route: /Infer
    Description: Initiates inference on a model session and returns the results as a completed array.
    
    Example:
    Request:
    {
        // Required
        "sessionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "prompt": "What is an apple?"

        // Optional
        "samplerType": "Default",
        "frequencyPenalty": 0,
        "maxTokens": -1,
        "mirostatEta": 0.1,
        "mirostatTau": 5,
        "penalizeNL": true,
        "presencePenalty": 0,
        "repeatLastTokensCount": 64,
        "repeatPenalty": 1.1,
        "temperature": 0.8,
        "tfsZ": -1,
        "tokensKeep": 0,
        "topK": 40,
        "topP": 0.95,
        "typicalP": 1,
        "logitBias": []
    }

    Response: 
    [
        {
            "tokenId": 0,
            "logit": 0,
            "probability": 0,
            "content": null,
            "isChild": false,
            "type": "Begin",
            "elapsed": 0
        },
        {
            "tokenId": 530,
            "logit": 27.43893,
            "probability": 0.5839324,
            "content": " An",
            "isChild": false,
            "type": "Content",
            "elapsed": 1292
        },
        {
            "tokenId": 26163,
            "logit": 31.901243,
            "probability": 0.9997987,
            "content": " apple",
            "isChild": false,
            "type": "Content",
            "elapsed": 1414
        },
        {
            "tokenId": 338,
            "logit": 32.91171,
            "probability": 0.99989986,
            "content": " is",
            "isChild": false,
            "type": "Content",
            "elapsed": 1672
        },
        .........
    ]

### InferAsync

    HTTP Method: POST
    Route: /InferAsync
    Description: Initiates asynchronous inference on a model session and streams the result as individual tokens.
    
    Example:
    Request:
    {
        // Required
        "sessionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "prompt": "What is an apple?"

        // Optional
        "samplerType": "Default",
        "frequencyPenalty": 0,
        "maxTokens": -1,
        "mirostatEta": 0.1,
        "mirostatTau": 5,
        "penalizeNL": true,
        "presencePenalty": 0,
        "repeatLastTokensCount": 64,
        "repeatPenalty": 1.1,
        "temperature": 0.8,
        "tfsZ": -1,
        "tokensKeep": 0,
        "topK": 40,
        "topP": 0.95,
        "typicalP": 1,
        "logitBias": []
    }

    Response: 
    [
        {
            "tokenId": 0,
            "logit": 0,
            "probability": 0,
            "content": null,
            "isChild": false,
            "type": "Begin",
            "elapsed": 0
        },
        {
            "tokenId": 530,
            "logit": 27.43893,
            "probability": 0.5839324,
            "content": " An",
            "isChild": false,
            "type": "Content",
            "elapsed": 1292
        },
        {
            "tokenId": 26163,
            "logit": 31.901243,
            "probability": 0.9997987,
            "content": " apple",
            "isChild": false,
            "type": "Content",
            "elapsed": 1414
        },
        {
            "tokenId": 338,
            "logit": 32.91171,
            "probability": 0.99989986,
            "content": " is",
            "isChild": false,
            "type": "Content",
            "elapsed": 1672
        },
        .........
    ]

### InferText

    HTTP Method: POST
    Route: /InferText
    Description: Initiates text-based inference on a model session and returns the results as a completed array.
    
    Example:
    Request:
    {
        // Required
        "sessionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "prompt": "What is an apple?"

        // Optional
        "samplerType": "Default",
        "frequencyPenalty": 0,
        "maxTokens": -1,
        "mirostatEta": 0.1,
        "mirostatTau": 5,
        "penalizeNL": true,
        "presencePenalty": 0,
        "repeatLastTokensCount": 64,
        "repeatPenalty": 1.1,
        "temperature": 0.8,
        "tfsZ": -1,
        "tokensKeep": 0,
        "topK": 40,
        "topP": 0.95,
        "typicalP": 1,
        "logitBias": []
    }

    Response: 
    [
        " An",
        " apple",
        " is",
        " a",
        " sweet",
        ",",
        " ed",
        "ible",
        " fruit",
        " that",
        " typically",
        .........
    ]

### InferTextAsync

    HTTP Method: POST
    Route: /InferTextAsync
    Description: Initiates asynchronous text-based inference on a model session and streams the result as individual token strings.
    
    Example:
    Request:
    {
        // Required
        "sessionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "prompt": "What is an apple?"

        // Optional
        "samplerType": "Default",
        "frequencyPenalty": 0,
        "maxTokens": -1,
        "mirostatEta": 0.1,
        "mirostatTau": 5,
        "penalizeNL": true,
        "presencePenalty": 0,
        "repeatLastTokensCount": 64,
        "repeatPenalty": 1.1,
        "temperature": 0.8,
        "tfsZ": -1,
        "tokensKeep": 0,
        "topK": 40,
        "topP": 0.95,
        "typicalP": 1,
        "logitBias": []
    }

    Response: 
    [
        " An",
        " apple",
        " is",
        " a",
        " sweet",
        ",",
        " ed",
        "ible",
        " fruit",
        " that",
        " typically",
        .........
    ]

### InferTextCompleteAsync

    HTTP Method: POST
    Route: /InferTextCompleteAsync
    Description: Initiates asynchronous text-based inference on a model session and return the full completed result.
    
    Example:
    {
        // Required
        "sessionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "prompt": "What is an apple?"

        // Optional
        "samplerType": "Default",
        "frequencyPenalty": 0,
        "maxTokens": -1,
        "mirostatEta": 0.1,
        "mirostatTau": 5,
        "penalizeNL": true,
        "presencePenalty": 0,
        "repeatLastTokensCount": 64,
        "repeatPenalty": 1.1,
        "temperature": 0.8,
        "tfsZ": -1,
        "tokensKeep": 0,
        "topK": 40,
        "topP": 0.95,
        "typicalP": 1,
        "logitBias": []
    }

    Response: 
    {
        "result": " An apple is a sweet, edible fruit that typically ranges in color from green to red..."
    }

### InferTextCompleteQueuedAsync

    HTTP Method: POST
    Route: /InferTextCompleteQueuedAsync
    Description: Initiates asynchronous text-based inference on a model session and return the full completed result, requests are queued and run one after the other, good for low-end systems of chatbots
    
    Example:
    {
        // Required
        "sessionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "prompt": "What is an apple?"

        // Optional
        "samplerType": "Default",
        "frequencyPenalty": 0,
        "maxTokens": -1,
        "mirostatEta": 0.1,
        "mirostatTau": 5,
        "penalizeNL": true,
        "presencePenalty": 0,
        "repeatLastTokensCount": 64,
        "repeatPenalty": 1.1,
        "temperature": 0.8,
        "tfsZ": -1,
        "tokensKeep": 0,
        "topK": 40,
        "topP": 0.95,
        "typicalP": 1,
        "logitBias": []
    }

    Response: 
    {
        "result": " An apple is a sweet, edible fruit that typically ranges in color from green to red..."
    }

These endpoints enable you to perform various operations related to `ModelSessions` using HTTP requests. The `ModelSessionController` relies on the `IApiSessionService` for managing these operations.



# ModelSessionStateController

The `ModelSessionStateController` is a controller designed to handle various `ModelSessionState` related HTTP requests.


## Endpoints

### Get

    HTTP Method: GET
    Route: /Get?SessionId=3fa85f64-5717-4562-b3fc-2c963f66afa6
    Description: Retrieves the state of a model session.
    
    Response: 
    {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "name": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "contextSize": 2048,
        "created": "2023-09-17T22:47:46.2768676Z",
        "sessionConfig": {
            "model": "WizardLM-7B",
            "inferenceType": "Instruct",
            "prompt": "Below is an instruction that describes a task. Write a response that appropriately completes the request.",
            "inputPrefix": "\n\n### Instruction:\n\n",
            "inputSuffix": "\n\n### Response:\n\n",
            "antiPrompt": "",
            "antiPrompts": [],
            "outputFilter": "",
            "outputFilters": []
        },
        "inferenceConfig": {
            "frequencyPenalty": 0,
            "logitBias": [],
            "maxTokens": -1,
            "samplerType": "Default",
            "mirostatEta": 0.1,
            "mirostatTau": 5,
            "penalizeNL": true,
            "presencePenalty": 0,
            "repeatLastTokensCount": 64,
            "repeatPenalty": 1.1,
            "temperature": 0.8,
            "tfsZ": -1,
            "tokensKeep": 0,
            "topK": 40,
            "topP": 0.95,
            "typicalP": 1
        },
        "sessionHistory": [
            {
                "content": "What is an apple?",
                "signature": null,
                "isResponse": false,
                "timestamp": "2023-09-17T22:47:12.893727Z"
            },
            {
                "content": " An apple is a sweet, edible fruit that typically ranges in color from green to red...",
                "signature": "Inference completed in 17 seconds",
                "isResponse": true,
                "timestamp": "2023-09-17T22:47:30.7531527Z"
            }
        ]
    }


### GetAll

    HTTP Method: GET
    Route: /GetAll
    Description: Retrieves the states of all model sessions.

    Response: 
    [
        {
            "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
            "name": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
            "contextSize": 2048,
            "created": "2023-09-17T22:47:46.2768676Z",
            "sessionConfig": {
                "model": "WizardLM-7B",
                "inferenceType": "Instruct",
                "prompt": "Below is an instruction that describes a task. Write a response that appropriately completes the request.",
                "inputPrefix": "\n\n### Instruction:\n\n",
                "inputSuffix": "\n\n### Response:\n\n",
                "antiPrompt": "",
                "antiPrompts": [],
                "outputFilter": "",
                "outputFilters": []
            },
            "inferenceConfig": {
                "frequencyPenalty": 0,
                "logitBias": [],
                "maxTokens": -1,
                "samplerType": "Default",
                "mirostatEta": 0.1,
                "mirostatTau": 5,
                "penalizeNL": true,
                "presencePenalty": 0,
                "repeatLastTokensCount": 64,
                "repeatPenalty": 1.1,
                "temperature": 0.8,
                "tfsZ": -1,
                "tokensKeep": 0,
                "topK": 40,
                "topP": 0.95,
                "typicalP": 1
            },
            "sessionHistory": [
                {
                    "content": "What is an apple?",
                    "signature": null,
                    "isResponse": false,
                    "timestamp": "2023-09-17T22:47:12.893727Z"
                },
                {
                    "content": " An apple is a sweet, edible fruit that typically ranges in color from green to red...",
                    "signature": "Inference completed in 17 seconds",
                    "isResponse": true,
                    "timestamp": "2023-09-17T22:47:30.7531527Z"
                }
            ]
        }
    ]

### Save

    HTTP Method: POST
    Route: /Save
    Description: Saves the state of a model session.
    
    Example:
    Request:
    {
        "sessionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    }

    Response: 
    {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "name": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "contextSize": 2048,
        "created": "2023-09-17T22:47:46.2768676Z",
        "sessionConfig": {
            "model": "WizardLM-7B",
            "inferenceType": "Instruct",
            "prompt": "Below is an instruction that describes a task. Write a response that appropriately completes the request.",
            "inputPrefix": "\n\n### Instruction:\n\n",
            "inputSuffix": "\n\n### Response:\n\n",
            "antiPrompt": "",
            "antiPrompts": [],
            "outputFilter": "",
            "outputFilters": []
        },
        "inferenceConfig": {
            "frequencyPenalty": 0,
            "logitBias": [],
            "maxTokens": -1,
            "samplerType": "Default",
            "mirostatEta": 0.1,
            "mirostatTau": 5,
            "penalizeNL": true,
            "presencePenalty": 0,
            "repeatLastTokensCount": 64,
            "repeatPenalty": 1.1,
            "temperature": 0.8,
            "tfsZ": -1,
            "tokensKeep": 0,
            "topK": 40,
            "topP": 0.95,
            "typicalP": 1
        },
        "sessionHistory": [
            {
                "content": "What is an apple?",
                "signature": null,
                "isResponse": false,
                "timestamp": "2023-09-17T22:47:12.893727Z"
            },
            {
                "content": " An apple is a sweet, edible fruit that typically ranges in color from green to red...",
                "signature": "Inference completed in 17 seconds",
                "isResponse": true,
                "timestamp": "2023-09-17T22:47:30.7531527Z"
            }
        ]
    }

### Load

    HTTP Method: POST
    Route: /Load
    Description: Loads the state of a model session.
    
    Example:
    {
        "sessionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    }

    Response: 
    {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "name": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "contextSize": 2048,
        "created": "2023-09-17T22:47:46.2768676Z",
        "sessionConfig": {
            "model": "WizardLM-7B",
            "inferenceType": "Instruct",
            "prompt": "Below is an instruction that describes a task. Write a response that appropriately completes the request.",
            "inputPrefix": "\n\n### Instruction:\n\n",
            "inputSuffix": "\n\n### Response:\n\n",
            "antiPrompt": "",
            "antiPrompts": [],
            "outputFilter": "",
            "outputFilters": []
        },
        "inferenceConfig": {
            "frequencyPenalty": 0,
            "logitBias": [],
            "maxTokens": -1,
            "samplerType": "Default",
            "mirostatEta": 0.1,
            "mirostatTau": 5,
            "penalizeNL": true,
            "presencePenalty": 0,
            "repeatLastTokensCount": 64,
            "repeatPenalty": 1.1,
            "temperature": 0.8,
            "tfsZ": -1,
            "tokensKeep": 0,
            "topK": 40,
            "topP": 0.95,
            "typicalP": 1
        },
        "sessionHistory": [
            {
                "content": "What is an apple?",
                "signature": null,
                "isResponse": false,
                "timestamp": "2023-09-17T22:47:12.893727Z"
            },
            {
                "content": " An apple is a sweet, edible fruit that typically ranges in color from green to red...",
                "signature": "Inference completed in 17 seconds",
                "isResponse": true,
                "timestamp": "2023-09-17T22:47:30.7531527Z"
            }
        ]
    } 

These endpoints enable you to perform various operations related to `ModelSessionState` using HTTP requests. The `ModelSessionStateController` relies on the `IApiSessionStateService` for managing these operations.