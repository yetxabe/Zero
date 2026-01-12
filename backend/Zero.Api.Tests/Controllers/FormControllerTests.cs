using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Zero.Api.Controllers;
using Zero.Api.Contracts.Form;
using Zero.Api.Data;
using Zero.Api.Models.Form;

namespace Zero.Api.Tests.Controllers
{
    public class FormControllerTests
    {
        private static AppDbContext CreateContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
            return new AppDbContext(options);
        }

        private static FormController CreateControllerWithUser(AppDbContext db, string userName = "tester")
        {
            var controller = new FormController(db);
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.Role, "Instalaciones")
            };
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"))
                }
            };
            return controller;
        }

        [Fact]
        public async Task CreateResponse_ReturnsNotFound_WhenFormDoesNotExist()
        {
            using var db = CreateContext(nameof(CreateResponse_ReturnsNotFound_WhenFormDoesNotExist));
            var controller = CreateControllerWithUser(db);

            var dto = new CreateFormResponseDto
            {
                Obra = "Obra X",
                Answers = new[]
                {
                    new CreateFormFieldResponseDto { FormFieldId = 1, Vaule = "v", FormFieldOptionId = null }
                }.ToList()
            };

            var result = await controller.CreateResponse(9999, dto);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task CreateResponse_ReturnsBadRequest_WhenFieldNotBelongToForm()
        {
            using var db = CreateContext(nameof(CreateResponse_ReturnsBadRequest_WhenFieldNotBelongToForm));

            // Seed required lookup data: category and field type
            var category = new Zero.Api.Models.Form.FormCategory { Name = "Cat1" };
            db.FormCategories.Add(category);
            var type = new Zero.Api.Models.Form.FormFieldType { Name = "Tipo" };
            db.FormFieldTypes.Add(type);
            await db.SaveChangesAsync();

            // Seed a form with one field (field belongs to the created form)
            var form1 = new Form { Name = "F1", CategoryId = category.Id };
            db.Forms.Add(form1);
            await db.SaveChangesAsync();

            var section = new FormSection { Name = "S1", FormId = form1.Id };
            db.FormSections.Add(section);
            await db.SaveChangesAsync();

            var field = new FormField { Name = "Field1", Description = "desc", FormFieldTypeId = type.Id, FormSectionId = section.Id };
            db.FormFields.Add(field);
            await db.SaveChangesAsync();

            var controller = CreateControllerWithUser(db);

            // Attempt to submit answer referencing a field id that does NOT belong to form1 (use id + 999)
            var dto = new CreateFormResponseDto
            {
                Obra = "Obra Y",
                Answers = new[]
                {
                    new CreateFormFieldResponseDto { FormFieldId = field.Id + 999, Vaule = "v", FormFieldOptionId = null }
                }.ToList()
            };

            var result = await controller.CreateResponse(form1.Id, dto);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task CreateResponse_CreatesResponse_WhenValid()
        {
            using var db = CreateContext(nameof(CreateResponse_CreatesResponse_WhenValid));

            // Seed required lookup data: category and field type
            var category2 = new Zero.Api.Models.Form.FormCategory { Name = "Cat2" };
            db.FormCategories.Add(category2);
            var type2 = new Zero.Api.Models.Form.FormFieldType { Name = "Tipo2" };
            db.FormFieldTypes.Add(type2);
            await db.SaveChangesAsync();

            // Seed form, section and field belonging to the form
            var form = new Form { Name = "Form OK", CategoryId = category2.Id };
            db.Forms.Add(form);
            await db.SaveChangesAsync();

            var section = new FormSection { Name = "Sec", FormId = form.Id };
            db.FormSections.Add(section);
            await db.SaveChangesAsync();

            var field = new FormField { Name = "Campo", Description = "desc", FormFieldTypeId = type2.Id, FormSectionId = section.Id };
            db.FormFields.Add(field);
            await db.SaveChangesAsync();

            var controller = CreateControllerWithUser(db, "usuario1");

            var dto = new CreateFormResponseDto
            {
                Obra = "Obra Z",
                Answers = new[]
                {
                    new CreateFormFieldResponseDto { FormFieldId = field.Id, Vaule = "respuesta", FormFieldOptionId = null }
                }.ToList()
            };

            var result = await controller.CreateResponse(form.Id, dto);

            var ok = Assert.IsType<OkObjectResult>(result);
            // verify DB state
            var savedResponse = db.FormResponses
                .Include(r => r.Fields)
                .FirstOrDefault();
            Assert.NotNull(savedResponse);
            Assert.Equal(form.Id, savedResponse.FormId);
            Assert.Equal("usuario1", savedResponse.CreatedBy);
            Assert.Equal("Obra Z", savedResponse.Obra);
            Assert.Single(savedResponse.Fields);
            Assert.Equal(field.Id, savedResponse.Fields.First().FormFieldId);
            Assert.Equal("respuesta", savedResponse.Fields.First().Value);
        }
    }
}
