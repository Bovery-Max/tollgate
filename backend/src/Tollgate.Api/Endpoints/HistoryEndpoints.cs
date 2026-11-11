// GET /api/history and GET /api/stats: read-only views of the persistent scan history.
// Like the scan endpoint, they only translate between HTTP and the Application ports.
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Tollgate.Api.Contracts;
using Tollgate.Application;

namespace Tollgate.Api.Endpoints;

public static class HistoryEndpoints
{
    public static IEndpointRouteBuilder MapHistoryEndpoints(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("/api").RequireRateLimiting(ScanEndpoints.RateLimitPolicy);

        api.MapGet("/history", HistoryAsync)
            .WithName("ScanHistory")
            .WithSummary("The most recent finished scans, newest first")
            .WithDescription(
                "Only finished scans are listed: provisional (partial) results and repeats served from the cache are not. "
                + "'limit' is capped by the server.")
            .Produces<HistoryResponse>()
            .ProducesProblem(StatusCodes.Status429TooManyRequests);

        api.MapGet("/stats", StatsAsync)
            .WithName("ScanStatistics")
            .WithSummary("Aggregate numbers over the scan history: totals, verdict split, most scanned hosts")
            .Produces<StatsResponse>()
            .ProducesProblem(StatusCodes.Status429TooManyRequests);

        return app;
    }

    private static async Task<IResult> HistoryAsync(
        [FromQuery] int? limit,
        IScanHistory history,
        ScanEnvironment environment,
        IOptions<HistoryOptions> options,
        CancellationToken cancellationToken)
    {
        var settings = options.Value;
        var count = Math.Clamp(limit ?? settings.DefaultPageSize, 1, settings.MaxPageSize);

        var items = await history.GetRecentAsync(count, cancellationToken);
        return Results.Ok(new HistoryResponse(environment.Mode, items));
    }

    private static async Task<IResult> StatsAsync(
        IScanStatistics statistics,
        ScanEnvironment environment,
        IOptions<HistoryOptions> options,
        CancellationToken cancellationToken)
    {
        var numbers = await statistics.GetAsync(cancellationToken);
        return Results.Ok(StatsResponse.From(numbers, environment.Mode, options.Value.RetentionDays));
    }
}
