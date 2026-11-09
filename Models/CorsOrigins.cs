namespace RecipeLewis.Models;

/// <summary>
/// Strict CORS origin allowlist. Only exact (case-insensitive) matches of normalized
/// scheme://host[:port] origins are accepted: no wildcards, no regex, no substring or suffix matching.
/// </summary>
public static class CorsOrigins
{
    /// <summary>Validates and normalizes configured origins; throws on wildcards or malformed entries.</summary>
    public static string[] Normalize(IEnumerable<string?>? configured)
    {
        var result = new List<string>();
        foreach (var raw in configured ?? Array.Empty<string?>())
        {
            if (string.IsNullOrWhiteSpace(raw)) continue;
            var entry = raw.Trim();
            if (entry.Contains('*'))
                throw new InvalidOperationException($"Cors:AllowedOrigins must not contain wildcards: '{entry}'.");
            if (!TryNormalize(entry, out var normalized))
                throw new InvalidOperationException($"Cors:AllowedOrigins entry is not a valid http(s) origin: '{entry}'.");
            if (!result.Contains(normalized, StringComparer.Ordinal)) result.Add(normalized);
        }
        return result.ToArray();
    }

    public static bool IsAllowed(string? origin, IReadOnlyCollection<string> allowed)
    {
        if (string.IsNullOrWhiteSpace(origin)) return false;
        if (!TryNormalize(origin.Trim(), out var normalized)) return false;
        return allowed.Contains(normalized, StringComparer.Ordinal);
    }

    private static bool TryNormalize(string value, out string normalized)
    {
        normalized = "";
        if (value.Contains('?') || value.Contains('#')) return false;
        if (!Uri.TryCreate(value, UriKind.Absolute, out var uri)) return false;
        if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps) return false;
        // origin = scheme://host[:port] only: reject paths, userinfo
        if (!string.IsNullOrEmpty(uri.UserInfo) || uri.AbsolutePath != "/") return false;
        normalized = uri.GetLeftPart(UriPartial.Authority).ToLowerInvariant();
        return true;
    }
}
