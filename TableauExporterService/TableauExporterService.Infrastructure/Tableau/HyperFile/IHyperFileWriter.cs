using Tableau.HyperAPI;

namespace ProductReviewAnalyzer.TableauExporterService.Infrastructure.Tableau.HyperFile;

public interface IHyperFileWriter
{
    byte[] Write(IEnumerable<TableDefinition> tableDefinitions, string jsonDataset);
}