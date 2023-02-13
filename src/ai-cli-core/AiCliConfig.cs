namespace ai_cli_core;

public class AiCliConfig
{
    public string OpenAIApiKey { get; set; } = string.Empty;

    public OpenAI_API.Models.Model Model { get; set; } = OpenAI_API.Models.Model.DavinciText;
}
