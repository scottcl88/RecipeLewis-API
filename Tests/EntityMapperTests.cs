using Database;
using RecipeLewis.Models;
using RecipeLewis.Models.Requests;
using RecipeLewis.Services.Mapping;
using Document = Database.Document;

namespace RecipeLewis.Tests.Mapping;

[TestClass]
public class EntityMapperTests
{
    private static readonly DateTime Created = new(2024, 1, 2, 3, 4, 5, DateTimeKind.Utc);

    private static User MakeUser() => new()
    {
        UserId = 7, UserGUID = Guid.NewGuid(), LastIPAddress = "1.2.3.4", LastLogin = Created, LastLogout = Created,
        TimeZone = "UTC", UtcOffset = -5, Email = "a@b.c", Name = "Ann", Role = Database.Role.Editor,
        RequestedAccess = true, PasswordHash = "secret", CreatedDateTime = Created,
        RefreshTokens = new List<RefreshToken> { new() { Id = 1, Token = "t", Expires = Created, Created = Created } }
    };

    [TestMethod]
    public void User_ToModel_MapsFieldsAndNestedTokens()
    {
        var u = MakeUser();
        var m = u.ToModel()!;
        Assert.AreEqual(7, m.UserId.Value);
        Assert.AreEqual(u.UserGUID, m.UserGUID);
        Assert.AreEqual(RecipeLewis.Models.Role.Editor, m.Role);
        Assert.AreEqual("Ann", m.Name);
        Assert.AreEqual(Created, m.CreatedDateTime);
        Assert.IsTrue(m.RequestedAccess);
        Assert.AreEqual("t", m.RefreshTokens![0].Token);
    }

    [TestMethod]
    public void Null_Sources_MapToNull_And_NullCollectionsToEmpty()
    {
        Assert.IsNull(((User?)null).ToModel());
        Assert.IsNull(((Recipe?)null).ToModel());
        Assert.IsNull(((DocumentModel?)null).ToEntity());
        Assert.AreEqual(0, ((List<Document>?)null).ToModels().Count);
        Assert.AreEqual(0, ((List<Recipe>?)null).ToModels().Count);
        var m = new User { Email = "x", Name = "n" }.ToModel()!;
        Assert.AreEqual(0, m.RefreshTokens!.Count);
    }

    [TestMethod]
    public void UserModel_ToEntity_MapsFields_AndDoesNotSetPasswordOrTokens()
    {
        var m = MakeUser().ToModel()!;
        var e = m.ToEntity()!;
        Assert.AreEqual(7, e.UserId);
        Assert.AreEqual(Database.Role.Editor, e.Role);
        Assert.AreEqual("a@b.c", e.Email);
        Assert.IsNull(e.PasswordHash);
        Assert.IsNull(e.RefreshTokens);
    }

    [TestMethod]
    public void RegisterRequest_ToEntity_MapsNameEmailOnly()
    {
        var u = new RegisterRequest { Name = "N", Email = "e@x.y", Password = "pw1234", ConfirmPassword = "pw1234" }.ToEntity();
        Assert.AreEqual("N", u.Name);
        Assert.AreEqual("e@x.y", u.Email);
        Assert.IsNull(u.PasswordHash);
    }

    [TestMethod]
    public void RefreshToken_ToModel_MapsFields()
    {
        var t = new RefreshToken { Id = 3, Token = "tok", Expires = Created, Created = Created, CreatedByIp = "ip", Revoked = Created, ReasonRevoked = "r" };
        var m = t.ToModel()!;
        Assert.AreEqual(3, m.Id);
        Assert.AreEqual("tok", m.Token);
        Assert.AreEqual("ip", m.CreatedByIp);
        Assert.AreEqual("r", m.ReasonRevoked);
        Assert.IsTrue(m.IsRevoked);
    }

    [TestMethod]
    public void Category_And_Tag_RoundTrip()
    {
        var c = new Category { CategoryId = 4, Name = "Dinner", CreatedBy = MakeUser() };
        var cm = c.ToModel()!;
        Assert.AreEqual(4, cm.CategoryId);
        Assert.AreEqual(7, cm.CreatedBy!.UserId.Value);
        Assert.AreEqual("Dinner", cm.ToEntity()!.Name);

        var tm = new Tag { TagId = 2, Name = "quick", RecipeId = 9 }.ToModel()!;
        Assert.AreEqual(2, tm.TagId);
        var te = tm.ToEntity()!;
        Assert.AreEqual("quick", te.Name);
        Assert.IsNull(te.RecipeId);
    }

    [TestMethod]
    public void Document_RoundTrip_MapsBytesAndKey()
    {
        var key = Guid.NewGuid();
        var d = new Document { DocumentId = 5, Name = "n", FileName = "f.png", ContentType = "image/png", ByteSize = 3, Bytes = new byte[] { 1, 2, 3 }, DocumentKey = key };
        var m = d.ToModel()!;
        Assert.AreEqual(5, m.DocumentId);
        CollectionAssert.AreEqual(new byte[] { 1, 2, 3 }, m.Bytes);
        var back = new List<DocumentModel> { m }.ToEntities();
        Assert.AreEqual(1, back.Count);
        Assert.AreEqual(key, back[0].DocumentKey);
        Assert.AreEqual("f.png", back[0].FileName);
    }

    [TestMethod]
    public void Recipe_ToModel_MapsScalarsCategoryTags_IgnoresDocuments()
    {
        var r = new Recipe
        {
            RecipeId = 11, Title = "T", Servings = 4, CookTime = TimeSpan.FromMinutes(5), IngredientsHTML = "<b>x</b>",
            Category = new Category { CategoryId = 2, Name = "Breakfast" },
            Tags = new List<Tag> { new() { TagId = 1, Name = "a" }, new() { TagId = 2, Name = "b" } },
            Documents = new List<Document> { new() { DocumentId = 1 } },
            CreatedBy = MakeUser(), CreatedDateTime = Created
        };
        var m = r.ToModel()!;
        Assert.AreEqual(11, m.RecipeId);
        Assert.AreEqual("T", m.Title);
        Assert.AreEqual(4, m.Servings);
        Assert.AreEqual(TimeSpan.FromMinutes(5), m.CookTime);
        Assert.AreEqual("Breakfast", m.Category.Name);
        Assert.AreEqual(2, m.Tags.Count);
        Assert.AreEqual(0, m.Documents.Count);
        Assert.AreEqual(7, m.CreatedBy!.UserId.Value);
        Assert.AreEqual(Created, m.CreatedDateTime);
    }

    [TestMethod]
    public void CreateRecipeRequest_ToEntity_MapsFieldsCategoryAndTags()
    {
        var req = new CreateRecipeRequest
        {
            Title = "T", Author = "A", Servings = 2,
            Category = new CategoryModel("Lunch") { CategoryId = 3 },
            Tags = new List<TagModel> { new() { TagId = 1, Name = "x" } }
        };
        var r = req.ToEntity();
        Assert.AreEqual("T", r.Title);
        Assert.AreEqual(2, r.Servings);
        Assert.AreEqual(3, r.Category.CategoryId);
        Assert.AreEqual("x", r.Tags[0].Name);
        Assert.AreEqual(0, r.Documents.Count);
        Assert.AreEqual("T", req.ToModel().Title);
    }

    [TestMethod]
    public void UpdateRecipeRequest_ApplyTo_OverwritesScalars_KeepsDocumentsCategoryTags()
    {
        var existing = new Recipe
        {
            RecipeId = 9, Title = "old", Description = "keep?",
            Category = new Category { CategoryId = 1, Name = "Any" },
            Tags = new List<Tag> { new() { TagId = 1, Name = "t" } },
            Documents = new List<Document> { new() { DocumentId = 1 } }
        };
        var docs = existing.Documents;
        var req = new UpdateRecipeRequest { Id = new RecipeId(9), Title = "new", Category = new CategoryModel("Dinner") { CategoryId = 4 } };
        var result = req.ApplyTo(existing);
        Assert.AreSame(existing, result);
        Assert.AreEqual(9, result.RecipeId);
        Assert.AreEqual("new", result.Title);
        Assert.IsNull(result.Description); // null overwrites existing values
        Assert.AreEqual(1, result.Category.CategoryId);
        Assert.AreEqual(1, result.Tags.Count);
        Assert.AreSame(docs, result.Documents);
    }
}

[TestClass]
public class CreateUserRequestMapperTests
{
    [TestMethod]
    public void CreateUserRequest_ToEntity_MapsNameEmailRole()
    {
        var u = new CreateUserRequest { FirstName = "A", LastName = "B", Email = "a@b.c", Role = "Admin", Password = "pw1234", ConfirmPassword = "pw1234" }.ToEntity();
        Assert.AreEqual("A B", u.Name);
        Assert.AreEqual("a@b.c", u.Email);
        Assert.AreEqual(Database.Role.Admin, u.Role);
        Assert.IsNull(u.PasswordHash);
        Assert.AreEqual(Database.Role.User, new CreateUserRequest { FirstName = "A", LastName = "B", Email = "e", Role = "bogus" }.ToEntity().Role);
    }
}
