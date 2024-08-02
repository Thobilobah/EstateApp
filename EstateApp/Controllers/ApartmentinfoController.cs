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
    public class ApartmentInfoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ApartmentInfoController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("Create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostApartmentInfo([FromBody] ApartmentInfo apartmentInfo)
        {
            if (apartmentInfo == null)
            {
                return BadRequest();
            }

            // Fetch the last ID and increment it for the new entry
            var lastApartmentInfo = await _context.ApartmentInfo.OrderByDescending(a => a.id).FirstOrDefaultAsync();
            int newId = (lastApartmentInfo?.id ?? 0) + 1;

            apartmentInfo.id = newId;

            // Add the new apartment info to the context
            _context.ApartmentInfo.Add(apartmentInfo);
            // Save changes to the database
            await _context.SaveChangesAsync();

            // Return a CreatedAtAction response with the new apartment info
            return CreatedAtAction(nameof(GetApartmentInfo), new { id = apartmentInfo.id }, apartmentInfo);
        }

        [HttpGet("{id:int}", Name = "GetApartmentInfoById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetApartmentInfo(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid ID");
            }

            var apartmentInfo = await _context.ApartmentInfo.FirstOrDefaultAsync(a => a.id == id);

            if (apartmentInfo == null)
            {
                return NotFound($"The apartment with id {id} not found");
            }

            return Ok(apartmentInfo);
        }

        [HttpGet("All")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllApartmentInfos()
        {
            var apartmentInfos = await _context.ApartmentInfo.ToListAsync();
            return Ok(apartmentInfos);
        }

        [HttpPut("Update")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateApartmentInfo([FromBody] ApartmentInfo apartmentInfo)
        {
            if (apartmentInfo == null || apartmentInfo.id <= 0)
            {
                return BadRequest();
            }

            var existingApartment = await _context.ApartmentInfo.FirstOrDefaultAsync(a => a.id == apartmentInfo.id);
            if (existingApartment == null)
            {
                return NotFound();
            }

            existingApartment.userId = apartmentInfo.userId;
            existingApartment.houseId = apartmentInfo.houseId;
            existingApartment.aName = apartmentInfo.aName;

            _context.ApartmentInfo.Update(existingApartment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id:int}/UpdatePartial")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateApartmentInfoPartial(int id, [FromBody] JsonPatchDocument<ApartmentInfo> patchDocument)
        {
            if (patchDocument == null || id <= 0)
            {
                return BadRequest();
            }

            var existingApartment = await _context.ApartmentInfo.FirstOrDefaultAsync(a => a.id == id);
            if (existingApartment == null)
            {
                return NotFound();
            }

            var apartmentDTO = new ApartmentInfo
            {
                id = existingApartment.id,
                userId = existingApartment.userId,
                houseId = existingApartment.houseId,
                aName = existingApartment.aName
            };

            patchDocument.ApplyTo(apartmentDTO, (error) => {
                ModelState.AddModelError(string.Empty, $"{error.Operation.op} operation failed on path {error.Operation.path} due to error: {error.ErrorMessage}");
            });

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            existingApartment.userId = apartmentDTO.userId;
            existingApartment.houseId = apartmentDTO.houseId;
            existingApartment.aName = apartmentDTO.aName;

            _context.ApartmentInfo.Update(existingApartment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("Delete/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteApartmentInfo(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid ID");
            }

            var apartmentInfo = await _context.ApartmentInfo.FirstOrDefaultAsync(a => a.id == id);

            if (apartmentInfo == null)
            {
                return NotFound($"The apartment with id {id} not found");
            }

            _context.ApartmentInfo.Remove(apartmentInfo);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
