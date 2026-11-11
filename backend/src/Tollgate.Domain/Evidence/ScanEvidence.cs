// Raw facts gathered from all sources for one URL; the scoring engine's only input.
// A null member means that source did not respond (timeout, quota, error), which is
// different from "responded and found nothing".
namespace Tollgate.Domain.Evidence;

public sealed record ScanEvidence(
    VirusTotalEvidence? VirusTotal,
    SafeBrowsingEvidence? SafeBrowsing,
    UrlHausEvidence? UrlHaus,
    RdapEvidence? Rdap)
{
    public IReadOnlyList<ThreatSource> RespondedSources =>
        Enum.GetValues<ThreatSource>().Where(HasResponded).ToList();

    public IReadOnlyList<ThreatSource> UnavailableSources =>
        Enum.GetValues<ThreatSource>().Where(source => !HasResponded(source)).ToList();

    private bool HasResponded(ThreatSource source) => source switch
    {
        ThreatSource.VirusTotal => VirusTotal is not null,
        ThreatSource.SafeBrowsing => SafeBrowsing is not null,
        ThreatSource.UrlHaus => UrlHaus is not null,
        ThreatSource.Rdap => Rdap is not null,
        _ => throw new ArgumentOutOfRangeException(nameof(source), source, null),
    };
}
