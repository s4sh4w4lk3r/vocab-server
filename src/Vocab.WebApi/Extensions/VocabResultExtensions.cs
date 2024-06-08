using Microsoft.AspNetCore.Mvc;
using Vocab.Application.Constants;
using Vocab.Application.ValueObjects;

namespace Vocab.WebApi.Extensions
{
    internal static class VocabResultExtensions
    {
        public static IActionResult ToActionResult(this ResultVocab resultVocab)
            => resultVocab.Success ? HandleOkResult(resultVocab) : HandleFailResult(resultVocab);

        private static IActionResult HandleOkResult(ResultVocab resultVocab)
        {
            if (resultVocab.Description == ResultMessages.Added)
            {
                return new CreatedResult();
            }

            return new OkObjectResult(resultVocab);
        }

        private static IActionResult HandleFailResult(ResultVocab resultVocab)
        {
            if (resultVocab.Description == ResultMessages.NotFound)
            {
                return new NotFoundObjectResult(resultVocab);
            }

            return new BadRequestObjectResult(resultVocab);
        }

    }
}
