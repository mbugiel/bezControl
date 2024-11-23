// See https://aka.ms/new-console-template for more information
using Microsoft.AspNetCore.SignalR.Client;
using System.Net.WebSockets;
using System.Text;

Console.WriteLine("Hello, World!");

/*HubConnection connection;

connection = new HubConnectionBuilder()
    .WithUrl("https://localhost:7273/session")
    .Build();

connection.On<bool>("votingState", (state) =>
{
    Console.WriteLine("SERVER RESPOSE: "+state);
});*/

//await connection.StartAsync();

ClientWebSocket server = new ClientWebSocket();

await server.ConnectAsync(new Uri("wss://localhost:7273/ws"), CancellationToken.None);

Task.Run(async () =>
{
    var buffer = new byte[32];
    while (true)
    {
        if (server.State == WebSocketState.Open)
        {
            await server.ReceiveAsync(buffer, CancellationToken.None);
            var result = Encoding.ASCII.GetString(buffer);
            Console.WriteLine(result);
        }
        else
        {
            break;
        }
    }
});

while (true)
{
    Console.WriteLine("Zagłosuj! y/n");
    var input = Console.ReadLine();
    if (input.ToLower() == "y")
    {
        await server.SendAsync(Encoding.ASCII.GetBytes("castVote:true"), WebSocketMessageType.Text, true, CancellationToken.None);

    }else if (input.ToLower() == "n")
    {
        await server.SendAsync(Encoding.ASCII.GetBytes("castVote:false"), WebSocketMessageType.Text, true, CancellationToken.None);
    }
    else
    {
        break;
    }

}
