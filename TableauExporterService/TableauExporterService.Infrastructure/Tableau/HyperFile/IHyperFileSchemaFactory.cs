using Tableau.HyperAPI;

namespace ProductReviewAnalyzer.TableauExporterService.Infrastructure.Tableau.HyperFile;
internal interface IHyperFileSchemaFactory
{
    IReadOnlyCollection<TableDefinition> Create();
}
