using System;
using System.Web.Http;
using SmartFactory.Data;
using SmartFactory.Models;
using System.Collections.Generic;

namespace SmartFactory.API.Controllers
{
    /// <summary>
    /// Controlador para Gestão de Regras das Máquinas (CRUD RESTful)
    /// Apenas utilizadores com a Role 'Admin' têm acesso a estes endpoints.
    /// </summary>
    [Authorize(Roles = "Admin")]
    [RoutePrefix("api/rules")]
    public class RulesController : ApiController
    {
        private readonly DbManager _db = new DbManager();

        // 1. GET: api/rules
        // LISTAR todas as regras
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetAllRules()
        {
            try
            {
                var rules = _db.GetMachineRules();
                return Ok(rules);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // 2. POST: api/rules
        // CRIAR uma nova regra
        [HttpPost]
        [Route("")]
        public IHttpActionResult CreateRule([FromBody] MachineRule rule)
        {
            if (rule == null) return BadRequest("Dados da regra inválidos.");

            try
            {
                if (_db.CreateRule(rule))
                {
                    return Ok(new { message = "Regra criada com sucesso!" });
                }
                return BadRequest("Não foi possível inserir a regra na base de dados.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // 3. PUT: api/rules
        // ATUALIZAR uma regra existente
        [HttpPut]
        [Route("")]
        public IHttpActionResult UpdateRule([FromBody] MachineRule rule)
        {
            if (rule == null || rule.Id <= 0) return BadRequest("ID de regra inválido.");

            try
            {
                if (_db.UpdateRule(rule))
                {
                    return Ok(new { message = "Regra atualizada com sucesso!" });
                }
                return NotFound(); // Caso o ID não exista
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // 4. DELETE: api/rules/{id}
        // ELIMINAR uma regra
        [HttpDelete]
        [Route("{id:int}")]
        public IHttpActionResult DeleteRule(int id)
        {
            try
            {
                if (_db.DeleteRule(id))
                {
                    return Ok(new { message = $"Regra {id} eliminada." });
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}