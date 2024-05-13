using Microsoft.AspNetCore.Mvc;
using Vocab.Application.Constants;
using Vocab.Application.ValueObjects;

namespace Vocab.WebApi.Extensions
{
    public static class VocabResultExtensions
    {
        public static IActionResult ToActionResult(this ResultVocab resultVocab)
        {
            if (resultVocab.Success is false && resultVocab.Description == ResultMessages.NotFound)
            {
                return new NotFoundObjectResult(resultVocab.Description);
            }

            if (resultVocab.Success is false)
            {
                return new BadRequestObjectResult(resultVocab);
            }

            return new OkObjectResult(resultVocab);
        }

    }
}
