using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Spatial;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using YetAnotherTrackingDevice.Models;

namespace YetAnotherTrackingDevice.Controllers
{
    public class TrackpointsController : ApiController
    {
        private YetAnotherTrackingDeviceContext db = new YetAnotherTrackingDeviceContext();

        // GET: api/Trackpoints
        public IQueryable<Trackpoint> GetTrackpoints()
        {
            return db.Trackpoints;
        }

        // GET: api/Trackpoints/5
        [ResponseType(typeof(Trackpoint))]
        public IHttpActionResult GetTrackpoint(Guid id)
        {
            Trackpoint trackpoint = db.Trackpoints.Find(id);
            if (trackpoint == null)
            {
                return NotFound();
            }

            return Ok(trackpoint);
        }

        // PUT: api/Trackpoints/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutTrackpoint(Guid id, Trackpoint trackpoint)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != trackpoint.Id)
            {
                return BadRequest();
            }

            db.Entry(trackpoint).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TrackpointExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Trackpoints
        [ResponseType(typeof(Trackpoint))]
        public IHttpActionResult PostTrackpoint(Trackpoint trackpoint)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Trackpoints.Add(trackpoint);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (TrackpointExists(trackpoint.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = trackpoint.Id }, trackpoint);
        }


        // POST: api/Trackpoints
        [ResponseType(typeof(Trackpoint))]
        [HttpGet]
        [Route("AddTrackpoint")]
        public IHttpActionResult AddTrackpoint(string device, string lat, string lon)
        {
            var dev = db.Devices.Where(d => d.Name.Equals(device, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (dev == null)
                return BadRequest("Device not registered!");

            var trackpoint = new Trackpoint()
            {
                Id = Guid.NewGuid(),
                Device = dev,
                LocalTimeOffset = 0,
                UtcTime = DateTime.UtcNow,
                Location =
                DbGeography.FromText(String.Format(CultureInfo.InvariantCulture, "POINT({0} {1})", lon, lat))
            };
            db.Trackpoints.Add(trackpoint);
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (TrackpointExists(trackpoint.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Ok(trackpoint);
        }


        // [AllowCrossSiteJson]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet]
        [Route("GetAll")]
        public IHttpActionResult GetAll(String device)
        {
            var dev = db.Devices.Where(d => d.Name.Equals(device, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (dev == null)
                return BadRequest("Device not registered!");
          //  var tp = db.Trackpoints.ToList<Trackpoint>().OrderByDescending(t=>t.UtcTime).Select(t => t.Latitude.ToString()).ToList();

            var list = db.Trackpoints.Where(t => t.Device.Id == dev.Id).OrderByDescending(t => t.UtcTime);
            var locationDocuments = list.Select(ci => new
            ResultTrackpoint
            {
                id = ci.Id,
                location = ci.Location.Latitude.ToString() + "," + ci.Location.Longitude.ToString(),
                path = dev.Name,
                title = ci.UtcTime.ToString(),
                utcTime = ci.UtcTime
            }).ToList<ResultTrackpoint>();
            //var json = Json(locationDocuments);
            return Json(locationDocuments);
        }



        // DELETE: api/Trackpoints/5
        [ResponseType(typeof(Trackpoint))]
        public IHttpActionResult DeleteTrackpoint(Guid id)
        {
            Trackpoint trackpoint = db.Trackpoints.Find(id);
            if (trackpoint == null)
            {
                return NotFound();
            }

            db.Trackpoints.Remove(trackpoint);
            db.SaveChanges();

            return Ok(trackpoint);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TrackpointExists(Guid id)
        {
            return db.Trackpoints.Count(e => e.Id == id) > 0;
        }

        public class ResultTrackpoint
        {
            public Guid id
            {
                set; get;
            }
            public string location
            {
                set; get;
            }
            public string path
            {
                set; get;
            }
            public string title
            {
                set; get;
            }
            public DateTime utcTime
            {
                set; get;
            }
        }
    }
}