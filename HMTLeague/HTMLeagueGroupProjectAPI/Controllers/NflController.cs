using HTMLeagueGroupProjectAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using HTMLeagueGroupProjectAPI.Data.DataModels;

namespace HTMLeagueGroupProjectAPI.Controllers
{
    [ApiController]
    [Route("api/NFL")]
    public class NflController : ControllerBase
    {
        private readonly ICRUDNfl repo;

        public NflController(ICRUDNfl repo)
        {
            this.repo = repo;
        }


        //Nfl Player

        //player

        //create player
        [HttpPost("CreatePlayer")]
        [ProducesResponseType(201, Type = typeof(NflPlayer))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreatePlayer([FromBody] NflPlayer player)
        {
            if (player == null)
            {
                return BadRequest();
            }
            NflPlayer? addedPlayer = await repo.CreatePlayer(player);
            if (addedPlayer == null)
            {
                return BadRequest("Failed to create new Csgo PLayer");
            }
            else
            {
                return CreatedAtRoute(routeName: nameof(GetPlayer), routeValues: new { id = addedPlayer.PlayerID }, value: addedPlayer);
            }
        }
        //display all players
        [HttpGet("Players")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CsgoPlayerStat>))]
        public async Task<IEnumerable<NflPlayer>> GetPlayers()
        {
            return await repo.RetrievePlayers();
        }

        //display single player
        [HttpGet("Player/{id}")]
        [ProducesResponseType(200, Type = typeof(CsgoPlayer))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetPlayer(int id)
        {
            NflPlayer? player = await repo.RetrievePlayer(id);
            if (player == null)
            {
                return NotFound();
            }
            return Ok(player);
        }

        //delete 
        [HttpDelete("DeletePlayer/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeletePlayer(int id)
        {
            NflPlayer? exists = await repo.RetrievePlayer(id);
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


        //--------------------------------------------------------------------------------------------

        //fumble

        //Update
        [HttpPut("UpdateFumble/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateFumbleStat(int id, [FromBody] NflFumbleStatUpdateDto dto)
        {
            // Get existing fumble stat object
            NflPlayerCareerFumbleStat existingFumbleStat = await repo.RetrieveFumble(id);

            // Return if fumble stat doesn't exist
            if (existingFumbleStat is null) return NotFound();

            // Replace existing
            existingFumbleStat.Fumbles = dto.Fumbles;

            // Send back over
            await repo.UpdateFumbleStat(id, existingFumbleStat);

            return NoContent();
        }


        //display all
        [HttpGet("Fumbles")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<NflPlayerCareerFumbleStat>))]
        public async Task<IEnumerable<NflPlayerCareerFumbleStat>> GetAllFumble()
        {
            return await repo.RetrieveAllFumble();
        }

        // display one
        [HttpGet("Fumble/{id}")]
        [ProducesResponseType(200, Type = typeof(NflPlayerCareerFumbleStat))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetFumble(int id)
        {
            NflPlayerCareerFumbleStat? player = await repo.RetrieveFumble(id);
            if (player == null)
            {
                return NotFound();
            }
            return Ok(player);
        }


        //--------------------------------------------------------------------------------------------
        //rush

        //Update
        [HttpPut("UpdateRush/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateRushStat(int id, [FromBody] NflRushStatUpdateDto dto)
        {
            // Get existing rush stat object
            NflPlayerCareerRushStat existingRushStat = await repo.RetrieveRush(id);

            // Return if rush stat doesn't exist
            if (existingRushStat is null) return NotFound();

            // Replace existing
            existingRushStat.RushAttempts = dto.RushAttempts;
            existingRushStat.RushYards = dto.RushYards;
            existingRushStat.RushTds = dto.RushTds;
            existingRushStat.RushFirstdowns = dto.RushFirstdowns;

            // Send back over
            await repo.UpdateRushStat(id, existingRushStat);

            return NoContent();
        }


        //display all
        [HttpGet("Rushes")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<NflPlayerCareerRushStat>))]
        public async Task<IEnumerable<NflPlayerCareerRushStat>> GetAllRush()
        {
            return await repo.RetrieveAllRush();
        }

        // display one
        [HttpGet("Rush/{id}")]
        [ProducesResponseType(200, Type = typeof(NflPlayerCareerRushStat))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetRush(int id)
        {
            NflPlayerCareerRushStat? player = await repo.RetrieveRush(id);
            if (player == null)
            {
                return NotFound();
            }
            return Ok(player);
        }


        //--------------------------------------------------------------------------------------------

        //kick

        //Update
        [HttpPut("UpdateKick/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateKickStat(int id, [FromBody] NflKickStatUpdateDto dto)
        {
            // Get existing kick stat object
            NflPlayerCareerKickStat existingKickStat = await repo.RetrieveKick(id);

            // Return if kick stat doesn't exist
            if (existingKickStat is null) return NotFound();

            // Replace existing
            existingKickStat.FieldGoalAttempts = dto.FieldGoalAttempts;
            existingKickStat.FieldGoals = dto.FieldGoals;
            existingKickStat.ExtraPointAttempts = dto.ExtraPointAttempts;
            existingKickStat.ExtraPoints = dto.ExtraPoints;

            // Send back over
            await repo.UpdateKickStat(id, existingKickStat);

            return NoContent();
        }


        //display all
        [HttpGet("Kicks")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<NflPlayerCareerKickStat>))]
        public async Task<IEnumerable<NflPlayerCareerKickStat>> GetAllKick()
        {
            return await repo.RetrieveAllKick();
        }

        // display one
        [HttpGet("Kick/{id}")]
        [ProducesResponseType(200, Type = typeof(NflPlayerCareerKickStat))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetKick(int id)
        {
            NflPlayerCareerKickStat? player = await repo.RetrieveKick(id);
            if (player == null)
            {
                return NotFound();
            }
            return Ok(player);
        }


        //--------------------------------------------------------------------------------------------

        //receive

        //Update
        [HttpPut("UpdateReceive/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateReceiveStat(int id, [FromBody] NflReceiveStatUpdateDto dto)
        {
            // Get existing receive stat object
            NflPlayerCareerReceiveStat existingReceiveStat = await repo.RetrieveReceive(id);

            // Return if receive stat doesn't exist
            if (existingReceiveStat is null) return NotFound();

            // Replace existing
            existingReceiveStat.Receptions = dto.Receptions;
            existingReceiveStat.ReceivingYards = dto.ReceivingYards;
            existingReceiveStat.ReceivingTds = dto.ReceivingTds;

            // Send back over
            await repo.UpdateReceiveStat(id, existingReceiveStat);

            return NoContent();
        }


        //display all
        [HttpGet("Receives")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<NflPlayerCareerReceiveStat>))]
        public async Task<IEnumerable<NflPlayerCareerReceiveStat>> GetAllReceive()
        {
            return await repo.RetrieveAllReceive();
        }

        // display one
        [HttpGet("Receive/{id}")]
        [ProducesResponseType(200, Type = typeof(NflPlayerCareerReceiveStat))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetReceive(int id)
        {
            NflPlayerCareerReceiveStat? player = await repo.RetrieveReceive(id);
            if (player == null)
            {
                return NotFound();
            }
            return Ok(player);
        }


        //--------------------------------------------------------------------------------------------

        //sack

        //Update
        [HttpPut("UpdateSack/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateSackStat(int id, [FromBody] NflPlayerCareerSackStat? stat)
        {
            if (stat == null)
            {
                return BadRequest();
            }
            NflPlayerCareerSackStat? exists = await repo.RetrieveSack(id);
            if (exists == null)
            {
                return NotFound();
            }
            await repo.UpdateSackStat(stat.PlayerID, stat);
            return new NoContentResult();
        }

        //display all
        [HttpGet("Sacks")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<NflPlayerCareerSackStat>))]
        public async Task<IEnumerable<NflPlayerCareerSackStat>> GetAllSack()
        {
            return await repo.RetrieveAllSack();
        }

        // display one
        [HttpGet("Sack/{id}")]
        [ProducesResponseType(200, Type = typeof(NflPlayerCareerSackStat))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetSack(int id)
        {
            NflPlayerCareerSackStat? player = await repo.RetrieveSack(id);
            if (player == null)
            {
                return NotFound();
            }
            return Ok(player);
        }


        //--------------------------------------------------------------------------------------------


        //pass


        //Update
        [HttpPut("UpdatePass/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdatePassStat(int id, [FromBody] NflPassStatUpdateDto dto)
        {
            // Get existing pass stat object
            NflPlayerCareerPassStat existingPassStat = await repo.RetrievePass(id);

            // Return if pass stat doesn't exist
            if (existingPassStat is null) return NotFound();

            // Replace existing
            existingPassStat.PassAttempts = dto.PassAttempts;
            existingPassStat.CompletePasses = dto.CompletePasses;
            existingPassStat.PassingYards = dto.PassingYards;
            existingPassStat.TdPasses = dto.TdPasses;
            existingPassStat.Interceptions = dto.Interceptions;
            existingPassStat.LongestPass = dto.LongestPass;

            // Send back over
            await repo.UpdatePassStat(id, existingPassStat);

            return NoContent();
        }


        //display all
        [HttpGet("Passes")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<NflPlayerCareerPassStat>))]
        public async Task<IEnumerable<NflPlayerCareerPassStat>> GetAllPass()
        {
            return await repo.RetrieveAllPass();
        }

        // display one
        [HttpGet("Pass/{id}")]
        [ProducesResponseType(200, Type = typeof(NflPlayerCareerPassStat))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetPass(int id)
        {
            NflPlayerCareerPassStat? player = await repo.RetrievePass(id);
            if (player == null)
            {
                return NotFound();
            }
            return Ok(player);
        }


        //--------------------------------------------------------------------------------------------

        //Nfl team

        //team

        //create
        [HttpPost("CreateTeam")]
        [ProducesResponseType(201, Type = typeof(NflTeam))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateTeam([FromBody] NflTeam team)
        {
            if (team == null)
            {
                return BadRequest();
            }
            NflTeam? addedTeam = await repo.CreateTeam(team);
            if (addedTeam == null)
            {
                return BadRequest();
            }
            else
            {
                return CreatedAtRoute(routeName: nameof(GetTeam), routeValues: new { id = addedTeam.TeamID }, value: addedTeam);
            }
        }

        //display all teams
        [HttpGet("Teams")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<NflTeam>))]
        public async Task<IEnumerable<NflTeam>> GetTeams()
        {
            return await repo.RetrieveTeams();
        }
        //display one team
        [HttpGet("Team/{id}")]
        [ProducesResponseType(200, Type = typeof(NflTeam))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetTeam(int id)
        {
            NflTeam? team = await repo.RetrieveTeam(id);
            if (team == null)
            {
                return NotFound();
            }
            return Ok(team);
        }
        //delete team
        [HttpDelete("DeleteTeam/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteTeam(int id)
        {
            NflTeam? exists = await repo.RetrieveTeam(id);
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
                return BadRequest("The team was not deleted");
            }
        }

        //--------------------------------------------------------------------------------------------
        //team season stat

        // Had to switch to DTOs for this part because of validation errors (requiring a Team object).
        // If there was more time this could maybe be handled a bit better.
        //Update 
        [HttpPut("UpdateTeamStat/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateTeamStat(
            int id,
            [FromBody] NflTeamSeasonStatUpdateDto dto)
        {

            // Get existing team stat object
            NflTeamSeasonStat existingTeamStat = await repo.RetrieveTeamStat(id);

            // Return if team stat doesn't even exist
            if (existingTeamStat is null) return NotFound();

            // Replace existing
            existingTeamStat.SeasonYear = dto.SeasonYear;
            existingTeamStat.RushingTD = dto.RushingTD;
            existingTeamStat.ReceivingTD = dto.ReceivingTD;
            existingTeamStat.TotalTD = dto.TotalTD;
            existingTeamStat.TwoPoints = dto.TwoPoints;
            existingTeamStat.Wins = dto.Wins;
            existingTeamStat.Losses = dto.Losses;
            existingTeamStat.Ties = dto.Ties;
            existingTeamStat.FieldGoalsMade = dto.FieldGoalsMade;
            existingTeamStat.FieldGoalAttempts = dto.FieldGoalAttempts;
            existingTeamStat.ExtraPointsMade = dto.ExtraPointsMade;
            existingTeamStat.RushYards = dto.RushYards;
            existingTeamStat.PassingYards = dto.PassingYards;
            existingTeamStat.Interceptions = dto.Interceptions;

            // Send back over
            await repo.UpdateTeamStat(id, existingTeamStat);

            return NoContent();
        }


        //display all 
        [HttpGet("Teams/Stats")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<NflTeamSeasonStat>))]
        public async Task<IEnumerable<NflTeamSeasonStat>> GetAllTeamStat()
        {
            return await repo.RetrieveAllTeamStat();
        }

        //display one
        [HttpGet("Team/Stats/{id}")]
        [ProducesResponseType(200, Type = typeof(NflTeamSeasonStat))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetTeamStat(int id)
        {
            NflTeamSeasonStat? player = await repo.RetrieveTeamStat(id);
            if (player == null)
            {
                return NotFound();
            }
            return Ok(player);
        }
    }
}
