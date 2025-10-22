namespace Zero.Api.Models.Form;

public class FormCategory
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<Form> Forms { get; set; }
}