using System.ComponentModel.DataAnnotations;

namespace RecipeLewis.Models.Requests;

public class CreateRecipeRequest
{
    public string? Title { get; set; }
    public string? Author { get; set; }
    public string? Description { get; set; }
    public string? Storage { get; set; }
    public string? Ingredients { get; set; }
    public string? Directions { get; set; }
    public string? IngredientsHTML { get; set; }
    public string? DirectionsHTML { get; set; }
    public TimeSpan? CookTime { get; set; }
    public TimeSpan? PrepTime { get; set; }
    public TimeSpan? TotalTime { get; set; }
    public int? Servings { get; set; }
    public string? Yield { get; set; }
    public string? Nutrition { get; set; }
    public CategoryModel Category { get; set; }
    public List<TagModel> Tags { get; set; } = new List<TagModel>();
    public List<DocumentModel> Documents { get; set; } = new List<DocumentModel>();
}
public class UpdateRecipeRequest
{
    public RecipeId Id { get; set; }
    public string? Title { get; set; }
    public string? Author { get; set; }
    public string? Description { get; set; }
    public string? Storage { get; set; }
    public string? Ingredients { get; set; }
    public string? Directions { get; set; }
    public string? IngredientsHTML { get; set; }
    public string? DirectionsHTML { get; set; }
    public TimeSpan? CookTime { get; set; }
    public TimeSpan? PrepTime { get; set; }
    public TimeSpan? TotalTime { get; set; }
    public int? Servings { get; set; }
    public string? Yield { get; set; }
    public string? Nutrition { get; set; }
    public CategoryModel Category { get; set; }
    public List<TagModel> Tags { get; set; } = new List<TagModel>();
    public List<DocumentModel> Documents { get; set; } = new List<DocumentModel>();
}