using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database
{
    public enum HistoryRating
    {
        Unknown = 0,
        Like = 1,
        Dislike = 2
    }

    public class History : EntityData
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long HistoryID { get; set; }

        public virtual User User { get; set; }
        public virtual Place Place { get; set; }
        public DateTime? Visited { get; set; }

        [Obsolete("No longer used as a measure")]
        public HistoryRating Rating { get; set; }
        public int ResultsRank { get; set; }
        public int ResultsIndex { get; set; }
        [NotMapped]
        public PreferenceRating CurrentPreferenceRating { get; set; }//This is really just for the UI

        [NotMapped]
        public string VisitTimeString { get; set; }//This is really just for the UI

        [NotMapped]
        public string RatingColor { get; set; }//This is really just for the UI

        [NotMapped]
        public bool Selected { get; set; }//This is really just for the UI

        [NotMapped]
        public long? GroupId { get; set; }//This is really just for the UI

        [NotMapped]
        public bool IsGroup { get; set; }//This is really just for the UI

        [NotMapped]
        public DateTime? RsvpEndDateTime { get; set; }//This is really just for the UI

        [NotMapped]
        public bool VoteOpen { get; set; }//This is really just for the UI

        [NotMapped]
        public bool IsUserStarted { get; set; }//This is really just for the UI

        [NotMapped]
        public bool WaitingForUserStarted { get; set; }//This is really just for the UI

        [NotMapped]
        public bool AlreadyVoted { get; set; }//This is really just for the UI

        [NotMapped]
        public string GroupName { get; set; }//This is really just for the UI

        [NotMapped]
        public string MemberDescription { get; set; }//This is really just for the UI

        [NotMapped]
        public string RsvpStatusDescription { get; set; }//just for the UI

        [NotMapped]
        public RsvpStatus CurrentRsvpStatus { get; set; }//just for the UI
    }
}