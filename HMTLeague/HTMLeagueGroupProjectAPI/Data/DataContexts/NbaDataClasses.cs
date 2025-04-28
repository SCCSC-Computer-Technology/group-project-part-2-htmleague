using Microsoft.EntityFrameworkCore;

namespace HTMLeagueGroupProjectAPI.Data.DataModels
{
    public partial class NbaContext : DbContext
    {

        public NbaContext() { }
        public NbaContext(DbContextOptions<NbaContext> options) : base(options)
        {
        }

        public virtual DbSet<NbaPlayer> NbaPlayers { get; set; } = null;
        public virtual DbSet<NbaPlayerCareerStat> NbaPlayersCareerStat { get; set; } = null;
        public virtual DbSet<NbaTeam> NbaTeams { get; set; } = null;
        public virtual DbSet<NbaTeamSeasonStat> NbaTeamsSeasonStat { get; set; } = null;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NbaTeamSeasonStat>()
                .HasKey(s => new { s.TeamID, s.SeasonYear });
        }


    }
}