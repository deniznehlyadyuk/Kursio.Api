namespace Kursio.Api.Extensions;

public static class HttpRequestExtensions
{
    public static Guid GetGuid(this HttpRequest httpRequest, string routeParam)
    {
        return Guid.Parse((httpRequest.RouteValues[routeParam] as string)!);
    }
}