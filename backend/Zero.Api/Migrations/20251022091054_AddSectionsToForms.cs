using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zero.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddSectionsToForms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Fields_Forms_FormId",
                schema: "Form",
                table: "Fields");

            migrationBuilder.RenameColumn(
                name: "FormId",
                schema: "Form",
                table: "Fields",
                newName: "FormSectionId");

            migrationBuilder.RenameIndex(
                name: "IX_Fields_FormId",
                schema: "Form",
                table: "Fields",
                newName: "IX_Fields_FormSectionId");

            migrationBuilder.CreateTable(
                name: "FormSection",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FormId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormSection", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FormSection_Forms_FormId",
                        column: x => x.FormId,
                        principalSchema: "Form",
                        principalTable: "Forms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FormSection_FormId",
                table: "FormSection",
                column: "FormId");

            migrationBuilder.AddForeignKey(
                name: "FK_Fields_FormSection_FormSectionId",
                schema: "Form",
                table: "Fields",
                column: "FormSectionId",
                principalTable: "FormSection",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Fields_FormSection_FormSectionId",
                schema: "Form",
                table: "Fields");

            migrationBuilder.DropTable(
                name: "FormSection");

            migrationBuilder.RenameColumn(
                name: "FormSectionId",
                schema: "Form",
                table: "Fields",
                newName: "FormId");

            migrationBuilder.RenameIndex(
                name: "IX_Fields_FormSectionId",
                schema: "Form",
                table: "Fields",
                newName: "IX_Fields_FormId");

            migrationBuilder.AddForeignKey(
                name: "FK_Fields_Forms_FormId",
                schema: "Form",
                table: "Fields",
                column: "FormId",
                principalSchema: "Form",
                principalTable: "Forms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
