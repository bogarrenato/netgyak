using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativa.AspNetCore;
using Rotativa.AspNetCore.Options;
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
    public async Task<IActionResult> Index(string searchBy, string? searchString, string sortBy = nameof(Person.PersonName), SortOrderOptions sortOrder = SortOrderOptions.ASC)
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
        List<PersonResponse> persons = await _personsService.GetFilteredPersons(searchBy, searchString);
        ViewBag.CurrentSearchBy = searchBy;
        ViewBag.CurrentSearchString = searchString;


        //Sort
        List<PersonResponse> sortedPersons = await _personsService.GetSortedPersons(persons, sortBy, sortOrder);
        ViewBag.CurrentSortBy = sortBy;
        ViewBag.CurrentSortOrder = sortOrder.ToString();

        return View(sortedPersons); //Views/Persons/Index.cshtml
    }


    //Executes when the user clicks on the Create PErson hyperlink
    [Route("create")]
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        List<CountryResponse> countries = await _countriesService.GetAllCountries();
        ViewBag.Countries = countries;

        return View();
    }


    [HttpPost]
    [Route("create")]
    public async Task<IActionResult> Create(PersonAddRequest personAddRequest)
    {
        if (!ModelState.IsValid)
        {
            List<CountryResponse> countries = await _countriesService.GetAllCountries();
            ViewBag.Countries = countries;

            ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return View();
        }

        //call the service method
        PersonResponse personResponse = await _personsService.AddPerson(personAddRequest);

        //navigate to Index() action method (it makes another get request to "persons/index"
        return RedirectToAction("Index", "Persons");
    }


    [HttpPost]
    [Route("[action]/{personID}")]
    public async Task<IActionResult> Edit(PersonUpdateRequest personUpdateRequest)
    {
        PersonResponse? personResponse = await _personsService.GetPersonByPersonID(personUpdateRequest.PersonID);

        if (personResponse == null)
        {
            return RedirectToAction("Index");
        }

        if (ModelState.IsValid)
        {
            PersonResponse updatedPerson = await _personsService.UpdatePerson(personUpdateRequest);
            return RedirectToAction("Index");
        }
        else
        {
            List<CountryResponse> countries = await _countriesService.GetAllCountries();
            ViewBag.Countries = countries.Select(temp =>
            new SelectListItem() { Text = temp.CountryName, Value = temp.CountryID.ToString() });

            ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return View(personResponse.ToPersonUpdateRequest());
        }
    }

    [HttpGet]
    [Route("[action]/{personID}")] //Eg: /persons/edit/1
    public async Task<IActionResult> Edit(Guid personID)
    {
        PersonResponse? personResponse = await _personsService.GetPersonByPersonID(personID);
        if (personResponse == null)
        {
            return RedirectToAction("Index");
        }

        PersonUpdateRequest personUpdateRequest = personResponse.ToPersonUpdateRequest();

        List<CountryResponse> countries = await _countriesService.GetAllCountries();
        ViewBag.Countries = countries.Select(temp =>
        new SelectListItem() { Text = temp.CountryName, Value = temp.CountryID.ToString() });

        return View(personUpdateRequest);
    }



    [HttpGet]
    [Route("[action]/{personID}")]
    public async Task<IActionResult> Delete(Guid? personID)
    {
        PersonResponse? personResponse = await _personsService.GetPersonByPersonID(personID);
        if (personResponse == null)
            return RedirectToAction("Index");

        return View(personResponse);
    }

    [HttpPost]
    [Route("[action]/{personID}")]
    public async Task<IActionResult> Delete(PersonUpdateRequest personUpdateResult)
    {
        PersonResponse? personResponse = await _personsService.GetPersonByPersonID(personUpdateResult.PersonID);
        if (personResponse == null)
            return RedirectToAction("Index");

        await _personsService.DeletePerson(personUpdateResult.PersonID);
        return RedirectToAction("Index");
    }


    //torolheto
    [Route("PersonsPDF")]
    public async Task<IActionResult> PersonPDF()
    {
        //get list of persons
        List<PersonResponse> persons = await _personsService.GetAllPersons();

        //Name of the view, and the name of the data

        return new ViewAsPdf("PersonPDF", persons, ViewData)
        {
            PageMargins = new Margins()
            {
                Top = 20,
                Right = 20,
                Bottom = 20,
                Left = 20
            },
            PageOrientation = Orientation.Landscape
        };
    }


}
