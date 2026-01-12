namespace Zero.Api.Models.Form;

public class FormResponse
{
    //ID GUID
    public Guid Id { get; set; }
    
    public int FormId { get; set; }
    public Form Form { get; set; }
    
    public string CreatedBy { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    
    public string? Obra { get; set; }
    public ICollection<FormResponseField> Fields { get; set; } = new List<FormResponseField>();
}