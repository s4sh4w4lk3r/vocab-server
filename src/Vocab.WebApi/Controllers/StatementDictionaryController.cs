using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Vocab.Application.Abstractions.Services;
using Vocab.Core.Entities;
using Vocab.WebApi.Extensions;

namespace Vocab.WebApi.Controllers
{
    [ApiController, Route("dictionaries")]
    public class StatementDictionaryController(IStatementDictionaryService statementDictionaryService, IStatementPairService statementPairService) : ControllerBase
    {
#warning проверить
        [HttpPost]
        public async Task<IActionResult> Insert([FromQuery, Required] string name)
        {
            Guid userId = this.GetUserGuid();
            StatementDictionary statementDictionary = new(default, name, userId, DateTime.UtcNow);
            var result = await statementDictionaryService.Insert(userId, statementDictionary);
            return result.ToActionResult();
        }

        [HttpDelete, Route("{dictionaryId:long:min(1)}")]
        public async Task<IActionResult> Delete([FromRoute] long dictionaryId)
        {
            Guid userId = this.GetUserGuid();
            var result = await statementDictionaryService.Delete(userId, dictionaryId);
            return result.ToActionResult();
        }

        [HttpPatch, Route("{dictionaryId:long:min(1)}")]
        public async Task<IActionResult> Rename([FromRoute] long dictionaryId, [FromQuery, Required] string name)
        {
            Guid userId = this.GetUserGuid();
            var result = await statementDictionaryService.SetName(userId, dictionaryId, name);
            return result.ToActionResult();
        }

        [HttpPost, Route("{dictionaryId:long:min(1)}/import")]
        public async Task<IActionResult> ImportStatements([FromRoute] long dictionaryId, IFormFile statements, [FromQuery] string separator = " - ")
        {
            Guid userId = this.GetUserGuid();
            using Stream stream = statements.OpenReadStream();

            var result = await statementDictionaryService.ImportStatements(userId, dictionaryId, stream, separator);
            return result.ToActionResult();
        }

        [HttpGet, Route("{dictionaryId:long:min(1)}")]
        public async Task<IActionResult> GetById(long dictionaryId)
        {
            Guid userId = this.GetUserGuid();
            var result = await statementDictionaryService.GetById(userId, dictionaryId);
            return result.ToActionResult();
        }

        [HttpGet, Route("")]
        public async Task<IActionResult> GetDictionariesArray([FromQuery] int offset = 0)
        {
            Guid userId = this.GetUserGuid();
            var result = await statementDictionaryService.GetUserDictionaries(userId, offset);
            return result.ToActionResult();
        }

        [HttpGet, Route("{dictionaryId:long:min(1)}/statements")]
        public async Task<IActionResult> GetStatementPairsArray(long dictionaryId, [FromQuery] int offset = 0)
        {
            Guid userGuid = this.GetUserGuid();
            var result = await statementPairService.GetDictionaryStatementPairs(userGuid, dictionaryId, offset);
            return result.ToActionResult();
        }
    }
}
