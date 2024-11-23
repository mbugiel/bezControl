using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace liberumVeto.database.tables
{
    [Table("question")]
    public class question
    {
        [Key]
        public long id { get; set; }
        public string name { get; set; }
        public questionSet set { get; set; }
        public List<questionStatistic> statisticList { get; set; }
    }
}
