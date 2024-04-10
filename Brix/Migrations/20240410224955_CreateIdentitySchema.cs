using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Brix.Migrations
{
    /// <inheritdoc />
    public partial class CreateIdentitySchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customer",
                columns: table => new
                {
                    customer_ID = table.Column<int>(type: "int", nullable: true),
                    first_name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    last_name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    birth_date = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    country_of_residence = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    gender = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    age = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "LineItem",
                columns: table => new
                {
                    transaction_ID = table.Column<int>(type: "int", nullable: true),
                    product_ID = table.Column<int>(type: "int", nullable: true),
                    qty = table.Column<int>(type: "int", nullable: true),
                    rating = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "Order",
                columns: table => new
                {
                    customer_ID = table.Column<int>(type: "int", nullable: true),
                    date = table.Column<int>(type: "int", nullable: true),
                    day_of_week = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    time = table.Column<int>(type: "int", nullable: true),
                    entry_mode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    amount = table.Column<float>(type: "real", nullable: true),
                    type_of_transaction = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    country_of_transaction = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    shipping_address = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    bank = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    type_of_card = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    fraud = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    product_ID = table.Column<int>(type: "int", nullable: true),
                    name = table.Column<string>(type: "varchar(64)", unicode: false, maxLength: 64, nullable: true),
                    year = table.Column<int>(type: "int", nullable: true),
                    num_parts = table.Column<int>(type: "int", nullable: true),
                    price = table.Column<int>(type: "int", nullable: true),
                    img_link = table.Column<string>(type: "varchar(256)", unicode: false, maxLength: 256, nullable: true),
                    primary_color = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    secondary_color = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    description = table.Column<string>(type: "varchar(4096)", unicode: false, maxLength: 4096, nullable: true),
                    category = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Customer");

            migrationBuilder.DropTable(
                name: "LineItem");

            migrationBuilder.DropTable(
                name: "Order");

            migrationBuilder.DropTable(
                name: "Product");
        }
    }
}
