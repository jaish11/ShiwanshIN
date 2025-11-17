
using AutoMapper;
using SS.Core.DTOs;
using SS.Core.Entities;

namespace SS.Application.Mapping
{
    public class MappingConfig:Profile
    {
        public MappingConfig()
        {
            //CreateMap<JobOpportunityDto, JobOpportunity>();
            //CreateMap<JobOpportunity, JobOpportunityDto>();
            CreateMap<JobOpportunity, JobOpportunityDto>().ReverseMap();
            CreateMap<ApplyJob, ApplyJobDto>().ReverseMap();
            CreateMap<User, UserDto>().ReverseMap();
        }
    }
}
