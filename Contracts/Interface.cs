using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Mvc;
using Candidate.Models;
using Roles.Models;

namespace Recruitment.Interface
{
   public interface ICandidateRepository
   {
       public Task<IEnumerable<CandidateModel>> GetCandidates();
       public Task<IEnumerable<CandidateModel>> GetCandidateById(string Id);
       public Task<string> CreateCandidate(CandidateModel payload);
       public Task<string> UpdateData(UpdateEmail payload);
       public Task<IEnumerable<CandidateModel>> GetCandidatesByRole(string id);
       public Task<string> UpdateStage(UpdateRole payload);
       public Task<string> CancelApplication(CancelApplication id);
       public Task<IEnumerable<CandidateModel>> GetStatus(GetStatus payload);
       public Task<IEnumerable<CandidateModel>> CheckEmail(CandidateModel payload);
       public Task<dynamic> ParseCvAsync(IFormFile formFile, Guid id);
       public Task<dynamic> ParseCvData(IFormFile cv);
       public Task<byte[]> GetBytes(IFormFile formFile);
       public Task<IEnumerable<string>> GetSkills(string id);
       public Task<IEnumerable<string>> GetCandidateBySkills(SkillsInput payload);
       public Task<string> FlagCandidate(FlagCandidateDto payload);
       public Task<IEnumerable<CandidateModel>> GetByFlag(CandidateByFlagDto payload);
       public void SendMail(EmailDto payload);
    }

   public interface IRolesRepository
   {
    public Task<IEnumerable<RoleModel>> GetRoles();
    public Task<IEnumerable<CandidateModel>> PostRole();


   }
}
