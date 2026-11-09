using System.Diagnostics.CodeAnalysis;
using Database;
using RecipeLewis.Models;
using RecipeLewis.Models.Requests;

namespace RecipeLewis.Services.Mapping;

/// <summary>
/// Explicit, hand-written mappings between database entities, API models and requests.
/// Behavior mirrors the former AutoMapper profile: null source objects map to null,
/// null source collections map to empty lists, and members not listed here keep their defaults.
/// </summary>
public static class EntityMapper
{
    private static List<TDest> MapList<TSrc, TDest>(IEnumerable<TSrc>? source, Func<TSrc, TDest> map)
        => source == null ? new List<TDest>() : source.Select(map).ToList();

    private static void CopyEntityData(EntityData src, EntityDataModel dest)
    {
        dest.CreatedDateTime = src.CreatedDateTime;
        dest.ModifiedDateTime = src.ModifiedDateTime;
        dest.DeletedDateTime = src.DeletedDateTime;
    }

    private static void CopyEntityData(EntityDataModel src, EntityData dest)
    {
        dest.CreatedDateTime = src.CreatedDateTime;
        dest.ModifiedDateTime = src.ModifiedDateTime;
        dest.DeletedDateTime = src.DeletedDateTime;
    }

    private static void CopyEntityDataUser(EntityDataUser src, EntityDataUserModel dest)
    {
        CopyEntityData(src, dest);
        dest.CreatedBy = src.CreatedBy.ToModel();
        dest.ModifiedBy = src.ModifiedBy.ToModel();
        dest.DeletedBy = src.DeletedBy.ToModel();
    }

    private static void CopyEntityDataUser(EntityDataUserModel src, EntityDataUser dest)
    {
        CopyEntityData(src, dest);
        dest.CreatedBy = src.CreatedBy.ToEntity();
        dest.ModifiedBy = src.ModifiedBy.ToEntity();
        dest.DeletedBy = src.DeletedBy.ToEntity();
    }

    // ---- User ----
    [return: NotNullIfNotNull(nameof(s))]
    public static UserModel? ToModel(this User? s)
    {
        if (s == null) return null;
        var d = new UserModel
        {
            UserId = new UserId(s.UserId),
            UserGUID = s.UserGUID,
            LastIPAddress = s.LastIPAddress,
            LastLogin = s.LastLogin,
            LastLogout = s.LastLogout,
            TimeZone = s.TimeZone,
            UtcOffset = s.UtcOffset,
            Email = s.Email,
            Name = s.Name,
            Role = (RecipeLewis.Models.Role)(int)s.Role,
            RequestedAccess = s.RequestedAccess,
            RequestedAccessExpires = s.RequestedAccessExpires,
            RefreshTokens = MapList(s.RefreshTokens, t => t.ToModel()!)
        };
        CopyEntityData(s, d);
        return d;
    }

    public static List<UserModel> ToModels(this IEnumerable<User>? s) => MapList(s, u => u.ToModel()!);

    [return: NotNullIfNotNull(nameof(s))]
    public static User? ToEntity(this UserModel? s)
    {
        if (s == null) return null;
        var d = new User
        {
            UserId = s.UserId?.Value ?? 0,
            UserGUID = s.UserGUID,
            LastIPAddress = s.LastIPAddress!,
            LastLogin = s.LastLogin ?? default,
            LastLogout = s.LastLogout ?? default,
            TimeZone = s.TimeZone,
            UtcOffset = s.UtcOffset,
            Email = s.Email!,
            Name = s.Name!,
            Role = (Database.Role)(int)s.Role,
            RequestedAccess = s.RequestedAccess,
            RequestedAccessExpires = s.RequestedAccessExpires
        };
        CopyEntityData(s, d);
        return d;
    }

    public static User ToEntity(this RegisterRequest s)
    {
        return new User { Name = s.Name, Email = s.Email };
    }

    /// <summary>
    /// CreateUserRequest had no AutoMapper map (calling it threw at runtime). Explicit mapping added:
    /// Name = "FirstName LastName", Email, and Role parsed from the request (defaults to User when unparseable).
    /// </summary>
    public static User ToEntity(this CreateUserRequest s)
    {
        var role = Enum.TryParse<Database.Role>(s.Role, true, out var parsed) ? parsed : Database.Role.User;
        return new User { Name = $"{s.FirstName} {s.LastName}".Trim(), Email = s.Email, Role = role };
    }

    // ---- RefreshToken ----
    [return: NotNullIfNotNull(nameof(s))]
    public static RefreshTokenModel? ToModel(this RefreshToken? s)
    {
        if (s == null) return null;
        var d = new RefreshTokenModel
        {
            Id = s.Id,
            Token = s.Token,
            Expires = s.Expires,
            Created = s.Created,
            CreatedByIp = s.CreatedByIp!,
            Revoked = s.Revoked,
            RevokedByIp = s.RevokedByIp!,
            ReplacedByToken = s.ReplacedByToken!,
            ReasonRevoked = s.ReasonRevoked!
        };
        CopyEntityData(s, d);
        return d;
    }

    // ---- Category ----
    [return: NotNullIfNotNull(nameof(s))]
    public static CategoryModel? ToModel(this Category? s)
    {
        if (s == null) return null;
        var d = new CategoryModel { CategoryId = s.CategoryId, Name = s.Name };
        CopyEntityDataUser(s, d);
        return d;
    }

    public static List<CategoryModel> ToModels(this IEnumerable<Category>? s) => MapList(s, x => x.ToModel()!);

    [return: NotNullIfNotNull(nameof(s))]
    public static Category? ToEntity(this CategoryModel? s)
    {
        if (s == null) return null;
        var d = new Category { CategoryId = s.CategoryId, Name = s.Name };
        CopyEntityDataUser(s, d);
        return d;
    }

    // ---- Tag ----
    [return: NotNullIfNotNull(nameof(s))]
    public static TagModel? ToModel(this Tag? s)
    {
        if (s == null) return null;
        var d = new TagModel { TagId = s.TagId, Name = s.Name };
        CopyEntityDataUser(s, d);
        return d;
    }

    [return: NotNullIfNotNull(nameof(s))]
    public static Tag? ToEntity(this TagModel? s)
    {
        if (s == null) return null;
        // Tag.RecipeId has no counterpart on TagModel and is left at its default.
        var d = new Tag { TagId = s.TagId, Name = s.Name };
        CopyEntityDataUser(s, d);
        return d;
    }

    // ---- Document ----
    [return: NotNullIfNotNull(nameof(s))]
    public static DocumentModel? ToModel(this Document? s)
    {
        if (s == null) return null;
        var d = new DocumentModel
        {
            DocumentId = s.DocumentId,
            Name = s.Name,
            FileName = s.FileName,
            Description = s.Description,
            ContentType = s.ContentType,
            ByteSize = s.ByteSize,
            Bytes = s.Bytes,
            DocumentKey = s.DocumentKey
        };
        CopyEntityDataUser(s, d);
        return d;
    }

    public static List<DocumentModel> ToModels(this IEnumerable<Document>? s) => MapList(s, x => x.ToModel()!);

    [return: NotNullIfNotNull(nameof(s))]
    public static Document? ToEntity(this DocumentModel? s)
    {
        if (s == null) return null;
        var d = new Document
        {
            DocumentId = s.DocumentId,
            Name = s.Name,
            FileName = s.FileName,
            Description = s.Description,
            ContentType = s.ContentType,
            ByteSize = s.ByteSize,
            Bytes = s.Bytes,
            DocumentKey = s.DocumentKey
        };
        CopyEntityDataUser(s, d);
        return d;
    }

    public static List<Document> ToEntities(this IEnumerable<DocumentModel>? s) => MapList(s, x => x.ToEntity()!);

    // ---- Recipe ----
    /// <summary>Documents are intentionally not mapped (they were ignored by the former profile).</summary>
    [return: NotNullIfNotNull(nameof(s))]
    public static RecipeModel? ToModel(this Recipe? s)
    {
        if (s == null) return null;
        var d = new RecipeModel
        {
            RecipeId = s.RecipeId,
            Title = s.Title,
            Author = s.Author,
            Description = s.Description,
            Storage = s.Storage,
            Ingredients = s.Ingredients,
            Directions = s.Directions,
            IngredientsHTML = s.IngredientsHTML,
            DirectionsHTML = s.DirectionsHTML,
            CookTime = s.CookTime,
            PrepTime = s.PrepTime,
            TotalTime = s.TotalTime,
            Servings = s.Servings,
            Yield = s.Yield,
            Nutrition = s.Nutrition,
            Category = s.Category.ToModel()!,
            Tags = MapList(s.Tags, t => t.ToModel()!)
        };
        CopyEntityDataUser(s, d);
        return d;
    }

    public static List<RecipeModel> ToModels(this IEnumerable<Recipe>? s) => MapList(s, x => x.ToModel()!);

    public static Recipe ToEntity(this CreateRecipeRequest s)
    {
        return new Recipe
        {
            Title = s.Title,
            Author = s.Author,
            Description = s.Description,
            Storage = s.Storage,
            Ingredients = s.Ingredients,
            Directions = s.Directions,
            IngredientsHTML = s.IngredientsHTML,
            DirectionsHTML = s.DirectionsHTML,
            CookTime = s.CookTime,
            PrepTime = s.PrepTime,
            TotalTime = s.TotalTime,
            Servings = s.Servings,
            Yield = s.Yield,
            Nutrition = s.Nutrition,
            Category = s.Category.ToEntity()!,
            Tags = MapList(s.Tags, t => t.ToEntity()!)
        };
    }

    public static RecipeModel ToModel(this CreateRecipeRequest s)
    {
        return new RecipeModel
        {
            Title = s.Title,
            Author = s.Author,
            Description = s.Description,
            Storage = s.Storage,
            Ingredients = s.Ingredients,
            Directions = s.Directions,
            IngredientsHTML = s.IngredientsHTML,
            DirectionsHTML = s.DirectionsHTML,
            CookTime = s.CookTime,
            PrepTime = s.PrepTime,
            TotalTime = s.TotalTime,
            Servings = s.Servings,
            Yield = s.Yield,
            Nutrition = s.Nutrition,
            Category = s.Category,
            Tags = s.Tags ?? new List<TagModel>()
        };
    }

    public static RecipeModel ToModel(this UpdateRecipeRequest s)
    {
        return new RecipeModel
        {
            Title = s.Title,
            Author = s.Author,
            Description = s.Description,
            Storage = s.Storage,
            Ingredients = s.Ingredients,
            Directions = s.Directions,
            IngredientsHTML = s.IngredientsHTML,
            DirectionsHTML = s.DirectionsHTML,
            CookTime = s.CookTime,
            PrepTime = s.PrepTime,
            TotalTime = s.TotalTime,
            Servings = s.Servings,
            Yield = s.Yield,
            Nutrition = s.Nutrition,
            Category = s.Category,
            Tags = s.Tags ?? new List<TagModel>()
        };
    }

    /// <summary>
    /// Applies scalar fields from the request onto an existing recipe (in place, overwriting with nulls
    /// like the former Map(source, destination)). Documents, Category and Tags are left untouched.
    /// </summary>
    public static Recipe ApplyTo(this UpdateRecipeRequest s, Recipe d)
    {
        d.Title = s.Title;
        d.Author = s.Author;
        d.Description = s.Description;
        d.Storage = s.Storage;
        d.Ingredients = s.Ingredients;
        d.Directions = s.Directions;
        d.IngredientsHTML = s.IngredientsHTML;
        d.DirectionsHTML = s.DirectionsHTML;
        d.CookTime = s.CookTime;
        d.PrepTime = s.PrepTime;
        d.TotalTime = s.TotalTime;
        d.Servings = s.Servings;
        d.Yield = s.Yield;
        d.Nutrition = s.Nutrition;
        return d;
    }
}
