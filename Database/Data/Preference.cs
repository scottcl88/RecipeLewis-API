using System.ComponentModel.DataAnnotations.Schema;

namespace Database
{
    public enum PreferenceRating
    {
        Unknown = 0,
        Love = 1,
        Like = 2,
        Dislike = 3,
        Hate = 4
    }

    public class Preference : EntityData
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long PreferenceID { get; set; }

        public virtual User User { get; set; }
        public virtual Place Place { get; set; }
        public PreferenceRating Rating { get; set; }
    }
}