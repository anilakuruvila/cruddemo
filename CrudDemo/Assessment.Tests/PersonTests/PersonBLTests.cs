using Assessment.Api.Business;
using Assessment.Api.Dto;
using Assessment.Api.Entity;
using Assessment.Api.Exceptions;
using Assessment.Api.Repository;
using Assessment.Api.Utilities;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;

namespace Assessment.Tests.PersonTests
{
    /// <summary>
    /// The Test class for testing PersonBL
    /// </summary>
    public class PersonBLTests
    {
        private readonly PersonBL _personBl;
        private readonly Mock<IPersonRepository> _personRepoMock = new Mock<IPersonRepository>();
        private readonly Mock<ICacheRepository<Person>> _cacheRepoMock = new Mock<ICacheRepository<Person>>();
        private readonly Mock<ILogger<PersonBL>> _loggerMock = new Mock<ILogger<PersonBL>>();
        private readonly IMapper _mapper;

        public PersonBLTests()
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new AgMapper());
            });
            IMapper mapper = mappingConfig.CreateMapper();
            _mapper = mapper;

            _personBl = new PersonBL(_personRepoMock.Object, _cacheRepoMock.Object, _loggerMock.Object, _mapper);
        }

        [Fact]
        public async void GetAll_CheckCount_Matching()
        {
            //Arrange
            _personRepoMock.Setup(x => x.GetAllAsync())
                .ReturnsAsync(PersonBLTestHelper.SeedPersonList());

            //Act
            IEnumerable<Person> persons = await _personBl.GetAllAsync();

            //Assert
            Assert.Equal(PersonBLTestHelper.SeedPersonList().Count, persons.Count());
        }

        [Fact]
        public async void CreatePerson_CheckUniqueExceptionThrown_When_Try_Insert_Duplicate()
        {
            //Arrange
            PersonDto dto = new PersonDto { Address = "19 XXX Ave", Name = "Lintu" };
            _personRepoMock.Setup(x => x.IsDuplicatePersonAsync(dto.Name, 0))
               .ReturnsAsync(true);
            var exceptionType = typeof(UniqueException);
            var exceptionMessage = ErrorMessages.PersonNameIsUnique;

            //Act
            var exception = await Assert.ThrowsAsync<UniqueException>(() => _personBl.CreatePersonAsync(dto));

            //Assert
            Assert.NotNull(exception);
            Assert.IsType(exceptionType, exception);
            Assert.Equal(exceptionMessage, exception.Message);
        }

        [Fact]
        public async void UpdatePerson_CheckNotFoundExceptionThrown_When_Searched_Id_NotFound()
        {
            //Arrange
            Person? model = null;
            PersonDto dto = new PersonDto { Address = "19 XXX Ave", Name = "Zeya" };
            _personRepoMock.Setup(x => x.GetPersonByIdAsync(5))
               .ReturnsAsync(model);

            var exceptionType = typeof(NotFoundException);
            var exceptionMessage = ErrorMessages.SearchedPersonNotFound;

            //Act
            var exception = await Assert.ThrowsAsync<NotFoundException>(() => _personBl.UpdatePersonAsync(5, dto));

            //Assert
            Assert.NotNull(exception);
            Assert.IsType(exceptionType, exception);
            Assert.Contains(exceptionMessage, exception.Message);
        }

        [Fact]
        public async void UpdatePerson_CheckUniqueExceptionThrown_When_Try_Insert_Duplicate()
        {
            //Arrange
            PersonDto dto = new PersonDto { Address = "19 XXX Ave", Name = "Aneeta" };
            Person model = new Person { Address = "19 XXX Ave", Name = "Aneeta" };
            _personRepoMock.Setup(x => x.GetPersonByIdAsync(3))
                    .ReturnsAsync(model);
            _personRepoMock.Setup(x => x.IsDuplicatePersonAsync(dto.Name, 3))
               .ReturnsAsync(true);
            var exceptionType = typeof(UniqueException);
            var exceptionMessage = ErrorMessages.PersonNameIsUnique;

            //Act
            var exception = await Assert.ThrowsAsync<UniqueException>(() => _personBl.UpdatePersonAsync(3, dto));

            //Assert
            Assert.NotNull(exception);
            Assert.IsType(exceptionType, exception);
            Assert.Equal(exceptionMessage, exception.Message);
        }

        [Fact]
        public async void DeletePerson_CheckNotFoundExceptionThrown_When_Searched_Id_NotFound()
        {
            //Arrange
            Person? model = null;
            _personRepoMock.Setup(x => x.GetPersonByIdAsync(5))
               .ReturnsAsync(model);
            var exceptionType = typeof(NotFoundException);
            var exceptionMessage = ErrorMessages.SearchedPersonNotFound;

            //Act
            var exception = await Assert.ThrowsAsync<NotFoundException>(() => _personBl.DeletePersonAsync(5));

            //Assert
            Assert.NotNull(exception);
            Assert.IsType(exceptionType, exception);
            Assert.Contains(exceptionMessage, exception.Message);
        }


        [Theory]
        [InlineData(" ")]
        [InlineData("")]
        [InlineData(null)]
        public async void ValidateModel_CheckRequiredExceptionThrown_WhenNameIsNullOrEmpty(string value)
        {
            //Arrange
            Person model = new Person { Address = "19 XXX Ave", Name = value };
            var exceptionType = typeof(RequiredException);
            var exceptionMessage = ErrorMessages.PersonNameIsRequired;

            //Act
            var exception = await Assert.ThrowsAsync<RequiredException>(() => _personBl.ValidateModelAsync(model));

            //Assert
            Assert.NotNull(exception);
            Assert.IsType(exceptionType, exception);
            Assert.Equal(exceptionMessage, exception.Message);
        }

        [Theory]
        [InlineData(" ")]
        [InlineData("")]
        [InlineData(null)]
        public async void ValidateModel_CheckRequiredExceptionThrown_WhenAddressIsNullOrEmpty(string value)
        {
            //Arrange
            Person model = new Person { Address = value, Name = "Anila" };
            var exceptionType = typeof(RequiredException);
            var exceptionMessage = ErrorMessages.PersonAddressIsRequired;

            //Act
            var exception = await Assert.ThrowsAsync<RequiredException>(() => _personBl.ValidateModelAsync(model));

            //Assert
            Assert.NotNull(exception);
            Assert.IsType(exceptionType, exception);
            Assert.Equal(exceptionMessage, exception.Message);
        }
        [Fact]
        public async void ValidateModel_CheckMaxLengthExceptionThrown_WhenNameLengthExceed()
        {
            //Arrange
            string name = "ABCDEFGHIJKLMNOPQRSTUVWXYZABCDEFGHIJKLMNOPQRSTUVWXYZA";
            Person model = new Person { Address = "19 XXX Ave", Name = name };
            var exceptionType = typeof(MaxLengthException);
            var exceptionMessage = ErrorMessages.PersonNameNotExceed50;

            //Act
            var exception = await Assert.ThrowsAsync<MaxLengthException>(() => _personBl.ValidateModelAsync(model));

            //Assert
            Assert.NotNull(exception);
            Assert.IsType(exceptionType, exception);
            Assert.Equal(exceptionMessage, exception.Message);
        }

        [Fact]
        public async void ValidateModel_CheckMaxLengthExceptionThrown_WhenAddressLengthExceed()
        {
            //Arrange
            string address = "ABCDEFGHIJKLMNOPQRSTUVWXYZABCDEFGHIJKLMNOPQRSTUVWXYZABCDEFGHIJKLMNOPQRSTUVWXYZABCDEFGHIJKLMNOPQRSTUVWXYZABCDEFGHIJKLMNOPQRSTUVWXYZABCDEFGHIJKLMNOPQRSTUVWXYZABCDEFGHIJKLMNOPQRSTUVWXYZABCDEFGHIJKLMNOPQRSTUVWXYZABCDEFGHIJKLMNOPQRSTUVWXYZABCDEFGHIJKLMNOPQRSTUVWXYZABCDEFGHIJKLMNOPQRSTUVWXYZ";
            Person model = new Person { Address = address, Name = "Anila" };

            var exceptionType = typeof(MaxLengthException);
            var exceptionMessage = ErrorMessages.PersonAddressNotExceed250;

            //Act
            var exception = await Assert.ThrowsAsync<MaxLengthException>(() => _personBl.ValidateModelAsync(model));

            //Assert
            Assert.NotNull(exception);
            Assert.IsType(exceptionType, exception);
            Assert.Equal(exceptionMessage, exception.Message);
        }
    }
}