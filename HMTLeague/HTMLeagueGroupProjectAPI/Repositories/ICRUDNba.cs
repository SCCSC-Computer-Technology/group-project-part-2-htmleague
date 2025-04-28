using HTMLeagueGroupProjectAPI.Data.DataModels;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;

namespace HTMLeagueGroupProjectAPI.Repositories
{
    public interface ICRUDNba
    {
        Task<NbaPlayer?> CreateAsync(NbaPlayer? player);
        Task<NbaTeam?> CreateAsync(NbaTeam? team);
        Task<IEnumerable<NbaPlayer>> RetreieveAllPlayersAsync();
        Task<IEnumerable<NbaTeam>> RetrieveAllTeamsAsync();
        Task<IEnumerable<NbaPlayerCareerStat>> RetrievePlayerStatsAsync();
        Task<IEnumerable<NbaTeamSeasonStat>> RetrieveTeamSeasonStatsAsync();
        Task<NbaPlayer?> RetrievePlayer(int id);
        Task<NbaTeam?> RetrieveTeam(int id);
        Task<NbaPlayerCareerStat?> RetrivePlayerStat(int id);
        Task<NbaTeamSeasonStat?> RetrieveTeamStat(int id);
        Task<NbaPlayerCareerStat> UpdatePlayerStat(int id, NbaPlayerCareerStat? player);
        Task<NbaTeamSeasonStat> UpdateTeamStat(int id, NbaTeamSeasonStat? team);
        Task<bool?> DeletePlayer(int id);
        Task<bool?> DeleteTeam(int id);

    }
}
