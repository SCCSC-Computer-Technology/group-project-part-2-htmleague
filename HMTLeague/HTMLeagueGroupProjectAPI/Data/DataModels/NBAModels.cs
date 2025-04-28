namespace HTMLeagueGroupProjectAPI.Data.DataModels
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;

    [Table("NbaPlayer")]
    public class NbaPlayer
    {
        [Key]
        public int PlayerID { get; set; }
        public int? TeamID { get; set; }
        [StringLength(50)]
        public string FirstName { get; set; }
        [StringLength(50)]
        public string LastName { get; set; }
        public bool? IsActive { get; set; }
        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";
        public NbaTeam Team { get; set; }
        public NbaPlayerCareerStat CareerStat { get; set; }
    }

    [Table("NbaPlayerCareerStat")]
    public class NbaPlayerCareerStat
    {
        [Key]
        public int PlayerID { get; set; }
        public int? FieldGoals { get; set; }
        public int? FieldGoalAttempts { get; set; }
        public int? ThreePoints { get; set; }
        public int? ThreePointAttempts { get; set; }
        public int? TwoPoints { get; set; }
        public int? TwoPointsAttempts { get; set; }
        public int? FreeThrows { get; set; }
        public int? FreeThrowAttempts { get; set; }
        public int? OffensiveRebounds { get; set; }
        public int? DefensiveRebounds { get; set; }
        public int? Assists { get; set; }
        public int? Steals { get; set; }
        public int? Blocks { get; set; }
        public int? Turnovers { get; set; }
        public int? PersonalFouls { get; set; }
        public int? TotalPoints { get; set; }
        public NbaPlayer Player { get; set; }
    }

    [Table("NbaTeam")]
    public class NbaTeam
    {
        [Key]
        public int TeamID { get; set; }
        [StringLength(50)]
        public string TeamName { get; set; }
        [StringLength(3)]
        [Column(TypeName = "char(3)")]
        public string TeamAbreviation { get; set; }
        public ICollection<NbaPlayer> Players { get; set; }
        public ICollection<NbaTeamSeasonStat> SeasonStats { get; set; }
    }

    [Table("NbaTeamSeasonStat")]
    public class NbaTeamSeasonStat
    {
        public int TeamID { get; set; }
        public int SeasonYear { get; set; }
        public int? FieldGoals { get; set; }
        public int? FieldGoalAttempts { get; set; }
        public int? ThreePoints { get; set; }
        public int? ThreePointAttempts { get; set; }
        public int? TwoPoints { get; set; }
        public int? TwoPointAttempts { get; set; }
        public int? FreeThrows { get; set; }
        public int? FreeThrowAttempts { get; set; }
        public int? OffensiveRebounds { get; set; }
        public int? DefensiveRebounds { get; set; }
        public int? Assists { get; set; }
        public int? Steals { get; set; }
        public int? Blocks { get; set; }
        public int? Turnovers { get; set; }
        public int? PersonalFouls { get; set; }
        public int? TotalPoints { get; set; }
        public NbaTeam Team { get; set; }
    }

    // DTOs (quick fix for validation errors)
    public record NbaTeamSeasonStatUpdateDto(
        int SeasonYear,
        int FieldGoals,
        int FieldGoalAttempts,
        int ThreePoints,
        int ThreePointAttempts,
        int TwoPoints,
        int TwoPointAttempts,
        int FreeThrows,
        int FreeThrowAttempts,
        int OffensiveRebounds,
        int DefensiveRebounds,
        int Assists,
        int Steals,
        int Blocks,
        int Turnovers,
        int PersonalFouls,
        int TotalPoints
    );

    public record NbaPlayerCareerStatUpdateDto(
        int FieldGoals,
        int FieldGoalAttempts,
        int ThreePoints,
        int ThreePointAttempts,
        int TwoPoints,
        int TwoPointsAttempts,
        int FreeThrows,
        int FreeThrowAttempts,
        int OffensiveRebounds,
        int DefensiveRebounds,
        int Assists,
        int Steals,
        int Blocks,
        int Turnovers,
        int PersonalFouls,
        int TotalPoints
    );

}
