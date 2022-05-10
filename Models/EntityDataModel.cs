using System.ComponentModel.DataAnnotations;

namespace RecipeLewis.Models;

public abstract class EntityDataModel
{
    [Required]
    [Display(Name = "Created Date")]
    public DateTime CreatedDateTime { get; set; }

    [Display(Name = "Modified Date")]
    public DateTime? ModifiedDateTime { get; set; }

    [Display(Name = "Deleted Date")]
    public DateTime? DeletedDateTime { get; set; }
}
