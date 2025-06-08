using ProductReviewAnalyzer.TableauExporterService.Features.Tableau;

namespace ProductReviewAnalyzer.TableauExporterService.Infrastructure.Tableau.HyperFile;

internal sealed class HyperFileBuilder(IHyperFileSchemaFactory schema, IHyperFileWriter writer)
    : IHyperFileBuilder
{
    public byte[] Build(string jsonDataset)
    {
        var schemaDef = schema.Create();
        return writer.Write(schemaDef, jsonDataset);
    }
}