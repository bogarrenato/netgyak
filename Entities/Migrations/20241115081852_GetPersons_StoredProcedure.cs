using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class GetPersons_StoredProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //dbo schema, name of the stored procedure
            //creates a store procedure in the db
            string sp_GetAllPersons = @"
                CREATE PROCEDURE [dbo].[GetAllPersons]
                AS
                BEGIN
                    SELECT PersonID, PersonName, Email, DateOfBirth, Gender, CountryId, Address, ReceiveNewsLetters FROM [dbo].[Persons];
                END";
            migrationBuilder.Sql(sp_GetAllPersons);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string sp_GetAllPersons = @"
                CREATE PROCEDURE [dbo].[GetAllPersons]
               ";
            migrationBuilder.Sql(sp_GetAllPersons);
        }
    }
}
