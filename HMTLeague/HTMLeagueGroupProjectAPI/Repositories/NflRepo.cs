using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections.Concurrent;
using System.Numerics;
using HTMLeagueGroupProjectAPI.Data.DataModels;
using Microsoft.EntityFrameworkCore;

namespace HTMLeagueGroupProjectAPI.Repositories
{
    public class NflRepo : ICRUDNfl
    {

        private NflContext dataCache;

        private static ConcurrentDictionary<int, NflPlayer>? playerCache;
        private static ConcurrentDictionary<int, NflTeam>? teamCache;
        private static ConcurrentDictionary<int, NflPlayerCareerFumbleStat>? fumbleCache;
        private static ConcurrentDictionary<int, NflPlayerCareerRushStat>? rushCache;
        private static ConcurrentDictionary<int, NflPlayerCareerKickStat>? kickCache;
        private static ConcurrentDictionary<int, NflPlayerCareerReceiveStat>? receiveCache;
        private static ConcurrentDictionary<int, NflPlayerCareerSackStat>? sackCache;
        private static ConcurrentDictionary<int, NflPlayerCareerPassStat>? passCache;
        private static ConcurrentDictionary<int, NflTeamSeasonStat>? teamStatCache;

        public NflRepo(NflContext context) 
        {
            dataCache = context;
            if(playerCache is null)
            {
                playerCache = new ConcurrentDictionary<int, NflPlayer>(
                    dataCache.NflPlayers
                             .Include(p => p.Team) // Ensure team is not null / is loaded
                             .ToDictionary(p => p.PlayerID)
                );
            }
            if (teamCache is null)
            {
                teamCache = new ConcurrentDictionary<int, NflTeam>(dataCache.NflTeams.ToDictionary(t => t.TeamID));
            }
            if (fumbleCache is null)
            {
                fumbleCache = new ConcurrentDictionary<int, NflPlayerCareerFumbleStat>(dataCache.NflPlayerCareerFumbleStats.ToDictionary(f => f.PlayerID));
            }
            if (rushCache is null)
            {
                rushCache = new ConcurrentDictionary<int, NflPlayerCareerRushStat>(dataCache.NflPlayerCareerRushStats.ToDictionary(r => r.PlayerID));
            }
            if (kickCache is null)
            {
                kickCache = new ConcurrentDictionary<int, NflPlayerCareerKickStat>(dataCache.NflPlayerCareerKickStats.ToDictionary(k => k.PlayerID));
            }
            if (receiveCache is null)
            {
                receiveCache = new ConcurrentDictionary<int, NflPlayerCareerReceiveStat>(dataCache.NflPlayerCareerReceiveStats.ToDictionary(rcv => rcv.PlayerID));
            }
            if (sackCache is null)
            {
                sackCache = new ConcurrentDictionary<int, NflPlayerCareerSackStat>(dataCache.NflPlayerCareerSackStats.ToDictionary(s => s.PlayerID));
            }
            if (passCache is null)
            {
                passCache = new ConcurrentDictionary<int, NflPlayerCareerPassStat>(dataCache.NflPlayerCareerPassStats.ToDictionary(p => p.PlayerID));
            }
            if (teamStatCache is null)
            {
                teamStatCache = new ConcurrentDictionary<int, NflTeamSeasonStat>(
                    dataCache.NflTeamSeasonStats
                             .GroupBy(t => t.TeamID)
                             .Select(g => g.OrderByDescending(t => t.SeasonYear).First())
                             .ToDictionary(t => t.TeamID)
                );
            }

        }

        //nfl player
        public async Task<NflPlayer?> CreatePlayer(NflPlayer player)
        {
            EntityEntry<NflPlayer> added = await dataCache.NflPlayers.AddAsync(player);

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
        private NflPlayer UpdatePlayerCache(int id, NflPlayer player)
        {
            NflPlayer? update;
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

        public async Task<IEnumerable<NflPlayer>> RetrievePlayers()
        {
            return await Task.FromResult(playerCache is null ? Enumerable.Empty<NflPlayer>() : playerCache.Values);
        }
        public async Task<NflPlayer?> RetrievePlayer(int id)
        {
            if (playerCache is null)
            {
                return null!;
            }
            playerCache.TryGetValue(id, out NflPlayer? player);
            return await Task.FromResult(player);
        }
        public async Task<bool?> DeletePlayer(int id)
        {
            NflPlayer? player = dataCache.NflPlayers.Find(id);

            if (player == null)
            {
                return null!;
            }
            dataCache.NflPlayers.Remove(player);
            int change = await dataCache.SaveChangesAsync();
            if (change == 1)
            {
                if (playerCache is null)
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

        //nfl team
        public async Task<NflTeam?> CreateTeam(NflTeam team)
        {
            EntityEntry<NflTeam> added = await dataCache.NflTeams.AddAsync(team);

            int change = await dataCache.SaveChangesAsync();
            if (change == 1)
            {
                if (teamCache == null)
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
        private NflTeam UpdateTeamCache(int id, NflTeam team)
        {
            NflTeam? update;
            if (teamCache is not null)
            {
                if (teamCache.TryGetValue(id, out update))
                {
                    if (teamCache.TryUpdate(id, team, update))
                    {
                        return team;
                    }
                }
            }
            return null!;
        }
        public async Task<NflTeam?> RetrieveTeam(int id)
        {
            if (teamCache is null)
            {
                return null!;
            }
            teamCache.TryGetValue(id, out NflTeam? team);
            return await Task.FromResult(team);
        }
        public async Task<IEnumerable<NflTeam>> RetrieveTeams()
        {
            return await Task.FromResult(teamCache is null ? Enumerable.Empty<NflTeam>() : teamCache.Values);
        }
        public async Task<bool?> DeleteTeam(int id)
        {
            NflTeam? team = dataCache.NflTeams.Find(id);

            if (team == null)
            {
                return null!;
            }
            dataCache.NflTeams.Remove(team);
            int change = await dataCache.SaveChangesAsync();
            if (change == 1)
            {
                if (playerCache is null)
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

        //player stats

        //fumble
        public async Task<NflPlayerCareerFumbleStat?> UpdateFumbleStat(int id, NflPlayerCareerFumbleStat? stat)
        {
            dataCache.NflPlayerCareerFumbleStats.Update(stat);
            int updated = await dataCache.SaveChangesAsync();
            if (updated == 1)
            {
                return UpdateFumbleStatCache(id, stat);
            }
            else
            {
                return null!;
            }
        }
        private NflPlayerCareerFumbleStat UpdateFumbleStatCache(int id, NflPlayerCareerFumbleStat stat)
        {
            NflPlayerCareerFumbleStat? updated;
            if (fumbleCache is not null)
            {
                if (fumbleCache.TryGetValue(id, out updated))
                {
                    if (fumbleCache.TryUpdate(id, stat, updated))
                    {
                        return stat;
                    }
                }
            }
            return null!;
        }
        public async Task<IEnumerable<NflPlayerCareerFumbleStat>> RetrieveAllFumble()
        {
            return await Task.FromResult(fumbleCache is null ? Enumerable.Empty<NflPlayerCareerFumbleStat>() : fumbleCache.Values);
        }
        public async Task<NflPlayerCareerFumbleStat?> RetrieveFumble(int id)
        {
            if (fumbleCache is null)
            {
                return null!;
            }
            fumbleCache.TryGetValue(id, out NflPlayerCareerFumbleStat? playerStat);
            return await Task.FromResult(playerStat);
        }

        //Rush
        public async Task<NflPlayerCareerRushStat?> UpdateRushStat(int id, NflPlayerCareerRushStat? stat)
        {
            dataCache.NflPlayerCareerRushStats.Update(stat);
            int updated = await dataCache.SaveChangesAsync();
            if (updated == 1)
            {
                return UpdateRushStatCache(id, stat);
            }
            else
            {
                return null!;
            }
        }
        private NflPlayerCareerRushStat UpdateRushStatCache(int id, NflPlayerCareerRushStat stat)
        {
            NflPlayerCareerRushStat? updated;
            if (rushCache is not null)
            {
                if (rushCache.TryGetValue(id, out updated))
                {
                    if (rushCache.TryUpdate(id, stat, updated))
                    {
                        return stat;
                    }
                }
            }
            return null!;
        }
        public async Task<IEnumerable<NflPlayerCareerRushStat>> RetrieveAllRush()
        {
            return await Task.FromResult(rushCache is null ? Enumerable.Empty<NflPlayerCareerRushStat>() : rushCache.Values);
        }
        public async Task<NflPlayerCareerRushStat?> RetrieveRush(int id)
        {
            if (rushCache is null)
            {
                return null!;
            }
            rushCache.TryGetValue(id, out NflPlayerCareerRushStat? playerStat);
            return await Task.FromResult(playerStat);
        }

        //Kick
        public async Task<NflPlayerCareerKickStat?> UpdateKickStat(int id, NflPlayerCareerKickStat? stat)
        {
            dataCache.NflPlayerCareerKickStats.Update(stat);
            int updated = await dataCache.SaveChangesAsync();
            if (updated == 1)
            {
                return UpdateKickStatCache(id, stat);
            }
            else
            {
                return null!;
            }
        }
        private NflPlayerCareerKickStat UpdateKickStatCache(int id, NflPlayerCareerKickStat stat)
        {
            NflPlayerCareerKickStat? updated;
            if (kickCache is not null)
            {
                if (kickCache.TryGetValue(id, out updated))
                {
                    if (kickCache.TryUpdate(id, stat, updated))
                    {
                        return stat;
                    }
                }
            }
            return null!;
        }
        public async Task<IEnumerable<NflPlayerCareerKickStat>> RetrieveAllKick()
        {
            return await Task.FromResult(kickCache is null ? Enumerable.Empty<NflPlayerCareerKickStat>() : kickCache.Values);
        }
        public async Task<NflPlayerCareerKickStat?> RetrieveKick(int id)
        {
            if (kickCache is null)
            {
                return null!;
            }
            kickCache.TryGetValue(id, out NflPlayerCareerKickStat? playerStat);
            return await Task.FromResult(playerStat);
        }

        //Recieve

        public async Task<NflPlayerCareerReceiveStat?> UpdateReceiveStat(int id, NflPlayerCareerReceiveStat? stat)
        {
            dataCache.NflPlayerCareerReceiveStats.Update(stat);
            int updated = await dataCache.SaveChangesAsync();
            if (updated == 1)
            {
                return UpdateReceiveStatCache(id, stat);
            }
            else
            {
                return null!;
            }
        }
        private NflPlayerCareerReceiveStat UpdateReceiveStatCache(int id, NflPlayerCareerReceiveStat stat)
        {
            NflPlayerCareerReceiveStat? updated;
            if (receiveCache is not null)
            {
                if (receiveCache.TryGetValue(id, out updated))
                {
                    if (receiveCache.TryUpdate(id, stat, updated))
                    {
                        return stat;
                    }
                }
            }
            return null!;
        }
        public async Task<IEnumerable<NflPlayerCareerReceiveStat>> RetrieveAllReceive()
        {
            return await Task.FromResult(receiveCache is null ? Enumerable.Empty<NflPlayerCareerReceiveStat>() : receiveCache.Values);
        }
        public async Task<NflPlayerCareerReceiveStat?> RetrieveReceive(int id)
        {
            if (receiveCache is null)
            {
                return null!;
            }
            receiveCache.TryGetValue(id, out NflPlayerCareerReceiveStat? playerStat);
            return await Task.FromResult(playerStat);
        }

        //Sack
        public async Task<NflPlayerCareerSackStat?> UpdateSackStat(int id, NflPlayerCareerSackStat? stat)
        {
            dataCache.NflPlayerCareerSackStats.Update(stat);
            int updated = await dataCache.SaveChangesAsync();
            if (updated == 1)
            {
                return UpdateSackStatCache(id, stat);
            }
            else
            {
                return null!;
            }
        }
        private NflPlayerCareerSackStat UpdateSackStatCache(int id, NflPlayerCareerSackStat stat)
        {
            NflPlayerCareerSackStat? updated;
            if (sackCache is not null)
            {
                if (sackCache.TryGetValue(id, out updated))
                {
                    if (sackCache.TryUpdate(id, stat, updated))
                    {
                        return stat;
                    }
                }
            }
            return null!;
        }
        public async Task<IEnumerable<NflPlayerCareerSackStat>> RetrieveAllSack()
        {
            return await Task.FromResult(sackCache is null ? Enumerable.Empty<NflPlayerCareerSackStat>() : sackCache.Values);
        }
        public async Task<NflPlayerCareerSackStat?> RetrieveSack(int id)
        {
            if (sackCache is null)
            {
                return null!;
            }
            sackCache.TryGetValue(id, out NflPlayerCareerSackStat? playerStat);
            return await Task.FromResult(playerStat);
        }

        //pass
        public async Task<NflPlayerCareerPassStat?> UpdatePassStat(int id, NflPlayerCareerPassStat? stat)
        {
            dataCache.NflPlayerCareerPassStats.Update(stat);
            int updated = await dataCache.SaveChangesAsync();
            if (updated == 1)
            {
                return UpdatePassStatCache(id, stat);
            }
            else
            {
                return null!;
            }
        }
        private NflPlayerCareerPassStat UpdatePassStatCache(int id, NflPlayerCareerPassStat stat)
        {
            NflPlayerCareerPassStat? updated;
            if (passCache is not null)
            {
                if (passCache.TryGetValue(id, out updated))
                {
                    if (passCache.TryUpdate(id, stat, updated))
                    {
                        return stat;
                    }
                }
            }
            return null!;
        }
        public async Task<IEnumerable<NflPlayerCareerPassStat>> RetrieveAllPass()
        {
            return await Task.FromResult(passCache is null ? Enumerable.Empty<NflPlayerCareerPassStat>() : passCache.Values);
        }
        public async Task<NflPlayerCareerPassStat?> RetrievePass(int id)
        {
            if (passCache is null)
            {
                return null!;
            }
            passCache.TryGetValue(id, out NflPlayerCareerPassStat? playerStat);
            return await Task.FromResult(playerStat);
        }

        //Team Stat
        public async Task<NflTeamSeasonStat?> UpdateTeamStat(int id, NflTeamSeasonStat? stat)
        {
            dataCache.NflTeamSeasonStats.Update(stat);
            int updated = await dataCache.SaveChangesAsync();
            if (updated == 1)
            {
                return UpdateTeamStatCache(id, stat);
            }
            else
            {
                return null!;
            }
        }
        private NflTeamSeasonStat UpdateTeamStatCache(int id, NflTeamSeasonStat stat)
        {
            NflTeamSeasonStat? updated;
            if (teamStatCache is not null)
            {
                if (teamStatCache.TryGetValue(id, out updated))
                {
                    if (teamStatCache.TryUpdate(id, stat, updated))
                    {
                        return stat;
                    }
                }
            }
            return null!;
        }
        public async Task<IEnumerable<NflTeamSeasonStat>> RetrieveAllTeamStat()
        {
            return await Task.FromResult(teamStatCache is null ? Enumerable.Empty<NflTeamSeasonStat>() : teamStatCache.Values);
        }
        public async Task<NflTeamSeasonStat?> RetrieveTeamStat(int id)
        {
            if (teamStatCache is null)
            {
                return null!;
            }
            teamStatCache.TryGetValue(id, out NflTeamSeasonStat? playerStat);
            return await Task.FromResult(playerStat);
        }
    }
}
