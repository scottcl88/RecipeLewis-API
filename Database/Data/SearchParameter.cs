using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database
{
    public class SearchParameter : EntityData
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long SearchParameterID { get; set; }

        public virtual User User { get; set; }
        [Obsolete("Is this needed?")]
        public string Query { get; set; }
        [Column(TypeName = "decimal(8, 6)")]
        public decimal Latitude { get; set; }

        [Column(TypeName = "decimal(9, 6)")]
        public decimal Longitude { get; set; }
        public string Location { get; set; }
        public int Radius { get; set; } = 40000;
        public bool OpenNow { get; set; }
        public int OpenAt { get; set; }
        public string Price { get; set; }
        public string Categories { get; set; } = "restaurants, All";
        public int Limit { get; set; } = 1;
        public int Offset { get; set; } = 0;
        public bool OnlyNew { get; set; }

        [JsonIgnore]
        public virtual List<SearchParameterRating> OnlyPreference { get; set; }
    }

    public class SearchParameterRating : EntityData
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long SearchParameterRatingID { get; set; }

        [JsonIgnore]
        public virtual SearchParameter SearchParameter { get; set; }

        public virtual User UserAdded { get; set; }
        public PreferenceRating PreferenceRating { get; set; }
    }
}