using Assessment.Api.Dto;
using Assessment.Api.Entity;

namespace Assessment.Api.Business
{
    public interface IPersonBL
    {
        Task<IEnumerable<Person>> GetAllAsync();
        Task<Person> CreatePersonAsync(PersonDto personDto);
        Task<Person> UpdatePersonAsync(int personId, PersonDto personDto);
        Task<bool> DeletePersonAsync(int personId);
    }
}
