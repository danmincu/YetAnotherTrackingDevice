using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace YetAnotherTrackingDevice.Models
{
    public class YetAnotherTrackingDeviceContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
    
        public YetAnotherTrackingDeviceContext() : base("name=YetAnotherTrackingDeviceContext")
        {
        }

        public System.Data.Entity.DbSet<YetAnotherTrackingDevice.Models.Trackpoint> Trackpoints { get; set; }

        public System.Data.Entity.DbSet<YetAnotherTrackingDevice.Models.Device> Devices { get; set; }
    }
}
