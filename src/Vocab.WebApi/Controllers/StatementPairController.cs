using Microsoft.AspNetCore.Mvc;
using Vocab.Application.Abstractions.Services;
using Vocab.Core.Enums;
using Vocab.WebApi.Extensions;
using Vocab.WebApi.Models;

namespace Vocab.WebApi.Controllers
{
    [ApiController, Route("statement-pairs")]
    public class StatementPairController(IStatementPairService statementPairService) : ControllerBase
    {
#warning проверить
        [HttpPatch, Route("{id:long:min(1)}/set/source/{sourceValue:required:minlength(1)}")] 
        public async Task<IActionResult> SetSource(long id, string sourceValue)
        {
            Guid userGuid = this.GetUserGuid();
            var result = await statementPairService.SetSource(userGuid, id, sourceValue);
            return result.ToActionResult();
        }

        [HttpPatch, Route("{id:long:min(1)}/set/target/{targetValue:required:minlength(1)}")]
        public async Task<IActionResult> SetTarget(long id, string targetValue)
        {
            Guid userGuid = this.GetUserGuid();
            var result = await statementPairService.SetTarget(userGuid, id, targetValue);
            return result.ToActionResult();
        }

        [HttpPatch, Route("{id:long:min(1)}/set/category/{categoryValue}")]
        public async Task<IActionResult> SetCategory(long id, StatementCategory categoryValue)
        {
            Guid userGuid = this.GetUserGuid();
            var result = await statementPairService.SetCategory(userGuid, id, categoryValue);
            return result.ToActionResult();
        }

        [HttpDelete, Route("{id:long:min(1)}")]
        public async Task<IActionResult> Delete(long id)
        {
            Guid userGuid = this.GetUserGuid();
            var result = await statementPairService.Delete(userGuid, id);
            return result.ToActionResult();
        }

        [HttpPost]
        public async Task<IActionResult> Add(StatementPairDto statementPairDto)
        {
            Guid userGuid = this.GetUserGuid();
            var result = await statementPairService.Insert(userGuid, statementPairDto.ToEntity());
            return result.ToActionResult();
        }
    }
}
