namespace Zero.Api.Contracts.Form;

public class CreateFormDto
{
    public string Name { get; set; } = default!;
    public int CategoryId { get; set; }
    public List<CreateFormSectionDto> Sections { get; set; }
}