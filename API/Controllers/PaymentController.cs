using API.Services;
using Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
         private readonly IMulticardService _multicardService;

        public PaymentController(IMulticardService multicardService)
        {
            _multicardService = multicardService;
        }

        [HttpPost("create-invoice")]
        public async Task<IActionResult> CreateInvoice([FromBody] PaymentInvoiceRequest request)
        {
            try
            {
                var checkoutUrl = await _multicardService.CreatePaymentInvoiceAsync(request);
                return Ok(new { checkoutUrl });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
