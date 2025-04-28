using HTMLeagueGroupProjectAPI.Data.DataModels;

namespace HTMLeagueGroupProjectAPI.Repositories
{
    public interface ICRUDCsgo
    {
        Task<CsgoPlayer?> CreateAsync(CsgoPlayer player);
        Task<CsgoTeam> CreateAsync(CsgoTeam team);
        Task<IEnumerable<CsgoPlayer>> RetrieveAllPlayersAsync();
        Task<IEnumerable<CsgoPlayerStat>> RetrieveAllPlayersStatsAsync();
        Task<IEnumerable<CsgoTeam>> RetrieveAllTeamsAsync();
        Task<IEnumerable<CsgoTeamStat>> RetrieveAllTeamStatsAsync();
        Task<CsgoPlayer?> RetrievePlayerAsync(int id);
        Task<CsgoPlayerStat?> RetrievePlayerStatAsync(int id);
        Task<CsgoTeam?> RetrieveTeamAsync(int id);
        Task<CsgoTeamStat?> RetrieveTeamStatAsync(int id);
        Task<CsgoPlayerStat> UpdatePlayerStatAsync(int id, CsgoPlayerStat player);
        Task<CsgoTeamStat?> UpdateTeamStatAsync(int id, CsgoTeamStat entity);
        Task<bool?> DeletePlayerAsync(int id);
        Task<bool?> DeleteTeamAsync(int id);

        Task<IEnumerable<CsgoPlayerTeam>> RetrieveAllPlayerTeamsAsync();

    }
}
