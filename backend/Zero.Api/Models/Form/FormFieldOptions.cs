namespace Zero.Api.Models.Form;

public class FormFieldOptions
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int FormFieldId { get; set; }
    public FormField FormField { get; set; }
}