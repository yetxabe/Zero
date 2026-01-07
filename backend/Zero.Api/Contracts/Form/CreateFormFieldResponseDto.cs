namespace Zero.Api.Contracts.Form;

public class CreateFormFieldResponseDto
{
    public int FormFieldId { get; set; }
    public string ? Vaule { get; set; }
    public int? FormFieldOptionId { get; set; }
}