namespace Zero.Api.Contracts.Form;

public class FormFieldDetailsDto
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public int FormFieldTypeId { get; set; }
    public string FormFieldTypeName { get; set; } = default!;
    public List<string> Options { get; set; } = new();
}