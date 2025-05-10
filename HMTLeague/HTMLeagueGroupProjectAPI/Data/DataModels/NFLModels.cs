namespace HTMLeagueGroupProjectAPI.Data.DataModels
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    [Table("NflPlayer")]
    public class NflPlayer
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
        public NflTeam Team { get; set; }
        public NflPlayerCareerFumbleStat CareerFumbleStat { get; set; }
        public NflPlayerCareerKickStat CareerKickStat { get; set; }
        public NflPlayerCareerPassStat CareerPassStat { get; set; }
        public NflPlayerCareerReceiveStat CareerReceiveStat { get; set; }
        public NflPlayerCareerRushStat CareerRushStat { get; set; }
        public NflPlayerCareerSackStat CareerSackStat { get; set; }
    }

    [Table("NflPlayerCareerFumbleStat")]
    public class NflPlayerCareerFumbleStat
    {
        [Key]
        public int PlayerID { get; set; }
        public int Fumbles { get; set; }
        public NflPlayer Player { get; set; }
    }

    [Table("NflPlayerCareerKickStat")]
    public class NflPlayerCareerKickStat
    {
        [Key]
        public int PlayerID { get; set; }
        public int FieldGoalAttempts { get; set; }
        public int FieldGoals { get; set; }
        public int ExtraPointAttempts { get; set; }
        public int ExtraPoints { get; set; }
        public NflPlayer Player { get; set; }
    }

    [Table("NflPlayerCareerPassStat")]
    public class NflPlayerCareerPassStat
    {
        [Key]
        public int PlayerID { get; set; }
        public int PassAttempts { get; set; }
        public int CompletePasses { get; set; }
        public int? PassingYards { get; set; }
        public int TdPasses { get; set; }
        public int Interceptions { get; set; }
        public int? LongestPass { get; set; }
        public NflPlayer Player { get; set; }
    }

    [Table("NflPlayerCareerReceiveStat")]
    public class NflPlayerCareerReceiveStat
    {
        [Key]
        public int PlayerID { get; set; }
        public int? Receptions { get; set; }
        public int? ReceivingYards { get; set; }
        public int? ReceivingTds { get; set; }
        public NflPlayer Player { get; set; }
    }

    [Table("NflPlayerCareerRushStat")]
    public class NflPlayerCareerRushStat
    {
        [Key]
        public int PlayerID { get; set; }
        public int RushAttempts { get; set; }
        public int? RushYards { get; set; }
        public int RushTds { get; set; }
        public int? RushFirstdowns { get; set; }
        public NflPlayer Player { get; set; }
    }

    [Table("NflPlayerCareerSackStat")]
    public class NflPlayerCareerSackStat
    {
        [Key]
        public int PlayerID { get; set; }
        public int Sacks { get; set; }
        public int? YardsLostToSacks { get; set; }
        public NflPlayer Player { get; set; }
    }

    [Table("NflTeam")]
    public class NflTeam
    {
        [Key]
        public int TeamID { get; set; }
        [Required]
        [StringLength(50)]
        public string TeamName { get; set; }
        public ICollection<NflPlayer> Players { get; set; }
        public ICollection<NflTeamSeasonStat> SeasonStats { get; set; }
    }

    [Table("NflTeamSeasonStat")]
    public class NflTeamSeasonStat
    {
        public int TeamID { get; set; }
        public int SeasonYear { get; set; }
        public int RushingTD { get; set; }
        public int ReceivingTD { get; set; }
        public int TotalTD { get; set; }
        public int TwoPoints { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Ties { get; set; }
        public int? FieldGoalsMade { get; set; }
        public int? FieldGoalAttempts { get; set; }
        public int? ExtraPointsMade { get; set; }
        public int? RushYards { get; set; }
        public int? PassingYards { get; set; }
        public int? Interceptions { get; set; }
        public NflTeam Team { get; set; }
    }

    // DTOs (quick fix for validation errors)

    public record NflTeamSeasonStatUpdateDto(
        int SeasonYear,
        int RushingTD,
        int ReceivingTD,
        int TotalTD,
        int TwoPoints,
        int Wins,
        int Losses,
        int Ties,
        int? FieldGoalsMade,
        int? FieldGoalAttempts,
        int? ExtraPointsMade,
        int? RushYards,
        int? PassingYards,
        int? Interceptions
    );

    // Fumbles
    public record NflFumbleStatUpdateDto(int Fumbles);

    // Kick
    public record NflKickStatUpdateDto(
        int FieldGoalAttempts,
        int FieldGoals,
        int ExtraPointAttempts,
        int ExtraPoints
    );

    // Pass
    public record NflPassStatUpdateDto(
        int PassAttempts,
        int CompletePasses,
        int PassingYards,
        int TdPasses,
        int Interceptions,
        int LongestPass
    );

    // Receive
    public record NflReceiveStatUpdateDto(
        int Receptions,
        int ReceivingYards,
        int ReceivingTds
    );

    // Rush
    public record NflRushStatUpdateDto(
        int RushAttempts,
        int RushYards,
        int RushTds,
        int RushFirstdowns
    );

    // Sack
    public record NflSackStatUpdateDto(
        int Sacks,
        int YardsLostToSacks
    );


}
