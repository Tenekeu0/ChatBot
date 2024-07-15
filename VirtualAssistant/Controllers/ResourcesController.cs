using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using VirtualAssistant.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using VirtualAssistant.Data;

namespace VirtualAssistant
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResourcesController : ControllerBase
    {
        private readonly AppDbContext dbContext;
        public ResourcesController(AppDbContext context)
        {
            dbContext = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Resource>>> Get()
        {
            return await dbContext.Resources.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Resource>> Get(int id)
        {
            Resource resource = await dbContext.Resources.FindAsync(id);
            if (resource == null)
            {
                return NotFound();
            }
            return resource;
        }

        [HttpPost]
        public async Task<ActionResult<Resource>> Post([FromBody] Resource resource)
        {
            if (resource == null)
            {
                return BadRequest("La ressource ne peut pas être nulle");
            }

            try
            {
                dbContext.Resources.Add(resource);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Une erreur interne du serveur s'est produite");
            }

            return CreatedAtAction(nameof(Get), new { id = resource.Id }, resource);
        }


    }
}
