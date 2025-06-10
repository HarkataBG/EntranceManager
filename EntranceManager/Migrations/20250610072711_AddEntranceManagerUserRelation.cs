using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntranceManager.Migrations
{
    /// <inheritdoc />
    public partial class AddEntranceManagerUserRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Entrances_Users_ManagerUserId",
                table: "Entrances");

            migrationBuilder.AddForeignKey(
                name: "FK_Entrances_Users_ManagerUserId",
                table: "Entrances",
                column: "ManagerUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Entrances_Users_ManagerUserId",
                table: "Entrances");

            migrationBuilder.AddForeignKey(
                name: "FK_Entrances_Users_ManagerUserId",
                table: "Entrances",
                column: "ManagerUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
