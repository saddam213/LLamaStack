﻿using System.ComponentModel.DataAnnotations;

namespace LLamaStack.WebApi.Models
{
    public record CancelRequest([Required] Guid SessionId);
}
