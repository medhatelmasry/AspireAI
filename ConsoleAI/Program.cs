using AiLibrary;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();

var azureOrOpenAI = config["AI:AzureOrOpenAI"] ?? "OpenAI";
Console.WriteLine($"**** Using {azureOrOpenAI} services");

var builder = Kernel.CreateBuilder();

// Add OpenAI services to the kernel
if (azureOrOpenAI.ToLower() == "azure")
{
    var connStr = config["ConnectionStrings:azureOpenAi"] ?? null;
    if (connStr == null)
    {
        Console.WriteLine("Connection string for Azure OpenAI is missing. Please add it to the appsettings.json file.");
        return;
    }
    var azureChatDeploymentName = config["AI:AzureChatDeploymentName"] ?? "gpt-35-turbo";
    Console.WriteLine($"**** Chat deployment name: {azureChatDeploymentName}");

    (string endpoint, string key) = Helper.ParseAiConnectionString(connStr);

    // use azure services
    builder.AddAzureOpenAIChatCompletion(azureChatDeploymentName, endpoint!, key!);
}
else
{
    var connStr = config["ConnectionStrings:openAi"] ?? null;
    if (connStr == null)
    {
        Console.WriteLine("Connection string for OpenAI is missing. Please add it to the appsettings.json file.");
        return;
    }
    var openAiChatModel = config["AI:OpenAiChatModel"] ?? "gpt-3.5-turbo";
    Console.WriteLine($"**** Chat deployment name: {openAiChatModel}");

    (string endpoint, string key) = Helper.ParseAiConnectionString(connStr);

    // use openai services
    builder.AddOpenAIChatCompletion(openAiChatModel, key!);
}

// Build the kernel
var kernel = builder.Build();

var chat = kernel.GetRequiredService<IChatCompletionService>();
var history = new ChatHistory();
history.AddSystemMessage("You are a useful chatbot. You always reply with a single sentence.");

List<string> questions = new List<string>
{
    "How many planets are there is our solar system?",
    "Which is the largest planet?",
    "Which is the smallest planet?",
    "Which is the closest planet to Earth?",
    "Which is the furthest planet to Earth?",
    "Why was pluto removed from the list of planets?"
};

foreach (var q in questions)
{
    Console.Write($"Q: {q}\n\tA: ");
    history.AddUserMessage(q);

    var settings = new PromptExecutionSettings();
    var result = chat.GetStreamingChatMessageContentsAsync(history, settings, kernel);
    var response = "";

    await foreach (var message in result)
    {
        response += message;
        Console.Write(message);
    }
    Console.WriteLine("");

    history.AddAssistantMessage(response);
}

