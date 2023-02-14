Dotnet GPT3 Powered CLI
============

<!-- [![package workflow](https://github.com/dotnetcore/Hk-Gosuto/ai-cli-core/workflows/release.yml/badge.svg)](https://github.com/Hk-Gosuto/ai-cli-core/actions/workflows/release.yml) -->
![GitHub stars](https://img.shields.io/github/stars/Hk-Gosuto/ai-cli-core)
![Commit Date](https://img.shields.io/github/last-commit/Hk-Gosuto/ai-cli-core/main.svg?logo=github&logoColor=green&label=commit)
![GitHub license](https://img.shields.io/github/license/Hk-Gosuto/ai-cli-core)
<!-- ![Docker image](https://img.shields.io/docker/v/Hk-Gosuto/ai-cli-core?label=docker%20image) -->

[![NuGet][nuget-badge] ![NuGet Downloads][nuget-download-badge]][nuget]

[nuget]: https://www.nuget.org/packages/dotnet-ai-core/
[nuget-badge]: https://img.shields.io/nuget/v/dotnet-ai-core.svg?style=flat-square
[nuget-download-badge]: https://img.shields.io/nuget/dt/dotnet-ai-core?style=flat-square

<!-- <img src="https://user-images.githubusercontent.com/36589645/202102237-6666f461-1aa8-496a-9438-de15cee1696e.gif" width="900" height="600"/> -->
<!-- ![image](https://user-images.githubusercontent.com/36589645/202654014-c1884be3-76d5-4b64-81d1-e3f1169fcb46.png) -->


This project is the .NET version of [ai-cli](https://github.com/abhagsain/ai-cli) project.

### Get started

[Install .NET 7 or newer](https://get.dot.net) and run this command:

```
$ dotnet tool install --global dotnet-ai-core
```

Usage

```
$ dotnet-ai-core ask "Check process running on port"
```

You'd need to enter your own OpenAI API key
Here's how you can get one

1. Go to https://openai.com/api/login
2. Create an account or log into your existing account
3. Go to https://beta.openai.com/account/api-keys or
   <img width="1904" alt="image" src="https://user-images.githubusercontent.com/36589645/202097820-dc6905e6-4514-413b-980f-169c35ffef9a.png">
4. Run `ai auth`, enter your API KEY and you're good to go!

Pricing

The current prompt length is `~840` tokens and the pricing for [`text-davinci-002`](https://openai.com/api/pricing/) is `$0.02` for `1K` tokens which is ~`$0.017/command`. We'll see if we can improve the response as well as reduce the per-command-cost with fine-tuning.

# Usage

<!-- usage -->
```sh-session
$ dotnet tool install --global dotnet-ai-core
$ dotnet-ai-core
...
```
<!-- usagestop -->

# Commands

<!-- commands -->
- [Dotnet GPT3 Powered CLI](#dotnet-gpt3-powered-cli)
    - [Get started](#get-started)
- [Usage](#usage)
- [Commands](#commands)
  - [`dotnet-ai-core ask [question]`](#dotnet-ai-core-ask-question)
  - [`dotnet-ai-core auth`](#dotnet-ai-core-auth)
  - [`dotnet-ai-core model`](#dotnet-ai-core-model)

## `dotnet-ai-core ask [question]`

Ask question to GPT3 from your terminal

```
USAGE
  $ dotnet-ai-core ask [question]

ARGUMENTS
  QUESTION  Your question

DESCRIPTION
  Ask question to GPT3 from your terminal

EXAMPLES
  $ dotnet-ai-core ask "Check running process on port 3000"
```

## `dotnet-ai-core auth`

Update existing or add new OpenAI API Key

```
USAGE
  $ dotnet-ai-core auth

DESCRIPTION
  Update existing or add new OpenAI API Key

EXAMPLES
  $ dotnet-ai-core auth (Follow the prompt)
```

## `dotnet-ai-core model`

Change model preference (default: text-davinci-003)

```
USAGE
  $ dotnet-ai-core model

DESCRIPTION
  Change model preference (default: text-davinci-003)

EXAMPLES
  $ dotnet-ai-core model (Follow the prompt)
```
