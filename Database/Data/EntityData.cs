using System;
using System.ComponentModel.DataAnnotations;

namespace Database
{
    public abstract class EntityData
    {
        [Required]
        [Display(Name = "Created Date")]
        public DateTime CreatedDateTime { get; set; }

        [Display(Name = "Modified Date")]
        public DateTime? ModifiedDateTime { get; set; }

        [Display(Name = "Deleted Date")]
        public DateTime? DeletedDateTime { get; set; }
    }

    public abstract class EntityDataUser : EntityData
    {
        [Display(Name = "Created By")]
        public virtual User? CreatedBy { get; set; }
        [Display(Name = "Modified By")]
        public virtual User? ModifiedBy { get; set; }
        [Display(Name = "Deleted By")]
        public virtual User? DeletedBy { get; set; }
    }
}