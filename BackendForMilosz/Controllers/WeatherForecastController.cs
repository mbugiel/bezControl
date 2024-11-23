using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net.WebSockets;
using System.Text;

namespace t2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        private async Task ReceiveMessagesAsync(ClientWebSocket webSocket)
        {
            var buffer = new byte[1024];

            while (webSocket.State == WebSocketState.Open)
            {
                try
                {
                    var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        Console.WriteLine("Received message: " + message);
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        // Handle WebSocket close
                        Console.WriteLine("Server requested close.");
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Close received from server", CancellationToken.None);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error while receiving message: " + ex.Message);
                    break;
                }
            }
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            string serverUri = "ws://172.20.10.2"; // Replace with your WebSocket server URL

            using (ClientWebSocket webSocket = new ClientWebSocket())
            {
                try
                {
                    // Connect to the WebSocket server
                    await webSocket.ConnectAsync(new Uri(serverUri), CancellationToken.None);
                    Console.WriteLine("Connected to the WebSocket server!");

                    // Send a message to the server
                    string message = "newVoting";
                    byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                    await webSocket.SendAsync(
                        new ArraySegment<byte>(messageBytes),
                        WebSocketMessageType.Text,
                        true, // Indicates the message is complete
                        CancellationToken.None
                    );

                    // Close the WebSocket connection
                    await webSocket.CloseAsync(
                        WebSocketCloseStatus.NormalClosure,
                        "Client closing connection",
                        CancellationToken.None
                    );
                    Console.WriteLine("WebSocket connection closed.");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error: {e.Message}");
                }
            }

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
