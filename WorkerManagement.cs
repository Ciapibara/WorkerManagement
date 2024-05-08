using MySql.Data.MySqlClient;
using WorkerManagement.Functions;
using WorkerManagement.Models;

// SHOWING JOBS AND TOTAL COUNT OF PEOPLE IN THEM
// SELECT id_role, COUNT(*) AS count_people FROM workers GROUP BY id_role;
namespace WorkerManagement
{
  internal class WorkerRepository
  {
    private readonly string[] stanowisko = ["", "Administrator", "Rekruter", "Programista", "HR"];

    private readonly string connStr = "server=localhost;" +
                                      "user=root;" +
                                      "database=workgroup;" +
                                      "password=";

    private static Worker MapWorker(MySqlDataReader dr)
    {
      List<Note> workerNotes = [];
      while (dr.Read())
      {
        if (!string.IsNullOrEmpty(dr["content"].ToString()))
          workerNotes.Add(new Note(Convert.ToInt32(dr["id_note"]), dr["content"].ToString()));
      }

      return new Worker(
        id: dr.GetInt32("id_worker"),
        name: dr.GetString("name"),
        surname: dr.GetString("surname"),
        login: dr.GetString("login"),
        password: dr.GetString("password"),
        id_role: dr.GetInt32("id_role"),
        age: dr.GetInt32("age"),
        hire_date: dr.GetDateTime("hire_date"),
        notes: workerNotes
        );
    }

    internal Worker? GetWorker(int id)
    {
      using MySqlConnection conn = new(connStr);
      MySqlCommand cmd = new("SELECT workers.*, note.content, note.id_note " +
                             "FROM workers " +
                             "LEFT JOIN note ON workers.id_worker = note.id_worker " +
                             $"WHERE workers.id_worker = {id}", conn);

      conn.Open();

      using MySqlDataReader dr = cmd.ExecuteReader();

      return MapWorker(dr);
    }

    internal List<Worker> GetAllWorkers()
    {
      List<Worker> allWorkers = [];

      using MySqlConnection conn = new(connStr);
      MySqlCommand cmd = new("SELECT workers.*, note.content, note.id_note " +
                             "FROM workers " +
                             "LEFT JOIN note ON workers.id_worker = note.id_worker", conn);

      conn.Open();

      using MySqlDataReader dr = cmd.ExecuteReader();
      while (dr.Read())
        allWorkers.Add(MapWorker(dr));

      return allWorkers;
    }

    internal void ShowWorker(int id)
    {
      Worker? worker = GetWorker(id);

      Console.WriteLine($"ID {worker.Id_worker}");
      Console.WriteLine($"{worker.Name} {worker.Surname} ({worker.Age})");
      Console.WriteLine($"Login: {worker.Login}");
      Console.WriteLine($"Zatrudniony: {worker.Hire_date.ToShortDateString()} na stanowisku {stanowisko[worker.Id_role]}");
      if (worker.Notes.Count != 0)
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

    internal void RemoveNote(int noteId) =>
      ExecuteNonQuery("DELETE FROM `note` WHERE id_note = @noteId}", ("@noteId", noteId));

    internal void AddWorker(string? msg = null)
    {
      if (string.IsNullOrEmpty(msg))
        Console.WriteLine(msg);

      Console.WriteLine("Podaj imie max. 24 znaki");
      string name = Console.ReadLine();
      if (name.Length > 24)
        AddWorker("Imie powinno miec max 24. znakow");

      Console.WriteLine("Podaj nazwisko max. 80 znakow");
      string surname = Console.ReadLine();
      if (surname.Length > 16)
        AddWorker("Nazwisko powinno miec max 80. znakow");

      Console.WriteLine("Podaj login max. 16 znakow");
      string login = Console.ReadLine();
      if (login.Length > 16)
        AddWorker("Login powinien miec max 16. znakow");

      Console.WriteLine("Podaj haslo max. 30 znakow");
      string password = Console.ReadLine();
      if (login.Length > 16)
        AddWorker("Haslo powinno miec max 30. znakow");

      Console.WriteLine("Podaj stanowisko");
      Console.WriteLine("1. Administrator, 2. Rekruter, 3. Programista 4. HR");
      if (!int.TryParse(Console.ReadLine(), out int id_role) && id_role <= 4 && id_role > 0)
        AddWorker("Nie prawidlowa liczba");

      Console.WriteLine("Podaj wiek");
      if (!int.TryParse(Console.ReadLine(), out int age))
        AddWorker("Nieprawidlowy wiek");

      Console.WriteLine("Podaj date zatrudnienia (np 2024-05-26), kliknij enter, żeby ustawić teraźniejszą.");
      string hire_dateString = Console.ReadLine();
      DateTime hire_date;

      if (string.IsNullOrEmpty(hire_dateString))
      {
        hire_date = DateTime.Now;
      }
      else
      {
        if (!DateTime.TryParse(hire_dateString, out hire_date))
        {
          Console.WriteLine("Nieprawidłowa data.");
          return;
        }
      }

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

    internal void RemoveWorker(int id) =>
      ExecuteNonQuery("DELETE FROM `workers` WHERE id_worker = @id", ("@id", id));

    internal void RegeneratePassword(int id)
    {
      string password = PasswordGenerator.GeneratePassword(12);

      ExecuteNonQuery("UPDATE `workers` SET `password`=@password WHERE id_worker = @id",
                      ("@password", password),
                      ("@id", id));
    }

    private void ExecuteNonQuery(string query, params (string, object)[] parameters)
    {
      using MySqlConnection conn = new MySqlConnection(connStr);
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