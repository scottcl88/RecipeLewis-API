using RecipeLewis.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipeLewis.Models;

public class CategoryModel : EntityDataUserModel
{
    public int CategoryId { get; set; }
    public string Name { get; set; }
    public string Alias { get; set; }
}