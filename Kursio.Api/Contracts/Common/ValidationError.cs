namespace Kursio.Api.Contracts.Common;

public record ValidationError(string Field, string ErrorCode, Dictionary<string, object>? Params);