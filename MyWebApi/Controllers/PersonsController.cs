using Entities;
using Microsoft.AspNetCore.Mvc;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using System;

namespace MyWebApi;

public class PersonsController : Controller
{
    private readonly IPersonsService _personsService;

    public PersonsController(IPersonsService personsService)
    {
        _personsService = personsService;
    }

    [Route("persons/index")]
    [Route("/")]
    public IActionResult Index(string searchBy, string? searchString, string sortBy = nameof(Person.PersonName), SortOrderOptions sortOrder = SortOrderOptions.ASC)
    {

        //Search
        ViewBag.SearchFields = new Dictionary<string, string>
        {
            { nameof(PersonResponse.PersonName), "Person Name" },
            { nameof(Person.Email), "Email" },
            { nameof(Person.DateOfBirth), "Date of Birth" },
            { nameof(Person.Address), "Address" },
            { nameof(Person.CountryID), "Country" },
        };
        List<PersonResponse> persons = _personsService.GetFilteredPersons(searchBy, searchString);
        ViewBag.CurrentSearchBy = searchBy;
        ViewBag.CurrentSearchString = searchString;


        //Sort
        List<PersonResponse> sortedPersons = _personsService.GetSortedPersons(persons, sortBy, sortOrder);
        ViewBag.CurrentSortBy = sortBy;
        ViewBag.CurrentSortOrder = sortOrder.ToString();

        return View(sortedPersons); //Views/Persons/Index.cshtml
    }


    //Executes when the user clicks on the Create PErson hyperlink
    [Route("persons/create")]
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

}
