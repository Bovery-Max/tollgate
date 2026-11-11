// Port for the persistent scan history: one entry per finished scan, kept for HistoryOptions.RetentionDays.
using Tollgate.Domain;

namespace Tollgate.Application;

public interface IScanHistory
{
    /// <summary>Appends an entry and drops entries that are older than the retention period.</summary>
    Task AddAsync(ScanHistoryEntry entry, CancellationToken cancellationToken);

    /// <summary>The newest entries first, at most <paramref name="limit"/> of them.</summary>
    Task<IReadOnlyList<ScanHistoryEntry>> GetRecentAsync(int limit, CancellationToken cancellationToken);
}

/// <param name="Url">The normalized URL that was scanned (the full URL, not only the host).</param>
/// <param name="Host">Punycode host of the URL; kept apart so statistics can group by domain.</param>
/// <param name="ScannedAt">When this scan finished (not when the cached evidence was first gathered).</param>
public sealed record ScanHistoryEntry(
    string Url,
    string Host,
    int Score,
    Verdict Verdict,
    Confidence Confidence,
    DateTimeOffset ScannedAt);
