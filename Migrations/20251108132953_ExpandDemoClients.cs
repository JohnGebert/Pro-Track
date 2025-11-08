using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ProTrack.Migrations
{
    /// <inheritdoc />
    public partial class ExpandDemoClients : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "demo-user-123",
                columns: new[] { "ConcurrencyStamp", "CreatedDate", "SecurityStamp" },
                values: new object[] { "9cef744a-cacd-40cd-a4c3-63a6f8639781", new DateTime(2025, 11, 8, 13, 29, 48, 949, DateTimeKind.Utc).AddTicks(7480), "9a73947e-d033-4fa4-8483-ff7f0b526e6a" });

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 11, 8, 13, 29, 48, 949, DateTimeKind.Utc).AddTicks(7772));

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 11, 8, 13, 29, 48, 949, DateTimeKind.Utc).AddTicks(7776));

            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM [Clients] WHERE [Id] = 3)
BEGIN
    SET IDENTITY_INSERT [Clients] ON;
    INSERT INTO [Clients] ([Id], [Address], [ContactEmail], [CreatedDate], [IsActive], [Name], [Notes], [PhoneNumber], [UserId])
    VALUES (3, N'1010 Inspiration Lane, Los Angeles, CA 90012', N'hello@creativevision.com', '2025-11-08T13:29:48.9497779Z', 1, N'Creative Vision Studios', N'Boutique design studio specializing in brand identity, motion graphics, and experiential marketing activations.', N'+1-555-CREATIVE', N'demo-user-123');
    SET IDENTITY_INSERT [Clients] OFF;
END

IF NOT EXISTS (SELECT 1 FROM [Clients] WHERE [Id] = 4)
BEGIN
    SET IDENTITY_INSERT [Clients] ON;
    INSERT INTO [Clients] ([Id], [Address], [ContactEmail], [CreatedDate], [IsActive], [Name], [Notes], [PhoneNumber], [UserId])
    VALUES (4, N'820 Freight Avenue, Chicago, IL 60654', N'ops@northwindlogistics.com', '2025-11-08T13:29:48.9497782Z', 1, N'Northwind Logistics', N'National logistics provider focused on cold-chain and last-mile delivery optimization.', N'+1-555-NORTH-01', N'demo-user-123');
    SET IDENTITY_INSERT [Clients] OFF;
END

IF NOT EXISTS (SELECT 1 FROM [Clients] WHERE [Id] = 5)
BEGIN
    SET IDENTITY_INSERT [Clients] ON;
    INSERT INTO [Clients] ([Id], [Address], [ContactEmail], [CreatedDate], [IsActive], [Name], [Notes], [PhoneNumber], [UserId])
    VALUES (5, N'550 Strategy Street, Austin, TX 78701', N'team@brightpathco.com', '2025-11-08T13:29:48.9497785Z', 1, N'BrightPath Consulting', N'Management consulting firm delivering digital transformation roadmaps for mid-market enterprises.', N'+1-555-BRIGHT-5', N'demo-user-123');
    SET IDENTITY_INSERT [Clients] OFF;
END

IF NOT EXISTS (SELECT 1 FROM [Clients] WHERE [Id] = 6)
BEGIN
    SET IDENTITY_INSERT [Clients] ON;
    INSERT INTO [Clients] ([Id], [Address], [ContactEmail], [CreatedDate], [IsActive], [Name], [Notes], [PhoneNumber], [UserId])
    VALUES (6, N'300 Peaks Plaza, Denver, CO 80202', N'info@summitfg.com', '2025-11-08T13:29:48.9497788Z', 1, N'Summit Financial Group', N'Wealth management and fractional CFO services for high-growth startups and professional services firms.', N'+1-555-SUMMIT-6', N'demo-user-123');
    SET IDENTITY_INSERT [Clients] OFF;
END

IF NOT EXISTS (SELECT 1 FROM [Clients] WHERE [Id] = 7)
BEGIN
    SET IDENTITY_INSERT [Clients] ON;
    INSERT INTO [Clients] ([Id], [Address], [ContactEmail], [CreatedDate], [IsActive], [Name], [Notes], [PhoneNumber], [UserId])
    VALUES (7, N'88 Sustainability Way, Portland, OR 97205', N'contact@greenearth.org', '2025-11-08T13:29:48.9497790Z', 1, N'GreenEarth Initiatives', N'Environmental non-profit coordinating sustainability campaigns, grant programs, and community outreach.', N'+1-555-GREEN-07', N'demo-user-123');
    SET IDENTITY_INSERT [Clients] OFF;
END

IF NOT EXISTS (SELECT 1 FROM [Clients] WHERE [Id] = 8)
BEGIN
    SET IDENTITY_INSERT [Clients] ON;
    INSERT INTO [Clients] ([Id], [Address], [ContactEmail], [CreatedDate], [IsActive], [Name], [Notes], [PhoneNumber], [UserId])
    VALUES (8, N'212 Wellness Parkway, Boston, MA 02110', N'support@apexhealth.com', '2025-11-08T13:29:48.9497793Z', 1, N'Apex Healthcare Solutions', N'Healthcare technology provider delivering telemedicine platforms and compliance automation.', N'+1-555-APEX-CARE', N'demo-user-123');
    SET IDENTITY_INSERT [Clients] OFF;
END

IF NOT EXISTS (SELECT 1 FROM [Clients] WHERE [Id] = 9)
BEGIN
    SET IDENTITY_INSERT [Clients] ON;
    INSERT INTO [Clients] ([Id], [Address], [ContactEmail], [CreatedDate], [IsActive], [Name], [Notes], [PhoneNumber], [UserId])
    VALUES (9, N'77 Insight Terrace, Seattle, WA 98104', N'partners@bluewaveanalytics.com', '2025-11-08T13:29:48.9497798Z', 1, N'BlueWave Analytics', N'Data analytics consultancy focusing on predictive modeling, data warehousing, and BI dashboards.', N'+1-555-WAVE-009', N'demo-user-123');
    SET IDENTITY_INSERT [Clients] OFF;
END

IF NOT EXISTS (SELECT 1 FROM [Clients] WHERE [Id] = 10)
BEGIN
    SET IDENTITY_INSERT [Clients] ON;
    INSERT INTO [Clients] ([Id], [Address], [ContactEmail], [CreatedDate], [IsActive], [Name], [Notes], [PhoneNumber], [UserId])
    VALUES (10, N'640 Skyline Boulevard, Miami, FL 33130', N'studio@urbanedgearch.com', '2025-11-08T13:29:48.9497837Z', 1, N'UrbanEdge Architects', N'Architectural firm creating sustainable mixed-use developments and adaptive reuse projects.', N'+1-555-URBAN-10', N'demo-user-123');
    SET IDENTITY_INSERT [Clients] OFF;
END

IF NOT EXISTS (SELECT 1 FROM [Clients] WHERE [Id] = 11)
BEGIN
    SET IDENTITY_INSERT [Clients] ON;
    INSERT INTO [Clients] ([Id], [Address], [ContactEmail], [CreatedDate], [IsActive], [Name], [Notes], [PhoneNumber], [UserId])
    VALUES (11, N'400 Commerce Court, Minneapolis, MN 55401', N'hq@silverlineretail.com', '2025-11-08T13:29:48.9497839Z', 1, N'Silverline Retailers', N'Regional retail chain investing in e-commerce modernization and omnichannel fulfillment.', N'+1-555-SILVER-1', N'demo-user-123');
    SET IDENTITY_INSERT [Clients] OFF;
END

IF NOT EXISTS (SELECT 1 FROM [Clients] WHERE [Id] = 12)
BEGIN
    SET IDENTITY_INSERT [Clients] ON;
    INSERT INTO [Clients] ([Id], [Address], [ContactEmail], [CreatedDate], [IsActive], [Name], [Notes], [PhoneNumber], [UserId])
    VALUES (12, N'950 Innovation Parkway, Detroit, MI 48226', N'projects@nextgenmfg.com', '2025-11-08T13:29:48.9497842Z', 1, N'NextGen Manufacturing', N'Advanced manufacturing company deploying IoT-enabled production lines and robotics integration.', N'+1-555-NEXTGEN', N'demo-user-123');
    SET IDENTITY_INSERT [Clients] OFF;
END
");

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "EndDate", "LastModified", "StartDate" },
                values: new object[] { new DateTime(2025, 11, 8, 13, 29, 48, 949, DateTimeKind.Utc).AddTicks(7917), new DateTime(2026, 1, 7, 13, 29, 48, 949, DateTimeKind.Utc).AddTicks(7916), new DateTime(2025, 11, 8, 13, 29, 48, 949, DateTimeKind.Utc).AddTicks(7918), new DateTime(2025, 10, 9, 13, 29, 48, 949, DateTimeKind.Utc).AddTicks(7905) });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "EndDate", "LastModified", "StartDate" },
                values: new object[] { new DateTime(2025, 11, 8, 13, 29, 48, 949, DateTimeKind.Utc).AddTicks(7924), new DateTime(2025, 12, 23, 13, 29, 48, 949, DateTimeKind.Utc).AddTicks(7923), new DateTime(2025, 11, 8, 13, 29, 48, 949, DateTimeKind.Utc).AddTicks(7925), new DateTime(2025, 10, 24, 13, 29, 48, 949, DateTimeKind.Utc).AddTicks(7922) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 12);

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
    }
}
