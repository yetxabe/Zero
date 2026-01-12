namespace Zero.Api.Contracts.Form;

public class FormDetailsDto
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = default!;
    public List<FormSectionDetailsDto> Sections { get; set; } = new();
}