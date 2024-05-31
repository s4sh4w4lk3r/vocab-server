using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Text.Json;
using Vocab.Application.Abstractions.Services;
using Vocab.WebApi.Extensions;

namespace Vocab.WebApi.Controllers
{
    [ApiController, Route("ws")]
    public class ChallengeController(IChallengeService challengeService) : ControllerBase
    {
        private static readonly JsonSerializerOptions jsonSerializerOptions = new()
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };

        [Route("challenge/{dictionaryId}")]
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
            if (initGameResult.Success is false)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }

            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();

            var webSocketHandlingResult = await challengeService.StartWebSocketHandling(webSocket);
            if (webSocketHandlingResult.Success is false)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }
        }

    }
}
