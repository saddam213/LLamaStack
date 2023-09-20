# LLamaStack.StableDiffusion - Inference Stable Diffusion with C# and ONNX Runtime

This is my attempt of the tutorial [here](https://onnxruntime.ai/docs/tutorials/csharp/stable-diffusion-csharp.html) and the [StableDiffusion](https://github.com/cassiebreviu/StableDiffusion) project.
This is a WIP and is subject to heavy changes while I learn how this all fits together

This repo contains the logic to do inferencing for the popular Stable Diffusion deep learning model in C#.  Stable Diffusion models take a text prompt and create an image that represents the text.

For the below example sentence the [CLIP model](https://huggingface.co/docs/transformers/model_doc/clip) creates a text embedding that connects text to image. A random noise image is created and then denoised with the `unet` model and scheduler algorithm to create an image that represents the text prompt. Lastly the decoder model `vae_decoder` is used to create a final image that is the result of the text prompt and the latent image.

```text
"Renaissance-style portrait of an astronaut in space, detailed starry background, reflective helmet." 
```
| LMSDiffuser | EulerAncestralDiffuser|
| :--- | :--- |
<img src="https://i.imgur.com/Ptaai09.png" width="256" height="256" alt="Image of browser inferencing on sample images."/> | <img src="https://i.imgur.com/6nZNi7A.png" width="256" height="256" alt="Image of browser inferencing on sample images."/> |



## Prerequisites
- A GPU enabled machine with DirectML on Windows
    - Windows comes with DirectML support. No additional configuration is needed. 
    - This was built on a GTX 1080 and it has not been tested on anything smaller.
- Clone this repo
```git
git clone https://github.com/saddam213/UnstableInfusion.git
```

## Use Hugging Face to download the Stable Diffusion models

Download the [ONNX Stable Diffusion models from Hugging Face](https://huggingface.co/models?sort=downloads&search=Stable+Diffusion).

 - [Stable Diffusion Models v1.5](https://huggingface.co/runwayml/stable-diffusion-v1-5/tree/onnx)


Once you have selected a model version repo, click `Files and Versions`, then select the `ONNX` branch. If there isn't an ONNX model branch available, use the `main` branch and convert it to ONNX. See the [ONNX conversion tutorial for PyTorch](https://learn.microsoft.com/windows/ai/windows-ml/tutorials/pytorch-convert-model) for more information.

- Clone the model repo:
```text
git lfs install
git clone https://huggingface.co/runwayml/stable-diffusion-v1-5 -b onnx
```


- MUST Set Build Target for x64 

__________________________
##  Resources
- [ONNX Runtime C# API Doc](https://onnxruntime.ai/docs/api/csharp/api)
- [Get Started with C# in ONNX Runtime](https://onnxruntime.ai/docs/get-started/with-csharp.html)
- [Hugging Face Stable Diffusion Blog](https://huggingface.co/blog/stable_diffusion)
- [Stable Diffusion C# Tutorial for this Repo](https://onnxruntime.ai/docs/tutorials/csharp/stable-diffusion-csharp.html)
