using Throw;
using Vocab.Application.Validators;
using Vocab.Application.ValueObjects.Result;
using Vocab.Application.ValueObjects.Result.Errors;
using Vocab.Core.Entities;

namespace Vocab.Infrastructure.Services
{
    internal class ParseStatementsHelper(Stream stream, long dictionaryId, string separator)
    {
        private async Task<string[]> ReadLinesAsync()
        {
            using StreamReader streamReader = new(stream);
            string documentStr = await streamReader.ReadToEndAsync();

            documentStr.ThrowIfNull().IfEmpty().IfWhiteSpace();

            return documentStr.Split('\n');
        }

        public async Task<ResultVocab<ParseStatementsHelperResult>> ParseDocument()
        {
            stream.ThrowIfNull();
            dictionaryId.Throw().IfDefault();
            separator.ThrowIfNull().IfEmpty().IfWhiteSpace();

            string[] lines = await ReadLinesAsync();

            if (lines.Length == 0)
            {
                return ResultVocab.Failure(StatementDictionaryErrors.EmptyLinesReceived).AddValue(default(ParseStatementsHelperResult));
            }

            List<string> failedStatementPairs = [];
            List<StatementPair> statementPairs = new(lines.Length);
            StatementPairValidator validator = new(willBeInserted: true);

            foreach (var line in lines)
            {
                string[] statements = line.Replace("\r", string.Empty).Split(separator);

                if (statements is not [_, _])
                {
                    failedStatementPairs.Add(line);
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

                if (!validator.Validate(statement).IsValid)
                {
                    failedStatementPairs.Add(line);
                    continue;
                }

                statementPairs.Add(statement);
            }

            // Проверка, если вдруг будут пропущены строки.
            lines.Length.Throw().IfNotEquals(failedStatementPairs.Count + statementPairs.Count);

            if (statementPairs.Count == 0)
            {
                return ResultVocab.Failure(StatementDictionaryErrors.NoExpressionImported).AddValue(new ParseStatementsHelperResult([], failedStatementPairs));
            }

            return ResultVocab.Success().AddValue(new ParseStatementsHelperResult(statementPairs, failedStatementPairs));
        }

        public record ParseStatementsHelperResult(List<StatementPair> StatementPairs, List<string> FailedStatementPairs);
    }
}
