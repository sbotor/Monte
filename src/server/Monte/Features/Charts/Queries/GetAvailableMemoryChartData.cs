using FluentValidation;
using MediatR;
using Monte.Contracts;
using Monte.Features.Charts.Helpers;
using Monte.Features.Charts.Models;

namespace Monte.Features.Charts.Queries;

public static class GetAvailableMemoryChartData
{
    public record Query(
        Guid AgentId,
        DateTime DateFrom,
        DateTime DateTo,
        ChartAggregationType AggregationType,
        bool Swap)
        : IRequest<ChartData<double>>, IMemoryChartRequest;

    internal class Handler : IRequestHandler<Query, ChartData<double>>
    {
        private readonly IMemoryChartHelper<Query> _helper;

        public Handler(IMemoryChartHelper<Query> helper)
        {
            _helper = helper;
        }

        public Task<ChartData<double>> Handle(Query request, CancellationToken cancellationToken)
            => _helper.GetAvailableMemoryData(request, cancellationToken);
    }

    public class Validator : AbstractValidator<GetCpuUsageChartData.Query>
    {
        public Validator()
        {
            Include(new DateRangeValidator());
        }
    }
}
