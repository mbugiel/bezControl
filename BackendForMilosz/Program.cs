using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

namespace WebSocketApi
{
    public class Program
    {
        // S³ownik przechowuj¹cy aktywne po³¹czenia klientów
        private static readonly ConcurrentDictionary<string, WebSocket> _clients = new ConcurrentDictionary<string, WebSocket>();

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Dodaj serwisy do kontenera DI
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Konfiguracja middleware'ów i pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();

            // Obs³uga WebSocket
            app.UseWebSockets();

            // Middleware obs³uguj¹ce WebSocket
            app.Use(async (context, next) =>
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                    Console.WriteLine("WebSocket connected!");

                    string clientId = Guid.NewGuid().ToString(); // Unikalny identyfikator klienta
                    _clients[clientId] = webSocket;

                    await HandleWebSocketCommunication(webSocket, clientId);
                }
                else
                {
                    await next();
                }
            });

            // Konfiguracja endpointów
            app.MapControllers();

            app.Run();
        }

        // Funkcja do obs³ugi komunikacji z pojedynczym klientem
        private static async Task HandleWebSocketCommunication(WebSocket webSocket, string clientId)
        {
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = null;

            try
            {
                while (webSocket.State == WebSocketState.Open)
                {
                    result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        Console.WriteLine($"Client {clientId} requested close. Sending acknowledgment...");
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing connection", CancellationToken.None);
                        break;
                    }

                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        Console.WriteLine($"Received from {clientId}: {message}");

                        // Obs³uga wiadomoœci
                        await ProcessClientMessage(message, clientId);
                    }
                }
            }
            catch (WebSocketException ex)
            {
                Console.WriteLine($"WebSocket error for client {clientId}: {ex.Message}");
            }
            finally
            {
                // Usuniêcie klienta z listy po roz³¹czeniu
                _clients.TryRemove(clientId, out _);

                if (webSocket.State == WebSocketState.Open)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                }

                Console.WriteLine($"Client {clientId} disconnected.");
            }
        }

        // Funkcja obs³uguj¹ca wiadomoœci od klientów
        private static async Task ProcessClientMessage(string message, string clientId)
        {
            if (message.StartsWith("sendVote;"))
            {
                string vote = message.Split(';')[1];
                Console.WriteLine($"{vote}");
            }
            else if (message.StartsWith("newVoting"))
            {
                await NotifyAllClients("newVoting;");
            }
        }

        // Funkcja do wysy³ania wiadomoœci do wszystkich pod³¹czonych klientów
        private static async Task NotifyAllClients(string message)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);

            foreach (var (clientId, clientSocket) in _clients)
            {
                if (clientSocket.State == WebSocketState.Open)
                {
                    try
                    {
                        await clientSocket.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, CancellationToken.None);
                        Console.WriteLine($"Message sent to client {clientId}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error sending message to client {clientId}: {ex.Message}");
                    }
                }
            }
        }

        // Funkcja do wysy³ania wiadomoœci do konkretnego klienta
        public static async Task SendMessageToClient(string clientId, string message)
        {
            if (_clients.TryGetValue(clientId, out WebSocket clientSocket))
            {
                if (clientSocket.State == WebSocketState.Open)
                {
                    var buffer = Encoding.UTF8.GetBytes(message);
                    await clientSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                    Console.WriteLine($"Message sent to client {clientId}");
                }
                else
                {
                    Console.WriteLine($"WebSocket for client {clientId} is not open.");
                }
            }
            else
            {
                Console.WriteLine($"Client with ID {clientId} not found.");
            }
        }
    }
}
