using AutoMapper;
using ProductReviewAnalyzer.AnalysisService.Application.DTOs;
using ProductReviewAnalyzer.Contracts.DTOs;
using ProductReviewAnalyzer.Contracts.Events;

namespace ProductReviewAnalyzer.AnalysisService.Messaging.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<ReviewAnalyzedData, ReviewAnalyzed>()
            .ReverseMap();

        CreateMap<AspectItemData, AspectItemDto>()
            .ReverseMap();

        CreateMap<UsageInsightItemData, UsageInsightItemDto>()
            .ReverseMap();

        CreateMap<AspectSentimentItemData, AspectSentimentItemDto>()
            .ReverseMap();
    }
}