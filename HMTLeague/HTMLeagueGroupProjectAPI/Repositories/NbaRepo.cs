using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections.Concurrent;
using System.Numerics;
using HTMLeagueGroupProjectAPI.Data.DataModels;

namespace HTMLeagueGroupProjectAPI.Repositories
{
    public class NbaRepo : ICRUDNba
    {
        private NbaContext dataCache;

        private static ConcurrentDictionary<int, NbaPlayer>? playerCache;
        private static ConcurrentDictionary<int, NbaTeam>? teamCache;
        private static ConcurrentDictionary<int, NbaPlayerCareerStat>? playerStatCache;
        private static ConcurrentDictionary<int,NbaTeamSeasonStat>? teamStatCache;
        public NbaRepo(NbaContext context)
        {
            dataCache = context;
            if (playerCache is null)
            {
                playerCache = new ConcurrentDictionary<int, NbaPlayer>(dataCache.NbaPlayers.ToDictionary(p => p.PlayerID));
            }
            if (playerStatCache is null)
            {
                playerStatCache = new ConcurrentDictionary<int, NbaPlayerCareerStat>(
                    dataCache.NbaPlayersCareerStat
                        .GroupBy(p => p.PlayerID)
                        .Select(g => g.First())
                        .ToDictionary(p => p.PlayerID)
                        );
            }
            if (teamCache is null)
            {
                teamCache = new ConcurrentDictionary<int, NbaTeam>(dataCache.NbaTeams.ToDictionary(t => t.TeamID));
            }
            if (teamStatCache is null)
            {
                teamStatCache = new ConcurrentDictionary<int, NbaTeamSeasonStat>(
                    dataCache.NbaTeamsSeasonStat
                        .GroupBy(t => t.TeamID)
                        .Select(g => g.OrderByDescending(t => t.SeasonYear).First())
                        .ToDictionary(t => t.TeamID)
                );
            }

        }


        public async Task<NbaPlayer?> CreateAsync(NbaPlayer? player)
        {
            EntityEntry<NbaPlayer> added = await dataCache.NbaPlayers.AddAsync(player);

            int change = await dataCache.SaveChangesAsync();
            if (change == 1)
            {
                if (playerCache == null)
                {
                    return player;
                }
                return playerCache.AddOrUpdate(player.PlayerID, player, UpdatePlayerCache);
            }
            else
            {
                return null;
            }
        }
        private NbaPlayer UpdatePlayerCache(int id,NbaPlayer player)
        {
            NbaPlayer? update;
            if(playerCache is not null)
            {
                if(playerCache.TryGetValue(id, out update))
                {
                    if (playerCache.TryUpdate(id, player, update))
                    {
                        return player;
                    }
                }
            }
            return null!;
        }

        public async Task<NbaTeam?> CreateAsync(NbaTeam? team)
        {
            EntityEntry<NbaTeam> added = await dataCache.NbaTeams.AddAsync(team);

            int change = await dataCache.SaveChangesAsync();
            if(change == 1)
            {
                if(playerCache == null)
                {
                    return team;
                }
                return teamCache.AddOrUpdate(team.TeamID, team, UpdateTeamCache);
            }
            else
            {
                return null;
            }
        }
        public NbaTeam UpdateTeamCache(int id, NbaTeam team)
        {
            NbaTeam? update;
            if(teamCache is not null)
            {
                if(teamCache.TryGetValue(id,out update))
                {
                    if (teamCache.TryUpdate(id, team, update))
                    {
                        return team;
                    }
                }
            }
            return null!;
        }
        public async Task<IEnumerable<NbaPlayer>> RetreieveAllPlayersAsync()
        {
            return await Task.FromResult(playerCache is null ? Enumerable.Empty<NbaPlayer>() : playerCache.Values);
        }
        public async Task<IEnumerable<NbaTeam>> RetrieveAllTeamsAsync()
        {
            return await  Task.FromResult(teamCache is null ? Enumerable.Empty<NbaTeam>() : teamCache.Values);
        }
        public async Task<IEnumerable<NbaPlayerCareerStat>> RetrievePlayerStatsAsync()
        {
            return await Task.FromResult(playerStatCache is null ? Enumerable.Empty<NbaPlayerCareerStat>() : playerStatCache.Values);
        }
        public async Task<IEnumerable<NbaTeamSeasonStat>> RetrieveTeamSeasonStatsAsync()
        {
            return await Task.FromResult(teamStatCache is null ? Enumerable.Empty<NbaTeamSeasonStat>() : teamStatCache.Values);
        }
        public async Task<NbaPlayer?> RetrievePlayer(int id)
        {
            if(playerCache is null)
            {
                return null!;
            }
            playerCache.TryGetValue(id,out NbaPlayer? player);
            return await Task.FromResult(player);
        }
        public async Task<NbaTeam?> RetrieveTeam(int id)
        {
            if (teamCache is null)
            {
                return null!;
            }
            teamCache.TryGetValue(id, out NbaTeam? team);
            return  await Task.FromResult(team);
        }
        public async Task<NbaPlayerCareerStat?> RetrivePlayerStat(int id)
        {
            if (playerStatCache is null)
            {
                return null!;
            }
            playerStatCache.TryGetValue(id, out NbaPlayerCareerStat? player);
            return await Task.FromResult(player);
        }
        public async Task<NbaTeamSeasonStat?> RetrieveTeamStat(int id)
        {
            if (teamStatCache is null)
            {
                return null!;
            }
            teamStatCache.TryGetValue(id, out NbaTeamSeasonStat team);
            return await Task.FromResult(team);
        }
        public async Task<NbaPlayerCareerStat> UpdatePlayerStat(int id, NbaPlayerCareerStat? player)
        {
            dataCache.NbaPlayersCareerStat.Update(player);
            int updated = await dataCache.SaveChangesAsync();
            if(updated == 1)
            {
                return UpdatePlayerStatCache(id, player);
            }
            else
            {
                return null!;
            }
        }
        private NbaPlayerCareerStat UpdatePlayerStatCache(int id, NbaPlayerCareerStat player)
        {
            NbaPlayerCareerStat? updated;
            if(playerStatCache is not null)
            {
                if(playerStatCache.TryGetValue(id,out updated))
                {
                    if (playerStatCache.TryUpdate(id, player, updated))
                    {
                        return player;
                    }
                }
            }
            return null!;
        }
        public async Task<NbaTeamSeasonStat> UpdateTeamStat(int id, NbaTeamSeasonStat? team)
        {
            dataCache.NbaTeamsSeasonStat.Update(team);
            int updated = await dataCache.SaveChangesAsync();
            if (updated == 1)
            {
                return UpdateTeamStatCache(id, team);
            }
            else
            {
                return null!;
            }
        }
        private NbaTeamSeasonStat UpdateTeamStatCache(int id, NbaTeamSeasonStat team)
        {
            NbaTeamSeasonStat? updated;
            if (teamStatCache is not null)
            {
                if (teamStatCache.TryGetValue(id, out updated))
                {
                    if (teamStatCache.TryUpdate(id, team, updated))
                    {
                        return team;
                    }
                }
            }
            return null!;
        }
        public async Task<bool?> DeletePlayer(int id)
        {
            NbaPlayer? player = dataCache.NbaPlayers.Find(id);

            if (player == null)
            {
                return null;
            }
            dataCache.NbaPlayers.Remove(player);
            int change = await dataCache.SaveChangesAsync();
            if(change == 1)
            {
                if(playerCache is null)
                {
                    return null;
                }
                return playerCache.TryRemove(id, out player);
            }
            else
            {
                return null!;
            }
        }
        public async Task<bool?> DeleteTeam(int id)
        {
            NbaTeam? team= dataCache.NbaTeams.Find(id);

            if (team == null)
            {
                return null;
            }
            dataCache.NbaTeams.Remove(team);
            int change = await dataCache.SaveChangesAsync();
            if (change == 1)
            {
                if (teamCache is null)
                {
                    return null;
                }
                return teamCache.TryRemove(id, out team);
            }
            else
            {
                return null!;
            }
        }
    }
    
}
