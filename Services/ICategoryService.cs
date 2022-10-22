using RecipeLewis.Models;

namespace RecipeLewis.Services
{
    public interface ICategoryService
    {
        public List<CategoryModel> GetAll();
    }
}