using System.Text;
using System.Text.RegularExpressions;
using ai_cli_core;
using Kurukuru;
using McMaster.Extensions.CommandLineUtils;
using Newtonsoft.Json;
using OpenAI.GPT3;
using OpenAI.GPT3.Managers;
using OpenAI.GPT3.ObjectModels;
using OpenAI.GPT3.ObjectModels.RequestModels;
using Sharprompt;
using Sharprompt.Fluent;
using static Crayon.Output;

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
            var apiKey = aiCliConfig.OpenAiApiKey;
            if (string.IsNullOrEmpty(apiKey))
            {
                Console.WriteLine(
                    "You haven't set your OpenAI API key. Please login with "
                        + Bold().Yellow().Text("ai auth")
                );
                return 1;
            }
            var openai = new OpenAIService(new OpenAiOptions { ApiKey = apiKey });
            var prompt =
                $"{(OperatingSystem.IsWindows() ? CommonConst.PowerShellPrompt : CommonConst.UnixPrompt)}{question?.Value?.Trim()}\nA - ";

            try
            {
                var ask = string.Empty;
                if (aiCliConfig.Model.Equals(Models.ChatGpt3_5Turbo))
                {
                    var completion = new ChatCompletionCreateRequest
                    {
                        Messages = new List<ChatMessage>
                        {
                            //ChatMessage.FromSystem($"{(OperatingSystem.IsWindows() ? CommonConst.PowerShellPrompt : CommonConst.UnixPrompt)}"),
                            ChatMessage.FromUser(prompt)
                        },
                        Model = Models.ChatGpt3_5Turbo,
                        Temperature = 0.8f,
                        FrequencyPenalty = 0.5f,
                        MaxTokens = 64,
                        Stop = "\"\"\""
                    };
                    var completionTask = openai.ChatCompletion.CreateCompletion(completion);
                    await Spinner.StartAsync(
                        "Processing...",
                        async () =>
                        {
                            await completionTask;
                        }
                    );
                    if (completionTask.Result.Successful)
                    {
                        ask = completionTask.Result.Choices.First().Message.Content;
                    }
                }
                else
                {
                    var completion = new CompletionCreateRequest
                    {
                        Prompt = prompt,
                        Model = aiCliConfig.Model,
                        Temperature = 0.8f,
                        FrequencyPenalty = 0.5f,
                        MaxTokens = 64,
                        Stop = "\"\"\""
                    };
                    var completionTask = openai.Completions.CreateCompletion(completion);
                    await Spinner.StartAsync(
                        "Processing...",
                        async () =>
                        {
                            await completionTask;
                        }
                    );
                    await Spinner.StartAsync(
                        "Processing...",
                        async () =>
                        {
                            await completionTask;
                        }
                    );
                    if (completionTask.Result.Successful)
                    {
                        ask = completionTask.Result.Choices.First().Text;
                    }
                }

                if (!string.IsNullOrEmpty(ask))
                {
                    var codeRegex = new Regex("`(.*?)`");
                    var match = codeRegex.Match(ask);
                    ask = match.Success ? match.Groups[1].Value : ask;
                    Console.WriteLine(
                        "> " + Green().Text("Command is ") + Bold().Yellow().Text($"`{ask}`")
                    );
                    var command = Sharprompt.Prompt.Select<string>(
                        o =>
                            o.WithMessage("Select an option")
                                .WithItems(new[] { "Copy to clipboard", "Exit" })
                                .WithDefaultValue("Copy to clipboard")
                    );
                    if (command.Equals("Copy to clipboard"))
                        TextCopy.ClipboardService.SetText(ask);
                    Console.WriteLine(
                        Red().Text("Please don't run a command that you don't understand.")
                            + Underline().Red().Text($"Especially destructive commands")
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
                                Models.ChatGpt3_5Turbo,
                                Models.TextDavinciV3,
                                Models.TextDavinciV2,
                                Models.CodeDavinciV2,
                                Models.CodeCushmanV1,
                                Models.TextCurieV1,
                                Models.TextBabbageV1,
                                Models.TextAdaV1,
                                Models.Davinci,
                                Models.Curie,
                                Models.Babbage,
                                Models.Ada
                            }
                        )
                        .WithDefaultValue(aiCliConfig.Model)
            );
            aiCliConfig.Model = model;
            await SaveConfigAsync(aiCliConfig, cancellationToken);
            Console.WriteLine(
                "✅ Model preference saved. You can change it anytime again with "
                    + Bold().Yellow().Text("ai model")
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
            aiCliConfig.OpenAiApiKey = apiKey;
            await SaveConfigAsync(aiCliConfig, cancellationToken);
            Console.WriteLine(
                "API Key is saved at " + Bold().Yellow().Text($"{GetConfigFilePath()}")
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
    try
    {
        var aiCliConfig =
            JsonConvert.DeserializeObject<AiCliConfig>(
                await File.ReadAllTextAsync(configFilePath, cancellationToken)
            ) ?? new AiCliConfig();
        return aiCliConfig;
    }
    catch (Exception ex)
    {
        File.Delete(configFilePath);
        Console.WriteLine(Bold().Red().Text(ex.Message));
    }
    return new AiCliConfig();
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

//return app.Execute(new[] { "ask", "获取开放端口" });

return app.Execute(args);
