using RecipeLewis.Services.Mapping;
using Database;
using RecipeLewis.Models;
using System.Data;

namespace RecipeLewis.Services;

public class DocumentService : IDocumentService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IUserService _userService;
    private readonly IRecipeService _recipeService;

    public DocumentService(ApplicationDbContext dbContext, IRecipeService recipeService, IUserService userService)
    {
        _dbContext = dbContext;
        _recipeService = recipeService;
        _userService = userService;
    }

    public List<DocumentModel> GetAll(RecipeId recipeId)
    {
        var recipe = _recipeService.GetRecipeById(recipeId);
        var documents = recipe?.Documents.ToList();
        var documentModels = documents.ToModels();
        return documentModels;
    }

    public DocumentModel Get(RecipeId recipeId, int documentId)
    {
        var recipe = _recipeService.GetRecipeById(recipeId);
        var document = recipe?.Documents.Find(x => x.DocumentId == documentId);
        if (document == null)
        {
            throw new AppException($"Document not found for RecipeId {recipeId.Value} and DocumentId {documentId}");
        }
        var documentModel = document.ToModel();
        return documentModel;
    }

    public bool AddDocuments(List<DocumentModel> documentsToAdd, RecipeId recipeId, UserModel currentUser)
    {
        var recipe = _recipeService.GetRecipeById(recipeId);
        if (recipe == null)
        {
            throw new AppException($"Recipe not found for RecipeId {recipeId.Value}");
        }
        var user = _userService.GetDbUserById(currentUser.UserId);
        var newDocuments = documentsToAdd.ToEntities();
        newDocuments.ForEach(x => x.DocumentId = 0);
        newDocuments.ForEach(x => x.CreatedBy = user);
        recipe.Documents.AddRange(newDocuments);
        _dbContext.SaveChanges();
        return true;
    }

    public bool DeleteDocuments(List<DocumentModel> documentsToDelete, RecipeId recipeId, UserModel currentUser)
    {
        if (!documentsToDelete.Any()) return true;
        var recipe = _recipeService.GetRecipeById(recipeId);
        var existingDocuments = recipe?.Documents.Where(x => x.DeletedDateTime == null).ToList();
        var user = _userService.GetDbUserById(currentUser.UserId);
        if (existingDocuments == null || user == null) return false;
        foreach (var document in existingDocuments)
        {
            var foundDocument = documentsToDelete.Find(x => x.DocumentId == document.DocumentId);
            if (foundDocument != null)
            {
                document.DeletedDateTime = DateTime.UtcNow;
                document.DeletedBy = user;
            }
        }
        _dbContext.SaveChanges();
        return true;
    }
}