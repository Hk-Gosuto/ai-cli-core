using OpenAI.GPT3.ObjectModels;

namespace ai_cli_core;

public class AiCliConfig
{
    public string OpenAiApiKey { get; set; } = string.Empty;

    public string Model { get; set; } = Models.ChatGpt3_5Turbo;
}
