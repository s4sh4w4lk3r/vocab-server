using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Vocab.Application.Abstractions.Services;
using Vocab.WebApi.Extensions;

namespace Vocab.WebApi.Controllers
{
    [Route("ws")]
    public class ChallengeController(IChallengeService challengeService) : ControllerBase
    {

        [HttpGet, Route("challenge/{dictionaryId}", Name = nameof(StartChallenge))]
        [ProducesResponseType(StatusCodes.Status101SwitchingProtocols)]
        [ProducesResponseType(StatusCodes.Status502BadGateway)]
        public async Task StartChallenge([FromRoute] long dictionaryId, [FromQuery, Range(25, 50)] int gameLength = 25)
        {
            var response = HttpContext.Response;

            if (HttpContext.WebSockets.IsWebSocketRequest is false)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }

            Guid userId = this.GetUserGuid();

            var initGameResult = await challengeService.InitGame(userId, dictionaryId, gameLength);
            if (initGameResult.IsSuccess is false)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }

            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();

            var webSocketHandlingResult = await challengeService.StartWebSocketHandling(webSocket);
            if (webSocketHandlingResult.IsSuccess is false)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }
        }

    }
}
