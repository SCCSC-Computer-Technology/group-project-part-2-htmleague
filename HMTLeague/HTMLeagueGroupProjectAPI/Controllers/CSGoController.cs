using HTMLeagueGroupProjectAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using HTMLeagueGroupProjectAPI.Data.DataModels;

namespace HTMLeagueGroupProjectAPI.Controllers
{
    [ApiController]
    [Route("api/CSGO")]
    public class CSGoController : ControllerBase
    {
        private readonly ICRUDCsgo repo;
        
        public CSGoController(ICRUDCsgo repo)
        {
            this.repo = repo;
        }

        [HttpPost("CreatePlayer")]
        [ProducesResponseType(201, Type = typeof(CsgoPlayer))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreatePlayer([FromBody] CsgoPlayer player)
        {
            if(player == null)
            {
                return BadRequest();
            }
            CsgoPlayer? addedPlayer = await repo.CreateAsync(player);
            if(addedPlayer == null)
            {
                return BadRequest("Failed to create new Csgo PLayer");
            }
            else
            {
                return CreatedAtRoute(routeName: nameof(GetPlayer), routeValues: new { id = addedPlayer.PlayerID }, value: addedPlayer);
            }
        }

        [HttpPost("CreateTeam")]
        [ProducesResponseType(201, Type = typeof(CsgoTeam))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateTeam([FromBody] CsgoTeam team)
        {
            if(team == null)
            {
                return BadRequest();
            }
            CsgoTeam? addedTeam = await repo.CreateAsync(team);
            if(addedTeam == null)
            {
                return BadRequest();
            }
            else
            {
                return CreatedAtRoute(routeName: nameof(GetTeam), routeValues: new {id = addedTeam.TeamID},value: addedTeam);
            }
        }

        [HttpGet("Players")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CsgoPlayer>))]
        public async Task<IEnumerable<CsgoPlayer>> GetPlayers()
        {      
                return await repo.RetrieveAllPlayersAsync();
        }

        [HttpGet("Players/Stats")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CsgoPlayerStat>))]
        public async Task<IEnumerable<CsgoPlayerStat>> GetPlayerStats()
        {
            return await repo.RetrieveAllPlayersStatsAsync();
        }

        [HttpGet("Player/{id}")]
        [ProducesResponseType(200, Type=typeof(CsgoPlayer))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetPlayer( int id)
        {
            CsgoPlayer? player =  await repo.RetrievePlayerAsync(id);
            if (player == null)
            {
                return NotFound();
            }
            return Ok(player);
        }

        [HttpGet("Player/Stats/{id}")]
        [ProducesResponseType(200, Type = typeof(CsgoPlayerStat))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetPlayerStat(int id)
        {
            var playerStat = await repo.RetrievePlayerStatAsync(id);
            if (playerStat == null)
            {
                return NotFound();
            }
            return Ok(playerStat);
        }

        [HttpGet("Teams")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CsgoTeam>))]
        public async Task<IEnumerable<CsgoTeam>> GetTeams()
        {
            return await repo.RetrieveAllTeamsAsync();
        }

        [HttpGet("Teams/Stats")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CsgoTeamStat>))]
        public async Task<IEnumerable<CsgoTeamStat>> GetTeamAllStats()
        {
            return await repo.RetrieveAllTeamStatsAsync();
        }

        [HttpGet("Team/Stats/{id}")]
        [ProducesResponseType(200, Type = typeof(CsgoTeamStat))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetTeamStat(int id)
        {
            CsgoTeamStat? teamStat = await repo.RetrieveTeamStatAsync(id);
            if (teamStat == null)
            {
                return NotFound();
            }
            return Ok(teamStat);
        }


        [HttpGet("Team/{id}")]
        [ProducesResponseType(200, Type = typeof(CsgoTeam))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetTeam(int id)
        {
            CsgoTeam? team = await repo.RetrieveTeamAsync(id);
            if(team == null)
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
        [FromBody] CsgoPlayerStatUpdateDto dto)
        {

            // Get existing player stat object
            CsgoPlayerStat existingPlayerStat = await repo.RetrievePlayerStatAsync(id);

            // Return if team stat doesn't even exist
            if (existingPlayerStat is null) return NotFound();

            // Replace existing
            existingPlayerStat.TotalMaps = dto.TotalMaps;
            existingPlayerStat.TotalRounds = dto.TotalRounds;
            existingPlayerStat.KdDiff = dto.KdDiff;
            existingPlayerStat.Kd = dto.Kd;
            existingPlayerStat.Rating = dto.Rating;
            
            // Send back over
            await repo.UpdatePlayerStatAsync(id, existingPlayerStat);

            return NoContent();
        }


        // Had to switch to DTOs for this part because of validation errors (requiring a Team object).
        // If there was more time this could be handled a bit better.
        // Update
        [HttpPut("UpdateTeamStat/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateTeamStat(int id, [FromBody] TeamStatUpdateDto dto)
        {

            // Get existing team stat object
            CsgoTeamStat existingTeamStat = await repo.RetrieveTeamStatAsync(id);

            // Return if team stat doesn't even exist
            if (existingTeamStat == null) return NotFound();

            // Replace existing
            existingTeamStat.TotalMaps = dto.TotalMaps;
            existingTeamStat.KdDiff = dto.KdDiff;
            existingTeamStat.Kd = dto.Kd;
            existingTeamStat.Rating = dto.Rating;

            // Send back over
            await repo.UpdateTeamStatAsync(id, existingTeamStat);

            return NoContent();
        }

        [HttpDelete("DeletePlayer/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeletePlayer(int id)
        {
            CsgoPlayer? exists = await repo.RetrievePlayerAsync(id);
            if (exists == null)
            {
                return NotFound();
            }
            bool? delete = await repo.DeletePlayerAsync(id);
            if(delete.HasValue && delete.Value)
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
            CsgoTeam? exists = await repo.RetrieveTeamAsync(id);
            if(exists == null)
            {
                return NotFound();
            }
            bool? delete = await repo.DeleteTeamAsync(id);
            if(delete.HasValue && delete.Value)
            {
                return new NoContentResult();
            }
            else
            {
                return BadRequest("The team was not deleted");
            }
        }

        [HttpGet("PlayerTeams")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CsgoPlayerTeam>))]
        public async Task<IEnumerable<CsgoPlayerTeam>> GetPlayerTeams()
        {
            return await repo.RetrieveAllPlayerTeamsAsync();
        }

    }
}
