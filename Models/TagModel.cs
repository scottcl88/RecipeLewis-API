using RecipeLewis.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipeLewis.Models;
public class TagModel : EntityDataUserModel
{
    public int TagId { get; set; }
    public string Name { get; set; }
}
