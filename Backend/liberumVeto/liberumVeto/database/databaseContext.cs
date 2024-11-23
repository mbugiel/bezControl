using liberumVeto.database.tables;
using Microsoft.EntityFrameworkCore;

namespace liberumVeto.database
{
    public class databaseContext :DbContext
    {
        public databaseContext(DbContextOptions<databaseContext> options): base(options) { }

        public DbSet<question> question { get; set; }
        public DbSet<questionSet> questionSet { get; set; }
        public DbSet<questionStatistic> questionStatistic { get; set; }
        public DbSet<votingSession> votingSession { get; set; }

    }
}
