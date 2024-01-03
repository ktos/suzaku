using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suza.Services
{
    public class PromptProvider
    {
        public PromptProvider()
        {
            SystemPrompt = File.ReadAllText("Prompts/system.txt");
            PassToTheHumanInstructionPrompt = File.ReadAllText("Prompts/passtothehuman.txt");
        }

        public string SystemPrompt { get; }
        public string PassToTheHumanInstructionPrompt { get; }
    }
}
