using Assessment.Api.Dto;

namespace Assessment.Api.Utilities
{
    public class AgMapper : AutoMapper.Profile
    {
        public AgMapper()
        {
            CreateMap<Entity.Person, PersonDto>().ReverseMap();
        }
    }
}
