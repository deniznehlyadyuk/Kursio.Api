using System.Text.Json;
using Kursio.Api.Contracts.Common;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Shouldly;

namespace Kursio.Tests.Extensions;

public static class ProblemDetailsExtensions
{
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

    public static void ShouldContainError(this ProblemDetails problemDetails, string field, string errorCode, Dictionary<string, object>? parameters = null)
    {
        var (_errorCode, _params) = problemDetails.GetError(field);
        
        _errorCode.ShouldBe(errorCode);

        if (parameters == null || parameters.Count == 0)
        {
            return;
        }

        _params.ShouldNotBeNull().ShouldNotBeEmpty();
        
        foreach (var parameter in parameters)
        {
            _params.ShouldContainKeyAndValue(parameter.Key, parameter.Value);
        }
    }
}