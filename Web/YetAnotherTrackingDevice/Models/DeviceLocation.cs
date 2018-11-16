using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Spatial;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YetAnotherTrackingDevice.Models
{


    [Table("devices")]
    public class Device
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { set; get; }
    }

    [Table("trackpoints")]
    public class Trackpoint
    {
        [Key]
        public Guid Id {get; set; }
        public Device Device { set; get; }
        public DbGeography Location { set; get; }
        public DateTime UtcTime { set; get; }
        public double LocalTimeOffset { set; get; }

        public Nullable<double> Latitude { get { return Location.Latitude; } }
        public Nullable<double> Longitude { get { return Location.Longitude; } }
    }
}
