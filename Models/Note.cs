namespace WorkerManagement.Models
{
  internal class Note(int id_note, string content)
  {
    public int Id_note { get; } = id_note;
    public string Content { get; } = content;
  }
}