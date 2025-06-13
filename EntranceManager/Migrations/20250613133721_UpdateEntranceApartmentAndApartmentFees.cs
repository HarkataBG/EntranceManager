using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntranceManager.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEntranceApartmentAndApartmentFees : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CountChildrenAsResidents",
                table: "Entrances",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfChildren",
                table: "Apartments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfPets",
                table: "Apartments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountAlreadyPaid",
                table: "ApartmentFees",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountForApartment",
                table: "ApartmentFees",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CountChildrenAsResidents",
                table: "Entrances");

            migrationBuilder.DropColumn(
                name: "NumberOfChildren",
                table: "Apartments");

            migrationBuilder.DropColumn(
                name: "NumberOfPets",
                table: "Apartments");

            migrationBuilder.DropColumn(
                name: "AmountAlreadyPaid",
                table: "ApartmentFees");

            migrationBuilder.DropColumn(
                name: "AmountForApartment",
                table: "ApartmentFees");
        }
    }
}
