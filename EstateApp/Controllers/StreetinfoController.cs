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
    public class StreetInfoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StreetInfoController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("Create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostStreetInfo([FromBody] StreetInfo streetInfo)
        {
            if (streetInfo == null)
            {
                return BadRequest();
            }

            // Fetch the last ID and increment it for the new entry
            var lastStreetInfo = await _context.StreetInfo.OrderByDescending(s => s.id).FirstOrDefaultAsync();
            int newId = (lastStreetInfo?.id ?? 0) + 1;

            streetInfo.id = newId;

            // Add the new street info to the context
            _context.StreetInfo.Add(streetInfo);
            // Save changes to the database
            await _context.SaveChangesAsync();

            // Return a CreatedAtAction response with the new street info
            return CreatedAtAction(nameof(GetStreetInfo), new { id = streetInfo.id }, streetInfo);
        }

        [HttpGet("{id:int}", Name = "GetStreetInfoById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetStreetInfo(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid ID");
            }

            var streetInfo = await _context.StreetInfo.FirstOrDefaultAsync(s => s.id == id);

            if (streetInfo == null)
            {
                return NotFound($"The street info with id {id} not found");
            }

            return Ok(streetInfo);
        }

        [HttpGet("All")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllStreetInfos()
        {
            var streetInfos = await _context.StreetInfo.ToListAsync();
            return Ok(streetInfos);
        }

        [HttpPut("Update")]
        
        public async Task<IActionResult> UpdateStreetInfo([FromBody] StreetInfo streetInfo)
        {
            if (streetInfo == null || streetInfo.id <= 0)
            {
                return BadRequest();
            }

            var existingStreetInfo = await _context.StreetInfo.FirstOrDefaultAsync(s => s.id == streetInfo.id);
            if (existingStreetInfo == null)
            {
                return NotFound();
            }

            existingStreetInfo.streetName = streetInfo.streetName;

            _context.StreetInfo.Update(existingStreetInfo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id:int}/UpdatePartial")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateStreetInfoPartial(int id, [FromBody] JsonPatchDocument<StreetInfo> patchDocument)
        {
            if (patchDocument == null || id <= 0)
            {
                return BadRequest();
            }

            var existingStreetInfo = await _context.StreetInfo.FirstOrDefaultAsync(s => s.id == id);
            if (existingStreetInfo == null)
            {
                return NotFound();
            }

            var streetInfoDTO = new StreetInfo
            {
                id = existingStreetInfo.id,
                streetName = existingStreetInfo.streetName
            };

            patchDocument.ApplyTo(streetInfoDTO, (error) =>
            {
                ModelState.AddModelError(string.Empty, $"{error.Operation.op} operation failed on path {error.Operation.path} due to error: {error.ErrorMessage}");
            });

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            existingStreetInfo.streetName = streetInfoDTO.streetName;

            _context.StreetInfo.Update(existingStreetInfo);
            await _context.SaveChangesAsync();

            return NoContent();
        }



        [HttpDelete("Delete/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteStreetInfo(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid ID");
            }

            var streetInfo = await _context.StreetInfo.FirstOrDefaultAsync(s => s.id == id);

            if (streetInfo == null)
            {
                return NotFound($"The street info with id {id} not found");
            }

            _context.StreetInfo.Remove(streetInfo);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
