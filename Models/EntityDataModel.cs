using System.ComponentModel.DataAnnotations;

namespace RecipeLewis.Models;

public class EntityDataModel
{
    [Required]
    [Display(Name = "Created Date")]
    public DateTime CreatedDateTime { get; set; }

    [Display(Name = "Modified Date")]
    public DateTime? ModifiedDateTime { get; set; }

    [Display(Name = "Deleted Date")]
    public DateTime? DeletedDateTime { get; set; }
}

public class EntityDataUserModel : EntityDataModel
{
    [Display(Name = "Created By")]
    public UserModel? CreatedBy { get; set; }

    [Display(Name = "Modified By")]
    public UserModel? ModifiedBy { get; set; }

    [Display(Name = "Deleted By")]
    public UserModel? DeletedBy { get; set; }
}