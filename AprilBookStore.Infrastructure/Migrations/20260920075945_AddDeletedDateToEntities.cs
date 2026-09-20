using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AprilBookStore.Infrastructure.Migrations
{
    public partial class AddDeletedDateToEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "Orders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "OrderItems",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "Categories",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "CartItems",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "Books",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "Authors",
                type: "datetime2",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "341743f0-asd2–42de-afbf-59kmkkmk72cf6",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "de1187ba-741d-481b-bcd6-cc1c86d230a6", "AQAAAAEAACcQAAAAEM1meMKS7A2+R8qlBWR/Fk2UvAwdfzn/GgRiRaM4ymKUhGyfAe3AvMrmNxuFMylP7A==", "89f45b4c-73d8-48ed-99de-cfb3ce0be4d4" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "Authors");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "341743f0-asd2–42de-afbf-59kmkkmk72cf6",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "7cf45ed8-52b7-4c26-bb58-765778e2a01d", "AQAAAAEAACcQAAAAEH6KTZszqlKGd3H+hz+/jeISo9Vohv6BbJ6pdv+NuwgtW2GWODSuS8beuPOoyOo4bg==", "a1fdf42f-0339-41b7-bef0-6c6d196d7f59" });
        }
    }
}
