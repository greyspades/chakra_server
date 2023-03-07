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
       public Task<dynamic> TestEndpoint();
       public Task<IEnumerable<CandidateModel>> GetCandidateById(string Id);
       public Task<string> CreateCandidate(CandidateModel payload);
       public Task<string> UpdateData(UpdateEmail payload);
       public Task<IEnumerable<CandidateModel>> GetCandidatesByRole(string id);
       public Task<string> UpdateStage(UpdateRole payload);
       public Task<string> CancelApplication(string id)

   }

   public interface IRolesRepository
   {
    public Task<IEnumerable<RoleModel>> GetRoles();
    public Task<IEnumerable<CandidateModel>> PostRole();


   }
}
