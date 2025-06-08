using System.ComponentModel.DataAnnotations;

namespace ProductReviewAnalyzer.WebApp.Shared.Models;

public class AnalyzeRequestModel
{
    [Required(ErrorMessage = "Поле не може бути порожнім")]
    public string Urls { get; set; } = string.Empty;
}