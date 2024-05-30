using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using System.Text.Json;
using Vocab.Infrastructure.Services;
using Vocab.WebApi.Extensions;

namespace Vocab.WebApi.Controllers
{
    [ApiController, Route("ws")]
    public class ChallengeController(ChallengeService challengeService) : ControllerBase
    {
        private static readonly JsonSerializerOptions jsonSerializerOptions = new()
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };

        [Route("challenge/{dictionaryId}")]
        public async Task StartChallenge([FromRoute] long dictionaryId, [FromQuery] int gameLength = 25)
        {
            if (HttpContext.WebSockets.IsWebSocketRequest is false)
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }

            Guid userId = this.GetUserGuid();

            var initGameResult = await challengeService.InitGame(userId, dictionaryId, gameLength);
            if (initGameResult.Success is false)
            {
#warning попробовать донести до юзера что пошло не так тут. Может запихнуть сообщение в body
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }

            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();

            await challengeService.StartWebSocketHandling(webSocket);
        }

    }
}
