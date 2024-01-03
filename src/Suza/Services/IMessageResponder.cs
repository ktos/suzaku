﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suzaku.Bot.Services
{
    public interface IMessageResponder
    {
        Task<string?> SummarizeBotMessageAsync(string bot, string? message);
        Task<string?> RespondAsync(string? message);
    }
}
