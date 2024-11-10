using System.ComponentModel.DataAnnotations;
using Entities;
using ServiceContracts.Enums;

namespace ServiceContracts.DTO;


/// <summary>
/// Acts as a DTO for inserting a new person
/// </summary>
public class PersonAddRequest
{
    [Required(ErrorMessage = "Person Name is required")]
    public string? PersonName { get; set; }
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    public string? Email { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public GenderOptions Gender { get; set; }
    public Guid? CountryID { get; set; }
    public string? Address { get; set; }
    public bool ReceiveNewsLetters { get; set; }



    /// <summary>
    /// Converts the current object of PersonAddRequest into a new object of Person type
    /// </summary>
    /// <returns></returns>
    public Person ToPerson()
    {
        return new Person()
        {
            PersonName = PersonName,
            Email = Email,
            DateOfBirth = DateOfBirth,
            Gender = Gender.ToString(),
            Address = Address,
            CountryID = CountryID,
            ReceiveNewsLetters = ReceiveNewsLetters
        };
    }
}
