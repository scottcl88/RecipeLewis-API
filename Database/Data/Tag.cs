using System.ComponentModel.DataAnnotations.Schema;

namespace Database;
public class Tag : EntityDataUser
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int TagId { get; set; }
    public string Name { get; set; }
    public string Alias { get; set; }
}
