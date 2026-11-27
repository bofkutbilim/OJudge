using System.ComponentModel.DataAnnotations;

namespace OJudge.Dtos;

public sealed class CreateOrganizationDto
{
    [Required]
    public required string Title { get; init; }
    public string? Description { get; init; }
}
