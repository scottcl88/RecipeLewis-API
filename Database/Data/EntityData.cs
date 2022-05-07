using System;
using System.ComponentModel.DataAnnotations;

namespace Database
{
    public abstract class EntityData
    {
        [Required]
        [Display(Name = "Created Date")]
        public DateTime CreatedDateTime { get; set; }
        [Required]
        [Display(Name = "Created By")]
        public User CreatedBy { get; set; }

        [Display(Name = "Modified Date")]
        public DateTime? ModifiedDateTime { get; set; }
        [Required]
        [Display(Name = "Modified By")]
        public User? ModifiedBy { get; set; }

        [Display(Name = "Deleted Date")]
        public DateTime? DeletedDateTime { get; set; }
        [Required]
        [Display(Name = "Deleted By")]
        public User? DeletedBy { get; set; }
    }
}