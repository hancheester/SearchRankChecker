using MediatR;
using Microsoft.Extensions.Logging;
using SearchRankChecker.Api.Application.Common;
using SearchRankChecker.Api.Application.Common.Result;
using SearchRankChecker.Api.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;
using IResult = SearchRankChecker.Api.Application.Common.Result.IResult;

namespace SearchRankChecker.Api.Application.Search.Commands;

public class DeleteSearchHistoryCommand : IRequest<IResult>
{
    public int Id { get; set; }
}

public class DeleteSearchHistoryCommandHandler : IRequestHandler<DeleteSearchHistoryCommand, IResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteSearchHistoryCommandHandler> _logger;

    public DeleteSearchHistoryCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteSearchHistoryCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<IResult> Handle(DeleteSearchHistoryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await _unitOfWork.Repository<SearchHistory>().GetById(request.Id, cancellationToken);
            if (entity is null)
                return await Result.FailAsync("Search history not found.");

            await _unitOfWork.Repository<SearchHistory>().Delete(entity);
            await _unitOfWork.Commit(cancellationToken);

            return await Result.SuccessAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while deleting search history.");
            return await Result.FailAsync("Sorry, an error has occurred. Please try again later.");
        }
    }
}