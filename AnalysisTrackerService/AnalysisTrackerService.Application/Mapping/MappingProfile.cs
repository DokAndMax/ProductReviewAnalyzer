using Mapster;
using ProductReviewAnalyzer.AnalysisTrackerService.Application.DTOs;
using ProductReviewAnalyzer.AnalysisTrackerService.Domain.Entities;

namespace ProductReviewAnalyzer.AnalysisTrackerService.Application.Mapping;

public class MappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<AnalysisRequest, AnalysisRequestDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest.CreatedAtUtc, src => src.CreatedAtUtc)
            .Map(dest => dest.Status, src => src.Status)
            .Map(dest => dest.DashboardUrl, src => src.DashboardUrl)
            .Map(dest => dest.ProductUrls, src => src.Items.Select(i => i.ProductUrl)
                .ToList());

        config.NewConfig<AnalysisRequestDto, AnalysisRequest>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest.CreatedAtUtc, src => src.CreatedAtUtc)
            .Map(dest => dest.Status, src => src.Status)
            .Map(dest => dest.DashboardUrl, src => src.DashboardUrl)
            .Map(dest => dest.Items, src => src.ProductUrls
                .Select(url => new AnalysisItem { ProductUrl = url })
                .ToList());
    }
}