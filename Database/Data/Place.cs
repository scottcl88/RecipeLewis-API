using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database
{
    public class Place : EntityData
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long PlaceID { get; set; }

        public string YelpId { get; set; }
        public string Name { get; set; }
        public string YelpImageUrl { get; set; }
        public string YelpUrl { get; set; }
        public string YelpAlias { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal YelpRating { get; set; }

        public string PhoneNumber { get; set; }

        [NotMapped]
        public string FormattedPhoneNumber { get; set; }

        [Column(TypeName = "decimal(8, 6)")]
        public decimal Latitude { get; set; }

        [Column(TypeName = "decimal(9, 6)")]
        public decimal Longitude { get; set; }

        public bool PermanentlyClosed { get; set; }
        public string Price { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
    }

    public class PlaceCategory
    {
        public string Alias { get; set; }
        public string Title { get; set; }
    }
}