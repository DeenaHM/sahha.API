using System.ComponentModel.DataAnnotations;

namespace sahha.API.Contracts;

public class RegisterProfileRequest
{
    [Required(ErrorMessage = "ExternalId is required")]
    public Guid ExternalId { get; set; }
}
