using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntranceManager.Migrations
{
    /// <inheritdoc />
    public partial class AddMultipleApartmentsEntrancesForUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Entrances_Users_ManagerUserId",
                table: "Entrances");

            migrationBuilder.AlterColumn<int>(
                name: "ManagerUserId",
                table: "Entrances",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "ApartmentUser",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ApartmentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApartmentUser", x => new { x.UserId, x.ApartmentId });
                    table.ForeignKey(
                        name: "FK_ApartmentUser_Apartments_ApartmentId",
                        column: x => x.ApartmentId,
                        principalTable: "Apartments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApartmentUser_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntranceUser",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    EntranceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntranceUser", x => new { x.UserId, x.EntranceId });
                    table.ForeignKey(
                        name: "FK_EntranceUser_Entrances_EntranceId",
                        column: x => x.EntranceId,
                        principalTable: "Entrances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntranceUser_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApartmentUser_ApartmentId",
                table: "ApartmentUser",
                column: "ApartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_EntranceUser_EntranceId",
                table: "EntranceUser",
                column: "EntranceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Entrances_Users_ManagerUserId",
                table: "Entrances",
                column: "ManagerUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Entrances_Users_ManagerUserId",
                table: "Entrances");

            migrationBuilder.DropTable(
                name: "ApartmentUser");

            migrationBuilder.DropTable(
                name: "EntranceUser");

            migrationBuilder.AlterColumn<int>(
                name: "ManagerUserId",
                table: "Entrances",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Entrances_Users_ManagerUserId",
                table: "Entrances",
                column: "ManagerUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
