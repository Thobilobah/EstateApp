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
    public class PaymentInfoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PaymentInfoController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("Create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostPaymentInfo([FromBody] PaymentInfo paymentInfo)
        {
            if (paymentInfo == null)
            {
                return BadRequest();
            }

            // Fetch the last ID and increment it for the new entry
            var lastPaymentInfo = await _context.PaymentInfo.OrderByDescending(p => p.id).FirstOrDefaultAsync();
            int newId = (lastPaymentInfo?.id ?? 0) + 1;

            paymentInfo.id = newId;

            // Add the new payment info to the context
            _context.PaymentInfo.Add(paymentInfo);
            // Save changes to the database
            await _context.SaveChangesAsync();

            // Return a CreatedAtAction response with the new payment info
            return CreatedAtAction(nameof(GetPaymentInfo), new { id = paymentInfo.id }, paymentInfo);
        }

        [HttpGet("{id:int}", Name = "GetPaymentInfoById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPaymentInfo(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid ID");
            }

            var paymentInfo = await _context.PaymentInfo.FirstOrDefaultAsync(p => p.id == id);

            if (paymentInfo == null)
            {
                return NotFound($"The payment info with id {id} not found");
            }

            return Ok(paymentInfo);
        }

        [HttpGet("All")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllPaymentInfos()
        {
            var paymentInfos = await _context.PaymentInfo.ToListAsync();
            return Ok(paymentInfos);
        }

        [HttpPut("Update")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdatePaymentInfo([FromBody] PaymentInfo paymentInfo)
        {
            if (paymentInfo == null || paymentInfo.id <= 0)
            {
                return BadRequest();
            }

            var existingPaymentInfo = await _context.PaymentInfo.FirstOrDefaultAsync(p => p.id == paymentInfo.id);
            if (existingPaymentInfo == null)
            {
                return NotFound();
            }

            existingPaymentInfo.invoiceRefNo = paymentInfo.invoiceRefNo;
            existingPaymentInfo.amountPaid = paymentInfo.amountPaid;
            existingPaymentInfo.paymentMethod = paymentInfo.paymentMethod;
            existingPaymentInfo.paymentStatus = paymentInfo.paymentStatus;
            existingPaymentInfo.paymentDate = paymentInfo.paymentDate;

            _context.PaymentInfo.Update(existingPaymentInfo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id:int}/UpdatePartial")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdatePaymentInfoPartial(int id, [FromBody] JsonPatchDocument<PaymentInfo> patchDocument)
        {
            if (patchDocument == null || id <= 0)
            {
                return BadRequest();
            }

            var existingPaymentInfo = await _context.PaymentInfo.FirstOrDefaultAsync(p => p.id == id);
            if (existingPaymentInfo == null)
            {
                return NotFound();
            }

            var paymentInfoDTO = new PaymentInfo
            {
                id = existingPaymentInfo.id,
                invoiceRefNo = existingPaymentInfo.invoiceRefNo,
                amountPaid = existingPaymentInfo.amountPaid,
                paymentMethod = existingPaymentInfo.paymentMethod,
                paymentStatus = existingPaymentInfo.paymentStatus,
                paymentDate = existingPaymentInfo.paymentDate
            };

            patchDocument.ApplyTo(paymentInfoDTO, (error) =>
            {
                ModelState.AddModelError(string.Empty, $"{error.Operation.op} operation failed on path {error.Operation.path} due to error: {error.ErrorMessage}");
            });

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            existingPaymentInfo.invoiceRefNo = paymentInfoDTO.invoiceRefNo;
            existingPaymentInfo.amountPaid = paymentInfoDTO.amountPaid;
            existingPaymentInfo.paymentMethod = paymentInfoDTO.paymentMethod;
            existingPaymentInfo.paymentStatus = paymentInfoDTO.paymentStatus;
            existingPaymentInfo.paymentDate = paymentInfoDTO.paymentDate;

            _context.PaymentInfo.Update(existingPaymentInfo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("Delete/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeletePaymentInfo(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid ID");
            }

            var paymentInfo = await _context.PaymentInfo.FirstOrDefaultAsync(p => p.id == id);

            if (paymentInfo == null)
            {
                return NotFound($"The payment info with id {id} not found");
            }

            _context.PaymentInfo.Remove(paymentInfo);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
