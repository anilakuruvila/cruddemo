using Assessment.Api.Entity;
using Microsoft.EntityFrameworkCore;

namespace Assessment.Api.Repository
{
    public class PersonRepository : IPersonRepository
    {
        private readonly AgDbContext _dbContext;
        public PersonRepository(AgDbContext agDbContext)
        {
            _dbContext = agDbContext;
        }
        public async Task<IEnumerable<Person>> GetAllAsync()
        {
            IEnumerable<Person> persons = await _dbContext.Persons.AsNoTracking().ToListAsync();
            return persons;
        }

        public async Task<Person?> GetPersonByIdAsync(int id)
        {
            return await _dbContext.Persons.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Person> CreatePersonAsync(Person person)
        {
            var model = _dbContext.Persons.Add(person);
            await Save();
            return model.Entity;
        }

        public async Task<Person> UpdatePersonAsync(Person person)
        {
            var model = _dbContext.Persons.Update(person);
            await Save();
            return model.Entity;
        }

        public async Task<bool> DeletePersonAsync(Person person)
        {
            _dbContext.Persons.Remove(person);
            return await Save();
        }

        public async Task<bool> IsDuplicatePersonAsync(string personName, int personId = 0)
        {
            return await _dbContext.Persons.Where(c => c.Id != personId).AnyAsync(c => c.Name.Equals(personName, StringComparison.OrdinalIgnoreCase));
        }

        private async Task<bool> Save()
        {
            return await _dbContext.SaveChangesAsync() > 0;
        }
    }
}
