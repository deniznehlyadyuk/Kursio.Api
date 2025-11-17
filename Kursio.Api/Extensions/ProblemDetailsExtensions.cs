using Kursio.Api.Contracts.Common;
using Microsoft.AspNetCore.Mvc;

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
}