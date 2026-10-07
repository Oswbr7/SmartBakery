using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartBakery.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixModelVersionIndexNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "ix_model_versions_status1",
                table: "model_versions",
                newName: "ux_model_versions_single_production");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "ux_model_versions_single_production",
                table: "model_versions",
                newName: "ix_model_versions_status1");
        }
    }
}
