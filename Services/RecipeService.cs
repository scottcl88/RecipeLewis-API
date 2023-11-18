using AutoMapper;
using Database;
using RecipeLewis.Models;
using RecipeLewis.Models.Requests;
using System.Data;

namespace RecipeLewis.Services;

public class RecipeService : IRecipeService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly LogService _logService;
    private readonly IMapper _mapper;
    private readonly IUserService _userService;

    public RecipeService(ApplicationDbContext dbContext, LogService logService, IUserService userService, IMapper mapper)
    {
        _dbContext = dbContext;
        _logService = logService;
        _mapper = mapper;
        _userService = userService;
    }

    public RecipeModel Create(CreateRecipeRequest request, UserModel currentUser)
    {
        // map model to new recipe object
        var recipe = _mapper.Map<Recipe>(request);
        recipe.CreatedDateTime = DateTime.UtcNow;
        var user = _userService.GetDbUserById(currentUser.UserId);
        recipe.CreatedBy = user;

        var foundCategory = _dbContext.Categories.FirstOrDefault(x => x.CategoryId == request.Category.CategoryId);
        if (foundCategory == null)
        {
            foundCategory = _dbContext.Categories.First(x => x.CategoryId == 1);
        }
        recipe.Category = foundCategory;

        // save recipe
        _dbContext.Recipes.Add(recipe);
        _dbContext.SaveChanges();

        return _mapper.Map<RecipeModel>(recipe);
    }

    public RecipeModel Update(UpdateRecipeRequest request, UserModel currentUser)
    {
        var recipe = GetRecipeById(request.Id);
        if (recipe == null)
        {
            throw new AppException("Recipe not found");
        }
        var oldDocuments = recipe.Documents;
        recipe = _mapper.Map(request, recipe);
        recipe.Documents = oldDocuments;
        recipe.ModifiedDateTime = DateTime.UtcNow;
        var user = _userService.GetDbUserById(currentUser.UserId);
        recipe.ModifiedBy = user;
        recipe.Tags = GetDBTags(request.Tags, request.Id, user);
        var foundCategory = _dbContext.Categories.FirstOrDefault(x => x.CategoryId == request.Category.CategoryId);
        if (foundCategory == null)
        {
            foundCategory = _dbContext.Categories.First(x => x.CategoryId == 1);
        }
        recipe.Category = foundCategory;
        _dbContext.Recipes.Update(recipe);
        _dbContext.SaveChanges();

        return _mapper.Map<RecipeModel>(recipe);
    }

    private List<Tag> GetDBTags(List<TagModel> tagModels, RecipeId recipeId, User? user)
    {
        var recipe = GetRecipeById(recipeId);
        List<Tag> tags = recipe.Tags;
        for (int i = tags.Count - 1; i >= 0; i--)
        {
            var currentTag = tags[i];
            var newTag = tagModels.Find(x => x.TagId == currentTag.TagId && x.Name == currentTag.Name);
            if (newTag == null)
            {
                //tag deleted
                tags.Remove(currentTag);
            }
            else
            {
                //tag not changed
                tagModels.Remove(newTag);
            }
        }

        //Attempt to get/find tag from remaining model list, otherwise create new tag
        foreach (var tagModel in tagModels)
        {
            var dbTag = GetTag(tagModel.TagId);
            if (dbTag == null)
            {
                dbTag = FindTag(tagModel.Name);
                if (dbTag == null)
                {
                    dbTag = new Tag() { Name = tagModel.Name, CreatedBy = user };
                }
            }
            tags.Add(dbTag);
        }
        return tags;
    }

    private Tag? GetTag(int tagId)
    {
        return _dbContext.Tags.FirstOrDefault(x => x.TagId == tagId && x.RecipeId == null);
    }

    private Tag? FindTag(string name)
    {
        return _dbContext.Tags.FirstOrDefault(x => x.Name == name && x.RecipeId == null);
    }

    public void Delete(RecipeId id, UserModel currentUser)
    {
        var recipe = GetRecipeById(id);
        var user = _userService.GetDbUserById(currentUser.UserId);
        recipe.DeletedBy = user;
        recipe.DeletedDateTime = DateTime.UtcNow;
        _dbContext.SaveChanges();
    }

    public List<RecipeModel> Search(string query)
    {
        var recipes = _dbContext.Recipes.Where(x => (x.Title == null || x.Title.Contains(query, StringComparison.CurrentCultureIgnoreCase)) && x.DeletedDateTime == null);
        var recipeModels = _mapper.Map<List<RecipeModel>>(recipes.ToList());
        return recipeModels;
    }

    public List<RecipeModel> GetAll()
    {
        var recipes = _dbContext.Recipes.Where(x => x.DeletedDateTime == null).ToList();
        var recipeModels = _mapper.Map<List<RecipeModel>>(recipes);
        return recipeModels;
    }

    public RecipeModel? Get(RecipeId recipeId)
    {
        var recipe = GetRecipeById(recipeId);
        var recipeModel = _mapper.Map<RecipeModel>(recipe);
        return recipeModel;
    }

    public List<DocumentModel> GetDocuments(RecipeId recipeId)
    {
        var recipe = GetRecipeById(recipeId);
        recipe?.Documents.RemoveAll(x => x.DeletedDateTime != null);
        var documentModels = _mapper.Map<List<DocumentModel>>(recipe?.Documents);
        return documentModels;
    }

    public Recipe GetRecipeById(RecipeId recipeId)
    {
        var recipe = _dbContext.Recipes.FirstOrDefault(x => x.RecipeId == recipeId.Value && x.DeletedDateTime == null);
        if (recipe == null)
        {
            _logService.Error("getRecipeById failed - Unable to find recipe", null, new { recipeId });
            throw new AppException($"getRecipeById failed - Unable to find recipe by Id '{recipeId?.Value}'");
        }
        return recipe;
    }
}