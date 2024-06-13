using Microsoft.AspNetCore.Mvc;
using Vocab.Application.Abstractions.Services;
using Vocab.Core.Entities;
using Vocab.Core.Enums;
using Vocab.WebApi.Extensions;
using Vocab.WebApi.Models;

namespace Vocab.WebApi.Controllers
{
    [Route("statements")]
    public class StatementPairController(IStatementPairService statementPairService) : ControllerBase
    {
        [HttpPatch, Route("{statementPairId:long:min(1)}/set/source/{sourceValue:length(1, 512)}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SetSource(long statementPairId, string sourceValue)
        {
            Guid userGuid = this.GetUserGuid();
            var result = await statementPairService.SetSource(userGuid, statementPairId, sourceValue);
            return result.Match(NoContent);
        }

        [HttpPatch, Route("{statementPairId:long:min(1)}/set/target/{targetValue:length(1, 512)}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SetTarget(long statementPairId, string targetValue)
        {
            Guid userGuid = this.GetUserGuid();
            var result = await statementPairService.SetTarget(userGuid, statementPairId, targetValue);
            return result.Match(NoContent);
        }

        [HttpPatch, Route("{statementPairId:long:min(1)}/set/category/{categoryValue}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SetCategory(long statementPairId, StatementCategory categoryValue)
        {
            Guid userGuid = this.GetUserGuid();
            var result = await statementPairService.SetCategory(userGuid, statementPairId, categoryValue);
            return result.Match(NoContent);
        }

        [HttpDelete, Route("{statementPairId:long:min(1)}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(long statementPairId)
        {
            Guid userGuid = this.GetUserGuid();
            var result = await statementPairService.Delete(userGuid, statementPairId);
            return result.Match(NoContent);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<long>> Add(StatementPairDto statementPairDto)
        {
            Guid userGuid = this.GetUserGuid();
            var result = await statementPairService.Add(userGuid, statementPairDto.ToEntity());

            return result.Match(id => CreatedAtRoute(
                routeName: nameof(GetStatementsPair), 
                routeValues: new { statementPairId = id }, 
                value: null));
        }

        [HttpGet, Route("{statementPairId:long:min(1)}", Name = nameof(GetStatementsPair))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<StatementPair>> GetStatementsPair(long statementPairId)
        {
            Guid userGuid = this.GetUserGuid();
            var result = await statementPairService.GetById(userGuid, statementPairId);
            return result.Match(value => Ok(value));
        }

    }
}
