namespace Zero.Api.Models.Form;

public class FormResponse
{
    public string Id { get; set; }
    public int FormId { get; set; }
    public Form Form { get; set; }
    public string? Obra { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public ICollection<FormResponseItem> Items { get; set; }
}