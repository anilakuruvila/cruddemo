using Assessment.Api.Business;
using Assessment.Api.Dto;
using Assessment.Api.Entity;
using Microsoft.AspNetCore.Mvc;

namespace Assessment.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private readonly IPersonBL _personBl;

        public PersonController(IPersonBL personBL)
        {
            _personBl = personBL;
        }

        /// <summary>
        /// Get All Person details persisted
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetPersons()
        {
            try
            {
                var profiles = await _personBl.GetAllAsync();
                return profiles.Any() ? Ok(profiles) : NoContent();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Create a new Person
        /// </summary>
        /// <param name="personDto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreatePerson([FromBody] PersonDto personDto)
        {
            try
            {
                Person person = await _personBl.CreatePersonAsync(personDto);
                return Ok(person);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Update the existing Person
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="personDto"></param>
        /// <returns></returns>
        [HttpPut("{personId}")]
        public async Task<IActionResult> UpdatePerson([FromRoute] int personId, [FromBody] PersonDto personDto)
        {
            try
            {
                Person person = await _personBl.UpdatePersonAsync(personId, personDto);
                return Ok(person);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Delete a Person by Id
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        [HttpDelete("{personId}")]
        public async Task<IActionResult> DeletePerson(int personId)
        {
            try
            {
                bool isDeleted = await _personBl.DeletePersonAsync(personId);
                return Ok(isDeleted);
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
