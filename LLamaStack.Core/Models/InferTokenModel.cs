﻿namespace LLamaStack.Core.Models
{
    public record InferTokenModel(int TokenId, float Logit, float Probability, string Content, InferTokenType Type, int Elapsed);

    public enum InferTokenType
    {
        Begin = 0,
        Content = 2,
        End = 4,
        Cancel = 10
    }
}
