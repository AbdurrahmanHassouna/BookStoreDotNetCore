using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AprilBookStore.Infrastructure.Migrations
{
    public partial class ModernizeDomainTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PublicationYear",
                table: "Books",
                newName: "PublicationDate");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalAmount",
                table: "Orders",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)");

            migrationBuilder.AlterColumn<string>(
                name: "ISBN",
                table: "Books",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<decimal>(
                name: "BookStar",
                table: "Books",
                type: "decimal(2,1)",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "341743f0-asd2–42de-afbf-59kmkkmk72cf6",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d73fe18c-f33f-40a8-9ec4-138c56657648", "AQAAAAEAACcQAAAAEMxJlpDMRLasGdg341WRjtGWCZp94qxLX+t+bF89PWUNksJ6r9LgUc2iFd9DCBl/+A==", "2bcf0f10-cb46-45b4-8b54-a1783f1f3778" });

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000314"),
                columns: new[] { "BookStar", "ISBN" },
                values: new object[] { 2.1m, "9781509858637" });

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000315"),
                columns: new[] { "BookStar", "ISBN" },
                values: new object[] { 1.5m, "9780141033570" });

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000316"),
                columns: new[] { "BookStar", "ISBN" },
                values: new object[] { 2.1m, "9781784701994" });

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000317"),
                columns: new[] { "BookStar", "ISBN" },
                values: new object[] { 3.5m, "9781845298258" });

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000318"),
                columns: new[] { "BookStar", "ISBN" },
                values: new object[] { 2.1m, "9781846041242" });

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000319"),
                columns: new[] { "BookStar", "ISBN" },
                values: new object[] { 5.0m, "9780330523622" });

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000320"),
                columns: new[] { "BookStar", "ISBN" },
                values: new object[] { 2.1m, "9781780722405" });

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000321"),
                columns: new[] { "BookStar", "ISBN" },
                values: new object[] { 4.5m, "9780062457714" });

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000322"),
                columns: new[] { "BookStar", "ISBN" },
                values: new object[] { 3.5m, "9780099511021" });

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000323"),
                columns: new[] { "BookStar", "ISBN" },
                values: new object[] { 2.1m, "9780141978611" });

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000324"),
                columns: new[] { "BookStar", "ISBN" },
                values: new object[] { 3.5m, "9780141983769" });

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000325"),
                columns: new[] { "BookStar", "ISBN" },
                values: new object[] { 2.1m, "9780330533447" });

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000326"),
                columns: new[] { "BookStar", "ISBN" },
                values: new object[] { 2.1m, "9780099590087" });

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000327"),
                columns: new[] { "BookStar", "ISBN" },
                values: new object[] { 3.5m, "9780007498086" });

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000328"),
                columns: new[] { "BookStar", "ISBN" },
                values: new object[] { 2.1m, "9781846683145" });

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000329"),
                columns: new[] { "BookStar", "ISBN" },
                values: new object[] { 4.5m, "9780007250929" });

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000330"),
                columns: new[] { "BookStar", "ISBN" },
                values: new object[] { 2.1m, "9780099584574" });

            migrationBuilder.AddCheckConstraint(
                name: "CK_OrderItem_Quantity",
                table: "OrderItems",
                sql: "[Quantity] > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_CartItem_Quantity",
                table: "CartItems",
                sql: "[Quantity] > 0");

            migrationBuilder.CreateIndex(
                name: "IX_Books_ISBN",
                table: "Books",
                column: "ISBN",
                unique: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Book_QuantityInStock",
                table: "Books",
                sql: "[QuantityInStock] >= 0");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_OrderItem_Quantity",
                table: "OrderItems");

            migrationBuilder.DropCheckConstraint(
                name: "CK_CartItem_Quantity",
                table: "CartItems");

            migrationBuilder.DropIndex(
                name: "IX_Books_ISBN",
                table: "Books");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Book_QuantityInStock",
                table: "Books");

            migrationBuilder.RenameColumn(
                name: "PublicationDate",
                table: "Books",
                newName: "PublicationYear");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalAmount",
                table: "Orders",
                type: "decimal(10,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<long>(
                name: "ISBN",
                table: "Books",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<double>(
                name: "BookStar",
                table: "Books",
                type: "float",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(2,1)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "341743f0-asd2–42de-afbf-59kmkkmk72cf6",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "de1187ba-741d-481b-bcd6-cc1c86d230a6", "AQAAAAEAACcQAAAAEM1meMKS7A2+R8qlBWR/Fk2UvAwdfzn/GgRiRaM4ymKUhGyfAe3AvMrmNxuFMylP7A==", "89f45b4c-73d8-48ed-99de-cfb3ce0be4d4" });

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000314"),
                columns: new[] { "BookStar", "ISBN" },
                values: new object[] { 2.1000000000000001, 9781509858637L });

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000315"),
                columns: new[] { "BookStar", "ISBN" },
                values: new object[] { 1.5, 9780141033570L });

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000316"),
                columns: new[] { "BookStar", "ISBN" },
                values: new object[] { 2.1000000000000001, 9781784701994L });

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000317"),
                columns: new[] { "BookStar", "ISBN" },
                values: new object[] { 3.5, 9781845298258L });

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000318"),
                columns: new[] { "BookStar", "ISBN" },
                values: new object[] { 2.1000000000000001, 9781846041242L });

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000319"),
                columns: new[] { "BookStar", "ISBN" },
                values: new object[] { 5.5, 9780330523622L });

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000320"),
                columns: new[] { "BookStar", "ISBN" },
                values: new object[] { 2.1000000000000001, 9781780722405L });

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000321"),
                columns: new[] { "BookStar", "ISBN" },
                values: new object[] { 4.5, 9780062457714L });

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000322"),
                columns: new[] { "BookStar", "ISBN" },
                values: new object[] { 3.5, 9780099511021L });

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000323"),
                columns: new[] { "BookStar", "ISBN" },
                values: new object[] { 2.1000000000000001, 9780141978611L });

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000324"),
                columns: new[] { "BookStar", "ISBN" },
                values: new object[] { 3.5, 9780141983769L });

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000325"),
                columns: new[] { "BookStar", "ISBN" },
                values: new object[] { 2.1000000000000001, 9780330533447L });

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000326"),
                columns: new[] { "BookStar", "ISBN" },
                values: new object[] { 2.1000000000000001, 9780099590087L });

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000327"),
                columns: new[] { "BookStar", "ISBN" },
                values: new object[] { 3.5, 9780007498086L });

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000328"),
                columns: new[] { "BookStar", "ISBN" },
                values: new object[] { 2.1000000000000001, 9781846683145L });

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000329"),
                columns: new[] { "BookStar", "ISBN" },
                values: new object[] { 4.5, 9780007250929L });

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000330"),
                columns: new[] { "BookStar", "ISBN" },
                values: new object[] { 2.1000000000000001, 9780099584574L });
        }
    }
}
