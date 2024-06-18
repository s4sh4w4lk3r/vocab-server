using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Vocab.Application.Abstractions.Services;
using Vocab.Core.Entities;
using Vocab.WebApi.Extensions;
using Vocab.WebApi.Models;

namespace Vocab.WebApi.Controllers
{
    [Route("dictionaries")]
    public class StatementDictionaryController(IStatementDictionaryService statementDictionaryService, 
        IStatementPairService statementPairService, IStatementsImportService statementsImportService) : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<long>> Add([FromQuery, Required] string name)
        {
            Guid userId = this.GetUserGuid();

            var result = await statementDictionaryService.Add(userId, name);
            return result.Match(onSuccess: id => CreatedAtRoute(
                routeName: nameof(GetDictionary),
                routeValues: new { dictionaryId = id },
                value: null));
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
        [AllowAnonymous]
        public async Task<ActionResult<string>> ImportStatements([FromRoute] long dictionaryId, [FromBody] ImportStatementsDto importStatementsModel)
        {
            Guid userId = this.GetUserGuid();

            var result = await statementsImportService.ImportStatements(userId, dictionaryId, importStatementsModel.DocumentBase64, importStatementsModel.Separator);

            return result.Match(jobid => Accepted("", result.Value));
        }

        [HttpGet, Route("{dictionaryId:long:min(1)}", Name = nameof(GetDictionary))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<StatementDictionary>> GetDictionary(long dictionaryId)
        {
            Guid userId = this.GetUserGuid();
            var result = await statementDictionaryService.GetById(userId, dictionaryId);
            return result.Match(value => Ok(value));
        }

        [HttpGet, Route("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<StatementDictionary[]>> GetDictionariesArray([FromQuery] int offset = 0, [FromQuery] bool appendTopStatements = false, [FromQuery] string? searchQuery = null)
        {
            Guid userId = this.GetUserGuid();
            var result = string.IsNullOrWhiteSpace(searchQuery) is true
                ? await statementDictionaryService.GetUserDictionaries(userId, appendTopStatements, offset)
                : await statementDictionaryService.SearchByName(userId, searchQuery, appendTopStatements, offset);

            return result.Match(value => Ok(value));
        }

        [HttpGet, Route("{dictionaryId:long:min(1)}/statements")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<StatementPair[]>> GetStatementPairsArray(long dictionaryId, [FromQuery] int offset = 0)
        {
            Guid userGuid = this.GetUserGuid();
            var result = await statementPairService.GetStatements(userGuid, dictionaryId, offset);
            return result.Match(value => Ok(value));
        }
    }
}
