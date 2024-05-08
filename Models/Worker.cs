namespace WorkerManagement.Models
{
  internal class Worker(int id, string name, string surname, string login, string password, int id_role, int age, DateTime hire_date, List<Note> notes)
  {
    public int Id_worker { get; } = id;
    public string Name { get; } = name;
    public string Surname { get; } = surname;
    public string Login { get; } = login;
    private string Password { get; } = password;
    public int Id_role { get; } = id_role;
    public int Age { get; } = age;
    public DateTime Hire_date { get; } = hire_date;
    public List<Note> Notes { get; set; } = notes.Count > 0 ? notes : [];
  }
}