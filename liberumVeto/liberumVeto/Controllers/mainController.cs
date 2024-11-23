using liberumVeto.database;
using liberumVeto.database.cacheMemory;
using liberumVeto.database.tables;
using liberumVeto.input;
using liberumVeto.output;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace liberumVeto.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class mainController : ControllerBase
    {

        private readonly databaseContext _db;
        public mainController(databaseContext db) 
        {
            _db = db;
        }


        [HttpPost]
        [Route("addSet")]
        public string addSet([FromBody] addSetInput input)
        {
            _db.questionSet.Add(new questionSet { name = input.name });
            _db.SaveChanges();

            return "successfully added";
        }

        [HttpPost]
        [Route("addQuestion")]
        public string addQuestion([FromBody] addQuestionInput input)
        {
            var set = _db.questionSet.Where(q => q.id.Equals(input.setId)).FirstOrDefault();
            if (set != null) 
            {
                _db.question.Add(new question { name = input.name, set = set });
                _db.SaveChanges();
            }
            else
            {
                throw new Exception("not found");
            }

            return "successfully added";
        }

        [HttpGet]
        [Route("getSetList")]
        public List<setListOutput> getSetList()
        {
            var setList = _db.questionSet.ToList();
            List<setListOutput> result = new List<setListOutput>();

            foreach (var set in setList)
            {
                result.Add(new setListOutput { setId = set.id, setName = set.name });
            }
            return result;
        }

        [HttpGet]
        [Route("getSetById")]
        public setOutput getSetById([FromQuery] long input)
        {
            var set = _db.questionSet.Where(s => s.id.Equals(input)).Include(s => s.questionList).FirstOrDefault();
            if (set != null)
            {
                var questionList = set.questionList;
                if(questionList != null)
                {
                    setOutput result = new setOutput
                    {
                        setId = set.id,
                        setName = set.name,
                        questionList = questionList.Select(q => new questionListOutput { questionId = q.id, questionName = q.name }).ToList()
                    };
                    return result;

                }
                else
                {
                    throw new Exception("not found");
                }
            }
            else
            {
                throw new Exception("not found");
            }
        }

        [HttpPost]
        [Route("startVoting")]
        public string startVoting([FromBody] startVotingInput input)
        {
            var set = _db.questionSet.Where(q => q.id.Equals(input.setId)).FirstOrDefault();
            if (set != null)
            {
                votingSession newSession = new votingSession { set = set, votingDate = DateTime.UtcNow };
                _db.votingSession.Add(newSession);
                _db.SaveChanges();

                votingCache.sessionId = newSession.id;
            }
            else
            {
                throw new Exception("not found");
            }

            return "successfully started";
        }

        [HttpPost]
        [Route("currentQuestion")]
        public async Task<string> currentQuestion(currentQuestionInput input)
        {
            var question = _db.question.Where(q => q.id.Equals(input.questionId)).FirstOrDefault();
            var session = _db.votingSession.Where(v => v.id.Equals(votingCache.sessionId)).FirstOrDefault();

            if (question != null && session != null)
            {
                votingCache.questionId = input.questionId;
                _db.questionStatistic.Add(new questionStatistic { question = question, votingSession = session, forQuantity = 0, againstQuantity = 0 });
                _db.SaveChanges();


                /*HubConnection connection;
                connection = new HubConnectionBuilder()
                    .WithUrl("https://localhost:7273/session")
                    .Build();
                await connection.StartAsync();
                await connection.InvokeAsync("SendState", true);

                await connection.DisposeAsync();*/

                await websocketController.sendState("true");

            }
            else
            {
                throw new Exception("not found");
            }
            return "got information";
        }

        [HttpPost]
        [Route("stopVoting")]
        public async Task<string> stopVoting()
        {
            votingCache.questionId = null;
            votingCache.sessionId = null;

            await websocketController.sendState("false");
            /*            HubConnection connection = new HubConnectionBuilder()
                                .WithUrl("https://localhost:7273/session")
                                .Build();
                        await connection.StartAsync();
                        await connection.InvokeAsync("SendState", false);

                        await connection.DisposeAsync();*/

            // send to ESP - stop voting
            return "stopped voting";
        }

        [HttpGet]
        [Route("votingHistory")]
        public List<votingSessionOutput> votingHistory()
        {
            List<votingSessionOutput> result = new List<votingSessionOutput>();

            var history = _db.votingSession.Where(v => v.id != votingCache.sessionId).Include(v => v.set).Include(v => v.statisticList).ThenInclude(s => s.question).ToList();

            if(history == null)
            {
                throw new Exception("not found");
            }

            foreach (var session in history)
            {
                votingSessionOutput data = new votingSessionOutput
                {
                    date = session.votingDate,
                    setName = session.set.name,
                    statisticList = session.statisticList.Select(s => 
                    new questionStatisticOutput 
                    { 
                        questionName = s.question.name, 
                        againstQuantity = s.againstQuantity, 
                        forQuantity = s.forQuantity
                    }).ToList()                    
                };

                result.Add(data);
            }


            return result;
        }


    }
}
