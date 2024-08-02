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
    public class Temp_ViewInfoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public Temp_ViewInfoController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("Create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostTempViewInfo([FromBody] Temp_ViewInfo tempViewInfo)
        {
            if (tempViewInfo == null)
            {
                return BadRequest();
            }

            // Fetch the last ID and increment it for the new entry
            var lastTempViewInfo = await _context.Temp_View.OrderByDescending(t => t.id).FirstOrDefaultAsync();
            int newId = (lastTempViewInfo?.id ?? 0) + 1;

            tempViewInfo.id = newId;

            // Add the new temp view info to the context
            _context.Temp_View.Add(tempViewInfo);
            // Save changes to the database
            await _context.SaveChangesAsync();

            // Return a CreatedAtAction response with the new temp view info
            return CreatedAtAction(nameof(GetTempViewInfoById), new { id = tempViewInfo.id }, tempViewInfo);
        }

        [HttpGet("{id:int}", Name = "GetTempViewInfoById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTempViewInfoById(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid ID");
            }

            var tempViewInfo = await _context.Temp_View.FirstOrDefaultAsync(t => t.id == id);

            if (tempViewInfo == null)
            {
                return NotFound($"The temp view info with id {id} not found");
            }

            return Ok(tempViewInfo);
        }

        [HttpGet("All")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllTempViewInfos()
        {
            var tempViewInfos = await _context.Temp_View.ToListAsync();
            return Ok(tempViewInfos);
        }

        [HttpPut("Update")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateTempViewInfo([FromBody] Temp_ViewInfo tempViewInfo)
        {
            if (tempViewInfo == null || tempViewInfo.id <= 0)
            {
                return BadRequest();
            }

            var existingTempViewInfo = await _context.Temp_View.FirstOrDefaultAsync(t => t.id == tempViewInfo.id);
            if (existingTempViewInfo == null)
            {
                return NotFound();
            }

            existingTempViewInfo.roleName = tempViewInfo.roleName;
            existingTempViewInfo.fName = tempViewInfo.fName;
            existingTempViewInfo.lName = tempViewInfo.lName;
            existingTempViewInfo.houseNo = tempViewInfo.houseNo;
            existingTempViewInfo.streetName = tempViewInfo.streetName;
            existingTempViewInfo.aName = tempViewInfo.aName;

            _context.Temp_View.Update(existingTempViewInfo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id:int}/UpdatePartial")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateTempViewInfoPartial(int id, [FromBody] JsonPatchDocument<Temp_ViewInfo> patchDocument)
        {
            if (patchDocument == null || id <= 0)
            {
                return BadRequest();
            }

            var existingTempViewInfo = await _context.Temp_View.FirstOrDefaultAsync(t => t.id == id);
            if (existingTempViewInfo == null)
            {
                return NotFound();
            }

            var tempViewInfoDTO = new Temp_ViewInfo
            {
                id = existingTempViewInfo.id,
                roleName = existingTempViewInfo.roleName,
                fName = existingTempViewInfo.fName,
                lName = existingTempViewInfo.lName,
                houseNo = existingTempViewInfo.houseNo,
                streetName = existingTempViewInfo.streetName,
                aName = existingTempViewInfo.aName
            };

            patchDocument.ApplyTo(tempViewInfoDTO, (error) =>
            {
                ModelState.AddModelError(string.Empty, $"{error.Operation.op} operation failed on path {error.Operation.path} due to error: {error.ErrorMessage}");
            });

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            existingTempViewInfo.roleName = tempViewInfoDTO.roleName;
            existingTempViewInfo.fName = tempViewInfoDTO.fName;
            existingTempViewInfo.lName = tempViewInfoDTO.lName;
            existingTempViewInfo.houseNo = tempViewInfoDTO.houseNo;
            existingTempViewInfo.streetName = tempViewInfoDTO.streetName;
            existingTempViewInfo.aName = tempViewInfoDTO.aName;

            _context.Temp_View.Update(existingTempViewInfo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteTempViewInfo(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid ID");
            }

            var tempViewInfo = await _context.Temp_View.FirstOrDefaultAsync(t => t.id == id);

            if (tempViewInfo == null)
            {
                return NotFound($"The temp view info with id {id} not found");
            }

            _context.Temp_View.Remove(tempViewInfo);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
