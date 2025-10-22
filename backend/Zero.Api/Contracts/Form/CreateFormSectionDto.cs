namespace Zero.Api.Contracts.Form;

public class CreateFormSectionDto
{
    public string Name { get; set; } = default!;
    public List<CreateFormFieldDto> Fields { get; set; } = new();
}