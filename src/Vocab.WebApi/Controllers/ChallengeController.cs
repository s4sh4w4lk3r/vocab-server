using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Vocab.Application.Abstractions.Services;
using Vocab.Application.Types;
using Vocab.Infrastructure.Services;
using Vocab.WebApi.Extensions;

namespace Vocab.WebApi.Controllers
{
    [ApiController, Route("ws")]
    public class ChallengeController(IRatingService ratingService, ChallengeService challengeService) : ControllerBase
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

            await challengeService.StartWebSocketHandling(webSocket); /*await Echo(webSocket, statementsPairsQueue, userId);*/
        }

        private async Task Echo(WebSocket webSocket, Queue<ChallengeStatementsPair> statementsPairsQueue, Guid userId)
        {
            var buffer = new byte[1024 * 4];

            while (statementsPairsQueue.TryDequeue(out ChallengeStatementsPair? challengeStatementsPair))
            {
                await SendStatement(webSocket, challengeStatementsPair);

                var receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (receiveResult.CloseStatus.HasValue)
                {
                    break;
                }

                string message = Encoding.UTF8.GetString(buffer, 0, buffer.Length).TrimEnd('\0');
                var result = await ratingService.HandleAnswer(userId, challengeStatementsPair.StatementsPairId, message);

                if (result.Success && result.Value is not null)
                {
                    await SendResult(webSocket, result.Value);
                }
                else
                {
#warning залогить это.
                    await webSocket.CloseAsync(WebSocketCloseStatus.InternalServerError, "Произошла ошибка на сервере.", CancellationToken.None);
                }
            }

            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
        }

        private static async Task SendStatement(WebSocket webSocket, ChallengeStatementsPair? challengeStatementsPair)
        {
            byte[] responseBuffer = JsonSerializer.SerializeToUtf8Bytes(challengeStatementsPair, jsonSerializerOptions);

            await webSocket.SendAsync(
               buffer: new ArraySegment<byte>(responseBuffer, 0, responseBuffer.Length),
               messageType: WebSocketMessageType.Text,
               endOfMessage: true,
               cancellationToken: default);
        }

        private static async Task SendResult(WebSocket webSocket, AnswerResult answerResult)
        {
            byte[] responseBuffer = JsonSerializer.SerializeToUtf8Bytes(answerResult, jsonSerializerOptions);

            await webSocket.SendAsync(
               buffer: new ArraySegment<byte>(responseBuffer, 0, responseBuffer.Length),
               messageType: WebSocketMessageType.Text,
               endOfMessage: true,
               cancellationToken: default);
        }
    }
}
