<p align="center" width="100%">
    <img width="80%" src="Assets/LLamaStack-640%C3%97320.png">
</p>

[![Discord](https://img.shields.io/discord/1147446100442226699?label=Discord)](https://discord.gg/cDpupfb2JB)
[![LLamaStack Badge](https://img.shields.io/nuget/v/LLamaStack?label=LLamaStack)](https://www.nuget.org/packages/LLamaSharp)
[![Web Demo](https://img.shields.io/website/https/www.llama-stack.com?label=Web%20Demo&up_message=online)](https://www.llama-stack.com)
[![Web API Demo](https://img.shields.io/website/https/api.llama-stack.com?label=Web%20API%20Demo&down_message=coming%20soon)](https://api.llama-stack.com)

### Welcome to LLamaStack!

LLamaStack is a library that provides higher-level services and integrations for .NET applications, enhancing the functionality and versatility of the LLamaSharp and llama.cpp projects.

 This repository also contains a collection of barebone/bootstrap UI & API projects. Here, you'll find various user interface applications, including Web, API, WPF, and Websocket, all built to showcase the capabilities of the LLamaStack.



## Overview
LLamaStack is built on top of the popular **[LLamaSharp](https://github.com/SciSharp/LLamaSharp)** and **[llama.cpp](https://github.com/ggerganov/llama.cpp)** projects, extending their functionalities with a range of user-friendly UI applications. LLamaSharp is a powerful library that provides C# interfaces and abstractions for the popular llama.cpp, the C++ counterpart that offers high-performance inference capabilities on low end hardware. LLamaStack complements these projects by creating intuitive UI & API interfaces, making the power of LLamaSharp and llama.cpp more accessible to users.


# Projects
LLamaStack is a comprehensive library with several projects tailored for different purposes:

1. **[LLamaStack.Core](LLamaStack.Core/README.md)**: This project offers high-level services and integrations for .NET applications.

2. **[LLamaStack.Web](LLamaStack.Web/README.md)**: The ASP.NET Core Web interface provides all the core functions of llama.cpp & LLamaSharp.

3. **[LLamaStack.WPF](LLamaStack.WPF/README.md)**: The WPF UI interface provides all the core functions of llama.cpp & LLamaSharp.

4. **LLamaStack.WebAPI**: This is an implementation of an ASP.NET Core WebAPI with all the essential features of llama.cpp & LLamaSharp.

5. **LLamaStack.Signalr**: SignalR websocket server and client implementations designed for use in web and .NET environments.




## Getting Started
To start using a specific UI project, please refer to the README file located in its respective directory.

You can store model configurations in the `appsettings.json` file. For examples and documentation on the `appsettings.json` structure, please see the [appsettings.json Documentation](Docs/appsettings.md).

LLamaStack also provides functionality for adding custom sections and loading/saving at runtime.








## Contribution

We welcome contributions to LLamaStack! If you have any ideas, bug reports, or improvements, feel free to open an issue or submit a pull request.
