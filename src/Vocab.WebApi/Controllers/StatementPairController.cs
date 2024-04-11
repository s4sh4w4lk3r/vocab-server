using Microsoft.AspNetCore.Mvc;
using System.Net;
using Vocab.Application.Abstractions.Services;

namespace Vocab.WebApi.Controllers
{
    [ApiController, Route("statement-pairs")]
    public class StatementPairController: ControllerBase
    {
        [HttpPut]
        public  IActionResult Put()
        {
            return StatusCode(((int)HttpStatusCode.NotImplemented));
        }

        [HttpDelete, Route("{id}")]
        public  IActionResult Delete(ulong id)
        {
            return StatusCode(((int)HttpStatusCode.NotImplemented));
        }

        [HttpPatch]
        public  IActionResult Patch()
        {
            return StatusCode(((int)HttpStatusCode.NotImplemented));
        }


        [HttpPatch, Route("{id}/rating/increase")]
        public  IActionResult IncreaseRating(ulong id)
        {
            return StatusCode(((int)HttpStatusCode.NotImplemented));
        }

        [HttpPatch, Route("{id}/rating/decrease")]
        public  IActionResult DecreaseRating(ulong id)
        {
            return StatusCode(((int)HttpStatusCode.NotImplemented));
        }

        [HttpGet, Route("{id}/rating")]
        public  IActionResult GetRating(ulong id)
        {
            return StatusCode(((int)HttpStatusCode.NotImplemented));
        }
    }
}
