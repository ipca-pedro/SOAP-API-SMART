using SmartFactory.Data;
using SmartFactory.Models;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace SmartFactory.API.Controllers
{
    public class SensorsController : ApiController
    {
        private readonly DbManager _db = new DbManager();

        // Este é o único método Get necessário agora
        [Authorize]
        [HttpGet]
        public IHttpActionResult Get()
        {
            try
            {
                var lista = _db.GetLatestReadings();

                if (lista == null)
                {
                    return NotFound();
                }

                return Ok(lista);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}