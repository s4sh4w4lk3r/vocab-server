﻿using Vocab.Application.ValueObjects;
using Vocab.Core.Entities;

namespace Vocab.Application.Abstractions.Services
{
    public interface IStatementDictionaryService
    {
        public Task<ResultVocab<StatementDictionary>> Insert(StatementDictionary dictionary);
        public Task<ResultVocab<StatementDictionary>> Update(StatementDictionary dictionary);
        public Task<ResultVocab> Delete(Guid userId, long dictionaryId);

        public Task<ResultVocab> SetName(Guid userId, long dictionaryId, string name);
        public Task<ResultVocab<List<StatementPair>>> GetStatementsForChallenge(Guid userId, long dictionaryId, int gameLength = 25);
    }   
}
