using Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RecipeLewis.Models;
using RecipeLewis.Services;
using Document = Database.Document;

namespace RecipeLewis.Tests.Services;

[TestClass]
public class DocumentServiceTests
{
    private Mock<IRecipeService> _recipeServiceMock;
    private Mock<IUserService> _userServiceMock;
    private Mock<IMapper> _mapperMock;
    private ApplicationDbContext _dbContext;
    private DocumentService _documentService;

    [TestInitialize]
    public void Setup()
    {
        _recipeServiceMock = new Mock<IRecipeService>();
        _userServiceMock = new Mock<IUserService>();
        _mapperMock = new Mock<IMapper>();

        // Use in-memory database for testing
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "DefaultConnection")
        .Options;

        _dbContext = new ApplicationDbContext(options);

        _documentService = new DocumentService(
            _dbContext,
            _recipeServiceMock.Object,
            _userServiceMock.Object,
            _mapperMock.Object);
    }

    [TestMethod]
    public void GetAll_ShouldReturnDocumentModels()
    {
        // Arrange
        var recipeId = new RecipeId(1);
        var recipe = new Recipe { Documents = new List<Document> { new Document() } };
        _recipeServiceMock.Setup(x => x.GetRecipeById(recipeId)).Returns(recipe);
        _mapperMock.Setup(x => x.Map<List<DocumentModel>>(It.IsAny<List<Document>>())).Returns(new List<DocumentModel>());

        // Act
        var result = _documentService.GetAll(recipeId);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(List<DocumentModel>));
    }

    [TestMethod]
    public void Get_ExistingDocument_ShouldReturnDocumentModel()
    {
        // Arrange
        var recipeId = new RecipeId(1);
        var documentId = 1;
        var recipe = new Recipe { Documents = new List<Document> { new Document { DocumentId = documentId } } };
        _recipeServiceMock.Setup(x => x.GetRecipeById(recipeId)).Returns(recipe);
        _mapperMock.Setup(x => x.Map<DocumentModel>(It.IsAny<Document>())).Returns(new DocumentModel());

        // Act
        var result = _documentService.Get(recipeId, documentId);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(DocumentModel));
    }

    [TestMethod]
    [ExpectedException(typeof(AppException))]
    public void Get_NonexistentDocument_ShouldThrowAppException()
    {
        // Arrange
        var recipeId = new RecipeId(1);
        var documentId = 1;
        var recipe = new Recipe { Documents = new List<Document> { new Document { DocumentId = 2 } } };
        _recipeServiceMock.Setup(x => x.GetRecipeById(recipeId)).Returns(recipe);

        // Act & Assert
        _documentService.Get(recipeId, documentId);
    }

    [TestMethod]
    public void DeleteDocuments_ShouldMarkDocumentsAsDeleted()
    {
        // Arrange
        var recipeId = new RecipeId(1);
        var currentUser = new UserModel { UserId = new UserId(1) };
        var documentsToDelete = new List<DocumentModel> { new DocumentModel { DocumentId = 1 }, new DocumentModel { DocumentId = 2 } };
        var existingDocuments = new List<Document>
            {
                new Document { DocumentId = 1 },
                new Document { DocumentId = 2 }
            };
        var recipe = new Recipe { Documents = existingDocuments };
        _recipeServiceMock.Setup(x => x.GetRecipeById(recipeId)).Returns(recipe);
        _userServiceMock.Setup(x => x.GetDbUserById(currentUser.UserId)).Returns(new User());

        // Act
        var result = _documentService.DeleteDocuments(documentsToDelete, recipeId, currentUser);

        // Assert
        Assert.IsTrue(result);
        Assert.IsNotNull(existingDocuments[0].DeletedDateTime);
        Assert.IsNotNull(existingDocuments[1].DeletedDateTime);
    }

    [TestMethod]
    public void DeleteDocuments_EmptyList_ShouldReturnTrue()
    {
        // Arrange
        var recipeId = new RecipeId(1);
        var currentUser = new UserModel { UserId = new UserId(1) };
        var documentsToDelete = new List<DocumentModel>();
        var existingDocuments = new List<Document> { new Document { DocumentId = 1 } };
        var recipe = new Recipe { Documents = existingDocuments };
        _recipeServiceMock.Setup(x => x.GetRecipeById(recipeId)).Returns(recipe);
        _userServiceMock.Setup(x => x.GetDbUserById(currentUser.UserId)).Returns(new User());

        // Act
        var result = _documentService.DeleteDocuments(documentsToDelete, recipeId, currentUser);

        // Assert
        Assert.IsTrue(result);
        Assert.IsNull(existingDocuments[0].DeletedDateTime);
    }
}
