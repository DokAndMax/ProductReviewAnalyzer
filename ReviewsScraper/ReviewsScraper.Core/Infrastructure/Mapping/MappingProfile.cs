using AutoMapper;
using ProductReviewAnalyzer.Contracts.Events;
using ProductReviewAnalyzer.ReviewsScraper.Core.Application.DTOs;

namespace ProductReviewAnalyzer.ReviewsScraper.Core.Infrastructure.Mapping;

public sealed class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<ReviewScrapedData, ReviewScraped>()
            .ForCtorParam(
                nameof(ReviewScraped.RequestId),
                opt => opt.MapFrom(_ => Guid.Empty)
            );

        CreateMap<Guid, ReviewScraped>()
            .ForMember(dst => dst.RequestId, opt => opt.MapFrom(src => src));


        CreateMap<(ReviewScrapedData review, Guid requestId), ReviewScraped>()
            .IncludeMembers(src => src.review, src => src.requestId);
    }
}