using EstateApp.Models; // Updated namespace
using EstateApp.Services; // Import the service namespace
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EstateApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class PaymentController : ControllerBase
    {
        private readonly PaymentService _paymentService;

        public PaymentController(PaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet("All")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<List<PaymentDTO>> GetPayments()
        {
            var payments = PaymentRepository.Payments.Select(p => new PaymentDTO()
            {
                id = p.id,
                email = p.email,
                amount = p.amount
            }).ToList();

            return Ok(payments);
        }

        [HttpGet("{id:int}", Name = "GetPaymentById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<PaymentDTO> GetPaymentById(int id)
        {
            if (id <= 0)
                return BadRequest();

            var payment = PaymentRepository.Payments.FirstOrDefault(p => p.id == id);
            if (payment == null)
                return NotFound($"The payment with id {id} not found");

            var paymentDTO = new PaymentDTO
            {
                id = payment.id,
                email = payment.email,
                amount = payment.amount
            };

            return Ok(paymentDTO);
        }

        [HttpPost("Create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentDTO model)
        {
            if (model == null)
                return BadRequest();

            int newId = PaymentRepository.Payments.LastOrDefault()?.id + 1 ?? 1;
            Payment payment = new Payment
            {
                id = newId,
                email = model.email,
                amount = model.amount,
                dateCreated = model.dateCreated,
                dateCompleted = model.dateCompleted
            };
            PaymentRepository.Payments.Add(payment);

            model.id = payment.id;

            // Call Paystack API to initiate payment
            var paystackResponse = await _paymentService.InitiatePayment(model);

            // Handle Paystack response
            if (paystackResponse.status)
            {

                var response = new
                {
                    paystackResponse.status,
                    paystackResponse.message,
                    paystackResponse.data.authorization_url,
                    paystackResponse.data.access_code,
                    paystackResponse.data.reference
                };

                return Ok(response);
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, paystackResponse.message);
            }
        }

        [HttpPut("Update")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult UpdatePayment([FromBody] PaymentDTO model)
        {
            if (model == null || model.id <= 0)
                return BadRequest();

            var existingPayment = PaymentRepository.Payments.FirstOrDefault(p => p.id == model.id);
            if (existingPayment == null)
                return NotFound();

            existingPayment.email = model.email;
            existingPayment.amount = model.amount;

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<bool> DeletePayment(int id)
        {
            if (id <= 0)
                return BadRequest();

            var payment = PaymentRepository.Payments.FirstOrDefault(p => p.id == id);
            if (payment == null)
                return NotFound($"The payment with id {id} not found");

            PaymentRepository.Payments.Remove(payment);
            return Ok(true);
        }
    }
}
