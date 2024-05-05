using Microsoft.AspNetCore.Mvc;
using Vocab.Application.ValueObjects;

namespace Vocab.WebApi.Extensions
{
    public static class VocabResultExtensions
    {
        public static IActionResult ToActionResult(this ResultVocab resultVocab) 
            => resultVocab.Success ? new OkObjectResult(resultVocab) : new BadRequestObjectResult(resultVocab);
    }
}
