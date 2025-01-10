using Webapi.shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Cors;
namespace Webapi.Controllers {

    [ApiController]
    [Route("[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly Context _context;
        private readonly ILogger<RoleController> _logger;

        public RoleController(Context context, ILogger<RoleController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("Get")]
        public ActionResult Get()
        {
            var roles = _context.Roles.ToList();
            return roles.Count() == 0 ? NotFound() : Ok(JsonConvert.SerializeObject(roles));
        }

        [HttpGet("Get/{id}")]
        public ActionResult Get(int id)
        {
            if (id <= 0)
            {
                _logger.LogError($"Invalid id parameter: {id}");
                return BadRequest("Invalid id parameter.");
            }

            var role = _context.Roles.Find(id);
            return role == null ? NotFound() : Ok(JsonConvert.SerializeObject(role));
        }

        [HttpPost("Post")]
        public ActionResult Post([FromBody] Roles role)
        {
            _context.Roles.Add(role);
            _context.SaveChanges();
            return Ok(JsonConvert.SerializeObject(role));
        }
        
        [HttpPut("Put/{id}")]
        public ActionResult Put(int id, [FromBody] Roles role)
        {
            if (id != role.RoleID)
            {
                _logger.LogError($"Role ID mismatch: {id}");
                return BadRequest("Role ID mismatch.");
            }

            _context.Roles.Update(role);
            _context.SaveChanges();
            return Ok(JsonConvert.SerializeObject(role));
        }

        // Delete role
        [HttpDelete("Delete/{id}")]
        public ActionResult Delete(int id)
        {
            var role = _context.Roles.Find(id);
            if (role == null)
            {
                _logger.LogError($"Role not found: {id}");
                return NotFound();
            }

            _context.Roles.Remove(role);
            _context.SaveChanges();
            return Ok(JsonConvert.SerializeObject(role));
        }
    }
}
