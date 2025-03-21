using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShortUrl.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedHashToUrlModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Hash",
                table: "Urls",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Hash",
                table: "Urls");
        }
    }
}
