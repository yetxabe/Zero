namespace Zero.Api.Contracts.Form;

public class FormSectionDetailsDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<FormFieldDetailsDto> Fields { get; set; } = new();
}