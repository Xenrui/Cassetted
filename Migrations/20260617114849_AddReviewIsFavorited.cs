using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cassetted.Migrations
{
    /// <inheritdoc />
    public partial class AddReviewIsFavorited : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFavorited",
                table: "Reviews",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFavorited",
                table: "Reviews");
        }
    }
}
