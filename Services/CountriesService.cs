using Entities;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services;

public class CountriesService : ICountriesService
{

    private readonly PersonsDbContext _db;

    public CountriesService(PersonsDbContext personsDbContext)
    {
        _db = personsDbContext;
    }

    //Check if countryAddRequest is not null
    //Validate all properties of "countryAddRequest"
    //convert "counterAddRequest" from "CountryAddRequest" to type "Country"
    //Generate a new CountryId
    //Add to List<Country>
    //Return CounteryResponse object with generated CountryID
    public CountryResponse AddCountry(CountryAddRequest? countryAddRequest)
    {
        if (countryAddRequest == null)
        {
            throw new ArgumentNullException(nameof(countryAddRequest));
        }

        if (countryAddRequest.CountryName == null)
        {
            throw new ArgumentException(nameof(countryAddRequest.CountryName));
        }

        if (_db.Countries.Where(temp => temp.CountryName == countryAddRequest.CountryName).Count() > 0)
        {
            throw new ArgumentException("Given country name already exists");
        }

        Country country = countryAddRequest.ToCountry();

        country.CountryID = Guid.NewGuid();

        _db.Countries.Add(country);
        // It will verify the changes and save it to the database
        _db.SaveChanges();

        return country.ToCountryResponse();
    }

    public List<CountryResponse> GetAllCountries()
    {
        return _db.Countries
            .Select(country => country.ToCountryResponse()).ToList();
    }

    public CountryResponse? GetCountryByCountryID(Guid? countryID)
    {
        if (countryID == null)
        {
            return null;
        }

        Country? country_response_from_list = _db.Countries.FirstOrDefault(temp => temp.CountryID == countryID);

        if (country_response_from_list == null)
        {
            return null;
        }

        return country_response_from_list.ToCountryResponse();
    }
}
