using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OJudge.Migrations
{
    /// <inheritdoc />
    public partial class Init3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProblemPage_Problems_ProblemId",
                table: "ProblemPage");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProblemPage",
                table: "ProblemPage");

            migrationBuilder.RenameTable(
                name: "ProblemPage",
                newName: "ProblemPages");

            migrationBuilder.RenameIndex(
                name: "IX_ProblemPage_ProblemId",
                table: "ProblemPages",
                newName: "IX_ProblemPages_ProblemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProblemPages",
                table: "ProblemPages",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Topics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Topics", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_ProblemPages_Problems_ProblemId",
                table: "ProblemPages",
                column: "ProblemId",
                principalTable: "Problems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProblemPages_Problems_ProblemId",
                table: "ProblemPages");

            migrationBuilder.DropTable(
                name: "Topics");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProblemPages",
                table: "ProblemPages");

            migrationBuilder.RenameTable(
                name: "ProblemPages",
                newName: "ProblemPage");

            migrationBuilder.RenameIndex(
                name: "IX_ProblemPages_ProblemId",
                table: "ProblemPage",
                newName: "IX_ProblemPage_ProblemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProblemPage",
                table: "ProblemPage",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProblemPage_Problems_ProblemId",
                table: "ProblemPage",
                column: "ProblemId",
                principalTable: "Problems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
