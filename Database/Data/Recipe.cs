using System.ComponentModel.DataAnnotations.Schema;

namespace Database
{
    public class Recipe : EntityDataUser
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RecipeId { get; set; }

        public string? Title { get; set; }
        public virtual string? Author { get; set; }
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
        public virtual Category Category { get; set; }
        public virtual List<Tag> Tags { get; set; } = new List<Tag>();
        public virtual List<Document> Documents { get; set; } = new List<Document>();
    }
}