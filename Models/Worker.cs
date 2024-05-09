namespace WorkerManagement.Models
{
  internal class Worker
  {
    public int Id_worker { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? Login { get; set; }
    private string? Password { get; }
    public int Id_role { get; set; }
    public int Age { get; set; }
    public DateTime Hire_date { get; set; }
    public List<Note> Notes { get; set; } = [];
  }
}