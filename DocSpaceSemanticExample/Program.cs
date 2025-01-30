using System.ClientModel;
using System.Text;
using DocSpaceSemanticExample.Api;
using DocSpaceSemanticExample.Plugins;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using OpenAI;

const string docSpaceUrl = "http://localhost:8092";
const string authKey = "";
const string ollamaUrl = "http://localhost:11434/v1";
const string modelName = "qwen2.5:7b";
const string apiKey = "ollama";

var client = new OpenAIClient(
    new ApiKeyCredential(apiKey), 
    new OpenAIClientOptions
    {
        Endpoint = new Uri(ollamaUrl)
    });

var builder = Kernel.CreateBuilder();
builder.AddOpenAIChatCompletion(modelName, client);
builder.Plugins.AddFromType<RoomsPlugin>();
builder.Services.TryAddSingleton<DocSpaceClient>(_ =>
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

var executionSettings = new OpenAIPromptExecutionSettings
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

var stringBuilder = new StringBuilder();

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
    
    Console.WriteLine();
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("===========Assistant===========");
    Console.ResetColor();

    await foreach (var message in chatCompletionService.GetStreamingChatMessageContentsAsync(history, executionSettings, kernel))
    {
        Console.Write(message);
        stringBuilder.Append(message);
    }
    
    Console.WriteLine();
    
    history.AddAssistantMessage(stringBuilder.ToString());
    stringBuilder.Clear();
}