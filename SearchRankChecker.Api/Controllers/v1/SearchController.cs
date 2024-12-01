using Microsoft.AspNetCore.Mvc;
using SearchRankChecker.Api.Application.Common.Pagination;
using SearchRankChecker.Api.Application.Common.Result;
using SearchRankChecker.Api.Application.Search.Commands;
using SearchRankChecker.Api.Application.Search.Dto;
using SearchRankChecker.Api.Application.Search.Queries;
using SearchRankChecker.Api.Filters;
using System.Net;
using System.Threading.Tasks;

namespace SearchRankChecker.Api.Controllers.v1;

public class SearchController : BaseApiController<SearchController>
{
    [HttpPut]
    [ProducesResponseType(typeof(IResult<SearchResult>), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(IResult<SearchResult>), (int)HttpStatusCode.OK)]
    [ServiceFilter(typeof(ValidateDtoFilter<SearchDtoValidator, SearchDto>))]
    public async Task<IActionResult> PutAsync([FromBody] SearchDto request)
    {
        var result = await Mediator.Send(new SearchQuery { Model = request });

        if (result.Succeeded)
            return Ok(result);

        return BadRequest(result);
    }

    [HttpGet("history")]
    [ProducesResponseType(typeof(IResult<IPagedList<SearchHistoryDto>>), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(IResult<IPagedList<SearchHistoryDto>>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetPaginatedSearchHistoryAsync([FromQuery] PaginationFilterModel request)
    {
        var result = await Mediator.Send(new GetPaginatedSearchHistoryQuery
        {
            PageIndex = request.PageIndex,
            PageSize = request.PageSize,
            Order = request.Order,
            ColumnSearch = request.ColumnSearch
        });

        if (result.Succeeded)
            return Ok(result);

        return BadRequest(result);
    }

    [HttpDelete("history/{id:int}")]
    [ProducesResponseType(typeof(IResult), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(IResult), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> DeleteSearchHistoryAsync(int id)
    {
        if (id <= 0)
            return BadRequest(Result.Fail("Invalid Id"));

        var result = await Mediator.Send(new DeleteSearchHistoryCommand { Id = id });

        if (result.Succeeded)
            return Ok(result);

        return BadRequest(result);
    }
}
