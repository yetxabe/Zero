namespace Zero.Api.Contracts.Form;

public class FormCategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public int FormsCount { get; set; }
}