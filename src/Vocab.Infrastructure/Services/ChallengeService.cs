using System.Net.WebSockets;
using System.Text.Encodings.Web;
using System.Text.Json;
using Throw;
using Vocab.Application.Abstractions.Services;
using Vocab.Application.Types;
using Vocab.Application.ValueObjects;

namespace Vocab.Infrastructure.Services
{
    public class ChallengeService(IStatementDictionaryService statementDictionaryService, IRatingService ratingService)
    {
        private Queue<ChallengeStatementsPair> statementsPairsQueue = [];
        private Guid userId;
        private long dictionaryId;
        private int gameLength;

        private static readonly JsonSerializerOptions jsonSerializerOptions = new()
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };

        public async Task<ResultVocab> InitGame(Guid userId, long dictionaryId, int gameLength)
        {
            this.userId = userId.Throw().IfDefault().Value;
            this.dictionaryId = dictionaryId.Throw().IfDefault().Value;
            this.gameLength = gameLength.Throw().IfDefault().Value;


            var getGameResult = await statementDictionaryService.GetStatementsForChallenge(userId, dictionaryId, gameLength);
            if (getGameResult.Success is false || getGameResult.Value is not { Length: >= 5 })
            {
                return ResultVocab.Fail("Выражения для игры не получены.").AddInnerResult(getGameResult);
            }

            statementsPairsQueue = new(getGameResult.Value);

            return ResultVocab.Ok();
        }

        public async Task StartWebSocketHandling(WebSocket webSocket)
        {
            webSocket.ThrowIfNull();
            throw new NotImplementedException();
        }
    }
}
