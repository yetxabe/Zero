namespace Zero.Api.Models.Form;

public class FormResponseField
{
    public int Id { get; set; }
    public Guid FormResponseId { get; set; }
    public FormResponse FormResponse { get; set; }
    public int FormFieldId { get; set; }
    public FormField FormField { get; set; }
    public string? Value { get; set; }
    public int? FormFieldOptionId { get; set; }
    public FormFieldOptions? FormFieldOption { get; set; }
}