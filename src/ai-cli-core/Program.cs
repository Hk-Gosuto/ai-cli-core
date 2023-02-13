using System.Text;
using System.Text.RegularExpressions;
using ai_cli_core;
using Kurukuru;
using McMaster.Extensions.CommandLineUtils;
using NerdyMishka;
using Newtonsoft.Json;
using OpenAI_API;
using OpenAI_API.Completions;
using Sharprompt;
using Sharprompt.Fluent;

Console.OutputEncoding = Encoding.UTF8;

var app = new CommandLineApplication
{
    Name = "dotnet-ai-core",
    Description = "Get answers for CLI commands from GPT3 right from your terminal"
};

app.HelpOption(inherited: true);

app.Command(
    "ask",
    config =>
    {
        config.Description = "Ask question to GPT3 from your terminal";
        var question = config.Argument("", "QUESTION  Your question").IsRequired();
        config.OnExecuteAsync(async cancellationToken =>
        {
            var aiCliConfig = await GetOrCreateConfigAsync(cancellationToken);
            var apiKey = aiCliConfig.OpenAIApiKey;
            if (string.IsNullOrEmpty(apiKey))
            {
                Console.WriteLine(
                    "You haven't set your OpenAI API key. Please login with "
                        + Chalk.Bold().BrightYellow().Draw("ai auth")
                );
                return 1;
            }
            var openai = new OpenAIAPI(apiKey);
            var prompt =
                $"{(OperatingSystem.IsWindows() ? CommonConst.PowerShellPrompt : CommonConst.UnixPrompt)}{question?.Value?.Trim()}\nA - ";
            var completion = new CompletionRequest
            {
                Prompt = prompt,
                Model = aiCliConfig.Model,
                Temperature = 0.8,
                FrequencyPenalty = 0.5,
                MaxTokens = 64,
                StopSequence = "\"\"\""
            };
            try
            {
                var completionTask = openai.Completions.CreateCompletionAsync(completion);
                await Spinner.StartAsync(
                    "Processing...",
                    async () =>
                    {
                        await completionTask;
                    }
                );
                var result = completionTask.Result;
                if (result != null)
                {
                    var codeRegex = new Regex("`(.*?)`");
                    var value = result.Completions.First().Text.Trim();
                    var match = codeRegex.Match(value);
                    value = match.Success ? match.Groups[1].Value : value;
                    Console.WriteLine(
                        "> "
                            + Chalk.Green().Draw("Command is ")
                            + Chalk.Bold().BrightYellow().Draw($"`{value}`")
                    );
                    var command = Sharprompt.Prompt.Select<string>(
                        o =>
                            o.WithMessage("Select an option")
                                .WithItems(new[] { "Copy to clipboard", "Exit" })
                                .WithDefaultValue("Copy to clipboard")
                    );
                    if (command.Equals("Copy to clipboard"))
                        TextCopy.ClipboardService.SetText(value);
                    Console.WriteLine(
                        Chalk.Red().Draw("Please don't run a command that you don't understand.")
                            + Chalk.Underline().Red().Draw($"Especially destructive commands")
                    );
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }

            return 0;
        });
    }
);

app.Command(
    "model",
    config =>
    {
        config.Description = "Change model preference (default: text-davinci-003)";
        config.OnExecuteAsync(async cancellationToken =>
        {
            var aiCliConfig = await GetOrCreateConfigAsync(cancellationToken);
            var model = Sharprompt.Prompt.Select<string>(
                o =>
                    o.WithMessage("Please select a model")
                        .WithItems(
                            new[]
                            {
                                OpenAI_API.Models.Model.DavinciText.ModelID,
                                OpenAI_API.Models.Model.CurieText.ModelID,
                                OpenAI_API.Models.Model.BabbageText.ModelID,
                                OpenAI_API.Models.Model.AdaText.ModelID
                            }
                        )
                        .WithDefaultValue(aiCliConfig.Model.ModelID)
            );
            aiCliConfig.Model = new OpenAI_API.Models.Model(model) { OwnedBy = "openai" };
            await SaveConfigAsync(aiCliConfig, cancellationToken);
            Console.WriteLine(
                "✅ Model preference saved. You can change it anytime again with " + Chalk.Bold().BrightYellow().Draw($"ai model")
            );
        });
    }
);

app.Command(
    "auth",
    config =>
    {
        config.Description = "Update existing or add new OpenAI API Key";
        config.OnExecuteAsync(async cancellationToken =>
        {
            var apiKey = Sharprompt.Prompt.Password(
                "Please enter your OpenAI API Key (This would overwrite the existing key)",
                validators: new[] { Validators.Required() }
            );
            var aiCliConfig = await GetOrCreateConfigAsync(cancellationToken);
            aiCliConfig.OpenAIApiKey = apiKey;
            await SaveConfigAsync(aiCliConfig, cancellationToken);
            Console.WriteLine(
                "API Key is saved at " + Chalk.Bold().BrightYellow().Draw($"{GetConfigFilePath()}")
            );
        });
    }
);

app.OnExecute(() =>
{
    app.ShowHelp();
    return 1;
});

static string GetConfigFilePath()
{
    var configPath = Path.Combine(GeUserConfigPath(), "ai-cli-core");
    if (!Path.Exists(configPath))
        Directory.CreateDirectory(configPath);
    return Path.Combine(configPath, ".ai-cli-core.json");
}

static async Task<AiCliConfig> SaveConfigAsync(
    AiCliConfig config,
    CancellationToken cancellationToken = default
)
{
    var configFilePath = GetConfigFilePath();
    await File.WriteAllTextAsync(
        configFilePath,
        JsonConvert.SerializeObject(config),
        cancellationToken
    );
    return await GetOrCreateConfigAsync(cancellationToken);
}

static async Task<AiCliConfig> GetOrCreateConfigAsync(CancellationToken cancellationToken = default)
{
    var configFilePath = GetConfigFilePath();
    if (!File.Exists(configFilePath))
        return new AiCliConfig();
    var aiCliConfig =
        JsonConvert.DeserializeObject<AiCliConfig>(
            await File.ReadAllTextAsync(configFilePath, cancellationToken)
        ) ?? new AiCliConfig();
    return aiCliConfig;
}

static string GeUserConfigPath()
{
    if (OperatingSystem.IsWindows())
        return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
    var userConfigFolder = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME");
    if (string.IsNullOrEmpty(userConfigFolder))
    {
        userConfigFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".config"
        );
    }
    return userConfigFolder;
}

return app.Execute(args);
