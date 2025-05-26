using AutoMapper;
using ProductReviewAnalyzer.ReviewsScraper.Rozetka.Domain.Entities;
using ProductReviewAnalyzer.ReviewsScraper.Rozetka.Application.DTOs;

namespace ProductReviewAnalyzer.ReviewsScraper.Rozetka.Application.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Review, ReviewDto>();
    }
}