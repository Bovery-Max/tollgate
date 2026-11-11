// The main threat-intelligence sources a scan queries.
// "Confidence" is defined as how many of these actually responded.
namespace Tollgate.Domain;

public enum ThreatSource
{
    VirusTotal,
    SafeBrowsing,
    UrlHaus,
    Rdap,
}
