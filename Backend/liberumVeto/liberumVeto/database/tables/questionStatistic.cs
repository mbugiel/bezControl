using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace liberumVeto.database.tables
{
    [Table("question_statistic")]
    public class questionStatistic
    {
        [Key]
        public long id { get; set; }
        public votingSession votingSession { get; set; }
        public question question { get; set; }
        public int forQuantity {  get; set; }
        public int againstQuantity { get; set; }
    }
}
