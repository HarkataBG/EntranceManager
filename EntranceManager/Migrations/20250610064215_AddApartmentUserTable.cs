using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntranceManager.Migrations
{
    /// <inheritdoc />
    public partial class AddApartmentUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApartmentUser_Apartments_ApartmentId",
                table: "ApartmentUser");

            migrationBuilder.DropForeignKey(
                name: "FK_ApartmentUser_Users_UserId",
                table: "ApartmentUser");

            migrationBuilder.DropForeignKey(
                name: "FK_EntranceUser_Entrances_EntranceId",
                table: "EntranceUser");

            migrationBuilder.DropForeignKey(
                name: "FK_EntranceUser_Users_UserId",
                table: "EntranceUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EntranceUser",
                table: "EntranceUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApartmentUser",
                table: "ApartmentUser");

            migrationBuilder.RenameTable(
                name: "EntranceUser",
                newName: "EntranceUsers");

            migrationBuilder.RenameTable(
                name: "ApartmentUser",
                newName: "ApartmentUsers");

            migrationBuilder.RenameIndex(
                name: "IX_EntranceUser_EntranceId",
                table: "EntranceUsers",
                newName: "IX_EntranceUsers_EntranceId");

            migrationBuilder.RenameIndex(
                name: "IX_ApartmentUser_ApartmentId",
                table: "ApartmentUsers",
                newName: "IX_ApartmentUsers_ApartmentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EntranceUsers",
                table: "EntranceUsers",
                columns: new[] { "UserId", "EntranceId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApartmentUsers",
                table: "ApartmentUsers",
                columns: new[] { "UserId", "ApartmentId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ApartmentUsers_Apartments_ApartmentId",
                table: "ApartmentUsers",
                column: "ApartmentId",
                principalTable: "Apartments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ApartmentUsers_Users_UserId",
                table: "ApartmentUsers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EntranceUsers_Entrances_EntranceId",
                table: "EntranceUsers",
                column: "EntranceId",
                principalTable: "Entrances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EntranceUsers_Users_UserId",
                table: "EntranceUsers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApartmentUsers_Apartments_ApartmentId",
                table: "ApartmentUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_ApartmentUsers_Users_UserId",
                table: "ApartmentUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_EntranceUsers_Entrances_EntranceId",
                table: "EntranceUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_EntranceUsers_Users_UserId",
                table: "EntranceUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EntranceUsers",
                table: "EntranceUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApartmentUsers",
                table: "ApartmentUsers");

            migrationBuilder.RenameTable(
                name: "EntranceUsers",
                newName: "EntranceUser");

            migrationBuilder.RenameTable(
                name: "ApartmentUsers",
                newName: "ApartmentUser");

            migrationBuilder.RenameIndex(
                name: "IX_EntranceUsers_EntranceId",
                table: "EntranceUser",
                newName: "IX_EntranceUser_EntranceId");

            migrationBuilder.RenameIndex(
                name: "IX_ApartmentUsers_ApartmentId",
                table: "ApartmentUser",
                newName: "IX_ApartmentUser_ApartmentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EntranceUser",
                table: "EntranceUser",
                columns: new[] { "UserId", "EntranceId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApartmentUser",
                table: "ApartmentUser",
                columns: new[] { "UserId", "ApartmentId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ApartmentUser_Apartments_ApartmentId",
                table: "ApartmentUser",
                column: "ApartmentId",
                principalTable: "Apartments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ApartmentUser_Users_UserId",
                table: "ApartmentUser",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EntranceUser_Entrances_EntranceId",
                table: "EntranceUser",
                column: "EntranceId",
                principalTable: "Entrances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EntranceUser_Users_UserId",
                table: "EntranceUser",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
