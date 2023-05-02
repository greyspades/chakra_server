namespace Meetings.Models;

public class Meeting {
    public string? MeetingId { get; set; }
    public string? Password { get; set; }
    public string? ParticipantId { get; set; }
    public string? Completed { get; set; }
    public DateTime? Date { get; set; }
    public string? Time { get; set; }
    public string? Link { get; set; }
    public string? JobId { get; set; }
}