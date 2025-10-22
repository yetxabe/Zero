using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zero.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddFormsDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Form");

            migrationBuilder.CreateTable(
                name: "Categories",
                schema: "Form",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FieldTypes",
                schema: "Form",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Forms",
                schema: "Form",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Forms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Forms_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "Form",
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Fields",
                schema: "Form",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FormId = table.Column<int>(type: "int", nullable: false),
                    FormFieldTypeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Fields_FieldTypes_FormFieldTypeId",
                        column: x => x.FormFieldTypeId,
                        principalSchema: "Form",
                        principalTable: "FieldTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Fields_Forms_FormId",
                        column: x => x.FormId,
                        principalSchema: "Form",
                        principalTable: "Forms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FieldOptions",
                schema: "Form",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FormFieldId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FieldOptions_Fields_FormFieldId",
                        column: x => x.FormFieldId,
                        principalSchema: "Form",
                        principalTable: "Fields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FieldOptions_FormFieldId",
                schema: "Form",
                table: "FieldOptions",
                column: "FormFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_Fields_FormFieldTypeId",
                schema: "Form",
                table: "Fields",
                column: "FormFieldTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Fields_FormId",
                schema: "Form",
                table: "Fields",
                column: "FormId");

            migrationBuilder.CreateIndex(
                name: "IX_Forms_CategoryId",
                schema: "Form",
                table: "Forms",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FieldOptions",
                schema: "Form");

            migrationBuilder.DropTable(
                name: "Fields",
                schema: "Form");

            migrationBuilder.DropTable(
                name: "FieldTypes",
                schema: "Form");

            migrationBuilder.DropTable(
                name: "Forms",
                schema: "Form");

            migrationBuilder.DropTable(
                name: "Categories",
                schema: "Form");
        }
    }
}
