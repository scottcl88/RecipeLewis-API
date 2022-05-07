using System.ComponentModel.DataAnnotations.Schema;

namespace Database
{
    public enum FeedbackType
    {
        General = 0,
        Suggestion = 1,
        Bug = 2
    } 
    public class Feedback : EntityData
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long FeedbackId { get; set; }
        public virtual User User { get; set; }
        public FeedbackType Type { get; set; }
        public string Comment { get; set; }
        public string AdditionalInfo { get; set; }
    }
}
