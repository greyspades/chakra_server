using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Mvc;
using Candidate.Models;
using Roles.Models;
using Credentials.Models;
using Meetings.Models;

namespace Candidate.Interface
{
   public interface ICandidateRepository
   {
       public Task<IEnumerable<CandidateModel>> GetCandidates();
       public Task<IEnumerable<CandidateModel>> GetCandidateById(string Id);
       public Task<string> CreateCandidate(CandidateModel payload);
       public Task<string> UpdateData(UpdateEmail payload);
       public Task<IEnumerable<CandidateModel>> GetCandidatesByRole(GetCandidatesDto payload);
       public Task<string> UpdateStage(UpdateRole payload);
       public Task<string> CancelApplication(CancelApplication id);
       public Task<IEnumerable<CandidateModel>> GetStatus(GetStatusDto payload);
       public Task<IEnumerable<BasicInfo>> CheckEmail(BasicInfo payload);
       public Task<dynamic> ParseCvAsync(IFormFile formFile, Guid id);
       public Task<dynamic> ParseCvData(IFormFile cv);
       public Task<byte[]> GetBytes(IFormFile formFile);
       public Task<IEnumerable<string>> GetSkills(string id);
       public Task<IEnumerable<string>> GetCandidateBySkills(SkillsInput payload);
       public Task<string> FlagCandidate(FlagCandidateDto payload);
       public Task<IEnumerable<CandidateModel>> GetByFlag(CandidateByFlagDto payload);
       public Task<string> HireCandidate(HireDto payload);
       public Task<CredentialsObj> GetCredentials();
       public Task<MeetingDto> CreateMeeting(MeetingDto payload);
       public Task<string> SendMail(EmailDto payload, CredentialsObj cred);
       public Task StoreSessionInfo(MeetingDto payload);
       public Task<IEnumerable<MeetingDto>> GetMeetings();
       public Task<IEnumerable<CandidateModel>> CheckCandidate(CandidateModel payload);
       public Task<IEnumerable<MeetingDto>> GetMeeting(MeetingDto payload);
       public Task<IEnumerable<CandidateModel>> GetCandidateByStage(StageDto payload);
       public Task<dynamic> GetMetrics();
       public Task<dynamic> CreateUser (BasicInfo payload);
       public Task<IEnumerable<BasicInfo>> GetBasicInfo(string email);
       public Task<dynamic> AdminAuth(AdminDto payload);
       public Task<IEnumerable<CandidateModel>> GetCandidateByMail(string mail);
       public Task<int> ConfirmEmail(string email);
       public Task CreateComment(CommentDto payload);
       public Task<IEnumerable<CommentDto>> GetComments(string id);
       public Task<dynamic> CreateTeamsMeeting();
    //    public Task<IEnumerable<MeetingDto>> GetMeetings();
    }
}
