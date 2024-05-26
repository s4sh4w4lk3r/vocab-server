using Microsoft.AspNetCore.Mvc;
using Vocab.Application.Constants;
using Vocab.Application.ValueObjects;

namespace Vocab.WebApi.Extensions
{
    internal static class VocabResultExtensions
    {
        public static IActionResult ToActionResult(this ResultVocab resultVocab)
        {
            if (resultVocab.Success is false && resultVocab.Description == ResultMessages.NotFound)
            {
                return new NotFoundObjectResult(resultVocab);
            }

            if (resultVocab.Success is false)
            {
                return new BadRequestObjectResult(resultVocab);
            }

            return new OkObjectResult(resultVocab);
        }

    }
}
