using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MetarReader.Web.Pages;

/// <summary>The generic error page shown when an unhandled exception reaches the ASP.NET Core exception handler.</summary>
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
[IgnoreAntiforgeryToken]
public class ErrorModel : PageModel
{
    /// <summary>The current request's trace identifier, shown so a user can reference it when reporting an issue.</summary>
    public string? RequestId { get; set; }

    /// <summary>Whether <see cref="RequestId"/> has a value worth displaying.</summary>
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    private readonly ILogger<ErrorModel> _logger;

    public ErrorModel(ILogger<ErrorModel> logger)
    {
        _logger = logger;
    }

    /// <summary>Captures the current request's trace identifier for display.</summary>
    public void OnGet()
    {
        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
    }
}

