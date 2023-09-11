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

    HTTP Method: GET
    Route: /Get
    Description: Retrieves a model with the specified name.
    Parameters:
        name (string, required): The name of the model to retrieve.

### GetModels

    HTTP Method: GET
    Route: /GetAll
    Description: Retrieves all available models.

These endpoints allow you to interact with model-related operations using HTTP requests. The `ModelController` relies on the `IApiModelService` for handling these operations.



# ModelSessionController

The `ModelSessionController` is a controller designed to handle various `ModelSession` related HTTP requests.

## Endpoints

### Create

    HTTP Method: POST
    Route: /Create
    Description: Creates a new model session.
    Parameters:
        request (CreateRequest): The request object containing session creation details.

### Close

    HTTP Method: POST
    Route: /Close
    Description: Closes an existing model session.
    Parameters:
        request (CloseRequest): The request object specifying the session to close.

### Cancel

    HTTP Method: POST
    Route: /Cancel
    Description: Cancels an ongoing model session.
    Parameters:
        request (CancelRequest): The request object indicating the session to cancel.

### Infer

    HTTP Method: POST
    Route: /Infer
    Description: Initiates inference on a model session and returns the results.
    Parameters:
        request (InferRequest): The request object for inference.

### InferAsync

    HTTP Method: POST
    Route: /InferAsync
    Description: Initiates asynchronous inference on a model session and returns the results.
    Parameters:
        request (InferRequest): The request object for asynchronous inference.

### InferText

    HTTP Method: POST
    Route: /InferText
    Description: Initiates text-based inference on a model session and returns the results.
    Parameters:
        request (InferRequest): The request object for text-based inference.

### InferTextAsync

    HTTP Method: POST
    Route: /InferTextAsync
    Description: Initiates asynchronous text-based inference on a model session and returns the results.
    Parameters:
        request (InferRequest): The request object for asynchronous text-based inference.

### InferTextCompleteAsync

    HTTP Method: POST
    Route: /InferTextCompleteAsync
    Description: Initiates asynchronous complete text-based inference on a model session.
    Parameters:
        request (InferRequest): The request object for asynchronous complete text-based inference.

### InferTextCompleteQueuedAsync

    HTTP Method: POST
    Route: /InferTextCompleteQueuedAsync
    Description: Initiates asynchronous queued complete text-based inference on a model session.
    Parameters:
        request (InferRequest): The request object for asynchronous queued complete text-based inference.

These endpoints enable you to perform various operations related to `ModelSessions` using HTTP requests. The `ModelSessionController` relies on the `IApiSessionService` for managing these operations.



# ModelSessionStateController

The `ModelSessionStateController` is a controller designed to handle various `ModelSessionState` related HTTP requests.


## Endpoints

### Get

    HTTP Method: GET
    Route: /Get
    Description: Retrieves the state of a model session.
    Parameters:
        request (GetRequest): The request object specifying the session state to retrieve.

### GetAll

    HTTP Method: GET
    Route: /GetAll
    Description: Retrieves the states of all model sessions.

### Save

    HTTP Method: POST
    Route: /Save
    Description: Saves the state of a model session.
    Parameters:
        request (SaveRequest): The request object specifying the session state to save.

### Load

    HTTP Method: POST
    Route: /Load
    Description: Loads the state of a model session.
    Parameters:
        request (LoadRequest): The request object specifying the session state to load.

These endpoints enable you to perform various operations related to `ModelSessionState` using HTTP requests. The `ModelSessionStateController` relies on the `IApiSessionStateService` for managing these operations.