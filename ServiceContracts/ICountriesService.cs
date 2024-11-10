using ServiceContracts.DTO;

namespace ServiceContracts;

/// <summary>
/// Represents business logic for manipulating Country entity
/// </summary>
public interface ICountriesService
{
    /// <summary>
    /// Adds a country object to the list of countries
    /// </summary>
    /// <param name="countryAddRequest"></param>
    /// <returns>Returns the country object after adding it</returns>
    CountryResponse AddCountry(CountryAddRequest? countryAddRequest);


    /// <summary>
    /// Returns all countries from the list
    /// </summary>
    /// <returns>All countries from the list as List of Countries response</returns>
    List<CountryResponse> GetAllCountries();

    /// <summary>
    /// Returns a country object based on a countryID
    /// </summary>
    /// <param name="CountryID">CountryID to search</param>
    /// <returns>Matching country by countryID</returns>
    CountryResponse? GetCountryByCountryID(Guid? CountryID);
}
