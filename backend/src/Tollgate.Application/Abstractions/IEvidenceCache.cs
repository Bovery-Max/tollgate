// Port for remembering gathered evidence per URL (scan flow step 2). The SQLite implementation lives
// in Infrastructure, so the cache survives a restart without the orchestrator knowing how it is stored.
using Tollgate.Domain.Evidence;

namespace Tollgate.Application;

public interface IEvidenceCache
{
    /// <summary>Returns the stored evidence, or null when none exists or it has expired.</summary>
    Task<CachedScan?> GetAsync(string normalizedUrl, CancellationToken cancellationToken);

    /// <summary>Stores or replaces the evidence; its lifetime is counted from <see cref="CachedScan.ScannedAt"/>.</summary>
    Task SetAsync(string normalizedUrl, CachedScan scan, CancellationToken cancellationToken);
}

/// <param name="Evidence">Only sources that answered are non-null, so a re-scan asks just the missing ones.</param>
/// <param name="ScannedAt">When this URL was first scanned; late answers (VirusTotal) do not extend the lifetime.</param>
public sealed record CachedScan(ScanEvidence Evidence, DateTimeOffset ScannedAt);
