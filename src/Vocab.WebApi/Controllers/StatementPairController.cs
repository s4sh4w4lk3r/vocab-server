using Microsoft.AspNetCore.Mvc;
using Vocab.Application.Abstractions.Services;
using Vocab.Core.Enums;
using Vocab.WebApi.Extensions;
using Vocab.WebApi.Models;

namespace Vocab.WebApi.Controllers
{
    [Route("statements")]
    public class StatementPairController(IStatementPairService statementPairService) : ControllerBase
    {
        [HttpPatch, Route("{statementPairId:long:min(1)}/set/source/{sourceValue:length(1, 512)}")] 
        public async Task<IActionResult> SetSource(long statementPairId, string sourceValue)
        {
            Guid userGuid = this.GetUserGuid();
            var result = await statementPairService.SetSource(userGuid, statementPairId, sourceValue);
            return result.ToActionResult();
        }

        [HttpPatch, Route("{statementPairId:long:min(1)}/set/target/{targetValue:length(1, 512)}")]
        public async Task<IActionResult> SetTarget(long statementPairId, string targetValue)
        {
            Guid userGuid = this.GetUserGuid();
            var result = await statementPairService.SetTarget(userGuid, statementPairId, targetValue);
            return result.ToActionResult();
        }

        [HttpPatch, Route("{statementPairId:long:min(1)}/set/category/{categoryValue}")]
        public async Task<IActionResult> SetCategory(long statementPairId, StatementCategory categoryValue)
        {
            Guid userGuid = this.GetUserGuid();
            var result = await statementPairService.SetCategory(userGuid, statementPairId, categoryValue);
            return result.ToActionResult();
        }

        [HttpPatch, Route("{statementPairId:long:min(1)}/set/position/{priority:int}")]
        public async Task<IActionResult> SetPositionPriority(long statementPairId, int priority)
        {
            Guid userGuid = this.GetUserGuid();
            var result = await statementPairService.SetPositionPriority(userGuid, statementPairId, priority);
            return result.ToActionResult();
        }

        [HttpDelete, Route("{statementPairId:long:min(1)}")]
        public async Task<IActionResult> Delete(long statementPairId)
        {
            Guid userGuid = this.GetUserGuid();
            var result = await statementPairService.Delete(userGuid, statementPairId);
            return result.ToActionResult();
        }

        [HttpPost]
        public async Task<IActionResult> Add(StatementPairDto statementPairDto)
        {
            Guid userGuid = this.GetUserGuid();
            var result = await statementPairService.Add(userGuid, statementPairDto.ToEntity());
            return result.ToActionResult();
        }

        [HttpGet, Route("{statementPairId:long:min(1)}")]
        public async Task<IActionResult> GetById(long statementPairId)
        {
            Guid userGuid = this.GetUserGuid();
            var result = await statementPairService.GetById(userGuid, statementPairId);
            return result.ToActionResult();
        }

    }
}
