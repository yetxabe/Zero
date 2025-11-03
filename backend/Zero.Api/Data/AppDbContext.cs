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
    public DbSet<FormResponseItem> FormResponseItems { get; set; }
    public DbSet<FormResponseItemOption> FormResponseItemOptions { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<FormResponse>()
            .HasOne(r => r.Form)
            .WithMany()
            .HasForeignKey(r => r.FormId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Entity<FormResponseItem>()
            .HasOne(r => r.FormResponse)
            .WithMany(r => r.Items)
            .HasForeignKey(r => r.FormResponseId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Entity<FormResponseItem>()
            .HasOne(r => r.FormField)
            .WithMany()
            .HasForeignKey(r => r.FormFieldId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Entity<FormResponseItemOption>()
            .HasOne(o => o.FormResponseItem)
            .WithMany(i => i.selectedOptions)
            .HasForeignKey(o => o.FormResponseItemId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Entity<FormResponseItemOption>()
            .HasOne(o => o.FormFieldOption)
            .WithMany() // no necesitas navegación inversa
            .HasForeignKey(o => o.FormFieldOptionId)
            .OnDelete(DeleteBehavior.Restrict);
        
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
        builder.Entity<FormResponseItem>().ToTable("ResponseItems", schema);
        builder.Entity<FormResponseItemOption>().ToTable("ResponseItemOptions", schema);
        
    }
}