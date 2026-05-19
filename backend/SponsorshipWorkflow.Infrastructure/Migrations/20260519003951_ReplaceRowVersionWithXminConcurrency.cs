using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SponsorshipWorkflow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceRowVersionWithXminConcurrency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "SponsorshipRequests");

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "SponsorshipRequests",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "xmin",
                table: "SponsorshipRequests");

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "SponsorshipRequests",
                type: "bytea",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);
        }
    }
}
