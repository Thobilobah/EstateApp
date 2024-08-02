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
    public class UserInfoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserInfoController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("Create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostUserInfo([FromBody] UserInfo userInfo)
        {
            if (userInfo == null)
            {
                return BadRequest();
            }

            // Fetch the last ID and increment it for the new entry
            var lastUserInfo = await _context.UserInfo.OrderByDescending(u => u.id).FirstOrDefaultAsync();
            int newId = (lastUserInfo?.id ?? 0) + 1;

            userInfo.id = newId;

            // Add the new user info to the context
            _context.UserInfo.Add(userInfo);
            // Save changes to the database
            await _context.SaveChangesAsync();

            // Return a CreatedAtAction response with the new user info
            return CreatedAtAction(nameof(GetUserInfo), new { id = userInfo.id }, userInfo);
        }

        [HttpGet("{id:int}", Name = "GetUserInfoById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserInfo(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid ID");
            }

            var userInfo = await _context.UserInfo.FirstOrDefaultAsync(u => u.id == id);

            if (userInfo == null)
            {
                return NotFound($"The user info with id {id} not found");
            }

            return Ok(userInfo);
        }

        [HttpPost("Login")]
        public async Task<Response> Login([FromBody] LoginRequest loginRequest)
        {
            Response response = new Response();
            var userLogin = await _context.UserInfo
                .FirstOrDefaultAsync(u => u.phoneNo == loginRequest.PhoneNumber && u.password == loginRequest.Password);

            if (userLogin == null)
            {
                response.ResponseCode = 404;
                response.Message = "Wrong User Name Or Password";
            }
            else
            {
                response.ResponseCode = 200;
                response.data = userLogin.roleName;
                response.Message = "Success";
            }

            return response;
        }

        public class LoginRequest
        {
            public string PhoneNumber { get; set; }
            public string Password { get; set; }
        }

        public class Response
        {
            public int ResponseCode { get; set; }
            public string data { get; set; }
            public string Message { get; set; }
        }


        [HttpGet("All")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllUserInfos()
        {
            var userInfos = await _context.UserInfo.ToListAsync();
            return Ok(userInfos);
        }

        [HttpPut("Update")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateUserInfo([FromBody] UserInfo userInfo)
        {
            if (userInfo == null || userInfo.id <= 0)
            {
                return BadRequest();
            }

            var existingUserInfo = await _context.UserInfo.FirstOrDefaultAsync(u => u.id == userInfo.id);
            if (existingUserInfo == null)
            {
                return NotFound();
            }

            existingUserInfo.roleName = userInfo.roleName;
            existingUserInfo.fName = userInfo.fName;
            existingUserInfo.lName = userInfo.lName;
            existingUserInfo.phoneNo = userInfo.phoneNo;
            existingUserInfo.password = userInfo.password;

            _context.UserInfo.Update(existingUserInfo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id:int}/UpdatePartial")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateUserInfoPartial(int id, [FromBody] JsonPatchDocument<UserInfo> patchDocument)
        {
            if (patchDocument == null || id <= 0)
            {
                return BadRequest();
            }

            var existingUserInfo = await _context.UserInfo.FirstOrDefaultAsync(u => u.id == id);
            if (existingUserInfo == null)
            {
                return NotFound();
            }

            var userInfoDTO = new UserInfo
            {
                id = existingUserInfo.id,
                roleName = existingUserInfo.roleName,
                fName = existingUserInfo.fName,
                lName = existingUserInfo.lName,
                
            };

            patchDocument.ApplyTo(userInfoDTO, (error) =>
            {
                ModelState.AddModelError(string.Empty, $"{error.Operation.op} operation failed on path {error.Operation.path} due to error: {error.ErrorMessage}");
            });

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            existingUserInfo.roleName = userInfoDTO.roleName;
            existingUserInfo.fName = userInfoDTO.fName;
            existingUserInfo.lName = userInfoDTO.lName;
            

            _context.UserInfo.Update(existingUserInfo);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        [HttpDelete("Delete/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteUserInfo(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid ID");
            }

            var userInfo = await _context.UserInfo.FirstOrDefaultAsync(u => u.id == id);

            if (userInfo == null)
            {
                return NotFound($"The user info with id {id} not found");
            }

            _context.UserInfo.Remove(userInfo);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

    public class Response {
        public string Message {get;set;}
        public int ResponseCode { get; set; }

    }
}
