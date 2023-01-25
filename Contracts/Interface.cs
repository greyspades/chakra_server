using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Mvc;
using Candidate.Models;

namespace Recruitment.Interface
{
   public interface IRecruitmentRepository
   {
       public Task<IEnumerable<CandidateModel>> GetCandidates();
       public Task<dynamic> TestEndpoint();
       public Task<IEnumerable<CandidateModel>> GetCandidateById(string Id);
       public Task<string> CreateCandidate(CandidateModel payload);
       public Task<string> UpdateData(UpdateEmail payload);

   }
}
