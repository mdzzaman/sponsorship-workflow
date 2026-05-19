using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SponsorshipWorkflow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameActionedAtToRecordedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ActionedAt",
                table: "WorkflowHistories",
                newName: "RecordedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RecordedAt",
                table: "WorkflowHistories",
                newName: "ActionedAt");
        }
    }
}
