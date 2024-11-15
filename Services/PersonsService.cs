using System.ComponentModel.DataAnnotations;
using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace Services;

public class PersonsService : IPersonsService
{

    private readonly PersonsDbContext _db;
    private readonly ICountriesService _countriesService;

    public PersonsService(PersonsDbContext personsDbContext, ICountriesService countriesService)
    {
        _db = personsDbContext;
        _countriesService = countriesService;
    }



    private PersonResponse ConvertPersonToPersonResponse(Person person)
    {
        PersonResponse personResponse = person.ToPersonResponse();
        personResponse.Country = _countriesService.GetCountryByCountryID(person.CountryID)?.CountryName;

        return personResponse;
    }

    //Check if personAddRequest is not null
    //Validate all properties of "personAddRequest"
    //convert "personAddRequest" from "PersonAddRequest" to type "Person"
    //Generate a new PersonId
    //Then add it into List<Person>
    //Return PersonResponse object with generated PersonID
    public PersonResponse AddPerson(PersonAddRequest? personAddRequest)
    {
        if (personAddRequest == null)
        {
            throw new ArgumentNullException(nameof(personAddRequest));
        }

        //Validate PersonName
        //Validation can happen only in a context
        ValidationHelper.ModelValidation(personAddRequest);
        //convert personAddRequest to Person type

        Person person = personAddRequest.ToPerson();

        //generate new PersonID
        person.PersonID = Guid.NewGuid();

        //Add person object to data store
        _db.Persons.Add(person);
        _db.SaveChanges();

        //convert the Person object into PersonResponse type
        return ConvertPersonToPersonResponse(person);

    }

    public List<PersonResponse> GetAllPersons()
    {
        return _db.Persons.ToList()
        .Select(temp => ConvertPersonToPersonResponse(temp)).ToList();
    }


    //Check if personId is not null
    //Get matching person from List<Person> based on personID
    //Convert matching person object from Person to PersonResponse type
    //Return PersonResponse object
    public PersonResponse? GetPersonByPersonID(Guid? personID)
    {
        if (personID == null)
        {
            return null;
        }

        Person? person = _db.Persons
            .FirstOrDefault(temp => temp.PersonID == personID);

        if (person == null)
        {
            return null;
        }

        return ConvertPersonToPersonResponse(person);
    }


    //Check if searchBy is not null
    //Get Matching persons from List<Person> based on searchBy and searchString
    //Convert matching person objects from Person to PersonResponse type
    //Return all matching PersonResponse objects
    public List<PersonResponse> GetFilteredPersons(string searchBy, string? searchString)
    {
        List<PersonResponse> allPersons = GetAllPersons();
        List<PersonResponse> matchingPersons = allPersons;

        if (string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty(searchString))
        {
            return matchingPersons;
        }


        switch (searchBy)
        {
            case nameof(Person.PersonName):
                matchingPersons = allPersons.Where(temp => !string.IsNullOrEmpty(temp.PersonName) ? temp.PersonName.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
                break;

            case nameof(Person.Email):
                matchingPersons = allPersons.Where(temp => !string.IsNullOrEmpty(temp.Email) ? temp.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
                break;

            case nameof(Person.DateOfBirth):
                matchingPersons = allPersons.Where(temp => (temp.DateOfBirth != null) ? temp.DateOfBirth.Value.ToString("dd MMMM yyyy").Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
                break;

            case nameof(Person.Gender):
                matchingPersons = allPersons.Where(temp => !string.IsNullOrEmpty(temp.Gender) ? temp.Gender.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
                break;

            case nameof(Person.Address):
                matchingPersons = allPersons.Where(temp => !string.IsNullOrEmpty(temp.Address) ? temp.Address.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
                break;

            default:
                matchingPersons = allPersons;
                break;
        }

        return matchingPersons;
    }

    public List<PersonResponse> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder)
    {
        if (string.IsNullOrEmpty(sortBy))
        {
            return allPersons;
        }

        List<PersonResponse> sortedPersons = (sortBy, sortOrder)
        switch
        {
            (nameof(PersonResponse.PersonName), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.PersonName).ToList(),
            (nameof(PersonResponse.PersonName), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.PersonName).ToList(),
            (nameof(PersonResponse.Email), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Email).ToList(),
            (nameof(PersonResponse.Email), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Email).ToList(),
            (nameof(PersonResponse.DateOfBirth), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.DateOfBirth).ToList(),
            (nameof(PersonResponse.DateOfBirth), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.DateOfBirth).ToList(),
            (nameof(PersonResponse.Address), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Address).ToList(),
            (nameof(PersonResponse.Address), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Address).ToList(),
            (nameof(PersonResponse.Age), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Age).ToList(),
            (nameof(PersonResponse.Age), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Age).ToList(),
            (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.ReceiveNewsLetters).ToList(),
            (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.ReceiveNewsLetters).ToList(),
            (nameof(PersonResponse.Country), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Country).ToList(),
            (nameof(PersonResponse.Country), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Country).ToList(),
            (nameof(PersonResponse.Gender), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Gender, StringComparer.OrdinalIgnoreCase).ToList(),
            (nameof(PersonResponse.Gender), SortOrderOptions.DESC) => allPersons.OrderBy(temp => temp.Gender, StringComparer.OrdinalIgnoreCase).ToList(),
            _ => allPersons
        };

        return sortedPersons;
    }



    //Check if personUpdateRequest is not null
    //Validate all properties of "personUpdateRequest"
    //Get the matching "Person" object from List<Person> based on personID
    //Check if matching "Person" object is not null
    //Update the properties of matching "Person" object with the properties of "personUpdateRequest"
    //Convert matching "Person" object from Person to PersonResponse type
    //Return PersonResponse object
    public PersonResponse UpdatePerson(PersonUpdateRequest? personUpdateRequest)
    {
        if (personUpdateRequest == null)
        {
            throw new ArgumentNullException(nameof(Person));
        }

        //Validate PersonName
        //Validation can happen only in a context
        ValidationHelper.ModelValidation(personUpdateRequest);

        //Get the matching person object from data store
        Person? matchingPerson = _db.Persons.FirstOrDefault(temp => temp.PersonID == personUpdateRequest.PersonID);

        if (matchingPerson == null)
        {
            throw new ArgumentException("Given person id does not exist");
        }

        //Update the person object with the new values
        matchingPerson.PersonName = personUpdateRequest.PersonName;
        matchingPerson.Email = personUpdateRequest.Email;
        matchingPerson.DateOfBirth = personUpdateRequest.DateOfBirth;
        matchingPerson.Gender = personUpdateRequest.Gender.ToString();
        matchingPerson.CountryID = personUpdateRequest.CountryID;
        matchingPerson.Address = personUpdateRequest.Address;
        matchingPerson.ReceiveNewsLetters = personUpdateRequest.ReceiveNewsLetters;

        _db.SaveChanges();
        return ConvertPersonToPersonResponse(matchingPerson);


    }



    //Check if PersonId is not null
    //Get the MAtching "Person" object fron List<Person> based on PersonID
    //Check if matching "Person" object is not null
    //Remove the matching "Person" object from List<Person>
    //Return Boolean value indicating the status of deletion was successful or not
    public bool DeletePerson(Guid? personID)
    {
        if (personID == null)
        {
            throw new ArgumentNullException(nameof(personID));
        }

        Person? person = _db.Persons.FirstOrDefault(temp => temp.PersonID == personID);

        if (person == null)
        {
            return false;
        }

        _db.Persons.Remove(_db.Persons.First(temp => temp.PersonID == personID));
        _db.SaveChanges();

        return true;
    }
}
