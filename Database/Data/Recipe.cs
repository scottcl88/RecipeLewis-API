using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database
{
    public class Recipe : EntityDataUser
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RecipeId { get; set; }
        public string? Title { get; set; }
        public virtual User? Author { get; set; }
        public string? Description { get; set; }
        public string? Storage { get; set; }
        public string? Ingredients { get; set; }
        public string? Directions { get; set; }
        public TimeSpan? CookTime { get; set; }
        public TimeSpan? PrepTime { get; set; }
        public TimeSpan? TotalTime { get; set; }
        public int? Servings { get; set; }
        public string? Yield { get; set; }
        public string? Nutrition { get; set; }
        public virtual List<Category> Categories { get; set; } = new List<Category>();
        public virtual List<Tag> Tags { get; set; } = new List<Tag>();
        public virtual List<Document> Documents { get; set; } = new List<Document>();

    }
}
