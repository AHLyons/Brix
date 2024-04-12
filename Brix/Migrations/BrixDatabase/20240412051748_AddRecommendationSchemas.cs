using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Brix.Migrations.BrixDatabase
{
    /// <inheritdoc />
    public partial class AddRecommendationSchemas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomerProductsRecommendations",
                columns: table => new
                {
                    CustomerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RecommendedProduct = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerProductsRecommendations", x => x.CustomerId);
                });

            migrationBuilder.CreateTable(
                name: "ProductRecommendations",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RecommendedProductId = table.Column<int>(type: "int", nullable: true),
                    SimilarityScore = table.Column<float>(type: "real", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductRecommendations", x => x.ProductId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerProductsRecommendations");

            migrationBuilder.DropTable(
                name: "ProductRecommendations");
        }
    }
}
