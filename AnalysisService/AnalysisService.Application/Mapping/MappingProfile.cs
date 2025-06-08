using AutoMapper;
using ProductReviewAnalyzer.AnalysisService.Application.DTOs;
using ProductReviewAnalyzer.AnalysisService.Domain.Entities;
using ProductReviewAnalyzer.AnalysisService.Domain.ValueObjects;

namespace ProductReviewAnalyzer.AnalysisService.Application.Mapping;
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<AspectItem, AspectItemData>()
             .ReverseMap();

        CreateMap<UsageInsightItem, UsageInsightItemData>()
            .ReverseMap();

        CreateMap<AspectSentimentItem, AspectSentimentItemData>()
            .ForMember(
                dest => dest.Sentiment,
                opt => opt.MapFrom(src => src.Sentiment.ToString()))
            .ReverseMap()
            .ForMember(
                dest => dest.Sentiment,
                opt => opt.MapFrom(src => Enum.Parse<Sentiment>(src.Sentiment)));

        CreateMap<ReviewAnalysis, ReviewAnalyzedData>()
            .ForMember(dest => dest.ProductSentimentScore,
                       opt => opt.MapFrom(src => (decimal)src.ProductSentimentScore))
            .ForMember(dest => dest.StoreSentimentScore,
                       opt => opt.MapFrom(src => (decimal)src.StoreSentimentScore))
            .ForMember(dest => dest.CreatedAtUtc,
                       opt => opt.MapFrom(src => src.ReviewCreatedAt))
            .ForMember(dest => dest.AnalyzedAtUtc,
                       opt => opt.MapFrom(src => src.AnalysisCreatedAtUtc))
            .ReverseMap()
            .ForMember(dest => dest.ProductSentimentScore,
                       opt => opt.MapFrom(src => (double)src.ProductSentimentScore))
            .ForMember(dest => dest.StoreSentimentScore,
                       opt => opt.MapFrom(src => (double)src.StoreSentimentScore))
            .ForMember(dest => dest.ReviewCreatedAt,
                       opt => opt.MapFrom(src => src.CreatedAtUtc))
            .ForMember(dest => dest.AnalysisCreatedAtUtc,
                       opt => opt.MapFrom(src => src.AnalyzedAtUtc));

        CreateMap<(ReviewAnalysis, Guid), ReviewAnalyzedData>()
            .IncludeMembers(src => src.Item1)
            .ForMember(src => src.RequestId,
                       opt => opt.MapFrom(src => src.Item2));
    }
}