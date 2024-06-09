using Microsoft.AspNetCore.Mvc;
using Vocab.Application.ValueObjects.ErrorResult;
using Vocab.Application.ValueObjects.Result;

namespace Vocab.WebApi.Extensions
{
    public static class ResultVocabExtenstions
    {
        public static ProblemDetails ToProblemDetails<T>(this ResultVocab<T> result)
        {
            if (result.IsSuccess) 
            {
                throw new InvalidOperationException($"Вызов метода {nameof(ToProblemDetails)} у объекта Result, у которого IsSuccess == true, запрещен.");
            }

            return new ProblemDetails()
            {
                Status = GetStatusCode(result.Error),
                Type = GetType(result.Error),
                Title = GetTitle(result.Error),
                Extensions = new Dictionary<string, object?>()
                {
                    { "error", new[] {result.Error } }
                }
            };
        }

        private static int GetStatusCode(ErrorVocab error)
        {
            return error.ErrorType switch
            {
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status500InternalServerError
            };
        }

        private static string GetTitle(ErrorVocab error)
        {
            return error.ErrorType switch
            {
                ErrorType.Validation => "Bad Request",
                ErrorType.NotFound => "NotFound",
                ErrorType.Conflict => "Conflict",
                _ => "Internal Server Error"
            };
        }

        private static string GetType(ErrorVocab error)
        {
#warning найти типы сюда
            return "bebra";
        }
    }
}
