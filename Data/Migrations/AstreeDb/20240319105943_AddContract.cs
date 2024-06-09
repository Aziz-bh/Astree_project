using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations.AstreeDb
{
    public partial class AddContract : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Automobiles");

            migrationBuilder.DropTable(
                name: "Properties");

            migrationBuilder.AddColumn<int>(
                name: "Coverage",
                table: "Contracts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EnginePower",
                table: "Contracts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Guarantees",
                table: "Contracts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Contracts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PropertyValue",
                table: "Contracts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RegistrationDate",
                table: "Contracts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegistrationNumber",
                table: "Contracts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SeatsNumber",
                table: "Contracts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "TrueVehicleValue",
                table: "Contracts",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Contracts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VehicleMake",
                table: "Contracts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VehicleType",
                table: "Contracts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "VehicleValue",
                table: "Contracts",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "YearOfConstruction",
                table: "Contracts",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Coverage",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "EnginePower",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "Guarantees",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "PropertyValue",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "RegistrationDate",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "RegistrationNumber",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "SeatsNumber",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "TrueVehicleValue",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "VehicleMake",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "VehicleType",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "VehicleValue",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "YearOfConstruction",
                table: "Contracts");

            migrationBuilder.CreateTable(
                name: "Automobiles",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    EnginePower = table.Column<int>(type: "int", nullable: false),
                    Guarantees = table.Column<int>(type: "int", nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RegistrationNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SeatsNumber = table.Column<int>(type: "int", nullable: false),
                    TrueVehicleValue = table.Column<float>(type: "real", nullable: false),
                    VehicleMake = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VehicleType = table.Column<int>(type: "int", nullable: false),
                    VehicleValue = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Automobiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Automobiles_Contracts_Id",
                        column: x => x.Id,
                        principalTable: "Contracts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Properties",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Coverage = table.Column<int>(type: "int", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PropertyValue = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    YearOfConstruction = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Properties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Properties_Contracts_Id",
                        column: x => x.Id,
                        principalTable: "Contracts",
                        principalColumn: "Id");
                });
        }
    }
}
