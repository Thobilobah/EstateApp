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
    public class InvoiceInfoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public InvoiceInfoController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("Create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostInvoiceInfo([FromBody] InvoiceInfo invoiceInfo)
        {
            if (invoiceInfo == null)
            {
                return BadRequest();
            }

            // Fetch the last ID and increment it for the new entry
            var lastInvoiceInfo = await _context.InvoiceInfo.OrderByDescending(i => i.id).FirstOrDefaultAsync();
            int newId = (lastInvoiceInfo?.id ?? 0) + 1;

            invoiceInfo.id = newId;

            // Add the new invoice info to the context
            _context.InvoiceInfo.Add(invoiceInfo);
            // Save changes to the database
            await _context.SaveChangesAsync();

            // Return a CreatedAtAction response with the new invoice info
            return CreatedAtAction(nameof(GetInvoiceInfo), new { id = invoiceInfo.id }, invoiceInfo);
        }

        [HttpGet("{id:int}", Name = "GetInvoiceInfoById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetInvoiceInfo(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid ID");
            }

            var invoiceInfo = await _context.InvoiceInfo.FirstOrDefaultAsync(i => i.id == id);

            if (invoiceInfo == null)
            {
                return NotFound($"The invoice info with id {id} not found");
            }

            return Ok(invoiceInfo);
        }

        [HttpGet("All")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllInvoiceInfos()
        {
            var invoiceInfos = await _context.InvoiceInfo.ToListAsync();
            return Ok(invoiceInfos);
        }

        [HttpPut("Update")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateInvoiceInfo([FromBody] InvoiceInfo invoiceInfo)
        {
            if (invoiceInfo == null || invoiceInfo.id <= 0)
            {
                return BadRequest();
            }

            var existingInvoiceInfo = await _context.InvoiceInfo.FirstOrDefaultAsync(i => i.id == invoiceInfo.id);
            if (existingInvoiceInfo == null)
            {
                return NotFound();
            }

            existingInvoiceInfo.userId = invoiceInfo.userId;
            existingInvoiceInfo.dateIssue = invoiceInfo.dateIssue;
            existingInvoiceInfo.dueDate = invoiceInfo.dueDate;
            existingInvoiceInfo.amount = invoiceInfo.amount;
            existingInvoiceInfo.refNo = invoiceInfo.refNo;
            existingInvoiceInfo.feeType = invoiceInfo.feeType;

            _context.InvoiceInfo.Update(existingInvoiceInfo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id:int}/UpdatePartial")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateInvoiceInfoPartial(int id, [FromBody] JsonPatchDocument<InvoiceInfo> patchDocument)
        {
            if (patchDocument == null || id <= 0)
            {
                return BadRequest();
            }

            var existingInvoiceInfo = await _context.InvoiceInfo.FirstOrDefaultAsync(i => i.id == id);
            if (existingInvoiceInfo == null)
            {
                return NotFound();
            }

            var invoiceInfoDTO = new InvoiceInfo
            {
                id = existingInvoiceInfo.id,
                userId = existingInvoiceInfo.userId,
                dateIssue = existingInvoiceInfo.dateIssue,
                dueDate = existingInvoiceInfo.dueDate,
                amount = existingInvoiceInfo.amount,
                refNo = existingInvoiceInfo.refNo,
                feeType = existingInvoiceInfo.feeType
            };

            patchDocument.ApplyTo(invoiceInfoDTO, (error) =>
            {
                ModelState.AddModelError(string.Empty, $"{error.Operation.op} operation failed on path {error.Operation.path} due to error: {error.ErrorMessage}");
            });

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            existingInvoiceInfo.userId = invoiceInfoDTO.userId;
            existingInvoiceInfo.dateIssue = invoiceInfoDTO.dateIssue;
            existingInvoiceInfo.dueDate = invoiceInfoDTO.dueDate;
            existingInvoiceInfo.amount = invoiceInfoDTO.amount;
            existingInvoiceInfo.refNo = invoiceInfoDTO.refNo;
            existingInvoiceInfo.feeType = invoiceInfoDTO.feeType;

            _context.InvoiceInfo.Update(existingInvoiceInfo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("Delete/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteInvoiceInfo(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid ID");
            }

            var invoiceInfo = await _context.InvoiceInfo.FirstOrDefaultAsync(i => i.id == id);

            if (invoiceInfo == null)
            {
                return NotFound($"The invoice info with id {id} not found");
            }

            _context.InvoiceInfo.Remove(invoiceInfo);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
