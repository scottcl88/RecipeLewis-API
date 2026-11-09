# RecipeLewis-API code review

Date: 2026-11-08 · Scope: static review of Program.cs, auth/JWT, middleware, controllers, models, package refs. No runtime/pen testing; no DB access. Author: AI assistant (anthropic/claude-sonnet-5-5); needs human confirmation.

## Security (priority order)
1. **HIGH – CORS allows any origin with credentials.** `Program.cs`: the global `app.UseCors(x => x.SetIsOriginAllowed(origin => true)...AllowCredentials())` reflects any Origin. The stricter named policy `MyPolicy` (`IsOriginAllowed`) is registered but never applied. Also `UseCors` runs after `UseAuthorization`; it should precede it. Fix: `app.UseCors("MyPolicy")` placed before auth. Not changed here (production behaviour change; verify the Web origin first).
2. **HIGH – Weak origin regex.** `IsOriginAllowed` builds an unescaped regex `^...{origin}...$` with `.*` before the origin, so e.g. `https://evil-recipelewis.com` or `https://x.localhost` match. Use exact scheme+host comparison (`Uri` parse, allow-list).
3. **HIGH – Committed identifiers / rotation.** The JWT secret was removed in PR #1 but remains in git history; rotation still pending (see projects/index.md). appsettings.json still contains Azure AD tenant/client IDs and admin email (not secrets, but PII/config; prefer env/App Config).
4. **MED – Custom JWT middleware instead of ASP.NET auth.** Tokens: HS256, no issuer/audience, 15-min lifetime, no `jti`; `ValidateJwtToken` swallows all exceptions; `int.Parse` on claim. The middleware does a DB lookup per request (including anonymous routes with a token). Consider `AddAuthentication().AddJwtBearer` (the JwtBearer/Identity.Web packages are already referenced but appear unused).
5. **MED – Anonymous document endpoints** (`DocumentController` download/view `[AllowAnonymous]`) take `recipeId/documentId`; confirm recipes are intended public and that documentId must belong to recipeId (IDOR check).
6. **MED – ErrorHandlerMiddleware returns `error.Message` for unhandled 500s** (information disclosure) and is registered *after* CORS/auth so error responses lack some middleware; it is also before JwtMiddleware so that is fine, but log the exception (currently not logged).
7. **MED – Recaptcha**: `GoogleRecaptcha.Verify` returns raw response; ensure callers check `success`/score and handle failure; new RestClient per call (socket exhaustion pattern); secret read from config (OK).
8. **MED – Forgot-password/validate-reset-token/register** are anonymous with no rate limiting; add `AddRateLimiter`. Check for user enumeration in messages.
9. **LOW – `LogController` POST has no `[Authorize]`/limits** (unauthenticated log injection / disk abuse).
10. **LOW – `AllowedHosts: "*"`; Swagger only in Development (good).**
11. **Dependencies (after this PR):** MailKit 4.14.1 still flagged NU1902 (GHSA-9j88-vvj5-vhgr) – check for a newer release. RestSharp, Azure.Identity, EF Core, AutoMapper advisories cleared by upgrade.

## Correctness
1. `HtmlSanitizer` is instantiated per call in three `SanitizeHtml()` copies (RecipeModel, Create/Update requests) – duplicated logic; verify the sanitizer is actually invoked on every write path (it is a manual method call, easy to miss). Default allow-list may strip tags the editor needs; HtmlSanitizer 10 changes defaults slightly – manual test of recipe save recommended.
2. 18 nullable warnings: `HttpContext.Items["User"]` can be null in controllers (CS8604/8602) → NullReference rather than 401 if `[Authorize]` is bypassed; `UserService` interface implementations have `UserId` nullability mismatches (CS8767).
3. `UseLazyLoadingProxies` can cause N+1 and serialization cycles; consider explicit `Include`/projection to DTOs.
4. `JwtUtils.getUniqueToken` does recursive DB scan of all users' refresh tokens (`Users.Any(u => u.RefreshTokens.Any(...))`) – slow as data grows; 64 random bytes make collision check unnecessary.
5. Hard-coded Key Vault URL, CORS origins in code → move to configuration.
6. Tests: only 5 tests; no coverage of auth, sanitization, CORS, or services against DB. Add regression tests for HTML sanitization and JWT validation.

## Maintainability
- Package versions duplicated across 6 csproj files (cause of the NU1605 downgrade errors seen in this upgrade) → adopt `Directory.Packages.props` (central package management).
- Unused packages likely (JwtBearer, OpenIdConnect, Identity.Web(.UI), Azure AppConfiguration, Newtonsoft + System.Text.Json both) – verify and prune.
- Typos: `GoogleRecaptchaSiteVeirfyModel`; `.nswag` file checked in.
- Services mix controllers' concerns (email, auth, DB) in a 500+ line `UserService`; split.
- Target framework net8.0 (LTS, supported to Nov 2026) → plan upgrade to net10.0 LTS.
- EF Core packages are 9.0.x on net8.0: supported, but align with the TFM upgrade.
