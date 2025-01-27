using DocSpaceSemanticExample.Api;
using DocSpaceSemanticExample.Plugins;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Ollama;
using OllamaSharp;

#pragma warning disable SKEXP0001
#pragma warning disable SKEXP0070

const string docSpaceUrl = "http://localhost:8092";
const string authKey = "";
const string ollamaUrl = "http://localhost:11434";
const string modelName = "qwen2.5:7b";

using var client = new OllamaApiClient(ollamaUrl, modelName);

var builder = Kernel.CreateBuilder();
builder.AddOllamaChatCompletion(client);
builder.Plugins.AddFromType<RoomsPlugin>();
builder.Services.TryAddSingleton<DocSpaceClient>(sp =>
{
    var httpClient = new HttpClient();
    httpClient.BaseAddress = new Uri(docSpaceUrl);
    httpClient.DefaultRequestHeaders.Add(
        "Cookie", 
        $"asc_auth_key={authKey}");
    
    return new DocSpaceClient(httpClient);
});

var kernel = builder.Build();

var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

var executionSettings = new OllamaPromptExecutionSettings
{
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
};

var history = new ChatHistory();
history.AddSystemMessage("""
                         You're an assistant. You know how to answer questions. You know how to create rooms, these are areas to work on documents. You can add arrays of users to a room
                         PublicRoom – for inviting users via external links to view documents without registration, you can also embed this room into any web interface.
                         FormFillingRoom allows you to upload PDF forms into the room, invite users to fill out a PDF form, review completed forms and analyze data automatically collected in a spreadsheet.
                         CollaborationRoom – for co-editing documents, spreadsheets, presentations.
                         VirtualDataRoom - for using advanced file security options: set watermarks, automatically index and track all content, restrict downloading and copying.
                         CustomRoom – Apply your own settings to use this room for any custom purpose.
                         """);

while (true)
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("==============You==============");
    Console.ResetColor();
    
    var userMessage = Console.ReadLine();
    if (userMessage == "exit")
    {
        break;
    }

    if (string.IsNullOrWhiteSpace(userMessage))
    {
        continue;
    }
    
    history.AddUserMessage(userMessage);

    var message = await chatCompletionService.GetChatMessageContentAsync(history, executionSettings, kernel);

    if (!string.IsNullOrEmpty(message.Content))
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("===========Assistant===========");
        Console.ResetColor();
        
        Console.WriteLine(message.Content);
        history.AddAssistantMessage(message.Content);

        Console.WriteLine();
    }
}