using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Mvc;
using Candidate.Models;
using Roles.Models;
using Credentials.Models;
using Meetings.Models;

namespace JobRole.Interface
{
   public interface IJobRoleRepository
   {
       public Task<IEnumerable<dynamic>> GetJobRoles();
       public Task<IEnumerable<MeetingDto>> GetMeetingsByJob(string id);
       public Task<IEnumerable<JobRoleModel>> GetJobByCode(string code);
       public Task<IEnumerable<dynamic>> GetJobDescription(string code);
       public Task<IEnumerable<JobRoleModel>> GetRoles();
       public Task AddJobRole(JobRoleModel payload);
       public Task<JobRoleModel> GetJobRoleById(string id);
       public Task<JobRoleModel> GetJobRoleByUnit(string unit);
    }
}
