<p align="center" width="100%">
    <img width="80%" src="Assets/LLamaStack-640%C3%97320.png">
</p>


![GitHub last commit (by committer)](https://img.shields.io/github/last-commit/saddam213/LLamaStack)
[![LLamaStack Badge](https://img.shields.io/nuget/v/LLamaStack?color=4bc51e&label=LLamaStack)](https://www.nuget.org/packages/LLamaStack)
[![Web Demo](https://img.shields.io/website/https/www.llama-stack.com?label=Web%20Demo&up_message=online)](https://www.llama-stack.com)
[![Web API Demo](https://img.shields.io/website/https/llama-stack.com?label=Web%20API%20Demo&up_message=online)](https://api.llama-stack.com/swagger/index.html)
[![Discord](https://img.shields.io/discord/1147446100442226699?label=Discord)](https://discord.gg/cDpupfb2JB)

### Welcome to LLamaStack!

LLamaStack is a library that provides higher-level services and integrations for .NET applications, enhancing the functionality and versatility of the LLamaSharp and llama.cpp projects.

This repository also contains a collection of barebone/bootstrap UI & API projects. Here, you'll find various user interface applications, including Web, API, WPF, and Websocket, all built to showcase the capabilities of the LLamaStack.



## Overview
LLamaStack is built on top of the popular **[LLamaSharp](https://github.com/SciSharp/LLamaSharp)** and **[llama.cpp](https://github.com/ggerganov/llama.cpp)** projects, extending their functionalities with a range of user-friendly UI applications. LLamaSharp is a powerful library that provides C# interfaces and abstractions for the popular llama.cpp, the C++ counterpart that offers high-performance inference capabilities on low end hardware. LLamaStack complements these projects by creating intuitive UI & API interfaces, making the power of LLamaSharp and llama.cpp more accessible to users.


## Projects
LLamaStack is a comprehensive library with several projects tailored for different purposes:

1. **[LLamaStack.Core](LLamaStack.Core/README.md)**: This project offers high-level services and integrations for .NET applications.

2. **[LLamaStack.Web](LLamaStack.Web/README.md)**: The ASP.NET Core Web interface provides all the core functions of llama.cpp & LLamaSharp.

3. **[LLamaStack.WPF](LLamaStack.WPF/README.md)**: The WPF UI interface provides all the core functions of llama.cpp & LLamaSharp.

4. **[LLamaStack.WebAPI](LLamaStack.WebApi/README.md)**: This is an implementation of an ASP.NET Core WebAPI with all the essential features of llama.cpp & LLamaSharp.

5. **[LLamaStack.StableDiffusion](LLamaStack.StableDiffusion/README.md)**: Text to Image Stable Diffusion with .NET and ONNX Runtime

### In Development
6. **LLamaStack.Signalr**: `SignalR` websocket server and client implementations designed for use in web and .NET environments.

7. **LLamaStack.SemanticKernel**: Support for `Microsoft.SemanticKernel` using local models and the `LLamaStack.WebAPI` and `LLamaStack.Signalr` implementations


## Installation

`LLamaStack` can be found via the nuget package manager, download and install it.

```
PM> Install-Package LLamaStack
```

**LLamaStack** relies on the `llama.cpp` and `LLamaSharp` libraries.

- `LLamaSharp` is conveniently included in the NuGet package.
- However, you will need to obtain `llama.cpp` separately. You can either **[download](https://github.com/ggerganov/llama.cpp/releases)** or compile it yourself.

Alternatively, you can opt for one of the `LLamaSharp` backend Nuget packages tailored for your specific system.
```
LLamaSharp.Backend.Cpu  # CPU for Windows, Linux and Mac
LLamaSharp.Backend.Cuda11  # GPU CUDA11 for Windows and Linux
LLamaSharp.Backend.Cuda12  # GPU CUDA12 for Windows and Linux
LLamaSharp.Backend.MacMetal  # GPU Metal for Mac
```


## Getting Started
To start using a specific UI project, please refer to the README file located in its respective directory.

You can store model configurations in the `appsettings.json` file. For examples and documentation on the `appsettings.json` structure, please see the [appsettings.json Documentation](Docs/appsettings.md).

LLamaStack also provides functionality for adding custom sections and loading/saving at runtime.








## Contribution

We welcome contributions to LLamaStack! If you have any ideas, bug reports, or improvements, feel free to open an issue or submit a pull request.
