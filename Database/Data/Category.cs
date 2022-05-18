using System.ComponentModel.DataAnnotations.Schema;

namespace Database
{
    public class Category : EntityDataUser
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CategoryId { get; set; }
        public string Name { get; set; }
    }
}
