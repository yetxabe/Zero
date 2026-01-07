namespace Zero.Api.Contracts.Form;

public class CreateFormResponseDto
{
    public string? Obra { get; set; }
    public List<CreateFormFieldResponseDto>? Answers { get; set; }
}