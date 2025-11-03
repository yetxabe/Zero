namespace Zero.Api.Models.Form;

public class FormResponseItemOption
{
    public int Id { get; set; }
    public int FormResponseItemId { get; set; }
    public FormResponseItem FormResponseItem { get; set; }
    public int FormFieldOptionId { get; set; }
    public FormFieldOptions FormFieldOption { get; set; }
}