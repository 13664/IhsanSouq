using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;

namespace Payment.WebApi.Controllers;

[ApiController]
[Route("api/payment")]
public class PaymentController : ControllerBase
{
    private readonly MulticardService _multicardService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _config;
    private readonly ILogger<PaymentController> _logger;
    private readonly CharityCaseContext _context;

    public PaymentController(MulticardService multicardService, IHttpClientFactory httpClientFactory, IConfiguration config, ILogger<PaymentController> logger, CharityCaseContext context)
    {
        _multicardService = multicardService;
        _httpClientFactory = httpClientFactory;
        _config = config;
        _logger = logger;
        _context = context;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreatePayment([FromBody] PaymentRequest request)
    {
        try
        {
            var token = await _multicardService.GetAuthTokenAsync();
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var paymentData = new
            {
                store_id = request.StoreId,
                amount = request.Amount,
                invoice_id = request.InvoiceId,
                return_url = request.ReturnUrl,
                callback_url = _config["Multicard:CallbackUrl"],
            };

            var content = new StringContent(JsonConvert.SerializeObject(paymentData), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(_config["Multicard:BaseUrl"] + _config["Multicard:InvoiceEndpoint"], content);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Payment creation failed: {ErrorContent}", errorContent);
                return BadRequest(errorContent);
            }
            var t = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<dynamic>(t);
            var responseData = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());

            if (responseData.success == true)
            {
                return Ok(new
                {
                    checkout_url = responseData.data.checkout_url?.ToString() ?? "N/A",
                    payment_uuid = responseData.data.uuid?.ToString() ?? "N/A"
                });
            }
            else
            {
                return BadRequest(responseData);
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating payment");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("callback")]
    public async Task<IActionResult> PaymentCallback([FromBody] PaymentCallbackRequest request)
    {
        try
        {
            string secret = _config["Multicard:Secret"];
            string expectedSign = CreateMD5($"{request.store_id}{request.invoice_id}{request.amount}{secret}");

            if (request.sign != expectedSign)
            {
                _logger.LogWarning("Sign verification failed for invoice {InvoiceId}", request.invoice_id);
                return BadRequest(new { success = false, message = "Sign verification failed" });
            }

            var charityCase = await _context.CharityCases
                .FirstOrDefaultAsync(c => c.Id.ToString() == request.invoice_id);

            if (charityCase != null)
            {
                _logger.LogInformation("Charity case found: Id={Id}, Current AmountCollected={AmountCollected}",
                    charityCase.Id, charityCase.AmountCollected);
            }
            else
            {
                _logger.LogWarning("Charity case NOT found for InvoiceId={InvoiceId}", request.invoice_id);
            }

            if (charityCase != null)
            {
                var payment = new Core.Entities.Payment
                {
                    Uuid = request.uuid,
                    CharityCaseId = charityCase.Id,
                    Amount = request.amount,
                    Status = "Success",
                    PaymentTime = DateTime.UtcNow,
                    CardPan = "" // You could fetch it if needed
                };

                charityCase.AmountCollected += request.amount; // 🔥 Update collected amount
                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();
            }

            return Ok(new { success = true, message = "OK" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during payment callback");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("status/{uuid}")]
    public async Task<IActionResult> GetPaymentStatus(string uuid)
    {
        try
        {
            var token = await _multicardService.GetAuthTokenAsync();
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await httpClient.GetAsync($"{_config["Multicard:BaseUrl"]}/payment/{uuid}");

            if (!response.IsSuccessStatusCode)
                return BadRequest(await response.Content.ReadAsStringAsync());

            var responseData = JsonConvert.DeserializeObject<PaymentResponse>(await response.Content.ReadAsStringAsync());
            //Console.WriteLine(await response.Content.ReadAsStringAsync());
            return Ok(responseData);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = "Server xatosi", error = ex.Message });
        }
    }

    [HttpGet("receipt-proxy/{uuid}")]
    public async Task<IActionResult> ProxyReceiptByUuid(string uuid)
    {
        if (string.IsNullOrWhiteSpace(uuid))
            return BadRequest("Missing UUID.");

        try
        {
            string baseReceiptUrl = "https://dev-checkout.multicard.uz/check/";
            string receiptUrl = $"{baseReceiptUrl}{uuid}";

            var httpClient = _httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 " +
                "(KHTML, like Gecko) Chrome/112.0.0.0 Safari/537.36"
            );

            var response = await httpClient.GetAsync(receiptUrl);

            if (!response.IsSuccessStatusCode)
            {
                var errorText = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Multicard receipt fetch failed: {StatusCode}, {Body}", response.StatusCode, errorText);
                return StatusCode((int)response.StatusCode, "Failed to load receipt.");
            }

            var content = await response.Content.ReadAsStringAsync();
            var contentType = response.Content.Headers.ContentType?.ToString() ?? "text/html";

            // 🧼 Удаляем кнопку "Вернуться к поставщику"
            content = Regex.Replace(
                content,
                @"<tr>\s*<td colspan=""2"">\s*<button[^>]*onclick=""[^""]*localhost:4200[^""]*""[^>]*>.*?</button>\s*</td>\s*</tr>",
                string.Empty,
                RegexOptions.IgnoreCase | RegexOptions.Singleline
            );

            // 🧼 Удаляем блок <div class="links">...</div>
            content = Regex.Replace(
                content,
                @"<div class=""links"">.*?</div>",
                string.Empty,
                RegexOptions.IgnoreCase | RegexOptions.Singleline
            );

            // ⚙️ Добавляем <base href> в <head>
            string baseTag = "<base href=\"https://dev-checkout.multicard.uz/\">";
            content = content.Replace("<head>", $"<head>{baseTag}");

            // ✅ Разрешаем загрузку в iframe
            Response.Headers.Remove("X-Frame-Options");
            Response.Headers.Add("Content-Security-Policy", "frame-ancestors *");

            return Content(content, contentType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error proxying receipt with UUID {Uuid}", uuid);
            return StatusCode(500, "Proxy error.");
        }
    }

    [HttpGet("case/{charityCaseId}")]
    public async Task<ActionResult<IReadOnlyList<Core.Entities.Payment>>> GetPaymentsForCharityCase(int charityCaseId)
    {
        var payments = await _context.Payments
            .Where(p => p.CharityCaseId == charityCaseId)
            .ToListAsync();

        if (payments == null || payments.Count == 0)
        {
            return NotFound("No payments found for the given charity case.");
        }

        return Ok(payments);
    }


    private static string CreateMD5(string input)
    {
        using (var md5 = System.Security.Cryptography.MD5.Create())
        {
            var inputBytes = Encoding.UTF8.GetBytes(input);
            var hashBytes = md5.ComputeHash(inputBytes);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }
}

public class PaymentResponse
{
    public bool Success { get; set; }
    public PaymentData Data { get; set; }
}

public class PaymentData
{
    public int Id { get; set; }
    public string Uuid { get; set; }
    public int StoreId { get; set; }
    public string Ps { get; set; }
    public string StoreInvoiceId { get; set; }
    public string Payment_Time { get; set; }
    public string Card_Pan { get; set; }
    public string Receipt_Url { get; set; }
    public decimal Payment_Amount { get; set; }
    public decimal? CommissionAmount { get; set; }
    public string CommissionType { get; set; }
    public string Status { get; set; }
    public string CallbackUrl { get; set; }
    public string AddedOn { get; set; }
    public ApplicationData Application { get; set; }
    public StoreData Store { get; set; }
    public string CheckoutUrl { get; set; }
}

public class ApplicationData
{
    public int Id { get; set; }
    public string ApplicationId { get; set; }
    public int OtpRequired { get; set; }
    public int AllowBankTransaction { get; set; }
    public int OtpLength { get; set; }
    public string OfficialName { get; set; }
    public string AddedOn { get; set; }
    public string UpdatedOn { get; set; }
}

public class StoreData
{
    public int Id { get; set; }
    public string Uuid { get; set; }
    public int MerchantId { get; set; }
    public string Note { get; set; }
    public string Logo { get; set; }
    public string CallbackClass { get; set; }
    public int TaxRegistration { get; set; }
    public int Active { get; set; }
    public string Title { get; set; }
    public MerchantData Merchant { get; set; }
    public List<ViewField> ViewFields { get; set; }
}

public class MerchantData
{
    public int Id { get; set; }
    public string Name { get; set; }
    public long Tin { get; set; }
    public string BankAccount { get; set; }
}

public class ViewField
{
    public string Name { get; set; }
    public bool ExcludeFromInvoice { get; set; }
    public string Value { get; set; }
    public string Type { get; set; }
    public int? Order { get; set; }
    public string Label { get; set; }
}

public class PaymentRequest
{
    public int StoreId { get; set; }
    public int Amount { get; set; }
    public string InvoiceId { get; set; }
    public string ReturnUrl { get; set; }
}

public class PaymentCallbackRequest
{
    public int store_id { get; set; }
    public int amount { get; set; }
    public string invoice_id { get; set; }
    public string invoice_uuid { get; set; }
    public string payment_time { get; set; }
    public string uuid { get; set; }
    public string sign { get; set; }
}