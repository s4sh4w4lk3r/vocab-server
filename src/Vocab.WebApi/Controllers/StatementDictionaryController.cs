using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Vocab.Application.Abstractions.Services;
using Vocab.Core.Entities;
using Vocab.WebApi.Extensions;

namespace Vocab.WebApi.Controllers
{
    [ApiController, Route("dictionaries")]
    public class StatementDictionaryController(IStatementDictionaryService service) : ControllerBase
    {
#warning проверить
        [HttpPost]
        public async Task<IActionResult> Insert([FromQuery, Required] string name)
        {
            Guid userId = this.GetUserGuid();
            StatementDictionary statementDictionary = new(default, name, userId, DateTime.UtcNow);
            var result = await service.Insert(userId, statementDictionary);
            return result.ToActionResult();
        }

        [HttpDelete, Route("{dictionaryId:long:min(1)}")]
        public async Task<IActionResult> Delete([FromRoute] long dictionaryId)
        {
            Guid userId = this.GetUserGuid();
            var result = await service.Delete(userId, dictionaryId);
            return result.ToActionResult();
        }

        [HttpPatch, Route("{dictionaryId:long:min(1)}")]
        public async Task<IActionResult> Rename([FromRoute] long dictionaryId, [FromQuery, Required] string name)
        {
            Guid userId = this.GetUserGuid();
            var result = await service.SetName(userId, dictionaryId, name);
            return result.ToActionResult();
        }

        [HttpGet, Route("{dictionaryId:long:min(1)}/challenge")]
        public async Task<IActionResult> GetStatementsForChallenge([FromRoute] long dictionaryId, [FromQuery] int gameLength = 25)
        {
            Guid userId = this.GetUserGuid();
            var result = await service.GetStatementsForChallenge(userId, dictionaryId, gameLength);
            return result.ToActionResult();
        }

        [HttpPost, Route("{dictionaryId:long:min(1)}/import")]
        public async Task<IActionResult> ImportStatements([FromRoute] long dictionaryId, IFormFile statements)
        {
            Guid userId = this.GetUserGuid();
            using Stream stream = statements.OpenReadStream();

            var result = await service.ImportStatements(userId, dictionaryId, stream);
            return result.ToActionResult();
        }
    }
}
