namespace Assessment.Tests.PersonTests
{
    /// <summary>
    /// The Helper class for seeding mock data
    /// </summary>
    public class PersonBLTestHelper
    {
        public static List<Api.Entity.Person> SeedPersonList()
        {
            List<Api.Entity.Person> persons = new List<Api.Entity.Person>
            {
                new Api.Entity.Person
                { Id = 1,Name="Anila",Address="19 XXX Ave"},
                 new Api.Entity.Person
                { Id = 2,Name="Varsha",Address="80 XXX Ave"},
                  new Api.Entity.Person
                { Id = 3,Name="Lintu",Address="34 XXX court"},
            };
            return persons;
        }
    }
}
