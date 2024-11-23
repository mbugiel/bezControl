using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace liberumVeto.database.tables
{
    [Table("voting_session")]
    public class votingSession
    {
        [Key]
        public long id { get; set; }
        public questionSet set { get; set; }
        public DateTime votingDate { get; set; }
        public List<questionStatistic> statisticList { get; set; }
    }
}
