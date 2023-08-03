using Assessment.Api.Entity;

namespace Assessment.Api.Repository
{
    public interface IPersonRepository
    {
        Task<IEnumerable<Person>> GetAllAsync();
        Task<Person?> GetPersonByIdAsync(int id);
        Task<Person> CreatePersonAsync(Person person);
        Task<Person> UpdatePersonAsync(Person person);
        Task<bool> DeletePersonAsync(Person person);
        Task<bool> IsDuplicatePersonAsync(string personName, int personId = 0);
    }
}
