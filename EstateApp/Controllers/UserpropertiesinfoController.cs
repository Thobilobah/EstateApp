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
    public class UserPropertiesInfoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserPropertiesInfoController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("Create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostUserPropertiesInfo([FromBody] UserPropertiesInfo userPropertiesInfo)
        {
            if (userPropertiesInfo == null)
            {
                return BadRequest();
            }

            // Fetch the last ID and increment it for the new entry
            var lastUserProperty = await _context.UserPropertiesInfo.OrderByDescending(u => u.id).FirstOrDefaultAsync();
            int newId = (lastUserProperty?.id ?? 0) + 1;

            userPropertiesInfo.id = newId;

            // Add the new user properties info to the context
            _context.UserPropertiesInfo.Add(userPropertiesInfo);
            // Save changes to the database
            await _context.SaveChangesAsync();

            // Return a CreatedAtAction response with the new user properties info
            return CreatedAtAction(nameof(GetUserPropertiesInfoById), new { id = userPropertiesInfo.id }, userPropertiesInfo);
        }

        [HttpGet("{id:int}", Name = "GetUserPropertiesInfoById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserPropertiesInfoById(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid ID");
            }

            var userPropertiesInfo = await _context.UserPropertiesInfo.FirstOrDefaultAsync(u => u.id == id);

            if (userPropertiesInfo == null)
            {
                return NotFound($"The user properties info with id {id} not found");
            }

            return Ok(userPropertiesInfo);
        }

        [HttpGet("All")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllUserPropertiesInfos()
        {
            var userPropertiesInfos = await _context.UserPropertiesInfo.ToListAsync();
            return Ok(userPropertiesInfos);
        }

        [HttpPut("Update")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateUserPropertiesInfo([FromBody] UserPropertiesInfo userPropertiesInfo)
        {
            if (userPropertiesInfo == null || userPropertiesInfo.id <= 0)
            {
                return BadRequest();
            }

            var existingUserPropertiesInfo = await _context.UserPropertiesInfo.FirstOrDefaultAsync(u => u.id == userPropertiesInfo.id);
            if (existingUserPropertiesInfo == null)
            {
                return NotFound();
            }

            existingUserPropertiesInfo.userId = userPropertiesInfo.userId;
            existingUserPropertiesInfo.houseId = userPropertiesInfo.houseId;

            _context.UserPropertiesInfo.Update(existingUserPropertiesInfo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id:int}/UpdatePartial")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateUserPropertiesInfoPartial(int id, [FromBody] JsonPatchDocument<UserPropertiesInfo> patchDocument)
        {
            if (patchDocument == null || id <= 0)
            {
                return BadRequest();
            }

            var existingUserPropertiesInfo = await _context.UserPropertiesInfo.FirstOrDefaultAsync(u => u.id == id);
            if (existingUserPropertiesInfo == null)
            {
                return NotFound();
            }

            var userPropertiesInfoDTO = new UserPropertiesInfo
            {
                id = existingUserPropertiesInfo.id,
                userId = existingUserPropertiesInfo.userId,
                houseId = existingUserPropertiesInfo.houseId
            };

            patchDocument.ApplyTo(userPropertiesInfoDTO, (error) =>
            {
                ModelState.AddModelError(string.Empty, $"{error.Operation.op} operation failed on path {error.Operation.path} due to error: {error.ErrorMessage}");
            });

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            existingUserPropertiesInfo.userId = userPropertiesInfoDTO.userId;
            existingUserPropertiesInfo.houseId = userPropertiesInfoDTO.houseId;

            _context.UserPropertiesInfo.Update(existingUserPropertiesInfo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("Delete/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteUserPropertiesInfo(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid ID");
            }

            var userPropertiesInfo = await _context.UserPropertiesInfo.FirstOrDefaultAsync(u => u.id == id);

            if (userPropertiesInfo == null)
            {
                return NotFound($"The user properties info with id {id} not found");
            }

            _context.UserPropertiesInfo.Remove(userPropertiesInfo);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
