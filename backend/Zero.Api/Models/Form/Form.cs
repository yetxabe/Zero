namespace Zero.Api.Models.Form;

public class Form
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int CategoryId { get; set; }
    public FormCategory Category { get; set; }
    public ICollection<FormSection> Sections { get; set; }
}