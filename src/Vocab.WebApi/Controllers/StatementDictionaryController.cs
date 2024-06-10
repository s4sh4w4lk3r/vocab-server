using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Vocab.Application.Abstractions.Services;
using Vocab.Core.Entities;
using Vocab.WebApi.Extensions;

namespace Vocab.WebApi.Controllers
{
    [Route("dictionaries")]
    public class StatementDictionaryController(IStatementDictionaryService statementDictionaryService, IStatementPairService statementPairService) : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<long>> Add([FromQuery, Required] string name)
        {
            Guid userId = this.GetUserGuid();

            var result = await statementDictionaryService.Add(userId, name);
            return result.Match(onSuccess: id => Created("", id));
        }


        [HttpDelete, Route("{dictionaryId:long:min(1)}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute] long dictionaryId)
        {
            Guid userId = this.GetUserGuid();
            var result = await statementDictionaryService.Delete(userId, dictionaryId);
            return result.Match(NoContent);
        }

        [HttpPatch, Route("{dictionaryId:long:min(1)}/set/name/{name:length(1, 256)}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SetName([FromRoute] long dictionaryId, string name)
        {
            Guid userId = this.GetUserGuid();
            var result = await statementDictionaryService.SetName(userId, dictionaryId, name);
            return result.Match(NoContent);
        }

        [HttpPost, Route("{dictionaryId:long:min(1)}/import")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ImportStatements([FromRoute] long dictionaryId, IFormFile statements, [FromQuery] string separator = " - ")
        {
            Guid userId = this.GetUserGuid();
            using Stream stream = statements.OpenReadStream();

            var result = await statementDictionaryService.ImportStatements(userId, dictionaryId, stream, separator);
            return result.Match(Accepted);
        }

        [HttpGet, Route("{dictionaryId:long:min(1)}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<StatementDictionary>> GetById(long dictionaryId)
        {
            Guid userId = this.GetUserGuid();
            var result = await statementDictionaryService.GetById(userId, dictionaryId);
            return result.Match(value => Ok(value));
        }

        [HttpGet, Route("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<StatementDictionary[]>> GetDictionariesArray([FromQuery] int offset = 0, [FromQuery] bool appendTopStatements = false)
        {
            Guid userId = this.GetUserGuid();
            var result = await statementDictionaryService.GetUserDictionaries(userId, appendTopStatements, offset);
            return result.Match(value => Ok(value));
        }

        [HttpGet, Route("{dictionaryId:long:min(1)}/statements")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<StatementPair[]>> GetStatementPairsArray(long dictionaryId, [FromQuery] int offset = 0)
        {
            Guid userGuid = this.GetUserGuid();
            var result = await statementPairService.GetDictionaryStatementPairs(userGuid, dictionaryId, offset);
            return result.Match(value => Ok(value));
        }
    }
}
