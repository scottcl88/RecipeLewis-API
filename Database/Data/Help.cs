using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database
{
    public class Help : EntityData
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long HelpID { get; set; }
        [ForeignKey("UserID")]
        public long UserID { get; set; }
        public virtual User User { get; set; }
        public bool SeenRatings { get; set; }
        public bool SeenHomeTutorial { get; set; }
        public bool SeenResultsTutorial { get; set; }
        public bool SeenInviteTutorial { get; set; }
        public bool SeenStartOutingTutorial { get; set; }
    }
}