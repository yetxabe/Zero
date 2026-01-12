namespace Zero.Api.Models.Form;

public class FormField
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int FormSectionId { get; set; }
    public FormSection FormSection { get; set; }
    public int FormFieldTypeId { get; set; }
    public FormFieldType FormFieldType { get; set; }
    public ICollection<FormFieldOptions>? FormFieldOptions { get; set; }
}