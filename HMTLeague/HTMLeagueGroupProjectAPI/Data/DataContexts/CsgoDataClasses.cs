using Microsoft.EntityFrameworkCore;

namespace HTMLeagueGroupProjectAPI.Data.DataModels
{
    public partial class CsgoContext : DbContext
    {
        public CsgoContext() { }
        public CsgoContext(DbContextOptions<CsgoContext> options): base(options)
        { }

        public virtual DbSet<CsgoPlayer> CsgoPlayers { get; set; } = null;
        public virtual DbSet<CsgoTeam> CsgoTeams { get; set;} = null;
        public virtual DbSet<CsgoPlayerStat> CsgoPlayerStats { get; set; } = null;
        public virtual DbSet<CsgoPlayerTeam> CsgoPlayerTeams { get; set; } = null;
        public virtual DbSet<CsgoTeamStat> CsgoTeamStats { get; set; } = null;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CsgoPlayerTeam>()
                .HasKey(pt => new { pt.PlayerID, pt.TeamID });
        }

    }
}