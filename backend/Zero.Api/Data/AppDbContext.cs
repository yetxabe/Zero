using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Zero.Api.Models.Auth;
using Zero.Api.Models.Form;

namespace Zero.Api.Data;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }
    
    public DbSet<FormCategory> FormCategories { get; set; }
    public DbSet<Form> Forms { get; set; }
    public DbSet<FormField> FormFields { get; set; }
    public DbSet<FormFieldOptions> FormFieldOptions { get; set; }
    public DbSet<FormFieldType> FormFieldTypes { get; set; }
    public DbSet<FormSection> FormSections { get; set; }
    
    public DbSet<FormResponse> FormResponses { get; set; }
    public DbSet<FormResponseField> FormResponseItems { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<FormResponse>(entity =>
        {
            entity.HasKey(fr => fr.Id);

            // EF ya genera GUID automáticamente si está por defecto, pero lo explicitamos
            entity.Property(fr => fr.Id)
                .ValueGeneratedOnAdd();

            entity.Property(fr => fr.CreatedBy)
                .IsRequired()
                .HasMaxLength(256);

            entity.Property(fr => fr.Obra)
                .HasMaxLength(10);

            entity.HasOne(fr => fr.Form)
                .WithMany() // si quieres navegar desde Form a Responses, crea ICollection<FormResponse> en Form
                .HasForeignKey(fr => fr.FormId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        builder.Entity<FormResponseField>(entity =>
        {
            entity.HasKey(frf => frf.Id);

            entity.HasOne(frf => frf.FormResponse)
                .WithMany(fr => fr.Fields)
                .HasForeignKey(frf => frf.FormResponseId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(frf => frf.FormField)
                .WithMany() // si quieres navegar desde FormField, añade ICollection<FormResponseField>
                .HasForeignKey(frf => frf.FormFieldId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(frf => frf.FormFieldOption)
                .WithMany()
                .HasForeignKey(frf => frf.FormFieldOptionId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        
        base.OnModelCreating(builder);

        // Esquema auth
        var schema = "Auth";

        builder.Entity<AppUser>().ToTable("Users", schema);
        builder.Entity<IdentityRole>().ToTable("Roles", schema);
        builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles", schema);
        builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims", schema);
        builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins", schema);
        builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims", schema);
        builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens", schema);

        schema = "Form";
        
        builder.Entity<FormCategory>().ToTable("Categories", schema);
        builder.Entity<Form>().ToTable("Forms", schema);
        builder.Entity<FormField>().ToTable("Fields", schema);
        builder.Entity<FormFieldOptions>().ToTable("FieldOptions", schema);
        builder.Entity<FormFieldType>().ToTable("FieldTypes", schema);
        builder.Entity<FormSection>().ToTable("Sections", schema);
        builder.Entity<FormResponse>().ToTable("Responses", schema);
        builder.Entity<FormResponseField>().ToTable("ResponseItems", schema);
    }
}