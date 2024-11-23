using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace liberumVeto.database.tables
{
    [Table("question_set")]
    public class questionSet
    {
        [Key]
        public long id {  get; set; }
        public string name { get; set; }
        public List<question> questionList { get; set; }
        public List<votingSession> votingSessionList { get; set; }
    }
}
