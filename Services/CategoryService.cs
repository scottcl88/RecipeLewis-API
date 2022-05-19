using AutoMapper;
using Database;
using RecipeLewis.Models;
using RecipeLewis.Models.Requests;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using RecipeLewis.Models.Results;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;

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
