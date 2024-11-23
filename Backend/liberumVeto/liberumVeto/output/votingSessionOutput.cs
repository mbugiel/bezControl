namespace liberumVeto.output
{
    public class votingSessionOutput
    {
        public string setName {  get; set; }
        public DateTime date { get; set; }
        public List<questionStatisticOutput> statisticList { get; set; }
    }
}
