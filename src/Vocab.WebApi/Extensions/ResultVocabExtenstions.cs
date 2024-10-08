﻿using Microsoft.AspNetCore.Mvc;
using Vocab.Application.Enums;
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
                    { "errors", new[] {result.Error } }
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
            if ((result.IsSuccess is true) && (result.Value is not null) && (result.Value.Equals(default) is false))
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
                _ => StatusCodes.Status400BadRequest,
            };
        }

        private static string GetTitle(ErrorVocab error)
        {
            return error.ErrorType switch
            {
                ErrorType.Validation => "Bad Request",
                ErrorType.NotFound => "Not Found",
                ErrorType.Conflict => "Conflict",
                _ => "Bad Request"
            };
        }

        private static ActionResult ToObjectResult(this ErrorVocab error, ProblemDetails problemDetails)
        {
            return error.ErrorType switch
            {
                ErrorType.Validation => new BadRequestObjectResult(problemDetails),
                ErrorType.NotFound => new NotFoundObjectResult(problemDetails),
                ErrorType.Conflict => new ConflictObjectResult(problemDetails),
                _ => new BadRequestObjectResult(problemDetails),
            };
        }

    }
}
