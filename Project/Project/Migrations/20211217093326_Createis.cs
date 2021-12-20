using Microsoft.EntityFrameworkCore.Migrations;

namespace Project.Migrations
{
    public partial class Createis : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Persons",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Persons",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool));
        }
    }
}
