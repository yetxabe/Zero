namespace Zero.Api.Models.Form;

public class FormSection
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int FormId { get; set; }
    public Form Form { get; set; }
    public ICollection<FormField> Fields { get; set; }
}