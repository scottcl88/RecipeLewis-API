using System.ComponentModel.DataAnnotations.Schema;

namespace Database
{
    public class Location : EntityData
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long LocationID { get; set; }

        public string YelpId { get; set; }
        public string Name { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string County { get; set; }

        [Column(TypeName = "decimal(8, 6)")]
        public decimal Latitude { get; set; }

        [Column(TypeName = "decimal(9, 6)")]
        public decimal Longitude { get; set; }
    }
}