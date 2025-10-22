namespace Zero.Api.Contracts.Form;

public class CreateFormFieldDto
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public int FormFieldTypeId { get; set; }
    public List<string>? FormFieldOptions { get; set; }
}