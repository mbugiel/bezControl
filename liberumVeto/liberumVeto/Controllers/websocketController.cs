using liberumVeto.database;
using liberumVeto.database.cacheMemory;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using System.Text;

namespace liberumVeto.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [ApiController]
    public class websocketController : ControllerBase
    {
        private readonly databaseContext _db;
        public websocketController(databaseContext db)
        {
            _db = db;
        }

        public static List<WebSocket> activeConnectionList = new List<WebSocket>();

        [Route("/ws")]
        public async Task socket()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();

                if (!activeConnectionList.Contains(webSocket))
                {
                    activeConnectionList.Add(webSocket);
                    Task.Run(() => handleSocket(webSocket));
                }

            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }

        }

        [Route("/www")]
        public static async Task sendState(string votingState)
        {
            foreach (var socket in activeConnectionList)
            {
                await socket.SendAsync(
                    Encoding.ASCII.GetBytes("sendState:" + votingState),
                    WebSocketMessageType.Text,
                    false,
                    CancellationToken.None);
            }
        }

        [Route("/w")]
        public async Task sendVote(string vote)
        {
            foreach (var socket in activeConnectionList) 
            {
                await socket.SendAsync(
                    Encoding.ASCII.GetBytes("sendVote:"+vote),
                    WebSocketMessageType.Text,
                    false,
                    CancellationToken.None);
            }
        }

        [Route("/ww")]
        public async Task handleSocket(WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            var receiveResult = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer), CancellationToken.None);

            while (!receiveResult.CloseStatus.HasValue)
            {
                var response = Encoding.ASCII.GetString(buffer);
                Console.WriteLine(response);

                if (response.Split(":")[0].Equals("castVote"))
                {
                    string vote = response.Split(":")[1];

                    var question = _db.question.Where(q => q.id.Equals(votingCache.questionId)).FirstOrDefault();
                    var session = _db.votingSession.Where(s => s.id.Equals(votingCache.sessionId)).FirstOrDefault();
                    if (session != null && question != null)
                    {
                        var statistic = _db.questionStatistic.Where(q => q.question.Equals(question) && q.votingSession.Equals(session)).FirstOrDefault();
                        if (statistic != null)
                        {
                            if (vote.Equals("true"))
                            {
                                statistic.forQuantity++;
                            }
                            else
                            {
                                statistic.againstQuantity++;
                            }
                            _db.SaveChanges();

                            await sendVote(vote);
                        }
                        else
                        {
                            Console.WriteLine("--- ERROR --- while updating statistics");
                        }
                    }

                }

            }

            await webSocket.CloseAsync(
                receiveResult.CloseStatus.Value,
                receiveResult.CloseStatusDescription,
                CancellationToken.None);

        }



    }
}
