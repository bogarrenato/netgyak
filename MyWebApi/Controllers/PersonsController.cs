﻿using Entities;
using Microsoft.AspNetCore.Mvc;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using System;

namespace MyWebApi;

[Route("persons")]
public class PersonsController : Controller
{
    private readonly IPersonsService _personsService;
    private readonly ICountriesService _countriesService;

    public PersonsController(IPersonsService personsService, ICountriesService countriesService)
    {
        _personsService = personsService;
        _countriesService = countriesService;
    }

    [Route("index")]
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
    [Route("create")]
    [HttpGet]
    public IActionResult Create()
    {
        List<CountryResponse> countries = _countriesService.GetAllCountries();
        ViewBag.Countries = countries;

        return View();
    }


    [HttpPost]
    [Route("create")]
    public IActionResult Create(PersonAddRequest personAddRequest)
    {
        if (!ModelState.IsValid)
        {
            List<CountryResponse> countries = _countriesService.GetAllCountries();
            ViewBag.Countries = countries;

            ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return View();
        }

        //call the service method
        PersonResponse personResponse = _personsService.AddPerson(personAddRequest);

        //navigate to Index() action method (it makes another get request to "persons/index"
        return RedirectToAction("Index", "Persons");
    }

}
