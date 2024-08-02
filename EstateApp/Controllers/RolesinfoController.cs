using EstateApp.Data;
using EstateApp.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace EstateApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesInfoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RolesInfoController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("Create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostRolesInfo([FromBody] RolesInfo rolesInfo)
        {
            if (rolesInfo == null)
            {
                return BadRequest();
            }

            // Fetch the last ID and increment it for the new entry
            var lastRolesInfo = await _context.RolesInfo.OrderByDescending(r => r.id).FirstOrDefaultAsync();
            int newId = (lastRolesInfo?.id ?? 0) + 1;

            rolesInfo.id = newId;

            // Add the new roles info to the context
            _context.RolesInfo.Add(rolesInfo);
            // Save changes to the database
            await _context.SaveChangesAsync();

            // Return a CreatedAtAction response with the new roles info
            return CreatedAtAction(nameof(GetRolesInfo), new { id = rolesInfo.id }, rolesInfo);
        }

        [HttpGet("{id:int}", Name = "GetRolesInfoById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetRolesInfo(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid ID");
            }

            var rolesInfo = await _context.RolesInfo.FirstOrDefaultAsync(r => r.id == id);

            if (rolesInfo == null)
            {
                return NotFound($"The roles info with id {id} not found");
            }

            return Ok(rolesInfo);
        }

        [HttpGet("All")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllRolesInfos()
        {
            var rolesInfos = await _context.RolesInfo.ToListAsync();
            return Ok(rolesInfos);
        }

        [HttpPut("Update")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateRolesInfo([FromBody] RolesInfo rolesInfo)
        {
            if (rolesInfo == null || rolesInfo.id <= 0)
            {
                return BadRequest();
            }

            var existingRolesInfo = await _context.RolesInfo.FirstOrDefaultAsync(r => r.id == rolesInfo.id);
            if (existingRolesInfo == null)
            {
                return NotFound();
            }

            existingRolesInfo.name = rolesInfo.name;

            _context.RolesInfo.Update(existingRolesInfo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id:int}/UpdatePartial")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateRolesInfoPartial(int id, [FromBody] JsonPatchDocument<RolesInfo> patchDocument)
        {
            if (patchDocument == null || id <= 0)
            {
                return BadRequest();
            }

            var existingRolesInfo = await _context.RolesInfo.FirstOrDefaultAsync(r => r.id == id);
            if (existingRolesInfo == null)
            {
                return NotFound();
            }

            var rolesInfoDTO = new RolesInfo
            {
                id = existingRolesInfo.id,
                name = existingRolesInfo.name
            };

            patchDocument.ApplyTo(rolesInfoDTO, (error) =>
            {
                ModelState.AddModelError(string.Empty, $"{error.Operation.op} operation failed on path {error.Operation.path} due to error: {error.ErrorMessage}");
            });

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            existingRolesInfo.name = rolesInfoDTO.name;

            _context.RolesInfo.Update(existingRolesInfo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("Delete/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteRolesInfo(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid ID");
            }

            var rolesInfo = await _context.RolesInfo.FirstOrDefaultAsync(r => r.id == id);

            if (rolesInfo == null)
            {
                return NotFound($"The roles info with id {id} not found");
            }

            _context.RolesInfo.Remove(rolesInfo);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
