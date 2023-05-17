
using System.ComponentModel.DataAnnotations;
using MongoDB.Driver;

namespace Candidate.Models;

public class BasicInfo {
    public dynamic? Id { get; set; }
    [Required]
    public string? FirstName { get; set; }
    [Required]
    public string? LastName { get; set; }
    [Required]
    public DateTime? Dob { get; set; }
    [Required]
    [EmailAddress(ErrorMessage = "Not a valid Email format")]
    public string? Email { get; set; }
    public string? Password { get; set; }
    [Required]
    [Phone(ErrorMessage = "Not a valid phone number")]
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? MaritalStatus { get; set; }
    public string? OtherName { get; set; }
    public string? Gender { get; set; }
    public string? EmailValid { get; set; }
}

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
    public string? Flag { get; set; }
    public string? OtherName { get; set; }
    public string? JobName { get; set; }
    public string? CoverLetter { get; set; }
    public string? TempId { get; set; }
    public string? Address { get; set; }
    public string? MaritalStatus { get; set; }
    public string? EmailValid { get; set; }
}

public class CandidateData {
    public CandidateModel? Data { get; set; }
    public List<string>? Skills { get; set; }
}

public class HireDto {
    public string? Id { get; set; }
}

public class SignInDto {
    public string? Email { get; set; }
    public string? Password { get; set; }
}

public class EmailDto {
    public string? Firstname { get; set; }
    // public string? Template { get; set; }
    public string? Subject { get; set; }
    public string? EmailAddress { get; set; }
    public string? Body { get; set; }
    public string? HasFile { get; set; }
    public string? MeetingLink { get; set; }
    public string? MeetingId { get; set; }
    public string? MeetingPassCode { get; set; }
}

public class CandidateByFlagDto {
    public string? RoleId { get; set; }
    public string? Flag { get; set; }
}

public class FlagCandidateDto {
    public string? Id { get; set; }
    public string? Flag { get; set; }

    public string? RoleName { get; set; }
}

public class SkillsInput {
    public string? Id { get; set; }
    public string? Unit { get; set; }
}

public class UpdateEmail {
    [Required]
    [EmailAddress(ErrorMessage = "Not a valid Email Address")]
    public string? Email { get; set; }
    [Required]
    public string? Id { get; set; }
}

public class GetStatusDto {
    [Required]
    [EmailAddress(ErrorMessage = "Not a valid Email Address")]
    public string? Email { get; set; }
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
public class MeetingDto {
    public string? Date { get; set; }
    public string? Time { get; set; }
    public string? Topic { get; set; }
    public string? ParticipantId { get; set; }
    public string? Completed { get; set; }
    public string? MeetingId { get; set; }
    public string? Password { get; set; }
    public string? Id { get; set; }
    public string? Link { get; set; }
    public string? FirstName { get; set; }
    public string? Email { get; set; }
    public string? JobTitle { get; set; }
    public string? JobId { get; set; }
    public string? LastName { get; set; }

}

public class StageDto {
    public string? Stage { get; set; }
    public string? RoleId { get; set; }
}

public class AdminDto {
    public string? Id { get; set; }
    public string? Password { get; set; }
}

public class CommentDto {
    public string? Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime? Date { get; set; }
    public string? Comment { get; set; }
}