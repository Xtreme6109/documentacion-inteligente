using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;

[ApiController]
[Route("api/[controller]")]
public class DocumentacionController : ControllerBase
{
    private readonly IConfiguration _config;

    public DocumentacionController(IConfiguration config)
    {
        _config = config;
    }

    [HttpPost("generar")]
    public async Task<IActionResult> GenerarDocumento([FromBody] PromptRequest request)
    {
        try
        {
            //Console.WriteLine($"Prompt recibido: {request.Prompt}");

            var apiKey = _config["OpenAI:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
                return StatusCode(500, "API Key no configurada.");

            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(30);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var data = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                new { role = "system", content = "Eres un generador de documentos especializado." },
                new { role = "user", content = request.Prompt }
            }
            };

            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, responseString);

            var jsonDoc = JsonDocument.Parse(responseString);
            var contentGenerated = jsonDoc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return Ok(new { text = contentGenerated });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Excepción: {ex.Message}");
            return StatusCode(500, $"Error interno: {ex.Message}");
        }
    }


    public class PromptRequest
    {
        public required string Prompt { get; set; }
    }
}
