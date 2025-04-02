using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Text;

public class MulticardService
{
    private readonly HttpClient _client;
    private readonly IConfiguration _config;
    private string _token = string.Empty;

    public MulticardService(HttpClient client, IConfiguration config)
    {
        _client = client;
        _config = config;
    }

    public async Task<string> GetAuthTokenAsync()
    {
        if (!string.IsNullOrEmpty(_token))
            return _token;

        var authData = new
        {
            application_id = _config["Multicard:ApplicationId"],
            secret = _config["Multicard:Secret"]
        };

        var content = new StringContent(JsonConvert.SerializeObject(authData), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync(_config["Multicard:BaseUrl"] + _config["Multicard:AuthEndpoint"], content);

        if (!response.IsSuccessStatusCode)
            throw new Exception("Failed to authenticate with Multicard");

        var responseData = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
        _token = responseData.token;
        return _token;
    }
}
