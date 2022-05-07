using System.ComponentModel.DataAnnotations.Schema;

namespace Database
{
    public class Category : EntityData
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long CategoryId { get; set; }

        public virtual User User { get; set; }
        public virtual string CategoryAlias { get; set; }
    }
}
