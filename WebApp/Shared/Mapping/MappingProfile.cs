using Mapster;
using ProductReviewAnalyzer.WebApp.Shared.Models;

namespace ProductReviewAnalyzer.WebApp.Shared.Mapping;

public class MappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<AnalyzeRequestModel, AnalyzeRequestModel>();
        config.NewConfig<CreateRequestDto, CreateRequestDto>();
    }
}