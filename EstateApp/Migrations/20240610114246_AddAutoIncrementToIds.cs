using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EstateApp.Migrations
{
    /// <inheritdoc />
    public partial class AddAutoIncrementToIds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Userinfos",
                table: "Userinfos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApartmentInfos",
                table: "ApartmentInfos");

            migrationBuilder.RenameTable(
                name: "Userinfos",
                newName: "UserInfo");

            migrationBuilder.RenameTable(
                name: "ApartmentInfos",
                newName: "ApartmentInfo");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserInfo",
                table: "UserInfo",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApartmentInfo",
                table: "ApartmentInfo",
                column: "id");

            migrationBuilder.CreateTable(
                name: "FeeInfo",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    feeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    feeAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeeInfo", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "HouseInfo",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    houseNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    streetName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    isRented = table.Column<bool>(type: "bit", nullable: false),
                    noOfApartment = table.Column<int>(type: "int", nullable: false),
                    houseType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    rentPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HouseInfo", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceInfo",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userId = table.Column<int>(type: "int", nullable: false),
                    dateIssue = table.Column<DateTime>(type: "datetime2", nullable: false),
                    dueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    refNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    feeType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceInfo", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentInfo",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    invoiceRefNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    amountPaid = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    paymentMethod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    paymentStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    paymentDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentInfo", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "RolesInfo",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    roleName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolesInfo", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "StreetInfo",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    streetName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StreetInfo", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "UserPropertiesInfo",
                columns: table => new
                {
                    userId = table.Column<int>(type: "int", nullable: false),
                    houseId = table.Column<int>(type: "int", nullable: false),
                    id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPropertiesInfo", x => new { x.userId, x.houseId });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FeeInfo");

            migrationBuilder.DropTable(
                name: "HouseInfo");

            migrationBuilder.DropTable(
                name: "InvoiceInfo");

            migrationBuilder.DropTable(
                name: "PaymentInfo");

            migrationBuilder.DropTable(
                name: "RolesInfo");

            migrationBuilder.DropTable(
                name: "StreetInfo");

            migrationBuilder.DropTable(
                name: "UserPropertiesInfo");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserInfo",
                table: "UserInfo");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApartmentInfo",
                table: "ApartmentInfo");

            migrationBuilder.RenameTable(
                name: "UserInfo",
                newName: "Userinfos");

            migrationBuilder.RenameTable(
                name: "ApartmentInfo",
                newName: "ApartmentInfos");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Userinfos",
                table: "Userinfos",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApartmentInfos",
                table: "ApartmentInfos",
                column: "id");
        }
    }
}
