using Microsoft.AspNetCore.SignalR;

namespace t2
{
    public class test : Hub
    {
        public async Task JoinAuthAppSession(string AppInstanceToken)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, AppInstanceToken);

            Console.WriteLine("Test");

            return;
        }

        public async Task sendVote(bool cos)
        {
            Console.WriteLine("veto");

            return;
        }

        public async Task newVoting()
        {
            Console.WriteLine("newVoting");

            await Clients.All.SendAsync("cosTutajJEst", "CD");
        

            return;
        }
    }
}
