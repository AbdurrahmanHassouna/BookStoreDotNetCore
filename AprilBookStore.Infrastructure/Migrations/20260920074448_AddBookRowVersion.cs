using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AprilBookStore.Infrastructure.Migrations
{
    public partial class AddBookRowVersion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Books",
                type: "rowversion",
                rowVersion: true,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "341743f0-asd2–42de-afbf-59kmkkmk72cf6",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "7cf45ed8-52b7-4c26-bb58-765778e2a01d", "AQAAAAEAACcQAAAAEH6KTZszqlKGd3H+hz+/jeISo9Vohv6BbJ6pdv+NuwgtW2GWODSuS8beuPOoyOo4bg==", "a1fdf42f-0339-41b7-bef0-6c6d196d7f59" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Books");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "341743f0-asd2–42de-afbf-59kmkkmk72cf6",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "54cdfd8c-12d9-42b6-bfc3-639b905a24b6", "AQAAAAEAACcQAAAAEMIRhIhJbKD+zuKLEQUQnUbKuiqZbdq0EpGkvljuF7/Rhqb3Bn5nKKH1vyQcYWUASw==", "4f71403d-4ee6-4e95-88da-557252588324" });
        }
    }
}
