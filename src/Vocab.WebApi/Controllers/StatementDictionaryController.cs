using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Vocab.Application.Abstractions.Services;
using Vocab.Application.Enums;
using Vocab.Core.Entities;
using Vocab.WebApi.Extensions;
using Vocab.WebApi.Models;

namespace Vocab.WebApi.Controllers
{
    [Route("dictionaries")]
    public class StatementDictionaryController(IStatementDictionaryService statementDictionaryService, 
        IStatementPairService statementPairService, IStatementsImportService statementsImportService) : ControllerBase
    {
        [HttpPost, Route("", Name = nameof(AddDictionary))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<long>> AddDictionary([FromQuery, Required] string name)
        {
            Guid userId = this.GetUserGuid();

            var result = await statementDictionaryService.Add(userId, name);
            return result.Match(onSuccess: id => CreatedAtRoute(
                routeName: nameof(GetDictionary),
                routeValues: new { dictionaryId = id },
                value: null));
        }


        [HttpDelete, Route("{dictionaryId:long:min(1)}", Name = nameof(DeleteDictionary))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteDictionary([FromRoute] long dictionaryId)
        {
            Guid userId = this.GetUserGuid();
            var result = await statementDictionaryService.Delete(userId, dictionaryId);
            return result.Match(NoContent);
        }

        [HttpPatch, Route("{dictionaryId:long:min(1)}", Name = nameof(RenameDictionary))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RenameDictionary([FromRoute] long dictionaryId, [FromQuery, MaxLength(256)] string name)
        {
            Guid userId = this.GetUserGuid();
            var result = await statementDictionaryService.SetName(userId, dictionaryId, name);
            return result.Match(NoContent);
        }

        [HttpPost, Route("{dictionaryId:long:min(1)}/import", Name = nameof(ImportStatements))]
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
        public async Task<ActionResult<StatementDictionary>> GetDictionary(long dictionaryId, AppendStatementsAction appendAction = AppendStatementsAction.NotRequired)
        {
            Guid userId = this.GetUserGuid();
            var result = await statementDictionaryService.GetById(userId, dictionaryId, appendAction);
            return result.Match(value => Ok(value));
        }

        [HttpGet, Route("", Name = nameof(GetDictionaries))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<StatementDictionary[]>> GetDictionaries(
            [FromQuery] int offset = 0, AppendStatementsAction appendAction = AppendStatementsAction.NotRequired, [FromQuery] string? searchQuery = null)
        {
            Guid userId = this.GetUserGuid();
            var result = string.IsNullOrWhiteSpace(searchQuery) is true
                ? await statementDictionaryService.GetUserDictionaries(userId, appendAction, offset)
                : await statementDictionaryService.SearchByName(userId, searchQuery, appendAction, offset);

            return result.Match(value => Ok(value));
        }

        [HttpGet, Route("{dictionaryId:long:min(1)}/statements", Name = nameof(GetStatementPairs))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<StatementPair[]>> GetStatementPairs(long dictionaryId, [FromQuery] int offset = 0)
        {
            Guid userGuid = this.GetUserGuid();
            var result = await statementPairService.GetStatements(userGuid, dictionaryId, offset);
            return result.Match(value => Ok(value));
        }
    }
}
