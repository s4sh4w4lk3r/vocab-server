using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Throw;
using Vocab.Application.Abstractions.Services;
using Vocab.Application.Types;
using Vocab.Application.ValueObjects.Result;
using Vocab.Application.ValueObjects.Result.Errors;
using Vocab.Core.Entities;
using Vocab.Infrastructure.Persistence;

namespace Vocab.Infrastructure.Services
{
    public class ChallengeService(IRatingService ratingService, VocabContext context) : IChallengeService
    {
        private static readonly JsonSerializerOptions jsonSerializerOptions = new()
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };

        private const int MIN_WORDS_REQUIRED = 5;
        private const int MAX_WORDS_REQUIRED = 150;
        private const int MAX_ATTEMPTS_TO_PICK_TARGET = 5;
        private const int WEBSOCKET_BUFFER_SIZE_KB = 4;

        private static readonly TimeSpan webSocketTimeout = TimeSpan.FromSeconds(310);

        private Queue<ChallengeStatementsPair> statementsPairsQueue = [];
        private Guid userId;

        public async Task<ResultVocab> InitGame(Guid userId, long dictionaryId, int gameLength)
        {
            this.userId = userId.Throw().IfDefault().Value;

            var getGameResult = await GetStatementsForChallenge(userId, dictionaryId, gameLength);
            if (getGameResult.IsSuccess is false || getGameResult.Value is not { Length: >= MIN_WORDS_REQUIRED })
            {
                return ResultVocab.Failure(getGameResult.Error);
            }

            statementsPairsQueue = new(getGameResult.Value);

            return ResultVocab.Success();
        }

        public async Task<ResultVocab> StartWebSocketHandling(WebSocket webSocket)
        {
            webSocket.ThrowIfNull();
            statementsPairsQueue.Throw(_ => new InvalidOperationException(
                $"Коллекция \"statementsPairsQueue\" пустая. Возможно не был вызван метод {nameof(ChallengeService)}.{nameof(InitGame)}."))
                .IfEmpty();

            var buffer = new byte[1024 * WEBSOCKET_BUFFER_SIZE_KB];

            while (statementsPairsQueue.TryDequeue(out ChallengeStatementsPair? challengeStatementsPair))
            {
                await SendStatement(webSocket, challengeStatementsPair);
                CancellationTokenSource cts = new(webSocketTimeout);

                try
                {
                    var receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cts.Token);
                    if (receiveResult.CloseStatus.HasValue)
                    {
                        break;
                    }
                }
                catch (OperationCanceledException)
                {
                    return ResultVocab.Failure(ChallengeErrors.ResponseTimeout);
                }

                string message = Encoding.UTF8.GetString(buffer, 0, buffer.Length).TrimEnd('\0');
                var result = await ratingService.HandleAnswer(userId, challengeStatementsPair.StatementsPairId, message);

                if (result.IsSuccess && result.Value is not null)
                {
                    await SendResult(webSocket, result.Value);
                }
                else
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.InternalServerError, "Произошла ошибка на сервере.", CancellationToken.None);
                    throw new InvalidOperationException(result.Error.Description);
                }
            }

            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
            return ResultVocab.Success();
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

        private async Task<ResultVocab<ChallengeStatementsPair[]>> GetStatementsForChallenge(Guid userId, long dictionaryId, int gameLength)
        {

            userId.Throw().IfDefault();
            dictionaryId.Throw().IfDefault();

            if (gameLength > MAX_WORDS_REQUIRED || MIN_WORDS_REQUIRED > gameLength)
            {
                return ResultVocab.Failure(ChallengeErrors.InvalidStatementsCount(MIN_WORDS_REQUIRED, MAX_WORDS_REQUIRED)).AddValue<ChallengeStatementsPair[]>(default);
            }

            StatementPair[] statementPairs = await context.StatementPairs
                .Where(sp => sp.StatementsDictionary!.Id == dictionaryId && sp.StatementsDictionary.OwnerId == userId)
                .OrderBy(sp => sp.GuessingLevel).Take(count: gameLength).ToArrayAsync();

            if (statementPairs is { Length: < MIN_WORDS_REQUIRED })
            {
                return ResultVocab.Failure(ChallengeErrors.TooFewStatements(statementPairs.Length, MIN_WORDS_REQUIRED)).AddValue<ChallengeStatementsPair[]>(default);
            }

            ChallengeStatementsPair[] challengeStatements = statementPairs.Select(x =>
            {
                string randomTarget = string.Empty;

                // Подобор рандомного таргета. С помощью цикла выполняется попытка исключить два одинаковых таргета.
                // Если попытки заканчиваются, то пользователь получает одинаковые таргеты.
                for (int i = 0; i < MAX_ATTEMPTS_TO_PICK_TARGET; i++)
                {
                    var maybeTarget = Random.Shared.GetItems(statementPairs, 1).Select(rx => new { rx.Id, rx.Target }).Single();

                    if (maybeTarget.Id != x.Id)
                    {
                        randomTarget = maybeTarget.Target;
                        break;
                    }
                }

                return int.IsEvenInteger(Random.Shared.Next())
                    ? new ChallengeStatementsPair(x.Id, x.Source, randomTarget, x.Target)
                    : new ChallengeStatementsPair(x.Id, x.Source, x.Target, randomTarget);

            }).ToArray();

            return ResultVocab.Success().AddValue(challengeStatements);
        }
    }
}
