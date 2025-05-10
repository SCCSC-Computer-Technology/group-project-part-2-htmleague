namespace HTMLeagueGroupProjectAPI.Data.DataModels
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;

    [Table("CsgoPlayer")]
    public class CsgoPlayer
    {
        [Key]
        public int PlayerID { get; set; }
        [NotMapped]
        public int? TeamID { get; set; }
        [NotMapped]
        public string? TeamName { get; set; }
        [Required]
        [StringLength(50)]
        public string PlayerName { get; set; }
        public CsgoPlayerStat PlayerStat { get; set; }
        public ICollection<CsgoPlayerTeam> PlayerTeams { get; set; }
    }

    [Table("CsgoPlayerStat")]
    public class CsgoPlayerStat
    {
        [Key]
        public int PlayerID { get; set; }
        [NotMapped]
        public int? TeamID { get; set; }
        [NotMapped]
        public string? TeamName { get; set; }

        public int TotalMaps { get; set; }
        public int TotalRounds { get; set; }
        public int KdDiff { get; set; }
        [Column(TypeName = "decimal(4,2)")]
        public decimal Kd { get; set; }
        [Column(TypeName = "decimal(4,2)")]
        public decimal Rating { get; set; }
        public CsgoPlayer Player { get; set; }
    }

    [Table("CsgoPlayerTeam")]
    public class CsgoPlayerTeam
    {
        public int PlayerID { get; set; }
        public int TeamID { get; set; }
        public CsgoPlayer Player { get; set; }
        public CsgoTeam Team { get; set; }
    }

    [Table("CsgoTeam")]
    public class CsgoTeam
    {
        [Key]
        public int TeamID { get; set; }
        [Required]
        [StringLength(50)]
        public string TeamName { get; set; }
        public CsgoTeamStat TeamStat { get; set; }
        public ICollection<CsgoPlayerTeam> TeamPlayers { get; set; }
    }

    [Table("CsgoTeamStat")]
    public class CsgoTeamStat
    {
        [Key]
        public int TeamID { get; set; }
        public int TotalMaps { get; set; }
        public int KdDiff { get; set; }
        [Column(TypeName = "decimal(4,2)")]
        public decimal Kd { get; set; }
        [Column(TypeName = "decimal(4,2)")]
        public decimal Rating { get; set; }
        public CsgoTeam? Team { get; set; }
    }

    // DTOs (quick fix for validation errors)
    public record TeamStatUpdateDto(int TotalMaps, int KdDiff, decimal Kd, decimal Rating);

    public record CsgoPlayerStatUpdateDto(int TotalMaps, int TotalRounds, int KdDiff, decimal Kd, decimal Rating);

}
