using HTMLeagueGroupProjectAPI.Data.DataModels;

namespace HTMLeagueGroupProjectAPI.Repositories
{
    public interface ICRUDNfl
    {
        //nfl player
        Task<NflPlayer?> CreatePlayer(NflPlayer player);
        Task<IEnumerable<NflPlayer>> RetrievePlayers();
        Task<NflPlayer?> RetrievePlayer(int id);
        Task<bool?> DeletePlayer(int id);

        //nfl team
        Task<NflTeam?> CreateTeam(NflTeam team);
        Task<NflTeam?> RetrieveTeam(int id);
        Task<IEnumerable<NflTeam>> RetrieveTeams();
        Task<bool?> DeleteTeam(int id);

        //player stats

        //fumble
        Task<NflPlayerCareerFumbleStat?> UpdateFumbleStat(int id , NflPlayerCareerFumbleStat stat);
        Task<IEnumerable<NflPlayerCareerFumbleStat>> RetrieveAllFumble();
        Task<NflPlayerCareerFumbleStat?> RetrieveFumble(int id);

        //Rush
        Task<NflPlayerCareerRushStat?> UpdateRushStat(int id, NflPlayerCareerRushStat stat);
        Task<IEnumerable<NflPlayerCareerRushStat>> RetrieveAllRush();
        Task<NflPlayerCareerRushStat?> RetrieveRush(int id);

        //Kick
        Task<NflPlayerCareerKickStat?> UpdateKickStat(int id, NflPlayerCareerKickStat stat);
        Task<IEnumerable<NflPlayerCareerKickStat>> RetrieveAllKick();
        Task<NflPlayerCareerKickStat?> RetrieveKick(int id);

        //Recieve

        Task<NflPlayerCareerReceiveStat?> UpdateReceiveStat(int id, NflPlayerCareerReceiveStat stat);
        Task<IEnumerable<NflPlayerCareerReceiveStat>> RetrieveAllReceive();
        Task<NflPlayerCareerReceiveStat?> RetrieveReceive(int id);

        //Sack
        Task<NflPlayerCareerSackStat?> UpdateSackStat(int id, NflPlayerCareerSackStat stat);
        Task<IEnumerable<NflPlayerCareerSackStat>> RetrieveAllSack();
        Task<NflPlayerCareerSackStat?> RetrieveSack(int id);

        //pass
        Task<NflPlayerCareerPassStat?> UpdatePassStat(int id, NflPlayerCareerPassStat stat);
        Task<IEnumerable<NflPlayerCareerPassStat>> RetrieveAllPass();
        Task<NflPlayerCareerPassStat?> RetrievePass(int id);

        //Team Stat
        Task<NflTeamSeasonStat?> UpdateTeamStat(int id, NflTeamSeasonStat stat);
        Task<IEnumerable<NflTeamSeasonStat>> RetrieveAllTeamStat();
        Task<NflTeamSeasonStat?> RetrieveTeamStat(int id);

    }
}
