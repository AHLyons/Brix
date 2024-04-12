using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Brix.Migrations.BrixDatabase
{
    /// <inheritdoc />
    public partial class UpdateRecommendationSchemas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EstProductID",
                table: "ProductRecommendations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EstCustomerID",
                table: "CustomerProductsRecommendations",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstProductID",
                table: "ProductRecommendations");

            migrationBuilder.DropColumn(
                name: "EstCustomerID",
                table: "CustomerProductsRecommendations");
        }
    }
}
