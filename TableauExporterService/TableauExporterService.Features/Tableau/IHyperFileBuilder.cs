namespace ProductReviewAnalyzer.TableauExporterService.Features.Tableau;

public interface IHyperFileBuilder
{
    byte[] Build(string jsonDataset);
}