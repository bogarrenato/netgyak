﻿using Entities;
using ServiceContracts.Enums;

namespace ServiceContracts.DTO;



/// <summary>
/// Represents DTO class that is used as return type of most methods of Persons Service
/// </summary>
public class PersonResponse
{
    public Guid PersonID { get; set; }
    public string? PersonName { get; set; }
    public string? Email { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public Guid? CountryID { get; set; }
    public string? Country { get; set; }
    public string? Address { get; set; }
    public bool ReceiveNewsLetters { get; set; }
    public double? Age { get; set; }



    /// <summary>
    /// Compares the current object data with the parameter object data
    /// </summary>
    /// <param name="obj"></param>
    /// <returns>True or false indicating whether all person details are matched with specified parameter</returns>
    public override bool Equals(object? obj)
    {
        if (obj == null)
        {
            return false;
        }

        if (obj.GetType() != typeof(PersonResponse))
        {
            return false;
        }

        PersonResponse person = (PersonResponse)obj;

        return this.PersonID == person.PersonID
            && this.PersonName == person.PersonName
            && this.Email == person.Email
            && this.DateOfBirth == person.DateOfBirth
            && this.Gender == person.Gender
            && this.CountryID == person.CountryID
            && this.Country == person.Country
            && this.Address == person.Address
            && this.ReceiveNewsLetters == person.ReceiveNewsLetters
            && this.Age == person.Age;

    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        return $"PersonID: {PersonID}, PersonName: {PersonName}, Email: {Email}, DateOfBirth: {DateOfBirth}, ReceiveNewsLetters: {ReceiveNewsLetters}, Gender:{Gender}, CountryID: {CountryID}, Country: {Country}, Address: {Address}, Age: {Age}";
    }


    public PersonUpdateRequest ToPersonUpdateRequest()
    {
        return new PersonUpdateRequest()
        {
            PersonID = this.PersonID,
            PersonName = this.PersonName,
            Email = this.Email,
            DateOfBirth = this.DateOfBirth,
            Gender = (GenderOptions)Enum.Parse(typeof(GenderOptions), Gender, true),
            Address = this.Address,
        };

    }
}

public static class PersonExtensions
{
    /// <summary>
    /// An extension method to convert an object of Person class into PersonResponse Class
    /// </summary>
    /// <param name="person"> Retruns the converted PersonResponse object</param>
    public static PersonResponse ToPersonResponse(this Person person)
    {
        return new PersonResponse()
        {
            PersonID = person.PersonID,
            PersonName = person.PersonName,
            Email = person.Email,
            DateOfBirth = person.DateOfBirth,
            Gender = person.Gender,
            CountryID = person.CountryID,
            Age = (person.DateOfBirth != null) ? Math.Floor((DateTime.Now - person.DateOfBirth.Value).TotalDays / 365.25) : null,
        };
    }



}

