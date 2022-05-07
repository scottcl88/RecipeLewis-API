using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database
{
    public class Recipe : EntityData
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long RecipeId { get; set; }
        public string? Name { get; set; }
    }
}
