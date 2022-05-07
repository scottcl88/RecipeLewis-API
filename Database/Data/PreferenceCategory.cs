using System.ComponentModel.DataAnnotations.Schema;

namespace Database
{
    public class CategoryPreference : EntityData
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long CategoryPreferenceID { get; set; }

        public virtual User User { get; set; }
        public virtual string CategoryAlias { get; set; }

        [NotMapped]
        public string Title { get; set; }//Just for UI

        [NotMapped]
        public bool Selected { get; set; }//Just for UI

        [NotMapped]
        public string Class { get; set; }//Just for UI

        [NotMapped]
        public string Image { get; set; }//Just for UI
    }
}