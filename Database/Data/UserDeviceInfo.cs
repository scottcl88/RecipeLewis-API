using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database
{
    public class UserDeviceInfo : EntityData
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long UserDeviceInfoID { get; set; }
        [ForeignKey("UserID")]
        public long UserID { get; set; }
        public virtual User User { get; set; }
        public string Model { get; set; }
        public string Manufacturer { get; set; }
        public string OperatingSystem { get; set; }
        public string OperatingSystemVersion { get; set; }
        public string Platform { get; set; }
        public string WebViewVersion { get; set; }
        public bool IsVirtual { get; set; }
    }
}