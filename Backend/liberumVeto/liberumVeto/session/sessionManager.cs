using liberumVeto.database;
using liberumVeto.database.cacheMemory;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace liberumVeto.session
{
    public class sessionManager : Hub
    {
        private readonly databaseContext _db;
        public sessionManager(databaseContext context)
        {
            _db = context;
        }

        public async Task sendState(bool votingState)
        {
            await Clients.All.SendAsync("votingState", votingState);
        }

        public async Task castVote(bool vote)
        {            
            var question = _db.question.Where(q => q.id.Equals(votingCache.questionId)).FirstOrDefault();
            var session = _db.votingSession.Where(s => s.id.Equals(votingCache.sessionId)).FirstOrDefault();
            if (session != null && question != null)
            {
                var statistic = _db.questionStatistic.Where(q => q.question.Equals(question) && q.votingSession.Equals(session)).FirstOrDefault();
                if(statistic != null)
                {
                    if (vote)
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


        public async Task sendVote(bool vote)
        {
            await Clients.All.SendAsync("voteValue", vote);
        }

    }

}
