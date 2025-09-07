using Microsoft.Extensions.Options;
using ViberGoodMorningBot.Models;

namespace ViberGoodMorningBot.Service
{
    public class ViberService
    {
        private readonly HttpClient _httpClient;
        private readonly string _botToken;

        public ViberService(HttpClient httpClient, IOptions<ViberSettings> viberOptions)
        {
            _httpClient = httpClient;
            _botToken = viberOptions.Value.AuthToken; // Store token in appsettings.json
        }

        public async Task<bool> RegisterWebhookAsync(string webhookUrl)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://chatapi.viber.com/pa/set_webhook");

            request.Headers.Add("X-Viber-Auth-Token", _botToken);

            var payload = new
            {
                url = webhookUrl,
                event_types = new string[]
                {
                "delivered",
                "seen",
                "failed",
                "subscribed",
                "unsubscribed",
                "conversation_started",
                "message"
                }
            };

            request.Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(payload), System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to register Viber webhook: {error}");
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Viber Webhook response: " + responseBody);

            return true;
        }
    }

}
