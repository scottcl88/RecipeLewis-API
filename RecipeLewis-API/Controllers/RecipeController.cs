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

namespace RecipeLewis.Controllers
{
    [Authorize]
    [ApiController]
    [Route("/api/recipes")]
    [EnableCors("MyPolicy")]
    public class RecipeController : BaseController
    {
        private readonly IRecipeService _recipeService;
        private readonly LogService _logService;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public RecipeController(IMapper mapper, IConfiguration configuration, IRecipeService recipeService, LogService logService, IHostEnvironment environment) : base(environment)
        {
            _mapper = mapper;
            _configuration = configuration;
            _recipeService = recipeService;
            _logService = logService;
        }

        [HttpGet("search/{query}")]
        [SwaggerOperation(Summary = "Search for recipes")]
        public List<RecipeModel> Search(string query)
        {
            try
            {
                var foundRecipes = _recipeService.Search(query);
                return foundRecipes;
            }
            catch (Exception ex)
            {
                _logService.Error(ex, "Error on Search", UserId, new { query, UserId });
                throw;
            }
        }
        [HttpGet("get-all")]
        [SwaggerOperation(Summary = "Get all recipes")]
        public List<RecipeModel> GetAll(int id)
        {
            try
            {
                var foundRecipes = _recipeService.GetAll();
                return foundRecipes;
            }
            catch (Exception ex)
            {
                _logService.Error(ex, "Error on Get", UserId, new { id, UserId });
                throw;
            }
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get recipe by id")]
        public RecipeModel Get(int id)
        {
            try
            {
                var foundRecipe = _recipeService.Get(new RecipeId(id));
                return foundRecipe;
            }
            catch (Exception ex)
            {
                _logService.Error(ex, "Error on Get", UserId, new { id, UserId });
                throw;
            }
        }
        [HttpPost("create")]
        [SwaggerOperation(Summary = "Create recipe")]
        public ActionResult<RecipeModel> Create(CreateRecipeRequest request)
        {
            try
            {
                var recipe = _recipeService.Create(request, User);
                return Ok(recipe);
            }
            catch (Exception ex)
            {
                _logService.Error(ex, "Error on Get", UserId, request);
                throw;
            }
        }
        [HttpPut("update")]
        [SwaggerOperation(Summary = "Update recipe")]
        public ActionResult<RecipeModel> Update(UpdateRecipeRequest request)
        {
            try
            {
                var recipe = _recipeService.Update(request, User);
                return Ok(recipe);
            }
            catch (Exception ex)
            {
                _logService.Error(ex, "Error on Get", UserId, new { request });
                throw;
            }
        }

    }
}