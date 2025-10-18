using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProTrack.Migrations
{
    /// <inheritdoc />
    public partial class MakeContactEmailNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ContactEmail",
                table: "Clients",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "demo-user-123",
                columns: new[] { "ConcurrencyStamp", "CreatedDate", "SecurityStamp" },
                values: new object[] { "29a732ee-e310-41f1-9e2e-f5f6c1335c74", new DateTime(2025, 10, 18, 16, 41, 49, 456, DateTimeKind.Utc).AddTicks(1063), "1ba05cbb-8722-4d40-9b06-ff388bfa778f" });

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 10, 18, 16, 41, 49, 456, DateTimeKind.Utc).AddTicks(1225));

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 10, 18, 16, 41, 49, 456, DateTimeKind.Utc).AddTicks(1228));

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "EndDate", "LastModified", "StartDate" },
                values: new object[] { new DateTime(2025, 10, 18, 16, 41, 49, 456, DateTimeKind.Utc).AddTicks(1261), new DateTime(2025, 12, 17, 16, 41, 49, 456, DateTimeKind.Utc).AddTicks(1260), new DateTime(2025, 10, 18, 16, 41, 49, 456, DateTimeKind.Utc).AddTicks(1261), new DateTime(2025, 9, 18, 16, 41, 49, 456, DateTimeKind.Utc).AddTicks(1253) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "EndDate", "LastModified", "StartDate" },
                values: new object[] { new DateTime(2025, 10, 18, 16, 41, 49, 456, DateTimeKind.Utc).AddTicks(1266), new DateTime(2025, 12, 2, 16, 41, 49, 456, DateTimeKind.Utc).AddTicks(1265), new DateTime(2025, 10, 18, 16, 41, 49, 456, DateTimeKind.Utc).AddTicks(1266), new DateTime(2025, 10, 3, 16, 41, 49, 456, DateTimeKind.Utc).AddTicks(1264) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ContactEmail",
                table: "Clients",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "demo-user-123",
                columns: new[] { "ConcurrencyStamp", "CreatedDate", "SecurityStamp" },
                values: new object[] { "497ca2d4-736d-4782-b960-0fd65fa0f611", new DateTime(2025, 10, 12, 21, 6, 54, 194, DateTimeKind.Utc).AddTicks(89), "fcaf6336-7d2a-486c-883e-7b68204d2e81" });

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 10, 12, 21, 6, 54, 194, DateTimeKind.Utc).AddTicks(243));

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 10, 12, 21, 6, 54, 194, DateTimeKind.Utc).AddTicks(246));

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "EndDate", "LastModified", "StartDate" },
                values: new object[] { new DateTime(2025, 10, 12, 21, 6, 54, 194, DateTimeKind.Utc).AddTicks(283), new DateTime(2025, 12, 11, 21, 6, 54, 194, DateTimeKind.Utc).AddTicks(283), new DateTime(2025, 10, 12, 21, 6, 54, 194, DateTimeKind.Utc).AddTicks(284), new DateTime(2025, 9, 12, 21, 6, 54, 194, DateTimeKind.Utc).AddTicks(276) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "EndDate", "LastModified", "StartDate" },
                values: new object[] { new DateTime(2025, 10, 12, 21, 6, 54, 194, DateTimeKind.Utc).AddTicks(289), new DateTime(2025, 11, 26, 21, 6, 54, 194, DateTimeKind.Utc).AddTicks(288), new DateTime(2025, 10, 12, 21, 6, 54, 194, DateTimeKind.Utc).AddTicks(289), new DateTime(2025, 9, 27, 21, 6, 54, 194, DateTimeKind.Utc).AddTicks(287) });
        }
    }
}
