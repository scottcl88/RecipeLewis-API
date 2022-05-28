using AutoMapper;
using RecipeLewis.Models;
using RecipeLewis.Services;
using RecipeLewis.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RestSharp;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Threading.Tasks;
using RecipeLewis.Models.Results;
using Ganss.XSS;

namespace RecipeLewis.Controllers
{
    [Authorize]
    [ApiController]
    [Route("/api/recipes")]
    [EnableCors("MyPolicy")]
    public class RecipeController : BaseController
    {
        private readonly IRecipeService _recipeService;
        private readonly ICategoryService _categoryService;
        private readonly IDocumentService _documentService;
        private readonly LogService _logService;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public RecipeController(IMapper mapper, IConfiguration configuration, IDocumentService documentService, IRecipeService recipeService, ICategoryService categoryService, LogService logService, IHostEnvironment environment) : base(environment)
        {
            _mapper = mapper;
            _configuration = configuration;
            _recipeService = recipeService;
            _documentService = documentService;
            _logService = logService;
            _categoryService = categoryService;
        }

        [AllowAnonymous]
        [HttpGet("categories")]
        [SwaggerOperation(Summary = "Get all categories")]
        public List<CategoryModel> GetCategories()
        {
            try
            {
                var list = _categoryService.GetAll();
                return list;
            }
            catch (Exception ex)
            {
                _logService.Error(ex, "Error on Get", UserId, new { UserId });
                throw;
            }
        }

        [AllowAnonymous]
        [HttpGet("search/{query}")]
        [SwaggerOperation(Summary = "Search for recipes")]
        public List<RecipeModel> Search(string query)
        {
            try
            {
                var foundRecipes = _recipeService.Search(query);
                foundRecipes.ForEach(x => x.SanitizeHtml());
                return foundRecipes;
            }
            catch (Exception ex)
            {
                _logService.Error(ex, "Error on Search", UserId, new { query, UserId });
                throw;
            }
        }
        [AllowAnonymous]
        [HttpGet("get-all")]
        [SwaggerOperation(Summary = "Get all recipes")]
        public List<RecipeModel> GetAll(int id)
        {
            try
            {
                var foundRecipes = _recipeService.GetAll();
                foundRecipes.ForEach(x => x.SanitizeHtml());
                return foundRecipes;
            }
            catch (Exception ex)
            {
                _logService.Error(ex, "Error on Get", UserId, new { id, UserId });
                throw;
            }
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get recipe by id")]
        public RecipeModel Get(int id)
        {
            try
            {
                var foundRecipe = _recipeService.Get(new RecipeId(id));
                foundRecipe?.SanitizeHtml();
                return foundRecipe;
            }
            catch (Exception ex)
            {
                _logService.Error(ex, "Error on Get", UserId, new { id, UserId });
                throw;
            }
        }
        [Authorize(Role.Editor, Role.Admin)]
        [HttpPost("create")]
        [SwaggerOperation(Summary = "Create recipe")]
        public ActionResult<RecipeModel> Create(CreateRecipeRequest request)
        {
            try
            {
                request.SanitizeHtml();
                var recipe = _recipeService.Create(request, User);
                return Ok(recipe);
            }
            catch (Exception ex)
            {
                _logService.Error(ex, "Error on Get", UserId, request);
                throw;
            }
        }
        [Authorize(Role.Editor, Role.Admin)]
        [HttpPut("update")]
        [SwaggerOperation(Summary = "Update recipe")]
        public ActionResult<RecipeModel> Update(UpdateRecipeRequest request)
        {
            try
            {
                request.SanitizeHtml();
                var recipe = _recipeService.Update(request, User);
                var deletedDocuments = _documentService.DeleteDocuments(request.DocumentsToDelete, request.Id, User);
                if (!deletedDocuments)
                {
                    return StatusCode(500, "Failure deleting documents");
                }
                return Ok(recipe);
            }
            catch (Exception ex)
            {
                _logService.Error(ex, "Error on Get", UserId, new { request });
                throw;
            }
        }
        [Authorize(Role.Editor, Role.Admin)]
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Delete recipe by id")]
        public ActionResult<bool> Delete(int id)
        {
            try
            {
                _recipeService.Delete(new RecipeId(id), User);
                return Ok(true);
            }
            catch (Exception ex)
            {
                _logService.Error(ex, "Error on Delete", UserId, new { id, UserId });
                throw;
            }
        }

    }
}