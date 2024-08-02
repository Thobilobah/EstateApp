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
    public class HouseInfoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public HouseInfoController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("Create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostHouseInfo([FromBody] HouseInfo houseInfo)
        {
            if (houseInfo == null)
            {
                return BadRequest();
            }

            // Fetch the last ID and increment it for the new entry
            var lastHouseInfo = await _context.HouseInfo.OrderByDescending(h => h.id).FirstOrDefaultAsync();
            int newId = (lastHouseInfo?.id ?? 0) + 1;

            houseInfo.id = newId;

            // Add the new house info to the context
            _context.HouseInfo.Add(houseInfo);
            // Save changes to the database
            await _context.SaveChangesAsync();

            // Return a CreatedAtAction response with the new house info
            return CreatedAtAction(nameof(GetHouseInfo), new { id = houseInfo.id }, houseInfo);
        }

        [HttpGet("{id:int}", Name = "GetHouseInfoById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetHouseInfo(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid ID");
            }

            var houseInfo = await _context.HouseInfo.FirstOrDefaultAsync(h => h.id == id);

            if (houseInfo == null)
            {
                return NotFound($"The house info with id {id} not found");
            }

            return Ok(houseInfo);
        }

        [HttpGet("All")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllHouseInfos()
        {
            var houseInfos = await _context.HouseInfo.ToListAsync();
            return Ok(houseInfos);
        }

        [HttpPut("Update")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateHouseInfo([FromBody] HouseInfo houseInfo)
        {
            if (houseInfo == null || houseInfo.id <= 0)
            {
                return BadRequest();
            }

            var existingHouseInfo = await _context.HouseInfo.FirstOrDefaultAsync(h => h.id == houseInfo.id);
            if (existingHouseInfo == null)
            {
                return NotFound();
            }

            existingHouseInfo.houseNo = houseInfo.houseNo;
            existingHouseInfo.streetName = houseInfo.streetName;
            existingHouseInfo.isRented = houseInfo.isRented;
            existingHouseInfo.noOfApartment = houseInfo.noOfApartment;
            existingHouseInfo.houseType = houseInfo.houseType;
            existingHouseInfo.rentPrice = houseInfo.rentPrice;

            _context.HouseInfo.Update(existingHouseInfo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id:int}/UpdatePartial")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateHouseInfoPartial(int id, [FromBody] JsonPatchDocument<HouseInfo> patchDocument)
        {
            if (patchDocument == null || id <= 0)
            {
                return BadRequest();
            }

            var existingHouseInfo = await _context.HouseInfo.FirstOrDefaultAsync(h => h.id == id);
            if (existingHouseInfo == null)
            {
                return NotFound();
            }

            var houseInfoDTO = new HouseInfo
            {
                id = existingHouseInfo.id,
                houseNo = existingHouseInfo.houseNo,
                streetName = existingHouseInfo.streetName,
                isRented = existingHouseInfo.isRented,
                noOfApartment = existingHouseInfo.noOfApartment,
                houseType = existingHouseInfo.houseType,
                rentPrice = existingHouseInfo.rentPrice
            };

            patchDocument.ApplyTo(houseInfoDTO, (error) =>
            {
                ModelState.AddModelError(string.Empty, $"{error.Operation.op} operation failed on path {error.Operation.path} due to error: {error.ErrorMessage}");
            });

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            existingHouseInfo.houseNo = houseInfoDTO.houseNo;
            existingHouseInfo.streetName = houseInfoDTO.streetName;
            existingHouseInfo.isRented = houseInfoDTO.isRented;
            existingHouseInfo.noOfApartment = houseInfoDTO.noOfApartment;
            existingHouseInfo.houseType = houseInfoDTO.houseType;
            existingHouseInfo.rentPrice = houseInfoDTO.rentPrice;

            _context.HouseInfo.Update(existingHouseInfo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("Delete/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteHouseInfo(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid ID");
            }

            var houseInfo = await _context.HouseInfo.FirstOrDefaultAsync(h => h.id == id);

            if (houseInfo == null)
            {
                return NotFound($"The house info with id {id} not found");
            }

            _context.HouseInfo.Remove(houseInfo);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
