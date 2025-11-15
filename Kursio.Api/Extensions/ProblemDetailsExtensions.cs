using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace Kursio.Api.Extensions;

public static class ProblemDetailsExtensions
{   
    public static ProblemDetails AddError(this ProblemDetails details, string field, string message)
    {
        if (!details.Extensions.TryGetValue("errors", out var existing))
        {
            details.Extensions["errors"] = new List<string[]>
            {
                new[] { field, message }
            };
            
            return details;
        }

        if (existing is List<string[]> list)
        {
            list.Add([field, message]);
        }
        
        return details;
    }

    public static string GetError(this ProblemDetails details, string field)
    {
        if (!details.Extensions.TryGetValue("errors", out var existing))
        {
            return string.Empty;
        }

        var raw = ((JsonElement)existing!).GetRawText();
        
        var errors = JsonSerializer.Deserialize<List<string[]>>(raw);
        
        return errors!.FirstOrDefault(error => error[0] == field)?[1] ?? string.Empty;
    }
}