using Microsoft.EntityFrameworkCore;

namespace HTMLeagueGroupProjectAPI.Data.DataModels
{
    public partial class NflContext : DbContext
    {
        public NflContext() { }

        public NflContext(DbContextOptions<NflContext> options) : base(options) { }


        public virtual DbSet<NflPlayer> NflPlayers { get; set; } = null;
        public virtual DbSet<NflPlayerCareerKickStat> NflPlayerCareerKickStats {  get; set; } = null;    
        public virtual DbSet<NflPlayerCareerFumbleStat> NflPlayerCareerFumbleStats { get; set; } = null;
        public virtual DbSet<NflPlayerCareerRushStat> NflPlayerCareerRushStats { get; set; } = null;
        public virtual DbSet<NflPlayerCareerSackStat> NflPlayerCareerSackStats { get; set; } = null;
        public virtual DbSet<NflPlayerCareerPassStat> NflPlayerCareerPassStats { get; set; } = null;
        public virtual DbSet<NflPlayerCareerReceiveStat> NflPlayerCareerReceiveStats { get; set; } = null;
        public virtual DbSet<NflTeam> NflTeams { get; set; } = null;
        public virtual DbSet<NflTeamSeasonStat> NflTeamSeasonStats { get; set;} = null;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<NflTeamSeasonStat>()
                .HasKey(t => new { t.TeamID, t.SeasonYear }); // composite primary key
        }

    }
}