using AutoMapper;
using Database;
using RecipeLewis.Models;
using System.Data;

namespace RecipeLewis.Services;

public class CategoryService : ICategoryService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public CategoryService(ApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public List<CategoryModel> GetAll()
    {
        var categories = _dbContext.Categories.Where(x => x.DeletedDateTime == null);
        var categoryModels = _mapper.Map<List<CategoryModel>>(categories.ToList());
        return categoryModels;
    }
}