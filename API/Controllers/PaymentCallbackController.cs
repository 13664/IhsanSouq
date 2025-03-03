using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentCallbackController : ControllerBase
    {
         private readonly string _secret = "YOUR_SECRET";

        [HttpPost("callback")]
        public IActionResult PaymentCallback([FromBody] PaymentCallbackModel callback)
        {
            // Validate the signature (calculate md5 hash of {store_id}{invoice_id}{amount}{secret})
            string signData = $"{callback.store_id}{callback.invoice_id}{callback.amount}{_secret}";
            string calculatedSign;
            using (var md5 = MD5.Create())
            {
                var hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(signData));
                calculatedSign = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            }

            if (calculatedSign != callback.sign)
            {
                return BadRequest(new { success = false, message = "Invalid signature" });
            }

            // Process the payment (update database, notify users, etc.)
            // ...

            return Ok(new { success = true, message = "OK" });
        }
    }

    // Define your callback model based on documentation
    public class PaymentCallbackModel
    {
        public int store_id { get; set; }
        public int amount { get; set; }
        public string invoice_id { get; set; }
        public string invoice_uuid { get; set; }
        public DateTime payment_time { get; set; }
        public string billing_id { get; set; }
        public string uuid { get; set; }
        public string sign { get; set; }
    }
}
