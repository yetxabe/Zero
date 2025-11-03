namespace Zero.Api.Models.Form;

public class FormResponseItem
{
    public int Id { get; set; }
    public string? Value { get; set; }
    public string FormResponseId { get; set; }
    public FormResponse FormResponse { get; set; }
    public int FormFieldId { get; set; }
    public FormField FormField { get; set; }

    public ICollection<FormResponseItemOption> selectedOptions { get; set; } =
        new List<FormResponseItemOption>();
}