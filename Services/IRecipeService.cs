using Database;
using RecipeLewis.Models.Results;
using RecipeLewis.Models;
using RecipeLewis.Models.Requests;

namespace RecipeLewis.Services
{
    public interface IRecipeService
    {
        public List<RecipeModel> Search(string query);
        public RecipeModel? Get(RecipeId userId);
        public List<RecipeModel> GetAll();
        public RecipeModel Create(CreateRecipeRequest request, UserModel currentUser);
        public RecipeModel Update(UpdateRecipeRequest request, UserModel currentUser);
        public void Delete(RecipeId id, UserModel currentUser);
        public Recipe? GetRecipeById(RecipeId recipeId);
    }
}
