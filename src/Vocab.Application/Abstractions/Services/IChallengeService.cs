using System.Net.WebSockets;
using Vocab.Application.ValueObjects;

namespace Vocab.Application.Abstractions.Services
{
    public interface IChallengeService
    {
        public Task<ResultVocab> InitGame(Guid userId, long dictionaryId, int gameLength);
        public Task<ResultVocab> StartWebSocketHandling(WebSocket webSocket);
    }
}