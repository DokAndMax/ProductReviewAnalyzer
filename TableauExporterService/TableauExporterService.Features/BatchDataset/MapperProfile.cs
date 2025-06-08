using AutoMapper;
using ProductReviewAnalyzer.Contracts.Events;

namespace ProductReviewAnalyzer.TableauExporterService.Features.BatchDataset;

public sealed class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<ReviewAnalyzed, object>();
    }
}
