using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suzaku.Bot.Services
{
    public class FilePromptProvider : IPromptProvider
    {
        public FilePromptProvider()
        {
            SystemPrompt = File.ReadAllText("Prompts/system.txt");
        }

        public string SystemPrompt { get; }
    }
}
