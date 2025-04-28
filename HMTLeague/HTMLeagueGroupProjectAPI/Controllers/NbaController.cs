using HTMLeagueGroupProjectAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using HTMLeagueGroupProjectAPI.Data.DataModels;

namespace HTMLeagueGroupProjectAPI.Controllers
{
    [ApiController]
    [Route("api/NBA")]
    public class NbaController : ControllerBase
    {
        private readonly ICRUDNba repo;

        public NbaController(ICRUDNba repo)
        {
            this.repo = repo;
        }

        [HttpPost("CreatePlayer")]
        [ProducesResponseType(201, Type = typeof(NbaPlayer))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreatePlayer([FromBody] NbaPlayer player)
        {
            if (player == null)
            {
                return BadRequest();
            }
            NbaPlayer? addedPlayer = await repo.CreateAsync(player);
            if (addedPlayer == null)
            {
                return BadRequest("Failed to create new Csgo PLayer");
            }
            else
            {
                return CreatedAtRoute(routeName: nameof(GetPlayer), routeValues: new { id = addedPlayer.PlayerID }, value: addedPlayer);
            }
        }

        [HttpPost("CreateTeam")]
        [ProducesResponseType(201, Type = typeof(NbaTeam))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateTeam([FromBody] NbaTeam team)
        {
            if (team == null)
            {
                return BadRequest();
            }
            NbaTeam? addedTeam = await repo.CreateAsync(team);
            if (addedTeam == null)
            {
                return BadRequest("Failed to create new Csgo PLayer");
            }
            else
            {
                return CreatedAtRoute(routeName: nameof(GetTeam), routeValues: new { id = addedTeam.TeamID }, value: addedTeam);
            }
        }

        [HttpGet("Players")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<NbaPlayer>))]
        public async Task<IEnumerable<NbaPlayer>> GetPlayers()
        {
            return await repo.RetreieveAllPlayersAsync();
        }

        [HttpGet("Teams")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<NbaTeam>))]
        public async Task<IEnumerable<NbaTeam>> GetTeams()
        {
            return await repo.RetrieveAllTeamsAsync();
        }

        [HttpGet("Players/Stats")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<NbaPlayerCareerStat>))]
        public async Task<IEnumerable<NbaPlayerCareerStat>> GetPlayerStats()
        {
            return await repo.RetrievePlayerStatsAsync();
        }

        [HttpGet("Teams/Stats")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<NbaTeamSeasonStat>))]
        public async Task<IEnumerable<NbaTeamSeasonStat>> GetTeamStats()
        {
            return await repo.RetrieveTeamSeasonStatsAsync();
        }

        [HttpGet("Player/{id}")]
        [ProducesResponseType(200, Type = typeof(NbaPlayer))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetPlayer(int id)
        {
            NbaPlayer? player = await repo.RetrievePlayer(id);
            if (player == null)
            {
                return NotFound();
            }
            return Ok(player);
        }

        [HttpGet("Team/{id}")]
        [ProducesResponseType(200, Type = typeof(NbaTeam))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetTeam(int id)
        {
            NbaTeam? team = await repo.RetrieveTeam(id);
            if (team == null)
            {
                return NotFound();
            }
            return Ok(team);
        }

        [HttpGet("PlayerStat/{id}")]
        [ProducesResponseType(200, Type = typeof(NbaPlayerCareerStat))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetPlayerStat(int id)
        {
            NbaPlayerCareerStat? player = await repo.RetrivePlayerStat(id);
            if (player == null)
            {
                return NotFound();
            }
            return Ok(player);
        }

        [HttpGet("Team/Stats/{id}")]
        [ProducesResponseType(200, Type = typeof(NbaTeamSeasonStat))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetTeamStat(int id)
        {
            NbaTeamSeasonStat? team = await repo.RetrieveTeamStat(id);
            if (team == null)
            {
                return NotFound();
            }
            return Ok(team);
        }

        [HttpPut("UpdatePlayerStat/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdatePlayerStat(
            int id,
            [FromBody] NbaPlayerCareerStatUpdateDto dto)
        {

            // Get existing player stat object
            NbaPlayerCareerStat existingPlayerStat = await repo.RetrivePlayerStat(id);

            // Return if player stat doesn't even exist
            if (existingPlayerStat is null) return NotFound();

            // Replace existing
            existingPlayerStat.FieldGoals = dto.FieldGoals;
            existingPlayerStat.FieldGoalAttempts = dto.FieldGoalAttempts;
            existingPlayerStat.ThreePoints = dto.ThreePoints;
            existingPlayerStat.ThreePointAttempts = dto.ThreePointAttempts;
            existingPlayerStat.TwoPoints = dto.TwoPoints;
            existingPlayerStat.TwoPointsAttempts = dto.TwoPointsAttempts;
            existingPlayerStat.FreeThrows = dto.FreeThrows;
            existingPlayerStat.FreeThrowAttempts = dto.FreeThrowAttempts;
            existingPlayerStat.OffensiveRebounds = dto.OffensiveRebounds;
            existingPlayerStat.DefensiveRebounds = dto.DefensiveRebounds;
            existingPlayerStat.Assists = dto.Assists;
            existingPlayerStat.Steals = dto.Steals;
            existingPlayerStat.Blocks = dto.Blocks;
            existingPlayerStat.Turnovers = dto.Turnovers;
            existingPlayerStat.PersonalFouls = dto.PersonalFouls;
            existingPlayerStat.TotalPoints = dto.TotalPoints;

            // Send back over
            await repo.UpdatePlayerStat(id, existingPlayerStat);

            return NoContent();
        }



        // Had to switch to DTOs for this part because of validation errors (requiring a Team object).
        // If there was more time this could maybe be handled a bit better.
        // Update
        [HttpPut("UpdateTeamStat/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateTeamStat(
            int id,
            [FromBody] NbaTeamSeasonStatUpdateDto dto)
        {

            // Get existing team stat object
            NbaTeamSeasonStat existingTeamStat = await repo.RetrieveTeamStat(id);

            // Return if team stat doesn't even exist
            if (existingTeamStat is null) return NotFound();

            // Replace existing
            existingTeamStat.SeasonYear = dto.SeasonYear;
            existingTeamStat.FieldGoals = dto.FieldGoals;
            existingTeamStat.FieldGoalAttempts = dto.FieldGoalAttempts;
            existingTeamStat.ThreePoints = dto.ThreePoints;
            existingTeamStat.ThreePointAttempts = dto.ThreePointAttempts;
            existingTeamStat.TwoPoints = dto.TwoPoints;
            existingTeamStat.TwoPointAttempts = dto.TwoPointAttempts;
            existingTeamStat.FreeThrows = dto.FreeThrows;
            existingTeamStat.FreeThrowAttempts = dto.FreeThrowAttempts;
            existingTeamStat.OffensiveRebounds = dto.OffensiveRebounds;
            existingTeamStat.DefensiveRebounds = dto.DefensiveRebounds;
            existingTeamStat.Assists = dto.Assists;
            existingTeamStat.Steals = dto.Steals;
            existingTeamStat.Blocks = dto.Blocks;
            existingTeamStat.Turnovers = dto.Turnovers;
            existingTeamStat.PersonalFouls = dto.PersonalFouls;
            existingTeamStat.TotalPoints = dto.TotalPoints;

            // Send back over
            await repo.UpdateTeamStat(id, existingTeamStat);

            return NoContent();
        }


        [HttpDelete("DeletePlayer/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeletePlayer(int id)
        {
            NbaPlayer? exists = await repo.RetrievePlayer(id);
            if (exists == null)
            {
                return NotFound();
            }
            bool? delete = await repo.DeletePlayer(id);
            if (delete.HasValue && delete.Value)
            {
                return new NoContentResult();
            }
            else
            {
                return BadRequest("The player was not deleted!!!");
            }
        }

        [HttpDelete("DeleteTeam/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteTeam(int id)
        {
            NbaTeam? exists = await repo.RetrieveTeam(id);
            if (exists == null)
            {
                return NotFound();
            }
            bool? delete = await repo.DeleteTeam(id);
            if (delete.HasValue && delete.Value)
            {
                return new NoContentResult();
            }
            else
            {
                return BadRequest("The player was not deleted!!!");
            }
        }

    }

}

