using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace RazorPagesAI.Services;

public class ChatServiceSK(ILogger<ChatServiceSK> _logger, Kernel _kernel)
{
    public async Task<string> ProcessMessageAsync(string message)
    {
        var chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();

        ChatHistory history = [];
        history.AddSystemMessage(@"You are an AI demonstration application. 
            You are a helpful chatbot. 
            Respond to the user' input responsibly.
            All responses should be safe for work.");
        // Get user input
        history.AddUserMessage(message);

        // Get the response from the AI
        var response = chatCompletionService.GetStreamingChatMessageContentsAsync(history, kernel: _kernel);

        string combinedResponse = string.Empty;
        CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(30)); // 30 seconds timeout

        try
        {
            var enumerator = response.WithCancellation(cts.Token).GetAsyncEnumerator();

            await foreach (var messageResponse in response.WithCancellation(cts.Token))
            {
                combinedResponse += messageResponse;
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("**** The operation was canceled.");
        }
        // Add the message from the agent to the chat history
        history.AddAssistantMessage(combinedResponse);

        return combinedResponse;
    }
}
