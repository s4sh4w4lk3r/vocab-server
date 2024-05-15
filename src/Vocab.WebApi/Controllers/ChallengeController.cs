using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Vocab.Application.Abstractions.Services;
using Vocab.Application.Types;
using Vocab.WebApi.Extensions;

namespace Vocab.WebApi.Controllers
{
#error отрефакторить
#error добавить токены отмены
#error разнести логику
    [ApiController, Route("ws")]
    public class ChallengeController(IStatementDictionaryService statementDictionaryService, IRatingService ratingService) : ControllerBase
    {
        private static readonly JsonSerializerOptions jsonSerializerOptions = new()
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = true
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

            var getGameResult = await statementDictionaryService.GetStatementsForChallenge(userId, dictionaryId, gameLength);
            if (getGameResult.Success is false || getGameResult.Value is not { Length: >= 5 })
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }

            Queue<ChallengeStatementsPair> statementsPairsQueue = new(getGameResult.Value);


            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            await Echo(webSocket, statementsPairsQueue, userId);
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
                    await webSocket.CloseAsync(WebSocketCloseStatus.InternalServerError, "Произошла ошибка на сервере.", CancellationToken.None);
                }
            }

            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
        }

        private static async Task SendStatement(WebSocket webSocket, ChallengeStatementsPair? challengeStatementsPair)
        {
            byte[] responseBuffer = JsonSerializer.SerializeToUtf8Bytes(challengeStatementsPair, jsonSerializerOptions);

            await webSocket.SendAsync(
               new ArraySegment<byte>(responseBuffer, 0, responseBuffer.Length),
               WebSocketMessageType.Text,
               true,
               CancellationToken.None);
        }

        private static async Task SendResult(WebSocket webSocket, AnswerResult answerResult)
        {
            byte[] responseBuffer = JsonSerializer.SerializeToUtf8Bytes(answerResult, jsonSerializerOptions);

            await webSocket.SendAsync(
               new ArraySegment<byte>(responseBuffer, 0, responseBuffer.Length),
               WebSocketMessageType.Text,
               true,
               CancellationToken.None);
        }
    }
}
