using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ViberGoodMorningBot.Models;
using ViberGoodMorningBot.Service;

namespace ViberGoodMorningBot.Controllers;

[ApiController]
[Route("viber/webhook")]
public class ViberController : ControllerBase
{
    private static readonly Dictionary<string, DateTime> LastGreeted = new();
    private readonly ViberSettings _viberSettings;
    private readonly ViberService _viberService;

    public ViberController(IOptions<ViberSettings> viberOptions, ViberService viberService)
    {
        // Access the strongly-typed settings
        _viberSettings = viberOptions.Value;
        _viberService = viberService;
    }


    [HttpPost("register-webhook")]
    public async Task<IActionResult> RegisterWebhook()
    {
        try
        {
            string webhookUrl = $"{_viberSettings.Url}viber/webhook";
            await _viberService.RegisterWebhookAsync(webhookUrl);
            return Ok("Webhook registered successfully");
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }
    [HttpPost]
    public IActionResult Receive([FromBody] ViberMessage update)
    {
        var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString();

        if (update.Message?.Type == "text" && !string.IsNullOrEmpty(update.Message.Text))
        {
            var text = update.Message.Text.ToLower();
            if (text.Contains("good morning") 
                || text.Contains("i am in")
                || text.Contains("m in")
                )
            {
                var userId = update.Sender?.Id ?? "";
                var userName = update.Sender?.Name ?? "friend";
                var now = DateTime.UtcNow.AddHours(5.75); // Nepal time

                // ensure greet once per day
                if (!LastGreeted.ContainsKey(userId) || LastGreeted[userId].Date != now.Date)
                {
                    LastGreeted[userId] = now;

                    var reply = new
                    {
                        receiver = userId,
                        min_api_version = 7,
                        sender = new { name = "GoodMorning Bot" },
                        type = "text",
                        text = $"🌞 Good morning, {userName}! Have a wonderful day!\n\n📡 IP: {clientIp}",
                        keyboard = new
                        {
                            Type = "keyboard",
                            Buttons = new[]
                            {
                                    new {
                                        ActionType = "open-url",
                                        ActionBody =  $"{_viberSettings.Url}showip?uid=" + userId,
                                        Text = "🔍 Show My Real IP"
                                    }
                                }
                        }
                    };

                    // send reply back to Viber
                    using var client = new HttpClient();
                    client.DefaultRequestHeaders.Add("X-Viber-Auth-Token", _viberSettings.AuthToken);
                    var response = client.PostAsJsonAsync("https://chatapi.viber.com/pa/send_message", reply).Result;

                    return Ok(new { status = "replied", ip = clientIp });
                }
            }
        }

        return Ok();
    }
}
