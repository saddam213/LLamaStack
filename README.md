# LLamaStack
Welcome to LLamaStack! This repository serves as a collection of multiple barebone/bootstrap UI & API projects related to the LLama project. Here, you'll find various user interface applications, including Web, API, WPF, and Websocket, all built to showcase the capabilities of the LLama project.

## Overview
LLamaStack is built on top of the popular LLamaSharp and llama.cpp projects, extending their functionalities with a range of user-friendly UI applications. LLamaSharp is a powerful library that provides C# interfaces and abstractions for the popular llama.cpp, the C++ counterpart that offers high-performance inference capabilities on low end hardware. LLamaStack complements these projects by creating intuitive UI & API interfaces, making the power of LLamaSharp and llama.cpp more accessible to users.


## Projects

Here are the UI projects included in LLamaStack:

1. **[LLamaStack.Web](LLamaStack.Web/README.md)**: ASP.NET Core Web interface with all the base functions of llama.cpp & LLamaSharp

2. **[LLamaStack.WPF](LLamaStack.WPF/README.md)**: WPF UI interface with all the base functions of llama.cpp & LLamaSharp

3. **LLamaStack.WebAPI**: ASP.NET Core WebAPI implemntation with all the base functions of llama.cpp & LLamaSharp

4. **LLamaStack.Signalr**: Signalr websocket server and cliet implemetations for use in web and .NET environments




## Getting Started

To get started with a specific UI project, please refer to the README file of each project located in their respective directories.

## Setup
You can setup Models in the appsettings.json

```json
{
	"Logging": {
		"LogLevel": {
			"Default": "Information",
			"Microsoft.AspNetCore": "Warning"
		}
	},
	"AllowedHosts": "*",
	"LLamaStackConfig": {
		"ModelStatePath": "D:\\Repositories\\AI\\Models\\States",
		"Models": [{
			"Name": "WizardLM-7B",
			"MaxInstances": 2,
			"ModelPath": "D:\\Repositories\\Models\\wizardLM-7B.ggmlv3.q4_0.bin",
			"ContextSize": 512,
			"BatchSize": 512,
			"Threads": -1,
			"GpuLayerCount": 20,
			"UseMemorymap": true,
			"UseMemoryLock": false,
			"MainGpu": 0,
			"LowVram": false,
			"Seed": 1686349486,
			"UseFp16Memory": true,
			"Perplexity": false,
			"ModelAlias": "unknown",
			"LoraAdapter": "",
			"LoraBase": "",
			"ConvertEosToNewLine": false,
			"EmbeddingMode": false,
			"TensorSplits": null,
			"GroupedQueryAttention": 1,
			"RmsNormEpsilon": 0.000005,
			"RopeFrequencyBase": 10000.0,
			"RopeFrequencyScale": 1.0,
			"MulMatQ": false,
			"Encoding": "UTF-8"
		}]
	}
}
```



## Contribution

We welcome contributions to LLamaStack! If you have any ideas, bug reports, or improvements, feel free to open an issue or submit a pull request.
