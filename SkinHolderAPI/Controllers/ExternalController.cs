using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkinHolderAPI.Application.External;
using SkinHolderAPI.Attributes;

namespace SkinHolderAPI.Controllers;

[ApiController]
[Route("[controller]")]
[AllowAnonymous]
public class ExternalController(IExternalLogic externalLogic) : ControllerBase
{
    private readonly IExternalLogic _externalLogic = externalLogic;

    [HttpPost("GetPlayerInfo")]
    [Limit(30)]
    public async Task<IActionResult> GetPlayerInfo([FromBody] string playerId)
    {
        if (playerId.Length > 25) return BadRequest("Wrong id format");

        //var result =  await _externalLogic.GetPlayerInfo(playerId);

        //if (string.IsNullOrEmpty(result)) return Forbid();
        
        return Ok(placeholder);
    }

    private readonly string placeholder = @"{
		""player"": {
			""steam64Id"": ""76561198879168165"",
			""name"": ""「 j 」"",
			""steamAvatarUrl"": ""https://avatars.steamstatic.com/58b0ad1525d99ff440e9b6abbbe2efcb7f793e0c_full.jpg"",
			""playerType"": ""normal"",
			""banStatus"": [],
			""isConvictedCheater"": false,
			""convictions"": [],
			""autoflagger"": [],
			""csWatchStats"": {
				""kdRatio"": 0.9874715261958997,
				""totalMatches"": 30,
				""winrate"": 0.5137614678899083,
				""hsPerc"": 20.7059,
				""leetifyRating"": -0.32,
				""aimRating"": 75.8036,
				""reactionTimeMs"": 655.5583,
				""preaim"": 10.5997,
				""faceitElo"": 889,
				""lastCalculated"": ""2025-09-25T00:00:42.405Z""
			},
			""leetifyPublicStats"": {
				""ranks"": {
					""leetify"": -0.32,
					""premier"": 16704,
					""faceit"": 2,
					""faceit_elo"": null,
					""wingman"": 13,
					""renown"": null,
					""competitive"": [
						{
							""map_name"": ""de_edin"",
							""rank"": 9,
							""_id"": ""68d4862946df0d0c5afee001""
						},
						{
							""map_name"": ""de_nuke"",
							""rank"": 9,
							""_id"": ""68d4862946df0d0c5afee002""
						},
						{
							""map_name"": ""cs_italy"",
							""rank"": 10,
							""_id"": ""68d4862946df0d0c5afee003""
						},
						{
							""map_name"": ""de_dust2"",
							""rank"": 0,
							""_id"": ""68d4862946df0d0c5afee004""
						},
						{
							""map_name"": ""de_grail"",
							""rank"": 9,
							""_id"": ""68d4862946df0d0c5afee005""
						},
						{
							""map_name"": ""de_thera"",
							""rank"": 0,
							""_id"": ""68d4862946df0d0c5afee006""
						},
						{
							""map_name"": ""de_train"",
							""rank"": 10,
							""_id"": ""68d4862946df0d0c5afee007""
						},
						{
							""map_name"": ""de_anubis"",
							""rank"": 3,
							""_id"": ""68d4862946df0d0c5afee008""
						},
						{
							""map_name"": ""de_mirage"",
							""rank"": 9,
							""_id"": ""68d4862946df0d0c5afee009""
						},
						{
							""map_name"": ""de_inferno"",
							""rank"": 0,
							""_id"": ""68d4862946df0d0c5afee00a""
						},
						{
							""map_name"": ""de_vertigo"",
							""rank"": 0,
							""_id"": ""68d4862946df0d0c5afee00b""
						},
						{
							""map_name"": ""de_overpass"",
							""rank"": 10,
							""_id"": ""68d4862946df0d0c5afee00c""
						}
					]
				},
				""rating"": {
					""aim"": 75.8036,
					""positioning"": 50,
					""utility"": 47.2097,
					""clutch"": 0.0896,
					""opening"": 0.0049,
					""ct_leetify"": 0.0035,
					""t_leetify"": -0.0095
				},
				""stats"": {
					""accuracy_enemy_spotted"": 39.0533,
					""accuracy_head"": 20.7059,
					""counter_strafing_good_shots_ratio"": 78.6358,
					""ct_opening_aggression_success_rate"": 45.5508,
					""ct_opening_duel_success_percentage"": 50.4773,
					""flashbang_hit_foe_avg_duration"": 2.8941,
					""flashbang_hit_foe_per_flashbang"": 0.6481,
					""flashbang_hit_friend_per_flashbang"": 0.4305,
					""flashbang_leading_to_kill"": 6.8451,
					""flashbang_thrown"": 12.2791,
					""he_foes_damage_avg"": 14.2964,
					""he_friends_damage_avg"": 0.3207,
					""preaim"": 10.5997,
					""reaction_time_ms"": 655.5583,
					""spray_accuracy"": 40.3426,
					""t_opening_aggression_success_rate"": 38.0835,
					""t_opening_duel_success_percentage"": 45.3322,
					""traded_deaths_success_percentage"": 41.4268,
					""trade_kill_opportunities_per_round"": 0.2439,
					""trade_kills_success_percentage"": 42.6347,
					""utility_on_death_avg"": 260.4926
				},
				""winrate"": 0.6,
				""total_matches"": 388,
				""first_match_date"": ""2021-03-31T16:28:35.000Z"",
				""name"": ""「 j 」"",
				""bans"": [],
				""steam64_id"": ""76561198879168165"",
				""id"": ""b8b3c093-30c3-4617-b28f-6a5213b47d60"",
				""recent_teammates"": [
					{
						""steam64_id"": ""76561198103478921"",
						""recent_matches_count"": 10,
						""_id"": ""68d4862946df0d0c5afee00d""
					},
					{
						""steam64_id"": ""76561198234280484"",
						""recent_matches_count"": 13,
						""_id"": ""68d4862946df0d0c5afee00e""
					},
					{
						""steam64_id"": ""76561198300699104"",
						""recent_matches_count"": 14,
						""_id"": ""68d4862946df0d0c5afee00f""
					}
				],
				""lastUpdated"": ""2025-09-25T00:00:41.777Z""
			},
			""leetifyStats"": {
				""recentGameRatings"": {
					""aim"": 75.8036416712803,
					""leetifyRatingRounds"": 609,
					""positioning"": 50.000000050000004,
					""utility"": 47.20967605671665,
					""gamesPlayed"": 30,
					""clutch"": 0.0896,
					""ctLeetify"": 0.0035,
					""leetify"": -0.0032,
					""opening"": 0.0049,
					""tLeetify"": -0.0095
				},
				""totalKills"": 867,
				""totalDeaths"": 878,
				""kdRatio"": 0.9874715261958997,
				""totalWins"": 168,
				""totalLoss"": 143,
				""totalTie"": 16,
				""winRate"": 0.5137614678899083,
				""lastUpdated"": ""2025-09-25T00:00:41.910Z""
			},
			""faceitInfo"": {
				""player_id"": ""7e6451c9-68fe-4478-82a5-150447db0949"",
				""nickname"": ""jagobainda"",
				""avatar"": """",
				""country"": ""es"",
				""cover_image"": """",
				""platforms"": {
					""steam"": ""STEAM_0:1:459451218""
				},
				""games"": {
					""cs2"": {
						""region"": ""EU"",
						""game_player_id"": ""76561198879168165"",
						""skill_level"": 3,
						""faceit_elo"": 889,
						""game_player_name"": ""「 jagobainda 」"",
						""skill_level_label"": """",
						""game_profile_id"": """"
					},
					""csgo"": {
						""region"": ""EU"",
						""game_player_id"": ""76561198879168165"",
						""skill_level"": 2,
						""faceit_elo"": 889,
						""game_player_name"": ""「 jagobainda 」"",
						""skill_level_label"": """",
						""game_profile_id"": """"
					}
				},
				""settings"": {
					""language"": ""es""
				},
				""memberships"": [
					""free""
				],
				""faceit_url"": ""https://www.faceit.com/{lang}/players/jagobainda"",
				""membership_type"": """",
				""verified"": false,
				""lastUpdated"": ""2025-09-23T23:25:07.776Z""
			},
			""faceitCs2Stats"": {
				""player_id"": ""7e6451c9-68fe-4478-82a5-150447db0949"",
				""game_id"": ""cs2"",
				""lifetime"": {
					""Average K/D Ratio"": ""1.33"",
					""Win Rate %"": ""50"",
					""Recent Results"": [
						""0"",
						""1"",
						""1"",
						""0"",
						""0""
					],
					""Current Win Streak"": ""0"",
					""Total Headshots %"": ""3102"",
					""Average Headshots %"": ""36"",
					""K/D Ratio"": ""114.14"",
					""Wins"": ""43"",
					""Matches"": ""86"",
					""Longest Win Streak"": ""4""
				},
				""lastUpdated"": ""2025-09-25T00:00:42.086Z""
			},
			""faceitCsgoStats"": {
				""player_id"": ""7e6451c9-68fe-4478-82a5-150447db0949"",
				""game_id"": ""csgo"",
				""lifetime"": {
					""Recent Results"": [
						""0"",
						""1"",
						""1"",
						""0"",
						""0""
					],
					""Matches"": ""86"",
					""Average K/D Ratio"": ""1.33"",
					""Average Headshots %"": ""36"",
					""Current Win Streak"": ""0"",
					""K/D Ratio"": ""114.14"",
					""Total Headshots %"": ""3102"",
					""Win Rate %"": ""50"",
					""Wins"": ""43"",
					""Longest Win Streak"": ""4""
				},
				""lastUpdated"": ""2025-09-25T00:00:42.218Z""
			},
			""xifyAnalysis"": {
				""steam_id"": ""76561198879168165"",
				""timestamp"": ""2025-09-25T00:00:42.219Z"",
				""analysis_components"": null,
				""lastUpdated"": ""2025-09-25T00:00:42.219Z""
			},
			""userData"": null
		},
		""csWatchAnalysis"": {
			""version"": ""2.0"",
			""totalScore"": 0,
			""rawScore"": 3.4,
			""faceitAdjustment"": -11.920292202211755,
			""faceitAdjustmentApplied"": true,
			""breakdown"": [
				{
					""type"": ""kd"",
					""score"": 10.67135678179085,
					""message"": ""K/D Ratio: 0.99 across 30 matches"",
					""value"": ""0.99"",
					""unit"": """"
				},
				{
					""type"": ""reaction"",
					""score"": 5.395862988627468,
					""message"": ""Time to Damage: 655.5583ms"",
					""value"": ""655.6"",
					""unit"": ""ms""
				},
				{
					""type"": ""preaim"",
					""score"": 0.020217964696671437,
					""message"": ""Preaim: 10.6 degrees"",
					""value"": ""10.6"",
					""unit"": ""deg""
				},
				{
					""type"": ""aim"",
					""score"": 0.6994424772264897,
					""message"": ""Aim Rating: 75.8036%"",
					""value"": ""75.8"",
					""unit"": """"
				},
				{
					""type"": ""winrate"",
					""score"": 2.020800536434158,
					""message"": ""Winrate: 51.4%"",
					""value"": ""51.4"",
					""unit"": ""%""
				}
			],
			""message"": ""Very Low Risk"",
			""unadjustedScore"": 0.1,
			""unadjustedMessage"": ""Very Low Risk"",
			""proAdjustedScore"": null,
			""proAdjustedMessage"": null,
			""playerType"": ""normal""
		},
		""totalSuspicionScore"": 0,
		""lastUpdated"": ""2025-09-25T00:00:42.405Z""
	}";
}
