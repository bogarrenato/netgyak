using Entities;
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


    public PersonsServiceTest(ITestOutputHelper testOutputHelper)
    {
        _countriesService = new CountriesService(new PersonsDbContext(new DbContextOptionsBuilder<PersonsDbContext>().Options));
        _personsService = new PersonsService(new PersonsDbContext(new DbContextOptionsBuilder<PersonsDbContext>().Options), _countriesService);

        _testOutputHelper = testOutputHelper;
    }


    #region AddPerson
    //When we supply null value as PersonAddRequest it should throw ArgumentNullException

    [Fact]
    public void AddPerson_NullPerson()
    {
        PersonAddRequest? personAddRequest = null;

        Assert.Throws<ArgumentNullException>(() => _personsService.AddPerson(personAddRequest));
    }
    [Fact]
    public void AddPerson_PersonNameIsNull()
    {
        PersonAddRequest? personAddRequest = new PersonAddRequest() { PersonName = null };

        Assert.Throws<ArgumentException>(() => _personsService.AddPerson(personAddRequest));
    }

    [Fact]
    public void AddPerson_EmailIsNull()
    {
        PersonAddRequest? personAddRequest = new PersonAddRequest() { Email = null };

        Assert.Throws<ArgumentException>(() => _personsService.AddPerson(personAddRequest));
    }

    [Fact]
    public void AddPerson_ProperPersonDetails()
    {
        PersonAddRequest? personAddRequest = new PersonAddRequest()
        {
            PersonName = "John Doe",
            Email = "qwe@wqe.com",
            Address = "123, ABC Street",
            CountryID = Guid.NewGuid(),
            DateOfBirth = DateTime.Now.AddYears(-30),
            Gender = GenderOptions.Female,
            ReceiveNewsLetters = true
        };

        PersonResponse person_response_from_add = _personsService.AddPerson(personAddRequest);
        List<PersonResponse> persons_list = _personsService.GetAllPersons();

        Assert.True(person_response_from_add.PersonID != Guid.Empty);
        Assert.Contains(person_response_from_add, persons_list);
    }

    #endregion


    #region GetPersonByPersonID

    [Fact]

    public void GetPersonByPersonID_NullPersonID()
    {
        ///Arrange
        Guid? personID = null;

        PersonResponse? person_response_from_get = _personsService.GetPersonByPersonID(personID);

        Assert.Null(person_response_from_get);
    }


    [Fact]
    public void GetPersonByPersonID_WithPersonID()
    {
        //Arrange
        CountryAddRequest countryAddRequest = new CountryAddRequest()
        {
            CountryName = "Canada",

        };
        CountryResponse country_response = _countriesService.AddCountry(countryAddRequest);

        //Act
        PersonAddRequest person_request = new PersonAddRequest()
        {
            PersonName = "John Doe",
            Email = "asd@a.com",
            Address = "123, ABC Street",
            CountryID = country_response.CountryID,
            DateOfBirth = DateTime.Now.AddYears(-30),
            Gender = GenderOptions.Male,
            ReceiveNewsLetters = true
        };
        PersonResponse person_response_from_add = _personsService.AddPerson(person_request);
        PersonResponse? person_response_from_get = _personsService.GetPersonByPersonID(person_response_from_add.PersonID);

        //Assert
        Assert.Equal(person_response_from_add, person_response_from_get);
    }

    #endregion



    #region GetAllPersons
    [Fact]
    public void GetAllPersons_EmptyList()
    {
        List<PersonResponse> person_from_get = _personsService.GetAllPersons();

        Assert.Empty(person_from_get);
    }

    [Fact]
    public void GetAllPersons_AddFewPersons()
    {

        CountryAddRequest country_request_1 = new CountryAddRequest()
        {
            CountryName = "usa"
        };
        CountryAddRequest country_request_2 = new CountryAddRequest()
        {
            CountryName = "india"
        };


        CountryResponse country_response_1 = _countriesService.AddCountry(country_request_1);
        CountryResponse country_response_2 = _countriesService.AddCountry(country_request_2);


        PersonAddRequest person_request_1 = new PersonAddRequest()
        {
            PersonName = "John Doe",
            Email = "a@a.com",
            Address = "123, ABC Street",
            CountryID = country_response_1.CountryID,
            DateOfBirth = DateTime.Now.AddYears(-30),
            Gender = GenderOptions.Female,
            ReceiveNewsLetters = true
        };


        PersonAddRequest person_request_2 = new PersonAddRequest()
        {
            PersonName = "Valami",
            Email = "b@b.com",
            Address = "123, ABC Streeet",
            CountryID = country_response_2.CountryID,
            DateOfBirth = DateTime.Now.AddYears(-30),
            Gender = GenderOptions.Female,
            ReceiveNewsLetters = true
        };

        List<PersonAddRequest> person_requests = new List<PersonAddRequest>() { person_request_1, person_request_2 };

        List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

        foreach (PersonAddRequest person_request in person_requests)
        {
            PersonResponse person_response = _personsService.AddPerson(person_request);
            person_response_list_from_add.Add(person_response);
        }

        _testOutputHelper.WriteLine("Expected:");
        foreach (PersonResponse person_response_from_add in person_response_list_from_add)
        {
            _testOutputHelper.WriteLine(person_response_from_add.ToString());
        }


        List<PersonResponse> persons_list_from_get = _personsService.GetAllPersons();

        _testOutputHelper.WriteLine("Actual:");
        foreach (PersonResponse person_response_from_add in persons_list_from_get)
        {
            _testOutputHelper.WriteLine(person_response_from_add.ToString());
        }


        foreach (PersonResponse person_response_from_add in person_response_list_from_add)
        {
            Assert.Contains(person_response_from_add, persons_list_from_get);
        }

    }


    #endregion


    #region GetFilteredPersons

    [Fact]
    //If the search text is empty and search is PersonName, it should return all persons
    public void GetFilteredPersons_EmptySearchText()
    {

        CountryAddRequest country_request_1 = new CountryAddRequest()
        {
            CountryName = "usa"
        };
        CountryAddRequest country_request_2 = new CountryAddRequest()
        {
            CountryName = "india"
        };


        CountryResponse country_response_1 = _countriesService.AddCountry(country_request_1);
        CountryResponse country_response_2 = _countriesService.AddCountry(country_request_2);


        PersonAddRequest person_request_1 = new PersonAddRequest()
        {
            PersonName = "John Doe",
            Email = "a@a.com",
            Address = "123, ABC Street",
            CountryID = country_response_1.CountryID,
            DateOfBirth = DateTime.Now.AddYears(-30),
            Gender = GenderOptions.Female,
            ReceiveNewsLetters = true
        };


        PersonAddRequest person_request_2 = new PersonAddRequest()
        {
            PersonName = "Valami",
            Email = "b@b.com",
            Address = "123, ABC Streeet",
            CountryID = country_response_2.CountryID,
            DateOfBirth = DateTime.Now.AddYears(-30),
            Gender = GenderOptions.Female,
            ReceiveNewsLetters = true
        };

        List<PersonAddRequest> person_requests = new List<PersonAddRequest>() { person_request_1, person_request_2 };
        List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

        foreach (PersonAddRequest person_request in person_requests)
        {
            PersonResponse person_response = _personsService.AddPerson(person_request);
            person_response_list_from_add.Add(person_response);
        }

        _testOutputHelper.WriteLine("Expected:");
        foreach (PersonResponse person_response_from_add in person_response_list_from_add)
        {
            _testOutputHelper.WriteLine(person_response_from_add.ToString());
        }

        //Act
        List<PersonResponse> persons_list_from_search = _personsService.GetFilteredPersons(nameof(PersonResponse.PersonName), "");

        _testOutputHelper.WriteLine("Actual:");
        foreach (PersonResponse person_response_from_add in persons_list_from_search)
        {
            _testOutputHelper.WriteLine(person_response_from_add.ToString());
        }


        foreach (PersonResponse person_response_from_add in person_response_list_from_add)
        {
            Assert.Contains(person_response_from_add, persons_list_from_search);
        }

    }


    //First we will add few persons and then we will search based on the person name with some search string. It should return the matching persons
    [Fact]
    //If the search text is empty and search is PersonName, it should return all persons
    public void GetFilteredPersons_SearchByPersonName()
    {

        CountryAddRequest country_request_1 = new CountryAddRequest()
        {
            CountryName = "usa"
        };
        CountryAddRequest country_request_2 = new CountryAddRequest()
        {
            CountryName = "india"
        };


        CountryResponse country_response_1 = _countriesService.AddCountry(country_request_1);
        CountryResponse country_response_2 = _countriesService.AddCountry(country_request_2);


        PersonAddRequest person_request_1 = new PersonAddRequest()
        {
            PersonName = "Mary",
            Email = "a@a.com",
            Address = "123, ABC Street",
            CountryID = country_response_1.CountryID,
            DateOfBirth = DateTime.Now.AddYears(-30),
            Gender = GenderOptions.Female,
            ReceiveNewsLetters = true
        };


        PersonAddRequest person_request_2 = new PersonAddRequest()
        {
            PersonName = "Rahman",
            Email = "b@b.com",
            Address = "123, ABC Streeet",
            CountryID = country_response_2.CountryID,
            DateOfBirth = DateTime.Now.AddYears(-30),
            Gender = GenderOptions.Female,
            ReceiveNewsLetters = true
        };

        List<PersonAddRequest> person_requests = new List<PersonAddRequest>() { person_request_1, person_request_2 };
        List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

        foreach (PersonAddRequest person_request in person_requests)
        {
            PersonResponse person_response = _personsService.AddPerson(person_request);
            person_response_list_from_add.Add(person_response);
        }

        _testOutputHelper.WriteLine("Expected:");
        foreach (PersonResponse person_response_from_add in person_response_list_from_add)
        {
            _testOutputHelper.WriteLine(person_response_from_add.ToString());
        }

        //Act
        List<PersonResponse> persons_list_from_search = _personsService.GetFilteredPersons(nameof(PersonResponse.PersonName), "ma");

        _testOutputHelper.WriteLine("Actual:");
        foreach (PersonResponse person_response_from_add in persons_list_from_search)
        {
            _testOutputHelper.WriteLine(person_response_from_add.ToString());
        }

        // Assert
        foreach (PersonResponse person_response_from_add in person_response_list_from_add)
        {
            if (person_response_from_add.PersonName != null)
            {
                if (person_response_from_add.PersonName.Contains("ma", StringComparison.OrdinalIgnoreCase))
                {
                    Assert.Contains(person_response_from_add, persons_list_from_search);
                }
            }


        }

    }


    #endregion


    #region GetSortedPersons

    //When we sort absed on the PersonName in DESC, it should return the persons list in descending on PersonName
    [Fact]
    public void GetSortedPersons()
    {
        CountryAddRequest country_request_1 = new CountryAddRequest()
        {
            CountryName = "usa"
        };
        CountryAddRequest country_request_2 = new CountryAddRequest()
        {
            CountryName = "india"
        };


        CountryResponse country_response_1 = _countriesService.AddCountry(country_request_1);
        CountryResponse country_response_2 = _countriesService.AddCountry(country_request_2);


        PersonAddRequest person_request_1 = new PersonAddRequest()
        {
            PersonName = "Mary",
            Email = "a@a.com",
            Address = "123, ABC Street",
            CountryID = country_response_1.CountryID,
            DateOfBirth = DateTime.Now.AddYears(-30),
            Gender = GenderOptions.Female,
            ReceiveNewsLetters = true
        };


        PersonAddRequest person_request_2 = new PersonAddRequest()
        {
            PersonName = "Rahman",
            Email = "b@b.com",
            Address = "123, ABC Streeet",
            CountryID = country_response_2.CountryID,
            DateOfBirth = DateTime.Now.AddYears(-30),
            Gender = GenderOptions.Female,
            ReceiveNewsLetters = true
        };

        List<PersonAddRequest> person_requests = new List<PersonAddRequest>() { person_request_1, person_request_2 };
        List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

        foreach (PersonAddRequest person_request in person_requests)
        {
            PersonResponse person_response = _personsService.AddPerson(person_request);
            person_response_list_from_add.Add(person_response);
        }

        _testOutputHelper.WriteLine("Expected:");
        foreach (PersonResponse person_response_from_add in person_response_list_from_add)
        {
            _testOutputHelper.WriteLine(person_response_from_add.ToString());
        }

        List<PersonResponse> allPersons = _personsService.GetAllPersons();
        //Act
        List<PersonResponse> persons_list_from_sort = _personsService.GetSortedPersons(allPersons, nameof(PersonResponse.PersonName), SortOrderOptions.DESC);

        _testOutputHelper.WriteLine("Actual:");
        foreach (PersonResponse person_response_from_add in persons_list_from_sort)
        {
            _testOutputHelper.WriteLine(person_response_from_add.ToString());
        }

        person_response_list_from_add = person_response_list_from_add.OrderByDescending(temp => temp.PersonName).ToList();

        // Assert
        for (int i = 0; i < person_response_list_from_add.Count; i++)
        {
            Assert.Equal(person_response_list_from_add[i], persons_list_from_sort[i]);
        }
    }

    #endregion


    #region UpdatePerson

    //When we supply null as PersonUpdateRequest, it should throw ArgumentNullException

    [Fact]
    public void UpdatePerson_ThrowsArgumentNullException()
    {


        //Arrange 
        PersonUpdateRequest? personUpdateRequest = null;
        //Act
        Assert.Throws<ArgumentNullException>(() =>
        {
            //Assert
            _personsService.UpdatePerson(personUpdateRequest);
        });


        // _personsService.UpdatePerson(personUpdateRequest);
    }


    //When we suppply invalid personid , it should throw ArgumentException
    [Fact]
    public void UpdatePerson_InvalidPersonID()
    {


        //Arrange 
        PersonUpdateRequest? person_update_request = new PersonUpdateRequest()
        {
            PersonID = Guid.Empty
        };
        //Act
        Assert.Throws<ArgumentException>(() =>
        {
            //Assert
            _personsService.UpdatePerson(person_update_request);
        });


        // _personsService.UpdatePerson(personUpdateRequest);
    }


    //When personName is null, it should throw ArgumentException
    [Fact]
    public void UpdatePerson_PersonNameIsNull()
    {
        CountryAddRequest country_add_request = new CountryAddRequest() { CountryName = "USA" };
        CountryResponse country_response_from_add = _countriesService.AddCountry(country_add_request);

        PersonAddRequest person_add_request = new PersonAddRequest()
        {
            PersonName = "John Doe",
            CountryID = country_response_from_add.CountryID,
            Email = "a@a.c",
            Address = "123, ABC Street",
            Gender = GenderOptions.Female
        };

        PersonResponse person_response_from_add = _personsService.AddPerson(person_add_request);
        PersonUpdateRequest person_update_request = person_response_from_add.ToPersonUpdateRequest();
        person_update_request.PersonName = null;

        Assert.Throws<ArgumentException>(() =>
        {
            _personsService.UpdatePerson(person_update_request);
        });


    }

    //First, we will add a new person and try to update the person name and email

    [Fact]
    public void UpdatePerson_PersonFullDetailUpdation()
    {
        CountryAddRequest country_add_request = new CountryAddRequest() { CountryName = "USA" };
        CountryResponse country_response_from_add = _countriesService.AddCountry(country_add_request);

        PersonAddRequest person_add_request = new PersonAddRequest()
        {
            PersonName = "John Doe",
            CountryID = country_response_from_add.CountryID,
            Address = "123, ABC Street",
            DateOfBirth = DateTime.Now.AddYears(-30),
            Email = "a@a.com",
            Gender = GenderOptions.Female,
            ReceiveNewsLetters = true
        };

        PersonResponse person_response_from_add = _personsService.AddPerson(person_add_request);
        PersonUpdateRequest person_update_request = person_response_from_add.ToPersonUpdateRequest();
        person_update_request.PersonName = "William";

        PersonResponse person_response_from_update = _personsService.UpdatePerson(person_update_request);

        PersonResponse? person_response_from_get = _personsService.GetPersonByPersonID(person_response_from_add.PersonID);

        Assert.Equal(person_response_from_get, person_response_from_update);
    }


    #endregion


    #region DeletePerson

    //If you supply an invalid PersonID, it should return false
    [Fact]
    public void DeletePerson_ValidPersonID()
    {
        CountryAddRequest country_add_request = new CountryAddRequest() { CountryName = "USA" };
        CountryResponse country_response_from_add = _countriesService.AddCountry(country_add_request);


        PersonAddRequest person_add_request = new PersonAddRequest()
        {
            PersonName = "John Doe",
            CountryID = country_response_from_add.CountryID,
            Address = "123, ABC Street",
            DateOfBirth = DateTime.Now.AddYears(-30),
            Email = "a@a.c",

        };

        PersonResponse person_response_from_add = _personsService.AddPerson(person_add_request);

        //Act
        bool isDeleted = _personsService.DeletePerson(person_response_from_add.PersonID);

        Assert.True(isDeleted);

    }

    //If you supply an invalid PersonID, it should return false
    [Fact]
    public void DeletePerson_InvalidPersonID()
    {

        //Act
        bool isDeleted = _personsService.DeletePerson(Guid.NewGuid());

        Assert.False(isDeleted);

    }

    #endregion
}
