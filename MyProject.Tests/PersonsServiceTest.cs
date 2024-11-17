using AutoFixture;
using Entities;
using EntityFrameworkCoreMock;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using Xunit.Abstractions;

namespace MyProject.Tests;

public class PersonsServiceTest
{
    private readonly IPersonsService _personsService;
    private readonly ICountriesService _countriesService;
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly IFixture _fixture;


    public PersonsServiceTest(ITestOutputHelper testOutputHelper)
    {
        _fixture = new Fixture();
        //New Empty list
        var countriesInitialData = new List<Country>() { };
        var personsInitialData = new List<Person>() { };
        //Mock object for application db context
        DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(new DbContextOptionsBuilder<ApplicationDbContext>().Options);
        //Mocked - acts as original db context
        ApplicationDbContext dbContext = dbContextMock.Object;
        //We have to mock the DB set too..
        dbContextMock.CreateDbSetMock(x => x.Countries, countriesInitialData);
        dbContextMock.CreateDbSetMock(x => x.Persons, personsInitialData);
        _countriesService = new CountriesService(dbContext);
        _personsService = new PersonsService(dbContext, _countriesService);

        _testOutputHelper = testOutputHelper;
    }


    #region AddPerson
    //When we supply null value as PersonAddRequest it should throw ArgumentNullException

    [Fact]
    public async Task AddPerson_NullPerson()
    {
        PersonAddRequest? personAddRequest = null;


        Func<Task> action = async () => { await _personsService.AddPerson(personAddRequest); };
        await action.Should().ThrowAsync<ArgumentNullException>();

        // await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        //   {
        //       await _personsService.AddPerson(personAddRequest);
        //   });
    }

    [Fact]
    public async Task AddPerson_PersonNameIsNull()
    {
        PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>()
            .With(temp => temp.PersonName, null as string)
            .Create();

        Func<Task> action = async () => { await _personsService.AddPerson(personAddRequest); };
        await action.Should().ThrowAsync<ArgumentException>();

        // await Assert.ThrowsAsync<ArgumentException>(async () =>
        // {
        //     await _personsService.AddPerson(personAddRequest);
        // });
    }

    [Fact]
    public async Task AddPerson_EmailIsNull()
    {
        PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>()
            .With(temp => temp.Email, null as string)
            .Create(); ;

        await Assert.ThrowsAsync<ArgumentException>(async () => await _personsService.AddPerson(personAddRequest));
    }

    [Fact]
    public async Task AddPerson_ProperPersonDetails()
    {

        PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>()
            .With(temp => temp.Email, "someone@example.com").Create();
        PersonResponse person_response_from_add = await _personsService.AddPerson(personAddRequest);
        List<PersonResponse> persons_list = await _personsService.GetAllPersons();

        person_response_from_add.PersonID.Should().NotBe(Guid.Empty);
        persons_list.Should().Contain(person_response_from_add);

        // Assert.True(person_response_from_add.PersonID != Guid.Empty);
        // Assert.Contains(person_response_from_add, persons_list);
    }

    #endregion


    #region GetPersonByPersonID

    [Fact]

    public async Task GetPersonByPersonID_NullPersonID()
    {
        ///Arrange
        Guid? personID = null;

        PersonResponse? person_response_from_get = await _personsService.GetPersonByPersonID(personID);

        person_response_from_get.Should().BeNull();

        // Assert.Null(person_response_from_get);
    }


    [Fact]
    public async Task GetPersonByPersonID_WithPersonID()
    {
        //Arrange
        CountryAddRequest countryAddRequest =
        _fixture.Create<CountryAddRequest>();

        //  new CountryAddRequest()
        // {
        //     CountryName = "Canada",

        // };
        CountryResponse country_response = await _countriesService.AddCountry(countryAddRequest);

        //Act
        PersonAddRequest person_request = _fixture.Build<PersonAddRequest>()
            .With(temp => temp.Email, "a@a.com")
            .Create();
        PersonResponse person_response_from_add = await _personsService.AddPerson(person_request);
        PersonResponse? person_response_from_get = await _personsService.GetPersonByPersonID(person_response_from_add.PersonID);

        person_response_from_get.Should().Be(person_response_from_add);
        //Assert
        // Assert.Equal(person_response_from_add, person_response_from_get);
    }

    #endregion



    #region GetAllPersons
    [Fact]
    public async Task GetAllPersons_EmptyList()
    {
        List<PersonResponse> person_from_get = await _personsService.GetAllPersons();

        person_from_get.Should().BeEmpty();

        // Assert.Empty(person_from_get);
    }

    [Fact]
    public async Task GetAllPersons_AddFewPersons()
    {

        CountryAddRequest country_request_1 =
        _fixture.Create<CountryAddRequest>();


        CountryAddRequest country_request_2 = _fixture.Create<CountryAddRequest>();

        CountryResponse country_response_1 = await _countriesService.AddCountry(country_request_1);
        CountryResponse country_response_2 = await _countriesService.AddCountry(country_request_2);


        PersonAddRequest person_request_1 = _fixture.Build<PersonAddRequest>()
            .With(temp => temp.Email, "a@a.com")
            .Create();


        PersonAddRequest person_request_2 = _fixture.Build<PersonAddRequest>()
            .With(temp => temp.Email, "a2@a.com")
            .Create();

        List<PersonAddRequest> person_requests = new List<PersonAddRequest>() { person_request_1, person_request_2 };

        List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

        foreach (PersonAddRequest person_request in person_requests)
        {
            PersonResponse person_response = await _personsService.AddPerson(person_request);
            person_response_list_from_add.Add(person_response);
        }

        _testOutputHelper.WriteLine("Expected:");
        foreach (PersonResponse person_response_from_add in person_response_list_from_add)
        {
            _testOutputHelper.WriteLine(person_response_from_add.ToString());
        }


        List<PersonResponse> persons_list_from_get = await _personsService.GetAllPersons();

        _testOutputHelper.WriteLine("Actual:");
        foreach (PersonResponse person_response_from_add in persons_list_from_get)
        {
            _testOutputHelper.WriteLine(person_response_from_add.ToString());
        }


        foreach (PersonResponse person_response_from_add in person_response_list_from_add)
        {

            // Assert.Contains(person_response_from_add, persons_list_from_get);
        }
        persons_list_from_get.Should().BeEquivalentTo(person_response_list_from_add);
    }


    #endregion


    #region GetFilteredPersons

    [Fact]
    //If the search text is empty and search is PersonName, it should return all persons
    public async Task GetFilteredPersons_EmptySearchText()
    {

        CountryAddRequest country_request_1 = _fixture.Create<CountryAddRequest>();
        CountryAddRequest country_request_2 = _fixture.Create<CountryAddRequest>();


        CountryResponse country_response_1 = await _countriesService.AddCountry(country_request_1);
        CountryResponse country_response_2 = await _countriesService.AddCountry(country_request_2);


        PersonAddRequest person_request_1 = _fixture.Build<PersonAddRequest>()
            .With(temp => temp.Email, "a@a.com")
            .Create();


        PersonAddRequest person_request_2 = _fixture.Build<PersonAddRequest>()
            .With(temp => temp.Email, "a2@a.com")
            .Create();

        List<PersonAddRequest> person_requests = new List<PersonAddRequest>() { person_request_1, person_request_2 };
        List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

        foreach (PersonAddRequest person_request in person_requests)
        {
            PersonResponse person_response = await _personsService.AddPerson(person_request);
            person_response_list_from_add.Add(person_response);
        }

        _testOutputHelper.WriteLine("Expected:");
        foreach (PersonResponse person_response_from_add in person_response_list_from_add)
        {
            _testOutputHelper.WriteLine(person_response_from_add.ToString());
        }

        //Act
        List<PersonResponse> persons_list_from_search = await _personsService.GetFilteredPersons(nameof(PersonResponse.PersonName), "");

        _testOutputHelper.WriteLine("Actual:");
        foreach (PersonResponse person_response_from_add in persons_list_from_search)
        {
            _testOutputHelper.WriteLine(person_response_from_add.ToString());
        }


        foreach (PersonResponse person_response_from_add in person_response_list_from_add)
        {
            // persons_list_from_search.Should().Contain(person_response_from_add);
            Assert.Contains(person_response_from_add, persons_list_from_search);
        }

        persons_list_from_search.Should().BeEquivalentTo(person_response_list_from_add);

    }


    //First we will add few persons and then we will search based on the person name with some search string. It should return the matching persons
    [Fact]
    //If the search text is empty and search is PersonName, it should return all persons
    public async Task GetFilteredPersons_SearchByPersonName()
    {

        CountryAddRequest country_request_1 = _fixture.Create<CountryAddRequest>();
        CountryAddRequest country_request_2 = _fixture.Create<CountryAddRequest>();


        CountryResponse country_response_1 = await _countriesService.AddCountry(country_request_1);
        CountryResponse country_response_2 = await _countriesService.AddCountry(country_request_2);


        PersonAddRequest person_request_1 = _fixture.Build<PersonAddRequest>()
            .With(temp => temp.PersonName, "Rahman")
            .With(temp => temp.Email, "a@a.com")
            .With(temp => temp.CountryID, country_response_1.CountryID)
            .Create();


        PersonAddRequest person_request_2 = _fixture.Build<PersonAddRequest>()
            .With(temp => temp.PersonName, "mary")
            .With(temp => temp.Email, "a2@a.com")
            .With(temp => temp.CountryID, country_response_1.CountryID)
            .Create();

        PersonAddRequest person_request_3 = _fixture.Build<PersonAddRequest>()
            .With(temp => temp.PersonName, "scott")
            .With(temp => temp.Email, "a2@a.com")
            .With(temp => temp.CountryID, country_response_2.CountryID)
            .Create();

        List<PersonAddRequest> person_requests = new List<PersonAddRequest>() { person_request_1, person_request_2, person_request_3 };
        List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

        foreach (PersonAddRequest person_request in person_requests)
        {
            PersonResponse person_response = await _personsService.AddPerson(person_request);
            person_response_list_from_add.Add(person_response);
        }

        _testOutputHelper.WriteLine("Expected:");
        foreach (PersonResponse person_response_from_add in person_response_list_from_add)
        {
            _testOutputHelper.WriteLine(person_response_from_add.ToString());
        }

        //Act
        List<PersonResponse> persons_list_from_search = await _personsService.GetFilteredPersons(nameof(PersonResponse.PersonName), "ma");

        _testOutputHelper.WriteLine("Actual:");
        foreach (PersonResponse person_response_from_add in persons_list_from_search)
        {
            _testOutputHelper.WriteLine(person_response_from_add.ToString());
        }

        // Assert
        // foreach (PersonResponse person_response_from_add in person_response_list_from_add)
        // {
        //     if (person_response_from_add.PersonName != null)
        //     {
        //         if (person_response_from_add.PersonName.Contains("ma", StringComparison.OrdinalIgnoreCase))
        //         {
        //             // persons_list_from_search.Should().Contain(person_response_from_add);
        //             Assert.Contains(person_response_from_add, persons_list_from_search);
        //         }
        //     }


        // }

        persons_list_from_search.Should().OnlyContain(temp => temp.PersonName.Contains("ma", StringComparison.OrdinalIgnoreCase));

    }


    #endregion


    #region GetSortedPersons

    //When we sort absed on the PersonName in DESC, it should return the persons list in descending on PersonName
    [Fact]
    public async Task GetSortedPersons()
    {
        CountryAddRequest country_request_1 = _fixture.Create<CountryAddRequest>();
        CountryAddRequest country_request_2 = _fixture.Create<CountryAddRequest>();


        CountryResponse country_response_1 = await _countriesService.AddCountry(country_request_1);
        CountryResponse country_response_2 = await _countriesService.AddCountry(country_request_2);


        PersonAddRequest person_request_1 = _fixture.Build<PersonAddRequest>()
            .With(temp => temp.PersonName, "Smith")
            .With(temp => temp.Email, "a@a.com")
            .With(temp => temp.CountryID, country_response_1.CountryID)
            .Create();


        PersonAddRequest person_request_2 = _fixture.Build<PersonAddRequest>()
            .With(temp => temp.PersonName, "Mary")
            .With(temp => temp.Email, "a2@a.com")
            .With(temp => temp.CountryID, country_response_1.CountryID)
            .Create();

        PersonAddRequest person_request_3 = _fixture.Build<PersonAddRequest>()
            .With(temp => temp.PersonName, "Rahman")
            .With(temp => temp.Email, "a2@a.com")
            .With(temp => temp.CountryID, country_response_2.CountryID)
            .Create();

        List<PersonAddRequest> person_requests = new List<PersonAddRequest>() { person_request_1, person_request_2, person_request_3 };
        List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

        foreach (PersonAddRequest person_request in person_requests)
        {
            PersonResponse person_response = await _personsService.AddPerson(person_request);
            person_response_list_from_add.Add(person_response);
        }

        _testOutputHelper.WriteLine("Expected:");
        foreach (PersonResponse person_response_from_add in person_response_list_from_add)
        {
            _testOutputHelper.WriteLine(person_response_from_add.ToString());
        }

        List<PersonResponse> allPersons = await _personsService.GetAllPersons();
        //Act
        List<PersonResponse> persons_list_from_sort = await _personsService.GetSortedPersons(allPersons, nameof(PersonResponse.PersonName), SortOrderOptions.DESC);

        _testOutputHelper.WriteLine("Actual:");
        foreach (PersonResponse person_response_from_add in persons_list_from_sort)
        {
            _testOutputHelper.WriteLine(person_response_from_add.ToString());
        }

        // person_response_list_from_add = person_response_list_from_add.OrderByDescending(temp => temp.PersonName).ToList();

        // Assert
        // for (int i = 0; i < person_response_list_from_add.Count; i++)
        // {
        //     Assert.Equal(person_response_list_from_add[i], persons_list_from_sort[i]);
        // }

        // persons_list_from_sort.Should().BeEquivalentTo(person_response_list_from_add);

        persons_list_from_sort.Should().BeInDescendingOrder(temp => temp.PersonName);
    }

    #endregion


    #region UpdatePerson

    //When we supply null as PersonUpdateRequest, it should throw ArgumentNullException

    [Fact]
    public async Task UpdatePerson_ThrowsArgumentNullException()
    {


        //Arrange 
        PersonUpdateRequest? personUpdateRequest = null;
        //Act
        // await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        //  {
        //      //Assert
        //      await _personsService.UpdatePerson(personUpdateRequest);
        //  });

        Func<Task> action = async () =>
        {
            await _personsService.UpdatePerson(personUpdateRequest);
        };

        await action.Should().ThrowAsync<ArgumentNullException>();

        // _personsService.UpdatePerson(personUpdateRequest);
    }


    //When we suppply invalid personid , it should throw ArgumentException
    [Fact]
    public async Task UpdatePerson_InvalidPersonID()
    {


        //Arrange 
        PersonUpdateRequest? person_update_request = _fixture.Build<PersonUpdateRequest>()
        .Create();
        //Act
        // await Assert.ThrowsAsync<ArgumentException>(async () =>
        //  {
        //      //Assert
        //      await _personsService.UpdatePerson(person_update_request);
        //  });

        Func<Task> action = async () =>
         {
             await _personsService.UpdatePerson(person_update_request);
         };

        await action.Should().ThrowAsync<ArgumentException>();


        // _personsService.UpdatePerson(personUpdateRequest);
    }


    //When personName is null, it should throw ArgumentException
    [Fact]
    public async Task UpdatePerson_PersonNameIsNull()
    {
        // CountryAddRequest country_add_request = new CountryAddRequest() { CountryName = "USA" };
        // CountryResponse country_response_from_add = await _countriesService.AddCountry(country_add_request);

        // PersonAddRequest person_add_request = new PersonAddRequest()
        // {
        //     PersonName = "John Doe",
        //     CountryID = country_response_from_add.CountryID,
        //     Email = "a@a.c",
        //     Address = "123, ABC Street",
        //     Gender = GenderOptions.Female
        // };

        CountryAddRequest country_request = _fixture.Create<CountryAddRequest>();

        CountryResponse country_response = await _countriesService.AddCountry(country_request);

        PersonAddRequest person_add_request = _fixture.Build<PersonAddRequest>()
            .With(temp => temp.PersonName, "Rahman")
            .With(temp => temp.Email, "a@a.com")
            .With(temp => temp.CountryID, country_response.CountryID)
            .Create();

        PersonResponse person_response_from_add = await _personsService.AddPerson(person_add_request);
        PersonUpdateRequest person_update_request = person_response_from_add.ToPersonUpdateRequest();
        person_update_request.PersonName = null;

        // await Assert.ThrowsAsync<ArgumentException>(async () =>
        //  {
        //      await _personsService.UpdatePerson(person_update_request);
        //  });

        Func<Task> action = async () =>
       {
           await _personsService.UpdatePerson(person_update_request);
       };

        await action.Should().ThrowAsync<ArgumentException>();


    }

    //First, we will add a new person and try to update the person name and email

    [Fact]
    public async Task UpdatePerson_PersonFullDetailsUpdation()
    {
        CountryAddRequest country_add_request = _fixture.Create<CountryAddRequest>();
        CountryResponse country_response_from_add = await _countriesService.AddCountry(country_add_request);

        PersonAddRequest person_add_request = _fixture.Build<PersonAddRequest>()
              .With(temp => temp.PersonName, "Rahman")
              .With(temp => temp.Email, "a@a.com")
              .With(temp => temp.CountryID, country_response_from_add.CountryID)
              .Create();

        PersonResponse person_response_from_add = await _personsService.AddPerson(person_add_request);
        PersonUpdateRequest person_update_request = person_response_from_add.ToPersonUpdateRequest();
        person_update_request.PersonName = "William";

        PersonResponse person_response_from_update = await _personsService.UpdatePerson(person_update_request);

        PersonResponse? person_response_from_get = await _personsService.GetPersonByPersonID(person_response_from_add.PersonID);

        // Assert.Equal(person_response_from_get, person_response_from_update);

        person_response_from_update.Should().Be(person_response_from_get);
    }


    #endregion


    #region DeletePerson

    //If you supply an invalid PersonID, it should return false
    [Fact]
    public async Task DeletePerson_ValidPersonID()
    {
        CountryAddRequest country_add_request = _fixture.Create<CountryAddRequest>();
        CountryResponse country_response_from_add = await _countriesService.AddCountry(country_add_request);
        PersonAddRequest person_add_request = _fixture.Build<PersonAddRequest>()
              .With(temp => temp.PersonName, "Rahman")
              .With(temp => temp.Email, "a@a.com")
              .With(temp => temp.CountryID, country_response_from_add.CountryID)
              .Create();
        PersonResponse person_response_from_add = await _personsService.AddPerson(person_add_request);

        //Act
        bool isDeleted = await _personsService.DeletePerson(person_response_from_add.PersonID);

        // Assert.True(isDeleted);
        isDeleted.Should().BeTrue();

    }

    //If you supply an invalid PersonID, it should return false
    [Fact]
    public async Task DeletePerson_InvalidPersonID()
    {

        //Act
        bool isDeleted = await _personsService.DeletePerson(Guid.NewGuid());

        // Assert.False(isDeleted);
        isDeleted.Should().BeFalse();

    }

    #endregion
}
