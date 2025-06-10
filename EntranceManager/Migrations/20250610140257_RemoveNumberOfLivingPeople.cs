using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntranceManager.Migrations
{
    /// <inheritdoc />
    public partial class RemoveNumberOfLivingPeople : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberOfLivingPeople",
                table: "Apartments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumberOfLivingPeople",
                table: "Apartments",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
