using System.Text.Json;
using Kursio.Api.Contracts.Common;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Kursio.Api.Extensions;

public static class ProblemDetailsExtensions
{
    public static bool HasErrors(this ProblemDetails details)
    {
        return details.Extensions.TryGetValue("errors", out var existing)
               && existing is List<ValidationError> list
               && list.Count > 0;
    }

    public static ProblemDetails AddError(
        this ProblemDetails details,
        string field,
        string errorCode,
        Dictionary<string, object>? parameters = null)
    {
        if (!details.Extensions.TryGetValue("errors", out var existing))
        {
            details.Extensions["errors"] = new List<ValidationError>
            {
                new(field, errorCode, parameters)
            };

            return details;
        }

        if (existing is List<ValidationError> list)
        {
            list.Add(new ValidationError(field, errorCode, parameters));
        }

        return details;
    }

    public static (string errorCode, Dictionary<string, object>? parameters)
        GetError(this ProblemDetails details, string field)
    {
        if (!details.Extensions.TryGetValue("errors", out var existing))
            return (string.Empty, null);

        List<ValidationError>? errors = null;

        if (existing is JsonElement element)
        {
            errors = JsonConvert.DeserializeObject<List<ValidationError>>(element.GetRawText());
        }

        if (errors == null || errors.Count == 0)
            return (string.Empty, null);

        var err = errors.FirstOrDefault(e => e.Field == field);

        return err == null 
            ? (string.Empty, null) 
            : (err.ErrorCode, err.Params);
    }
}