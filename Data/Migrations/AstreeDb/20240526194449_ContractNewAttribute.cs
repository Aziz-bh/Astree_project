using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations.AstreeDb
{
    public partial class ContractNewAttribute : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Validated",
                table: "Contracts",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Validated",
                table: "Contracts");
        }
    }
}
