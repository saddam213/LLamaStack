LLamaStack uses the appsettings.json to store model information.

The WPF application has a UI for building and saving a model list to make it simpler

Basic Example
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