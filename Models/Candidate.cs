
using System.ComponentModel.DataAnnotations;

namespace Candidate.Models;

public class EncryptedPayload<T>
{
    public string EncryptedData { get; set; }
}
public class CreateCandidate {
    public CandidateModel? CanData { get; set; }
    public IFormFile? Cv { get; set; }
}

public class BasicInfo {
    public dynamic? Id { get; set; }
    [Required]
    [StringLength(30)]
    public string? FirstName { get; set; }
    [Required]
    [StringLength(30)]
    public string? LastName { get; set; }
    [Required]
    public DateTime? Dob { get; set; }
    [Required]
    [EmailAddress(ErrorMessage = "Not a valid Email format")]
    public string? Email { get; set; }
    [Required]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,50}$", ErrorMessage = "Not a valid Password")]
    public string? Password { get; set; }
    [Required]
    [Phone(ErrorMessage = "Not a valid phone number")]
    public string? Phone { get; set; }
    [Required]
    public string? Address { get; set; }
    public string? MaritalStatus { get; set; }
    [StringLength(30)]
    public string? OtherName { get; set; }
    public string? Gender { get; set; }
    public string? EmailValid { get; set; }
    public string? State { get; set; }
    public string? Lga { get; set; }
}

public class CreateCandidateDto {
    public IFormFile Cv { get; set; }
    public CandidateModel Data { get; set; }

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
    public string? State { get; set; }
    public string? Lga { get; set; }
    public string? JobType { get; set; }
    public string? HireDate { get; set; }

    public static explicit operator string(CandidateModel v)
    {
        throw new NotImplementedException();
    }
}

public class CandidateData {
    public CandidateModel? Data { get; set; }
    public List<string>? Skills { get; set; }
}

public class HireDto {
    public string? Id { get; set; }
    [Required]
    public string? Location { get; set; }
    [Required]
    public string? Position { get; set; }
    public string? Rank { get; set; }
    public DateTime? StartDate { get; set; }
    public string? ReportTo { get; set; }
    public DateTime? Date { get; set; }
    [Required]
    public string? FirstName { get; set; }
    [Required]
    public string? LastName { get; set; }
    public string? City { get; set; }
    public string? Salary { get; set; }
    public string? Address { get; set; }
    public string? SalWords { get; set; }
    public string? JobType { get; set; }
    public bool? SendMail { get; set; }
    public DateTime? HireDate { get; set; }
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
    // public string? MeetingLink { get; set; }
    // public string? MeetingId { get; set; }
    // public string? MeetingPassCode { get; set; }
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
    public int? Page { get; set; }
    public int? Take { get; set; }
}

public class AdminDto {
    public string? Id { get; set; }
    public string? Password { get; set; }
}

public class GetCandidatesDto {
    public string? Id { get; set; }
    public int? Page { get; set; }
    public int? Take { get; set; }
    public string? Filter { get; set; }
    public string? FilterValue { get; set; }
}

public class CandidateDto {
    public string? Id { get; set; }
}

public class CommentDto {
    public string? Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime? Date { get; set; }
    public string? Comment { get; set; }
}

public class CandidateEmailDto {
    public string? Email { get; set; }
}

public class PasswordResetDto {
    public string? Password { get; set; }
    public string? Email { get; set; }
    public string? Id { get; set; }
}
public class PasswordResetFields {
    public string? FirstName { get; set; }
    public string? Email { get; set; }
}

public class OfferMailDto {
    public string? Location { get; set; }
    public string? Position { get; set; }
    public string? Rank { get; set; }
    public DateTime? StartDate { get; set; }
    public string? ReportTo { get; set; }
    public DateTime? Date { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? City { get; set; }
    public string? Salary { get; set; }
    public string? SalWords { get; set; }
}

public class DocFields {
    public string? Key { get; set; }
    public string? Value { get; set; }
}