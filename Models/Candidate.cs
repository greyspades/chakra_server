
namespace Candidate.Models;

public class CandidateModel
{
    public dynamic? Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime? Dob { get; set; }
    public int? RoleId { get; set; }
    public string? Status { get; set; }
    public string? Stage { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public DateTime? ApplicationDate { get; set; }
}

public class UpdateEmail {
    public string? Email { get; set; }
    public string? Id { get; set; }
}