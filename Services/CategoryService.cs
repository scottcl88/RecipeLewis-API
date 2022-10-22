using AutoMapper;
using Database;
using Microsoft.Extensions.Options;
using RecipeLewis.Models;
using System.Data;

namespace RecipeLewis.Services;

public class CategoryService : ICategoryService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IJwtUtils _jwtUtils;
    private readonly AppSettings _appSettings;
    private readonly LogService _logService;
    private readonly IMapper _mapper;
    private readonly IEmailService _emailService;

    public CategoryService(ApplicationDbContext dbContext, LogService logService, IOptions<AppSettings> appSettings, IJwtUtils jwtUtils, IEmailService emailService, IMapper mapper)
    {
        _dbContext = dbContext;
        _logService = logService;
        _mapper = mapper;
        _jwtUtils = jwtUtils;
        _emailService = emailService;
        _appSettings = appSettings.Value;
    }

    public List<CategoryModel> GetAll()
    {
        var categories = _dbContext.Categories.Where(x => x.DeletedDateTime == null);
        var categoryModels = _mapper.Map<List<CategoryModel>>(categories.ToList());
        return categoryModels;
    }
}