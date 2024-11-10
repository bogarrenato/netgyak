using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ServiceContracts;


/// <summary>
/// Represents business logic for maipulation Person Entity
/// </summary>
public interface IPersonsService
{

    /// <summary>
    /// Adds a new person into the list of persons
    /// </summary>
    /// <param name="personAddRequest"></param>
    /// <returns>Returns the same person details along with newly generated PersonID</returns>
    PersonResponse AddPerson(PersonAddRequest? personAddRequest);


    /// <summary>
    /// Returns all persons
    /// </summary>
    /// <returns>Returns a list of objects of PersonResponse type</returns>
    List<PersonResponse> GetAllPersons();



    /// <summary>
    /// Returns the person object based on the given person id
    /// </summary>
    /// <param name="personID"></param>
    /// <returns>Matching person object</returns>
    PersonResponse? GetPersonByPersonID(Guid? personID);




    /// <summary>
    /// Returns all persons objects that matches with the given search filed and search string
    /// </summary>
    /// <param name="searchBy">Search field to search</param>
    /// <param name="searchString">Search string to search</param>
    /// <returns>Returns all matching persons based on the given search field and search string</returns>
    List<PersonResponse> GetFilteredPersons(string searchBy, string? searchString);


    /// <summary>
    /// Returns sorted list of persons
    /// </summary>
    /// <param name="allpersons">Represents list of persons to sort</param>
    /// <param name="sortBy">Name of the property based on which the person should be sorted</param>
    /// <param name="sortOrderOptions">ASC or DESC</param>
    /// <returns>Returns sorted persons as Response List</returns>
    List<PersonResponse> GetSortedPersons(List<PersonResponse> allpersons, string sortBy, SortOrderOptions sortOrderOptions);



    /// <summary>
    ///  Updates the specified person details based on the given person ID
    /// </summary>
    /// <param name="personUpdateRequest">PErson details to update including personID</param>
    /// <returns>Person response object after updation</returns>
    public PersonResponse UpdatePerson(PersonUpdateRequest? personUpdateRequest);

    /// <summary>
    ///    Deletes the person based on the given person ID
    /// </summary>
    /// <param name="personID"></param>
    /// <returns>Returns a boolean based on if the deletion was successful or not </returns>
    public bool DeletePerson(Guid? personID);
}
