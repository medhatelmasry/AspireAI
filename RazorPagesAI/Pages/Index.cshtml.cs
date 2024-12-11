using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPagesAI.Services;

namespace RazorPagesAI.Pages;

public class IndexModel(
    ILogger<IndexModel> _logger, 
    ChatServiceSK _chatService
) : PageModel
{
    [BindProperty]
    public string? Reply { get; set; }

    public async Task OnGetAsync()
    {
        string response = await _chatService.ProcessMessageAsync("HELLO");
        _logger.LogInformation(response);
    }

    // action method that receives prompt from the form
    public async Task<IActionResult> OnPostAsync(string prompt)
    {
        // call the Azure Function
        string response = await _chatService.ProcessMessageAsync(prompt);
        Reply = response;
        return Page();
    }
}
