using Microsoft.Extensions.Primitives;

public class SecurityHeaders {
    private readonly RequestDelegate next;

    public SecurityHeaders(RequestDelegate next) {
        this.next = next;
    }

    public Task Invoke(HttpContext context) {
        context.Response.Headers.Add("thc-super-secure", new Microsoft.Extensions.Primitives.StringValues("enable"));
        return next(context);
    }
}