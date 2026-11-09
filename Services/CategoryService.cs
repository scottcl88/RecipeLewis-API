using RecipeLewis.Services.Mapping;
using Database;
using RecipeLewis.Models;
using System.Data;

namespace RecipeLewis.Services;

public class CategoryService : ICategoryService
{
    private readonly ApplicationDbContext _dbContext;

    public CategoryService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public List<CategoryModel> GetAll()
    {
        var categories = _dbContext.Categories.Where(x => x.DeletedDateTime == null);
        var categoryModels = categories.ToList().ToModels();
        return categoryModels;
    }
}