using RecipeLewis.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipeLewis.Models;

public class CategoryModel : EntityDataUserModel
{
    public CategoryModel() { }
    public CategoryModel(string name)
    {
        Name = name;
    }
    public int CategoryId { get; set; }
    public string Name { get; set; }
}