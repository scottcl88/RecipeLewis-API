using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Database
{
    public class Group : EntityData
    {
        public Group()
        {
            Members = new List<GroupMember>();
            History = new List<GroupHistory>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long GroupID { get; set; }

        public virtual User Owner { get; set; }
        public virtual List<GroupMember> Members { get; set; }
        public virtual List<GroupHistory> History { get; set; }
        public bool DefaultSkipVoting { get; set; }
        public bool DefaultSkipRSVP { get; set; }
        public TimeSpan DefaultRSVPEndTimeSpan { get; set; }
        public TimeSpan DefaultVoteEndTimeSpan { get; set; }
        public string Name { get; set; }

        [NotMapped]
        public int AdditionalMembersDisplay { get; set; }//Just for the UI

        [NotMapped]
        public int MembersNotIncludingSelf { get; set; }//Just for the UI

        [NotMapped]
        public string MemberDescription { get; set; }//Just for the UI

        [NotMapped]
        public RsvpStatus CurrentRsvpStatus { get; set; }//just for the UI

        [NotMapped]
        public string RsvpStatusDescription { get; set; }//just for the UI

        [NotMapped]
        public bool CanVote { get; set; }//just for the UI

        [NotMapped]
        public bool UserVoted { get; set; }//just for the UI

        [NotMapped]
        public bool Started { get; set; }//just for the UI
    }

    public enum InvitationStatus
    {
        Unknown = 0,
        NotInvited = 1,
        Invited = 2,
        Accepted = 3,
        Declined = 4
    }

    public enum RsvpStatus
    {
        Unknown = 0,
        Accept = 1,
        ArriveLate = 2,
        Tentative = 3,
        Decline = 4
    }

    public class GroupMember : EntityData
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long GroupMemberID { get; set; }

        [JsonIgnore]
        public virtual Group Group { get; set; }

        public virtual User User { get; set; }
        public InvitationStatus InvitationStatus { get; set; }
    }

    public class GroupVote : EntityData
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long GroupVoteID { get; set; }

        [JsonIgnore]
        public virtual Group Group { get; set; }

        [JsonIgnore]
        public virtual GroupHistory GroupHistory { get; set; }

        [JsonIgnore]
        public virtual Place Place { get; set; }

        public virtual User User { get; set; }
    }

    public class GroupHistory : EntityData
    {
        public GroupHistory()
        {
            Attendees = new List<GroupHistoryAttendee>();
            Recommendations = new List<Place>();
            Votes = new List<GroupVote>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long GroupHistoryID { get; set; }

        public virtual List<GroupHistoryAttendee> Attendees { get; set; }

        [JsonIgnore]
        public virtual Group Group { get; set; }

        [JsonIgnore]
        public virtual Place SelectedPlace { get; set; }

        public virtual List<Place> Recommendations { get; set; }
        public virtual List<GroupVote> Votes { get; set; }
        public virtual User UserStarted { get; set; }
        public bool SkipVoting { get; set; }
        public bool SkipRSVP { get; set; }
        public DateTime VisitStartDateTime { get; set; }
        public DateTime VisitEndDateTime { get; set; }
        public DateTime? RSVPStartDateTime { get; set; }
        public DateTime? RSVPEndDateTime { get; set; }
        public DateTime? VotingStartDateTime { get; set; }
        public DateTime? VotingEndDateTime { get; set; }
        [Column(TypeName = "decimal(8, 6)")]
        public decimal Latitude { get; set; }

        [Column(TypeName = "decimal(9, 6)")]
        public decimal Longitude { get; set; }
        public string Location { get; set; }
    }

    public class GroupHistoryAttendee : EntityData
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long GroupHistoryAttendeeID { get; set; }

        [JsonIgnore]
        public virtual GroupHistory GroupHistory { get; set; }

        [JsonIgnore]
        public virtual Group Group { get; set; }

        public virtual User User { get; set; }
        public RsvpStatus RsvpStatus { get; set; }
    }
}