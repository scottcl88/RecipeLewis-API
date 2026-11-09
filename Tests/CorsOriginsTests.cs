using RecipeLewis.Models;

namespace RecipeLewis.Tests;

[TestClass]
public class CorsOriginsTests
{
    private static readonly string[] Allowed = CorsOrigins.Normalize(new[]
    {
        "https://recipelewis.com", "https://www.recipelewis.com/", "https://main.d2v2unaw7gu6b3.amplifyapp.com", "http://localhost:5045"
    });

    [DataTestMethod]
    [DataRow("https://recipelewis.com")]
    [DataRow("https://RecipeLewis.com")]
    [DataRow("https://www.recipelewis.com")]
    [DataRow("https://main.d2v2unaw7gu6b3.amplifyapp.com")]
    [DataRow("http://localhost:5045")]
    public void Allows_ExactConfiguredOrigins(string origin) => Assert.IsTrue(CorsOrigins.IsAllowed(origin, Allowed));

    [DataTestMethod]
    [DataRow("https://recipelewis.com.evil.com")]
    [DataRow("https://evilrecipelewis.com")]
    [DataRow("https://evil.com/https://recipelewis.com")]
    [DataRow("https://evil.com?x=https://recipelewis.com")]
    [DataRow("https://recipelewis.com@evil.com")]
    [DataRow("http://recipelewis.com")]
    [DataRow("https://recipelewis.com:8443")]
    [DataRow("https://sub.recipelewis.com")]
    [DataRow("https://other.amplifyapp.com")]
    [DataRow("http://localhost:5046")]
    [DataRow("http://localhost")]
    [DataRow("https://recipelewis.com/path")]
    [DataRow("null")]
    [DataRow("*")]
    [DataRow("")]
    [DataRow(null)]
    public void Rejects_LookalikesAndUnlistedOrigins(string? origin) => Assert.IsFalse(CorsOrigins.IsAllowed(origin, Allowed));

    [DataTestMethod]
    [DataRow("*")]
    [DataRow("https://*.recipelewis.com")]
    [DataRow("recipelewis.com")]
    [DataRow("https://recipelewis.com/app")]
    [DataRow("ftp://recipelewis.com")]
    public void Normalize_RejectsWildcardsAndMalformedEntries(string entry)
        => Assert.ThrowsException<InvalidOperationException>(() => CorsOrigins.Normalize(new[] { entry }));

    [TestMethod]
    public void Normalize_IgnoresBlanksAndDedupes()
    {
        var r = CorsOrigins.Normalize(new string?[] { "", " ", null, "https://a.com", "https://A.com/" });
        CollectionAssert.AreEqual(new[] { "https://a.com" }, r);
        Assert.AreEqual(0, CorsOrigins.Normalize(null).Length);
    }

    [TestMethod]
    public void EmptyAllowlist_AllowsNothing() => Assert.IsFalse(CorsOrigins.IsAllowed("https://recipelewis.com", Array.Empty<string>()));
}
