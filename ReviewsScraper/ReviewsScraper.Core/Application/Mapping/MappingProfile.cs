using AutoMapper;
using ProductReviewAnalyzer.ReviewsScraper.Core.Application.DTOs;
using ProductReviewAnalyzer.ReviewsScraper.Core.Domain.Entities;

namespace ProductReviewAnalyzer.ReviewsScraper.Core.Application.Mapping;

public sealed class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Review, ReviewDto>()
            .ForCtorParam("ProductIds",
                opt => opt.MapFrom(r => r.Products.Select(p => p.ExternalId).ToList()));

        CreateMap<(Review review, Product product), ReviewScrapedData>()
            .IncludeMembers(src => src.review, src => src.product);

        CreateMap<Review, ReviewScrapedData>()
            .ForMember(dst => dst.ReviewId, opt => opt.MapFrom(src => src.ExternalId));

        CreateMap<Product, ReviewScrapedData>()
            .ForMember(dst => dst.ProductTitle, opt => opt.MapFrom(src => src.Title))
            .ForMember(dst => dst.ProductId, opt => opt.MapFrom(src => src.ExternalId))
            .ForMember(dst => dst.Store, opt => opt.MapFrom(src => src.Store.Name));
    }
}