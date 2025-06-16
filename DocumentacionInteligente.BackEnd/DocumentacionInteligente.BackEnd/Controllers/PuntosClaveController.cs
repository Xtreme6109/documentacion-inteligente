using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;

[ApiController]
[Route("api/[controller]")]
public class PuntosClaveController : ControllerBase
{
    private readonly IConfiguration _config;

    public PuntosClaveController(IConfiguration config)
    {
        _config = config;
    }

    public class TextoRequest
    {
        public required string Texto { get; set; }
    }

    [HttpPost("extraer")]
    public async Task<IActionResult> ExtraerPuntosClave([FromBody] TextoRequest request)
    {
        try
        {
            var apiKey = _config["OpenAI:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
                return StatusCode(500, "API Key no configurada.");

            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(180); // 3 minutos timeout
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var prompt = $@"
        Actúa como un experto en el tema y extrae los puntos clave del siguiente texto. 
        Para cada punto clave, incluye además una explicación breve que ayude a entenderlo mejor.
        Organiza la respuesta así:

        1. Los títulos principales deben ir entre **dobles asteriscos** y representarán encabezados grandes.
        2. Los subtítulos o títulos secundarios deben ir entre *asteriscos simples* y serán encabezados más pequeños.
        3. Cada punto clave debe estar en una viñeta que comience con un guion (-).
        4. Después de cada punto clave, añade una o dos frases explicativas en texto normal, sin asteriscos.
        5. Se deben respetar los saltos de línea para separar cada punto y su explicación, facilitando el parseo posterior.
        6. Si es posible, incluye ejemplos o detalles relevantes para que el contenido sea más útil y comprensible.

        Aquí está el texto a procesar:

        {request.Texto}
        ";

            var data = new
            {
                model = "gpt-4o-mini",
                messages = new[]
                {
                new { role = "system", content = "Eres un asistente experto en resumir y extraer puntos clave." },
                new { role = "user", content = prompt }
            }
            };

            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, responseString);

            var jsonDoc = JsonDocument.Parse(responseString);
            var puntosClaveGenerados = jsonDoc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return Ok(new { puntosClave = puntosClaveGenerados });
        }
        catch (TaskCanceledException ex)
        {
            // Timeout específico
            Console.WriteLine($"Timeout: {ex.Message}");
            return StatusCode(504, "La solicitud a OpenAI tomó demasiado tiempo y fue cancelada (timeout).");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Excepción: {ex.Message}");
            return StatusCode(500, $"Error interno: {ex.Message}");
        }
    }

}
