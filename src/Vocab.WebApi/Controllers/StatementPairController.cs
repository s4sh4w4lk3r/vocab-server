﻿using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
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
        [HttpPatch, Route("{statementPairId:long:min(1)}", Name = nameof(UpdateStatement))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateStatement(
            [FromRoute] long statementPairId, 
            [FromQuery, MaxLength(512)] string? source = default,
            [FromQuery, MaxLength(512)] string? target = default,
            [FromQuery] StatementCategory statementCategory = StatementCategory.None)
        {
            Guid userGuid = this.GetUserGuid();
            var result = await statementPairService.Patch(userGuid, statementPairId, source, target, statementCategory);
            return result.Match(NoContent);
        }

        [HttpDelete, Route("{statementPairId:long:min(1)}", Name = nameof(DeleteStatement))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteStatement(long statementPairId)
        {
            Guid userGuid = this.GetUserGuid();
            var result = await statementPairService.Delete(userGuid, statementPairId);
            return result.Match(NoContent);
        }

        [HttpPost, Route("", Name = nameof(AddStatement))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<long>> AddStatement(StatementPairDto statementPairDto)
        {
            Guid userGuid = this.GetUserGuid();
            var result = await statementPairService.Add(userGuid, statementPairDto.ToEntity());

            return result.Match(id => CreatedAtRoute(
                routeName: nameof(GetStatementPair), 
                routeValues: new { statementPairId = id }, 
                value: null));
        }

        [HttpGet, Route("{statementPairId:long:min(1)}", Name = nameof(GetStatementPair))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<StatementPair>> GetStatementPair(long statementPairId)
        {
            Guid userGuid = this.GetUserGuid();
            var result = await statementPairService.GetById(userGuid, statementPairId);
            return result.Match(value => Ok(value));
        }

    }
}
