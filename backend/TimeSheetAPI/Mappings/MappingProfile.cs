using AutoMapper;
using TimeSheet.Api.DTOs;
using TimeSheet.Api.Models;


namespace TimeSheet.Api.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<TimesheetEntry, TimesheetEntryDto>().ReverseMap();
        }
    }
}