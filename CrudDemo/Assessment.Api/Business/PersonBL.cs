using Assessment.Api.Dto;
using Assessment.Api.Entity;
using Assessment.Api.Exceptions;
using Assessment.Api.Repository;
using Assessment.Api.Utilities;
using AutoMapper;

namespace Assessment.Api.Business
{
    public class PersonBL : IPersonBL
    {
        private readonly IPersonRepository _personRepository;
        private readonly ICacheRepository<Person> _cacheRepository;
        private readonly ILogger<PersonBL> _logger;
        private readonly IMapper _mapper;

        public PersonBL(IPersonRepository personRepository,
            ICacheRepository<Person> cacheRepository,
            ILogger<PersonBL> logger,
            IMapper mapper)
        {
            _personRepository = personRepository;
            _cacheRepository = cacheRepository;
            _logger = logger;
            _mapper = mapper;
        }
        public async Task<IEnumerable<Person>> GetAllAsync()
        {
            IEnumerable<Person>? persons = _cacheRepository.TryGetList(Constants.CachePerson);
            if (persons == null)
            {
                _logger.LogInformation("No cache found.Fetch Person details from Repository");
                persons = await _personRepository.GetAllAsync();
                if (persons.Any())
                {
                    _cacheRepository.UpdateCache(Constants.CachePerson, persons.ToList());
                    _logger.LogInformation($"{persons.Count()} records fetched and updated to cache");
                }
            }
            return persons;
        }

        public async Task<Person> CreatePersonAsync(PersonDto personDto)
        {
            Person person = _mapper.Map<Person>(personDto);
            if (await ValidateModelAsync(person))
            {
                if (await _personRepository.IsDuplicatePersonAsync(person.Name))
                {
                    throw new UniqueException(ErrorMessages.PersonNameIsUnique);
                }
                await _personRepository.CreatePersonAsync(person);
                _cacheRepository.AddEntityToList(Constants.CachePerson, person);
                _logger.LogInformation($"Person created with Id {person.Id} and stored to cache");
            }
            return person;
        }

        public async Task<Person> UpdatePersonAsync(int personId, PersonDto personDto)
        {
            Person person = _mapper.Map<Person>(personDto);
            if (await ValidateModelAsync(person))
            {
                _ = await _personRepository.GetPersonByIdAsync(personId) ?? throw new NotFoundException($"{ErrorMessages.SearchedPersonNotFound} {personId}");
                if (await _personRepository.IsDuplicatePersonAsync(person.Name, personId))
                {
                    throw new UniqueException(ErrorMessages.PersonNameIsUnique);
                }
                person.Id = personId;
                await _personRepository.UpdatePersonAsync(person);
                _cacheRepository.UpdateEntityInList(Constants.CachePerson, personId, person);
                _logger.LogInformation($"Person with Id {person.Id} updated and stored to cache");
            }
            return person;
        }

        public async Task<bool> DeletePersonAsync(int personId)
        {
            Person? person = await _personRepository.GetPersonByIdAsync(personId) ?? throw new NotFoundException($"{ErrorMessages.SearchedPersonNotFound} {personId}");
            bool isDeleted = await _personRepository.DeletePersonAsync(person);
            if (isDeleted)
            {
                _cacheRepository.RemoveFromList(Constants.CachePerson, personId);
                _logger.LogInformation($"Person with Id {person.Id} deleted and removed from cache");
            }
            return isDeleted;
        }

        public async Task<bool> ValidateModelAsync(Person person)
        {
            if (string.IsNullOrWhiteSpace(person.Name))
            {
                throw new RequiredException(ErrorMessages.PersonNameIsRequired);
            }
            if (string.IsNullOrWhiteSpace(person.Address))
            {
                throw new RequiredException(ErrorMessages.PersonAddressIsRequired);
            }
            if (person.Name.Length > 50)
            {
                throw new MaxLengthException(ErrorMessages.PersonNameNotExceed50);
            }
            if (person.Address.Length > 250)
            {
                throw new MaxLengthException(ErrorMessages.PersonAddressNotExceed250);
            }
            await Task.Run(() => { _logger.LogInformation("All Validations executed successful"); });
            return true;
        }
    }
}
