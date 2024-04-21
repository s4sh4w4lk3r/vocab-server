using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;
using Vocab.Application.Abstractions.Services;
using Vocab.Application.ValueObjects;

namespace Vocab.WebApi.Controllers
{
    [ApiController, Route("statement-pairs")]
    public class StatementPairController(IStatementPairService statementPairService) : ControllerBase
    {
#warning проверить
        [HttpPut]
        public IActionResult Put()
        {
            return StatusCode(((int)HttpStatusCode.NotImplemented));
        }

        [HttpDelete, Route("{id}")]
        public IActionResult Delete(ulong id)
        {
            return StatusCode(((int)HttpStatusCode.NotImplemented));
        }

        [HttpPatch]
        public IActionResult Patch()
        {
            return StatusCode(((int)HttpStatusCode.NotImplemented));
        }


        [HttpPatch, Route("{id}/rating")]
        public async Task<IActionResult> PatchRating(ulong id, [FromQuery, Required] string action)
        {
            // https://daninacan.com/how-to-use-enums-in-asp-net-core-routes/
        }

        [HttpGet, Route("{id}/rating")]
        public IActionResult GetRating(ulong id)
        {
            return StatusCode(((int)HttpStatusCode.NotImplemented));
        }
    }
}
