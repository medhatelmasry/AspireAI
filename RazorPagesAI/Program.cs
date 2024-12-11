using AiLibrary;
using Microsoft.SemanticKernel;
using RazorPagesAI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var azureChatDeploymentName = builder.Configuration["AI:AzureChatDeploymentName"] ?? "gpt-35-turbo";
var openAiChatModel = builder.Configuration["AI:OpenAiChatModel"] ?? "gpt-3.5-turbo";

Console.WriteLine($"**** Chat deployment name: {azureChatDeploymentName}");

var azureOrOpenAi = builder.Configuration["AI:AzureOrOpenAI"] ?? "OpenAI";
if (azureOrOpenAi.ToLower() == "openai")
{
    var connStr = builder.Configuration["ConnectionStrings:azureOpenAi"] ?? null;
    if (connStr == null)
    {
        Console.WriteLine("Connection string for Azure OpenAI is missing. Please add it to the appsettings.json file.");
        return;
    }

    Console.WriteLine($"**** connStr: {connStr}");

    (string endpoint, string key) = Helper.ParseAiConnectionString(connStr);

    builder.Services.AddKernel()
        .AddAzureOpenAIChatCompletion(azureChatDeploymentName, endpoint!, key!);
}
else
{
    var connStr = builder.Configuration["ConnectionStrings:openAi"] ?? null;
    if (connStr == null)
    {
        Console.WriteLine("Connection string for OpenAI is missing. Please add it to the appsettings.json file.");
        return;
    }

    (string endpoint, string key) = Helper.ParseAiConnectionString(connStr);

    builder.Services.AddKernel()
        .AddOpenAIChatCompletion(openAiChatModel, key);
}

builder.Services.AddTransient<ChatServiceSK>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
