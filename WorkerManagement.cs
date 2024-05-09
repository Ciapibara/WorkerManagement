using MySql.Data.MySqlClient;
using System.Xml.Linq;
using WorkerManagement.Functions;
using WorkerManagement.Models;

namespace WorkerManagement
{
  internal class WorkerRepository
  {
    private readonly string[] stanowisko = ["", "Administrator", "Rekruter", "Programista", "HR"];

    private readonly string connStr = "server=localhost;" +
                                      "user=root;" +
                                      "database=workgroup;" +
                                      "password=";

    private Worker? GetWorker(int id)
    {
      Worker? worker = null;
      using MySqlConnection conn = new(connStr);
      MySqlCommand cmd = new("SELECT workers.*, note.content, note.id_note " +
                             "FROM workers " +
                             "LEFT JOIN note ON workers.id_worker = note.id_worker " +
                             $"WHERE workers.id_worker = {id}", conn);

      conn.Open();

      using MySqlDataReader dr = cmd.ExecuteReader();

      while (dr.Read())
      {
        int id_worker = dr.GetInt32("id_worker");
        string name = dr.GetString("name");
        string surname = dr.GetString("surname");
        string login = dr.GetString("login");
        int id_role = dr.GetInt32("id_role");
        int age = dr.GetInt32("age");
        DateTime hire_date = dr.GetDateTime("hire_date");

        return new Worker
        {
          Id_worker = id_worker,
          Name = name,
          Surname = surname,
          Login = login,
          Id_role = id_role,
          Age = age,
          Hire_date = hire_date,
          Notes = GetNotes(id_worker)
        };
      }

      return worker;
    }

    private List<Note> GetNotes(int id_worker)
    {
      List<Note> workerNotes = [];

      using MySqlConnection conn = new MySqlConnection(connStr);
      MySqlCommand cmd = new MySqlCommand("SELECT id_note, content FROM note WHERE id_worker = @id_worker", conn);
      cmd.Parameters.AddWithValue("@id_worker", id_worker);

      conn.Open();

      using MySqlDataReader dr = cmd.ExecuteReader();
      while (dr.Read())
      {
        if (!dr.IsDBNull(dr.GetOrdinal("id_note")))
        {
          workerNotes.Add(new Note(dr.GetInt32("id_note"), dr.GetString("content")));
        }
      }

      return workerNotes;
    }

    private List<Worker> GetAllWorkers()
    {
      List<Worker> allWorkers = [];

      using MySqlConnection conn = new(connStr);
      MySqlCommand cmd = new("SELECT `id_worker`, `name`, `surname`, `id_role` " +
                             "FROM workers", conn);

      conn.Open();

      using MySqlDataReader dr = cmd.ExecuteReader();
      while (dr.Read())
      {
        int id_worker = dr.GetInt32("id_worker");
        string name = dr.GetString("name");
        string surname = dr.GetString("surname");
        int id_role = dr.GetInt32("id_role");

        allWorkers.Add(new Worker
        {
          Id_worker = id_worker,
          Name = name,
          Surname = surname,
          Id_role = id_role,
        });
      }

      return allWorkers;
    }

    internal void ShowWorker(int id)
    {
      Worker? worker = GetWorker(id);

      Console.WriteLine($"ID {worker.Id_worker}");
      Console.WriteLine($"{worker.Name} {worker.Surname} ({worker.Age})");
      Console.WriteLine($"Login: {worker.Login}");
      Console.WriteLine($"Zatrudniony: {worker.Hire_date.ToShortDateString()} na stanowisku {stanowisko[worker.Id_role]}");
      if (worker.Notes.Count > 0)
      {
        Console.WriteLine("Dodatkowe informacje:");
        worker.Notes.ForEach(note => Console.WriteLine($"{note.Id_note}. {note.Content}"));
      }
    }

    internal void ShowAllWorkers()
    {
      List<Worker> workers = GetAllWorkers();
      workers.ForEach(worker =>
      {
        Console.Write($"{worker.Id_worker}. ");
        Console.Write($"{worker.Name} {worker.Surname} | ");
        Console.Write(stanowisko[worker.Id_role]);
        Console.WriteLine();
      });
    }

    internal void AddNote(int id, string content)
    {
      ExecuteNonQuery("INSERT INTO `note`(`id_worker`, `content`) VALUES (@id, @content)",
                     ("@id", id),
                     ("@content", content));
    }

    internal void RemoveNote(int noteId)
    {
      ExecuteNonQuery("DELETE FROM `note` WHERE id_note = @noteId", ("@noteId", noteId));
    }

    internal void AddWorker(string name, string surname, string login, string password, int id_role, int age, DateTime hire_date)
    {
      ExecuteNonQuery("INSERT INTO `workers`(`name`, `surname`, `login`, `password`, `id_role`, `age`, `hire_date`, `is_working`) " +
                      "VALUES(@name, @surname, @login, @password, @id_role, @age, @hire_date, 1)",
                      ("@name", name),
                      ("@surname", surname),
                      ("@login", login),
                      ("@password", password),
                      ("@id_role", id_role),
                      ("@age", age),
                      ("@hire_date", hire_date.ToString("yyyy-MM-dd HH:mm:ss")));
    }

    internal void RemoveWorker(int id)
    {
      ExecuteNonQuery("DELETE FROM `workers` WHERE id_worker = @id", ("@id", id));
    }

    internal void CountAllPeople()
    {
      using MySqlConnection conn = new(connStr);
      MySqlCommand cmd = new("SELECT id_role, COUNT(*) AS count_people FROM workers GROUP BY id_role", conn);

      conn.Open();

      using MySqlDataReader dr = cmd.ExecuteReader();
      for (int i = 1; dr.Read(); i++)
      {
        Console.WriteLine($"{stanowisko[i]}: {dr.GetInt32("count_people")}");
      }
    }

    internal void RegeneratePassword(int id)
    {
      string password = PasswordGenerator.GeneratePassword(12);

      ExecuteNonQuery("UPDATE `workers` SET `password`=@password WHERE id_worker = @id",
                      ("@password", password),
                      ("@id", id));
    }

    private void ExecuteNonQuery(string query, params (string, object)[] parameters)
    {
      using MySqlConnection conn = new(connStr);
      conn.Open();
      using MySqlCommand cmd = new(query, conn);
      foreach (var (column, columnValue) in parameters)
      {
        cmd.Parameters.AddWithValue(column, columnValue);
      }
      cmd.ExecuteNonQuery();
    }
  }
}