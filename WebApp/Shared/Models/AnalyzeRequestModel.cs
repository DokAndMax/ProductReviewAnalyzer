using System.ComponentModel.DataAnnotations;

namespace ProductReviewAnalyzer.WebApp.Shared.Models;

public class AnalyzeRequestModel
{
    [Required(ErrorMessage = "URL обов’язковий")]
    [Url(ErrorMessage = "Неправильний формат URL")]
    public string Url { get; set; } = string.Empty;
}