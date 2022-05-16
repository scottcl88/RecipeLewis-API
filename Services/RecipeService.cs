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

    public RecipeService(ApplicationDbContext dbContext, LogService logService, IMapper mapper)
    {
        _dbContext = dbContext;
        _logService = logService;
        _mapper = mapper;
    }
    public RecipeModel Create(CreateRecipeRequest request)
    {
        // map model to new recipe object
        var recipe = _mapper.Map<Recipe>(request);
        recipe.CreatedDateTime = DateTime.UtcNow;

        // save recipe
        _dbContext.Recipes.Add(recipe);
        _dbContext.SaveChanges();

        return _mapper.Map<RecipeModel>(recipe);
    }

    public RecipeModel Update(UpdateRecipeRequest request)
    {
        var recipe = getRecipeById(request.Id);

        _mapper.Map(request, recipe);
        recipe.ModifiedDateTime = DateTime.UtcNow;
        _dbContext.Recipes.Update(recipe);
        _dbContext.SaveChanges();

        return _mapper.Map<RecipeModel>(recipe);
    }

    public void Delete(RecipeId id)
    {
        var recipe = getRecipeById(id);
        _dbContext.Recipes.Remove(recipe);
        _dbContext.SaveChanges();
    }
    public List<RecipeModel> Search(string query)
    {
        var recipes = _dbContext.Recipes.Where(x => x.Title.ToLower().Contains(query) && x.DeletedDateTime == null);
        var recipeModels = _mapper.Map<List<RecipeModel>>(recipes.ToList());
        return recipeModels;
    }
    public List<RecipeModel> GetAll()
    {
        var recipes = _dbContext.Recipes.Where(x => x.DeletedDateTime == null);
        var recipeModels = _mapper.Map<List<RecipeModel>>(recipes.ToList());
        return recipeModels;
    }

    public RecipeModel? Get(RecipeId recipeId)
    {
        var recipe = getRecipeById(recipeId);
        var recipeModel = _mapper.Map<RecipeModel>(recipe);
        return recipeModel;
    }
    private Recipe? getRecipeById(RecipeId recipeId)
    {
        var user = _dbContext.Recipes.FirstOrDefault(x => x.RecipeId == recipeId.Value && x.DeletedDateTime == null);
        if (user == null)
        {
            _logService.Error("getRecipeById failed - Unable to find recipe", null, new { recipeId });
        }
        return user;
    }
}
