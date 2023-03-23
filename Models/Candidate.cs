
using System.ComponentModel.DataAnnotations;

namespace Candidate.Models;

public class CandidateModel
{
    public dynamic? Id { get; set; }
    [Required]
    public string? FirstName { get; set; }
    [Required]
    public string? LastName { get; set; }
    [Required]
    public DateTime? Dob { get; set; }
    public string? RoleId { get; set; }
    public string? Status { get; set; }
    public string? Stage { get; set; }
    [Required]
    [EmailAddress(ErrorMessage = "Not a valid Email format")]
    public string? Email { get; set; }
    public string? Password { get; set; }
    public DateTime? ApplDate { get; set; }
    [Required]
    [Phone(ErrorMessage = "Not a valid phone number")]
    public string? Phone { get; set; }
    public string? CvPath { get; set; }
    public string? CvExtension { get; set; }
    // public ExperienceModel[]? Expereince { get; set; }
    public string? Experience { get; set; }
    public List<string>? Skills { get; set; }
    public string? Education { get; set; }
    public IFormFile? Cv { get; set; }
    public string? Gender { get; set; }
}

public class CandidateData {
    public CandidateModel? data { get; set; }
    public List<string>? skills { get; set; }
}

public class UpdateEmail {
    [Required]
    [EmailAddress(ErrorMessage = "Not a valid Email Address")]
    public string? Email { get; set; }
    [Required]
    public string? Id { get; set; }
}

public class GetStatus {
    [Required]
    [EmailAddress(ErrorMessage = "Not a valid Email Address")]
    public string? Email { get; set; }
    [Required]
    public string? Password { get; set; }
}

public class UpdateRole {
    public string? Id { get; set; }
    public string? Stage { get; set; }
}

public class CancelApplication {
    public string? Id { get; set; }
}
public class ExperienceModel {
    public string? Employer { get; set; }
    public string? StartDate { get; set; }
    public string? EndDate { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
}