using Hangfire;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Throw;
using Vocab.Application.Abstractions.Services;
using Vocab.Application.Validators;
using Vocab.Application.ValueObjects.Result;
using Vocab.Application.ValueObjects.Result.Errors;
using Vocab.Core.Entities;
using Vocab.Infrastructure.Persistence;

namespace Vocab.Infrastructure.Services
{
    public class StatementsImportService(VocabContext context) : IStatementsImportService
    {
        public async Task<ResultVocab<string>> ImportStatements(Guid userId, long dictionaryId, string documentBase64, string separator)
        {
            userId.Throw().IfDefault();
            dictionaryId.Throw().IfDefault();

            if (await context.StatementDictionaries.AnyAsync(x => x.OwnerId == userId && x.Id == dictionaryId) is false)
            {
                return ResultVocab.Failure(ImportStatementsErrors.NotFound).AddValue<string>(default);
            }

            string jobId = BackgroundJob.Enqueue(() => Perform(dictionaryId, documentBase64, separator));

            return ResultVocab.Success().AddValue(jobId);
        }

        public async Task<ResultVocab> Perform(long dictionaryId, string documentBase64, string separator)
        {
            dictionaryId.Throw().IfDefault();
            documentBase64.ThrowIfNull();

            if (TryConvertBase64ToUtf8(documentBase64, out string documentStr) is false)
            {
                return ResultVocab.Failure(ImportStatementsErrors.Base64ToUtf8Error);
            }


            ResultVocab<List<StatementPair>> getStatementsResult = ParseStatements(documentStr, dictionaryId, separator);
            if (getStatementsResult.IsSuccess is false || getStatementsResult.Value is null)
            {
                return getStatementsResult;
            }

            await context.StatementPairs.AddRangeAsync(getStatementsResult.Value);
            await context.TrySaveChangesAsync();

            return ResultVocab.Success();
        }

        private static ResultVocab<List<StatementPair>> ParseStatements(string documentStr, long dictionaryId, string separator)
        {
            documentStr.ThrowIfNull();

            string[] lines = documentStr.Split('\n');

            if (lines.Length == 0)
            {
                return ResultVocab.Failure(ImportStatementsErrors.EmptyLinesReceived).AddValue<List<StatementPair>>(default);
            }

            List<StatementPair> statementPairs = new(lines.Length);

            foreach (var line in lines)
            {
                string[] statements = line.Replace("\r", string.Empty).Split(separator);

                if (statements is not [_, _])
                {
                    continue;
                }

                StatementPair statement = new()
                {
                    Id = default,
                    LastModified = DateTime.UtcNow,
                    StatementsDictionaryId = dictionaryId,
                    Source = statements[0],
                    Target = statements[1],
                    StatementCategory = Core.Enums.StatementCategory.None,
                };

                if (new StatementPairValidator(willBeInserted: true).Validate(statement).IsValid is false)
                {
                    continue;
                }

                statementPairs.Add(statement);
            }

            if (statementPairs.Count == 0)
            {
                return ResultVocab.Failure(ImportStatementsErrors.NoExpressionImported).AddValue<List<StatementPair>>(default); ;
            }


            return ResultVocab.Success().AddValue(statementPairs); ;
        }

        private static bool TryConvertBase64ToUtf8(string base64, out string utf8Str)
        {
            base64.ThrowIfNull();

            int bufferLength = ((base64.Length * 3) + 3) / 4;
            Span<byte> buffer = stackalloc byte[bufferLength];

            if (Convert.TryFromBase64String(base64, buffer, out int _))
            {
                utf8Str = Encoding.UTF8.GetString(buffer);
                return true;
            }

            utf8Str = string.Empty;
            return false;
        }
    }
}
