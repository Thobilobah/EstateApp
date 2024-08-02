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
    public class FeeInfoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FeeInfoController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("Create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostFeeInfo([FromBody] FeeInfo feeInfo)
        {
            if (feeInfo == null)
            {
                return BadRequest();
            }

            // Fetch the last ID and increment it for the new entry
            var lastFeeInfo = await _context.FeeInfo.OrderByDescending(f => f.id).FirstOrDefaultAsync();
            int newId = (lastFeeInfo?.id ?? 0) + 1;

            feeInfo.id = newId;

            // Add the new fee info to the context
            _context.FeeInfo.Add(feeInfo);
            // Save changes to the database
            await _context.SaveChangesAsync();

            // Return a CreatedAtAction response with the new fee info
            return CreatedAtAction(nameof(GetFeeInfo), new { id = feeInfo.id }, feeInfo);
        }

        [HttpGet("{id:int}", Name = "GetFeeInfoById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetFeeInfo(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid ID");
            }

            var feeInfo = await _context.FeeInfo.FirstOrDefaultAsync(f => f.id == id);

            if (feeInfo == null)
            {
                return NotFound($"The fee info with id {id} not found");
            }

            return Ok(feeInfo);
        }

        [HttpGet("All")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllFeeInfos()
        {
            var feeInfos = await _context.FeeInfo.ToListAsync();
            return Ok(feeInfos);
        }

        [HttpPut("Update")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateFeeInfo([FromBody] FeeInfo feeInfo)
        {
            if (feeInfo == null || feeInfo.id <= 0)
            {
                return BadRequest();
            }

            var existingFeeInfo = await _context.FeeInfo.FirstOrDefaultAsync(f => f.id == feeInfo.id);
            if (existingFeeInfo == null)
            {
                return NotFound();
            }

            existingFeeInfo.feeName = feeInfo.feeName;
            existingFeeInfo.feeAmount = feeInfo.feeAmount;

            _context.FeeInfo.Update(existingFeeInfo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id:int}/UpdatePartial")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateFeeInfoPartial(int id, [FromBody] JsonPatchDocument<FeeInfo> patchDocument)
        {
            if (patchDocument == null || id <= 0)
            {
                return BadRequest();
            }

            var existingFeeInfo = await _context.FeeInfo.FirstOrDefaultAsync(f => f.id == id);
            if (existingFeeInfo == null)
            {
                return NotFound();
            }

            var feeInfoDTO = new FeeInfo
            {
                id = existingFeeInfo.id,
                feeName = existingFeeInfo.feeName,
                feeAmount = existingFeeInfo.feeAmount
            };

            patchDocument.ApplyTo(feeInfoDTO, (error) =>
            {
                ModelState.AddModelError(string.Empty, $"{error.Operation.op} operation failed on path {error.Operation.path} due to error: {error.ErrorMessage}");
            });

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            existingFeeInfo.feeName = feeInfoDTO.feeName;
            existingFeeInfo.feeAmount = feeInfoDTO.feeAmount;

            _context.FeeInfo.Update(existingFeeInfo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("Delete/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteFeeInfo(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid ID");
            }

            var feeInfo = await _context.FeeInfo.FirstOrDefaultAsync(f => f.id == id);

            if (feeInfo == null)
            {
                return NotFound($"The fee info with id {id} not found");
            }

            _context.FeeInfo.Remove(feeInfo);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
