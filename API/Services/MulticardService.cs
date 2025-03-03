using System;
using System.Text;
using System.Text.Json;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Core.Entities;

namespace API.Services;
public interface IMulticardService
    {
    Task<string> CreatePaymentInvoiceAsync(PaymentInvoiceRequest request);
    Task<string> GetAuthTokenAsync();
    }
public class MulticardService : IMulticardService
{
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<MulticardService> _logger;

        private string _cachedToken;
        private DateTime _tokenExpiry = DateTime.MinValue;

        public MulticardService(HttpClient httpClient, IConfiguration configuration, ILogger<MulticardService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<string> GetAuthTokenAsync()
        {
            if (!string.IsNullOrEmpty(_cachedToken) && DateTime.UtcNow < _tokenExpiry)
            {
                return _cachedToken;
            }

            var authEndpoint = _configuration["Multicard:AuthEndpoint"];
            var applicationId = _configuration["Multicard:ApplicationId"];
            var secret = _configuration["Multicard:Secret"];

            var payload = new
            {
                application_id = applicationId,
                secret = secret
            };

            var jsonPayload = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync(authEndpoint, content);
                response.EnsureSuccessStatusCode();

                var responseString = await response.Content.ReadAsStringAsync();
                using (JsonDocument doc = JsonDocument.Parse(responseString))
                {
                    var root = doc.RootElement;
                    if (root.TryGetProperty("token", out JsonElement tokenElement) &&
                        root.TryGetProperty("expiry", out JsonElement expiryElement))
                    {
                        _cachedToken = tokenElement.GetString();

                        if (DateTime.TryParse(expiryElement.GetString(), out DateTime expiry))
                        {
                            _tokenExpiry = expiry.ToUniversalTime();
                        }
                        else
                        {
                            _tokenExpiry = DateTime.UtcNow.AddHours(24);
                        }
                        _logger.LogInformation("Multicard token obtained successfully.");
                        return _cachedToken;
                    }
                    else
                    {
                        _logger.LogError("Token or expiry field not found in the response.");
                        throw new Exception("Invalid authentication response.");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obtaining Multicard token.");
                throw;
            }
          }
        

    public async Task<string> CreatePaymentInvoiceAsync(PaymentInvoiceRequest request)
    {
        // Get the token (this method already exists)
        var token = await GetAuthTokenAsync();

        // Get the base URL from configuration - you might use the sandbox URL for testing
        var invoiceEndpoint = _configuration["Multicard:InvoiceEndpoint"] ?? "https://dev-mesh.multicard.uz/payment/invoice";

        var jsonPayload = JsonSerializer.Serialize(request);
        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        // Set the Authorization header
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.PostAsync(invoiceEndpoint, content);
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        using (JsonDocument doc = JsonDocument.Parse(responseString))
        {
            var root = doc.RootElement;
            if (root.TryGetProperty("success", out JsonElement successElement) && successElement.GetBoolean())
            {
                if (root.TryGetProperty("data", out JsonElement dataElement) &&
                    dataElement.TryGetProperty("checkout_url", out JsonElement checkoutUrlElement))
                {
                    return checkoutUrlElement.GetString();
                }
                else
                {
                    throw new Exception("checkout_url not found in the response.");
                }
            }
            else
            {
                throw new Exception("Payment invoice creation failed.");
            }
        }
    }

    }
      

        
