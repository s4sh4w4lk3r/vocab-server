using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Vocab.WebApi.Controllers
{
    [ApiController, Route("dictionaries")]
    public class DictionaryController : ControllerBase
    {
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
    }
}
