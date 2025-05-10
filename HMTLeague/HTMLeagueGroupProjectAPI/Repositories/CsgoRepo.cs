using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using HTMLeagueGroupProjectAPI.Data.DataModels;
using static HTMLeagueGroupProjectAPI.Controllers.CSGoController;

namespace HTMLeagueGroupProjectAPI.Repositories
{
    public class CsgoRepo : ICRUDCsgo
    {
        private  CsgoContext dataCache;

        private static ConcurrentDictionary<int, CsgoPlayer>? playerCache;

        private static ConcurrentDictionary<int,CsgoPlayerStat>? playerStatCache;

        private static ConcurrentDictionary<int,CsgoTeamStat>? teamStatCache;

        private static ConcurrentDictionary<int, CsgoTeam>? teamCache;

        private static ConcurrentDictionary<int, CsgoPlayerTeam> playerTeamCache;
        
        public CsgoRepo(CsgoContext context)
        {
            dataCache = context;

            if (playerCache is null)
            {
                playerCache = new ConcurrentDictionary<int, CsgoPlayer>(dataCache.CsgoPlayers.ToDictionary(p => p.PlayerID));
            }
            if(playerStatCache is null)
            {
                playerStatCache = new ConcurrentDictionary<int, CsgoPlayerStat>(dataCache.CsgoPlayerStats.ToDictionary(p => p.PlayerID));
            }
            if (teamCache is null)
            {
                teamCache = new ConcurrentDictionary<int, CsgoTeam>(dataCache.CsgoTeams.ToDictionary(t => t.TeamID));
            }
            if(teamStatCache is null)
            {
                teamStatCache = new ConcurrentDictionary<int, CsgoTeamStat>(dataCache.CsgoTeamStats.ToDictionary(t => t.TeamID));
            }
            if (playerTeamCache is null)
            {
                playerTeamCache = new ConcurrentDictionary<int, CsgoPlayerTeam>(
                    dataCache.CsgoPlayerTeams
                        .GroupBy(pt => pt.PlayerID)
                        .Select(g => g.First()) // Use only the very FIRST instance of each PlayerID. 
                        .ToDictionary(pt => pt.PlayerID)
                );
            }
        }

        public async Task<CsgoPlayer?> CreateAsync(CsgoPlayer player)
        {
            EntityEntry<CsgoPlayer> added = await dataCache.CsgoPlayers.AddAsync(player);

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
        private CsgoPlayer UpdatePlayerCache(int id, CsgoPlayer player)
        {
            CsgoPlayer? update;
            if (playerCache is not null)
            {
                if (playerCache.TryGetValue(id, out update))
                {
                    if (playerCache.TryUpdate(id, player, update))
                    {
                        return player;
                    }
                }
            }
            return null!;
        }

        public async Task<CsgoTeam> CreateAsync(CsgoTeam team)
        {
            EntityEntry<CsgoTeam> added = await dataCache.CsgoTeams.AddAsync(team);
            int change = await dataCache.SaveChangesAsync();
            if (change == 1)
            {
                if(teamCache == null)
                {
                    return team;
                }
                return teamCache.AddOrUpdate(team.TeamID, team, UpdateTeamCache);
            }
            else
            {
                return null!;
            }
        }
        private CsgoTeam UpdateTeamCache(int id,CsgoTeam team)
        {
            CsgoTeam? update;
            if(teamCache is not null)
            {
                if(teamCache.TryGetValue(id, out update))
                {
                    if (teamCache.TryUpdate(id, team, update))
                    {
                        return team;
                    }
                }
            }
            return null!;
        }
        public async Task<IEnumerable<CsgoPlayer>> RetrieveAllPlayersAsync()
        {
            return await Task.FromResult(playerCache is null ? Enumerable.Empty<CsgoPlayer>() : playerCache.Values);
        }
        public async Task<IEnumerable<CsgoPlayerStat>> RetrieveAllPlayersStatsAsync()
        {
            return await Task.FromResult(playerStatCache is null ? Enumerable.Empty<CsgoPlayerStat>() : playerStatCache.Values);
        }
        public async Task<IEnumerable<CsgoTeam>> RetrieveAllTeamsAsync()
        {
            return await Task.FromResult(teamCache is null ? Enumerable.Empty<CsgoTeam>() : teamCache.Values);
        }
        public async Task<IEnumerable<CsgoTeamStat>> RetrieveAllTeamStatsAsync()
        {
            return await Task.FromResult(teamStatCache is null ? Enumerable.Empty<CsgoTeamStat>() : teamStatCache.Values);
        }
        public async Task<CsgoPlayer?> RetrievePlayerAsync(int id)
        {
            if (playerCache is null)
            {
                return null!;
            }
            playerCache.TryGetValue(id, out CsgoPlayer? player);
            return await Task.FromResult(player);
        }
        public async Task<CsgoPlayerStat?> RetrievePlayerStatAsync(int id)
        {
            if(playerStatCache is null)
            {
                return null!;
            }
            playerStatCache.TryGetValue(id, out CsgoPlayerStat? playerStat);
            return await Task.FromResult(playerStat);
        }
        public async Task<CsgoTeam?> RetrieveTeamAsync(int id)
        {
            if (teamCache is null)
            {
                return null!;
            }
            teamCache.TryGetValue(id, out CsgoTeam? team);
            return await  Task.FromResult(team);
        }
        public async Task<CsgoTeamStat?> RetrieveTeamStatAsync(int id)
        {
            if (teamStatCache is null)
            {
                return null!;
            }
            teamStatCache.TryGetValue(id, out CsgoTeamStat? teamStat);
            return await Task.FromResult(teamStat);
        }
        public async Task<CsgoPlayerStat> UpdatePlayerStatAsync(int id, CsgoPlayerStat player)
        {
            
            dataCache.CsgoPlayerStats.Update(player);
            int updated = await dataCache.SaveChangesAsync();
            if (updated == 1)
            {
                return UpdatePlayerStatCache(id, player);
            }
            else
            {
                return null!;
            }
        }
        private CsgoPlayerStat UpdatePlayerStatCache(int id, CsgoPlayerStat player)
        {
            CsgoPlayerStat? updated;
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
        public async Task<CsgoTeamStat?> UpdateTeamStatAsync(int id, CsgoTeamStat entity)
        {
            dataCache.CsgoTeamStats.Update(entity);

            var updated = await dataCache.SaveChangesAsync();

            if (updated == 1)
            {
                return UpdateTeamStatCache(id, entity);
            }

            return null;

        }

        public CsgoTeamStat UpdateTeamStatCache(int id,CsgoTeamStat team)
        {
            CsgoTeamStat? updated;
            if(teamStatCache is not null)
            {
                if(teamStatCache.TryGetValue(id,out updated))
                {
                    if (teamStatCache.TryUpdate(id, team, updated))
                    {
                        return team;
                    }
                }
            }
            return null!;
        }
        public async Task<bool?> DeletePlayerAsync(int id) 
        {
            CsgoPlayer? player = dataCache.CsgoPlayers.Find(id);

            if(player == null)
            {
                return null!;
            }
            dataCache.CsgoPlayers.Remove(player);
            int change = await dataCache.SaveChangesAsync();
            if (change == 1)
            {
                if(playerCache is null)
                {
                    return null;
                }
                return playerCache.TryRemove(id, out player);
            }
            else
            {
                return null;
            }

        }
        public async Task<bool?> DeleteTeamAsync(int id)
        {
            CsgoTeam? team = dataCache.CsgoTeams.Find(id);

            if (team == null)
            {
                return null!;
            }
            dataCache.CsgoTeams.Remove(team);
            int change = await dataCache.SaveChangesAsync();

            if(change == 1)
            {
                if(teamCache is null)
                {
                    return null;
                }
                return teamCache.TryRemove(id, out team);
            }
            else
            {
                return null;
            }
        }
        public async Task<IEnumerable<CsgoPlayerTeam>> RetrieveAllPlayerTeamsAsync()
        {
            return await Task.FromResult(playerTeamCache is null ? Enumerable.Empty<CsgoPlayerTeam>() : playerTeamCache.Values);
        }

    }
}
