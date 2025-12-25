using SmartFactory.Data;
using SmartFactory.Models;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace SmartFactory.API.Controllers
{
    [Authorize]
    [RoutePrefix("api/sensors")] 
    public class SensorsController : ApiController
    {
        private DbManager _db = new DbManager();

        [HttpGet]
        [Route("")] // Rota: GET api/sensors
        public IHttpActionResult Get()
        {
            try
            {
                var readings = _db.GetLatestReadings();
                return Ok(readings);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}