using AutoMapper;
using GoalTracker.DTOs.GoalDTOs;
using GoalTracker.DTOs.GoalTasksDTOs;
using GoalTracker.Models;

namespace GoalTracker.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<GoalCreateDto, Goal>();
            CreateMap<GoalUpdateDto, Goal>();
            CreateMap<Goal, GoalGetDto>()
                .ForMember(dest => dest.IsCompleted,
                    opt => opt.MapFrom(src => src.CompletedAt.HasValue))
                .ForMember(dest => dest.IsMissed, opt => opt.MapFrom(src =>
                    // Case 1: Completed late
                    (src.CompletedAt.HasValue && src.CompletedAt > src.Deadline) ||
                    // Case 2: Not completed and deadline passed
                    (!src.CompletedAt.HasValue && src.Deadline < DateTime.UtcNow)
                ));
            CreateMap<GoalTask, GoalTaskGetDto>();
            CreateMap<GoalTaskCreateDto, GoalTask>();
            CreateMap<GoalTaskUpdateDto, GoalTask>();
        }
    }
}
