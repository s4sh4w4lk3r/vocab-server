using Microsoft.AspNetCore.Mvc;
using Vocab.Application.ValueObjects.Result;

namespace Vocab.WebApi.Extensions
{
    public static class ResultVocabExtenstions
    {
        public static ProblemDetails ToProblemDetails(this ResultVocab result)
        {
            if (result.IsSuccess)
            {
                throw new InvalidOperationException($"Вызов метода {nameof(ToProblemDetails)} у объекта Result, у которого IsSuccess == true, запрещен.");
            }

            return new ProblemDetails()
            {
                Status = GetStatusCode(result.Error),
                Title = GetTitle(result.Error),
                Extensions = new Dictionary<string, object?>()
                {
                    { "error", new[] {result.Error } }
                }
            };
        }

        public static IActionResult Match(this ResultVocab result, Func<IActionResult> onSuccess)
        {
            if (result.IsSuccess)
            {
                return onSuccess.Invoke();
            }
            else
            {
                return result.Error.ToObjectResult(result.ToProblemDetails());
            }
        }

        public static ActionResult<T> Match<T>(this ResultVocab<T> result, Func<T, ActionResult<T>> onSuccess)
        {
            if (result.IsSuccess && result.Value is not null && result.Value.Equals(default))
            {
                return onSuccess.Invoke(result.Value);
            }
            else
            {
                return result.Error.ToObjectResult(result.ToProblemDetails());
            }
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

        private static ActionResult ToObjectResult(this ErrorVocab error, ProblemDetails problemDetails)
        {
            return error.ErrorType switch
            {
                ErrorType.Validation => new BadRequestObjectResult(problemDetails),
                ErrorType.NotFound => new NotFoundObjectResult(problemDetails),
                ErrorType.Conflict => new ConflictObjectResult(problemDetails),
                _ => new StatusCodeResult(StatusCodes.Status500InternalServerError)
            };
        }

    }
}
