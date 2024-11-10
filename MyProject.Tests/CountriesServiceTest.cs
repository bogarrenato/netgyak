using ServiceContracts;
using ServiceContracts.DTO;
using Services;

namespace MyProject.Tests;

public class CountriesServiceTest
{
    private readonly ICountriesService _countriesService;

    public CountriesServiceTest()
    {
        _countriesService = new CountriesService();
    }


    #region AddCountry
    //When CountryAddRequest is null, it should provide ArgumentNullException
    [Fact]
    public void AddCountry_NullCountry()
    {
        //Arrange 
        CountryAddRequest? request = null;


        //Assert
        Assert.Throws<ArgumentNullException>(() =>
        {
            //Act
            _countriesService.AddCountry(request);
        });
    }

    //When CountryName is null, it should provide ArgumentException

    [Fact]
    public void AddCountry_CountryNameIsNull()
    {
        //Arrange 
        CountryAddRequest? request = new CountryAddRequest() { CountryName = null };


        //Assert
        Assert.Throws<ArgumentException>(() =>
        {
            //Act
            _countriesService.AddCountry(request);
        });
    }

    //When CountryName is duplicate it should throw ArgumentException

    [Fact]
    public void AddCountry_DuplicateCountryName()
    {
        //Arrange 
        CountryAddRequest? request1 = new CountryAddRequest() { CountryName = "USA" };
        CountryAddRequest? request2 = new CountryAddRequest() { CountryName = "USA" };


        //Assert
        Assert.Throws<ArgumentException>(() =>
        {
            //Act
            _countriesService.AddCountry(request1);
            _countriesService.AddCountry(request2);
        });
    }


    //When you supply proper CountryName it should insert the to the list of countries
    [Fact]
    public void AddCountry_ProperCountryDetails()
    {
        //Arrange 
        CountryAddRequest? request1 = new CountryAddRequest() { CountryName = "Japan" };

        //Act
        CountryResponse response = _countriesService.AddCountry(request1);

        List<CountryResponse> counteries_from_GetAllCountries = _countriesService.GetAllCountries();

        //Assert
        Assert.True(response.CountryID != Guid.Empty);
        Assert.Contains(response, counteries_from_GetAllCountries);

    }
    #endregion


    #region GetAllCountries

    [Fact]
    //The list of countries should be empty by default
    public void GetAllCountries_EmptyList()
    {
        //Acts
        List<CountryResponse> actual_country_response_list = _countriesService.GetAllCountries();

        //Assert
        Assert.Empty(actual_country_response_list);
    }

    [Fact]
    public void GetAllCountries_AddFewCountries()
    {
        //Arrange
        List<CountryAddRequest> country_request_list = new List<CountryAddRequest>()
        {
            new CountryAddRequest() { CountryName = "USA"},
            new CountryAddRequest() { CountryName = "UK"},
        };

        //Act
        List<CountryResponse> counteries_list_from_add_country = new List<CountryResponse>();
        foreach (CountryAddRequest country_request in country_request_list)
        {
            counteries_list_from_add_country.Add(_countriesService.AddCountry(country_request));
        }

        List<CountryResponse> actualCountryResponseList = _countriesService.GetAllCountries();

        foreach (CountryResponse expected_country in counteries_list_from_add_country)
        {
            Assert.Contains(expected_country, actualCountryResponseList);
        }
    }

    #endregion



    #region GetCountryByCountryID

    [Fact]
    public void GetCountryByCountryID_NullCountryID()
    {
        //Arrange
        Guid? countryID = null;

        //Act
        CountryResponse? country_response_from_get_method = _countriesService.GetCountryByCountryID(countryID);

        //Assert
        Assert.Null(country_response_from_get_method);
    }


    [Fact]
    //If id is calid it should give back the country
    public void GetCountryByCountryID_ValidCountryID()
    {
        //Arrange
        CountryAddRequest country_add_request = new CountryAddRequest() { CountryName = "China" };
        CountryResponse country_response_from_add = _countriesService.AddCountry(country_add_request);

        //Act
        CountryResponse? country_response_from_get = _countriesService.GetCountryByCountryID(country_response_from_add.CountryID);

        //Assert
        Assert.Equal(country_response_from_add, country_response_from_get);
    }

    #endregion

}
