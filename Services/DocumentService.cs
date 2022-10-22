using AutoMapper;
using Database;
using Microsoft.Extensions.Options;
using RecipeLewis.Models;
using System.Data;

namespace RecipeLewis.Services;

public class DocumentService : IDocumentService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly AppSettings _appSettings;
    private readonly LogService _logService;
    private readonly IMapper _mapper;
    private readonly IUserService _userService;
    private readonly IRecipeService _recipeService;

    public DocumentService(ApplicationDbContext dbContext, LogService logService, IOptions<AppSettings> appSettings, IRecipeService recipeService, IUserService userService, IMapper mapper)
    {
        _dbContext = dbContext;
        _logService = logService;
        _mapper = mapper;
        _recipeService = recipeService;
        _userService = userService;
        _appSettings = appSettings.Value;
    }

    public List<DocumentModel> GetAll(RecipeId recipeId)
    {
        var recipe = _recipeService.GetRecipeById(recipeId);
        var documents = recipe.Documents.ToList();
        var documentModels = _mapper.Map<List<DocumentModel>>(documents);
        return documentModels;
    }

    public DocumentModel Get(RecipeId recipeId, int documentId)
    {
        var recipe = _recipeService.GetRecipeById(recipeId);
        var document = recipe.Documents.FirstOrDefault(x => x.DocumentId == documentId);
        if (document == null)
        {
            throw new AppException($"Document not found for RecipeId {recipeId.Value} and DocumentId {documentId}");
        }
        var documentModel = _mapper.Map<DocumentModel>(document);
        return documentModel;
    }

    public bool AddDocuments(List<DocumentModel> documentsToAdd, RecipeId recipeId, UserModel currentUser)
    {
        var recipe = _recipeService.GetRecipeById(recipeId);
        var existingDocuments = recipe.Documents.Where(x => x.DeletedDateTime == null).ToList();
        var user = _userService.GetDbUserById(currentUser.UserId);
        var newDocuments = _mapper.Map<List<Document>>(documentsToAdd);
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
        var existingDocuments = recipe.Documents.Where(x => x.DeletedDateTime == null).ToList();
        var user = _userService.GetDbUserById(currentUser.UserId);
        foreach (var document in existingDocuments)
        {
            var foundDocument = documentsToDelete.FirstOrDefault(x => x.DocumentId == document.DocumentId);
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