using WorkerManagement;

WorkerRepository wr = new();

StartMenu();

void StartMenu(string? errorMsg = null)
{
  Console.Clear();
  string[] options = ["Wyswietl pracownikow", "Wyswiet liczbe osob na stanowiskach", "Dodaj pracownika"];
  PrintMenuOptions(options, errorMsg, ConsoleColor.Red);
  string cStr = Console.ReadLine();

  switch (cStr)
  {
    default:
      StartMenu("Nie prawidłowa opcja");
      break;

    case "0":
      AllWorkerSubMenu();
      break;

    case "1":
      wr.CountAllPeople();
      Console.ReadKey();
      StartMenu();
      break;

    case "2":
      AddWorker();
      break;
  }
}
void AddWorker()
{
  string name = StringInput("Podaj imie", 24);
  string surname = StringInput("Podaj nazwisko", 80);
  string login = StringInput("Podaj login", 16);
  string password = StringInput("Podaj haslo", 30);

  Console.WriteLine("Podaj stanowisko");
  Console.WriteLine("1. Administrator, 2. Rekruter, 3. Programista 4. HR");
  if (!int.TryParse(Console.ReadLine(), out int id_role) && id_role <= 4 && id_role > 0)
    StartMenu("Nie prawidlowa liczba");

  Console.WriteLine("Podaj wiek");
  if (!int.TryParse(Console.ReadLine(), out int age))
    StartMenu("Nieprawidlowy wiek");

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
      StartMenu("Nieprawidłowa data.");
    }
  }
  wr.AddWorker(name, surname, login, password, id_role, age, hire_date);
}
string StringInput(string msg, int maxLenght)
{
  Console.WriteLine($"{msg} max dlugosc: {maxLenght}");
  string? s = Console.ReadLine();
  if (s.Length > maxLenght)
    StartMenu($"Przekroczono limit znakow ({maxLenght})");

  return s;
}

void AllWorkerSubMenu()
{
  Console.Clear();

  wr.ShowAllWorkers();

  string[] options = ["Podaj id pracownika"];
  PrintMenuOptions(options);

  if (int.TryParse(Console.ReadLine(), out int id))
  {
    wr.ShowWorker(id);
    WorkerSubMenu(id);
  }
  else
  {
    StartMenu("Nie prawidlowa cyfra");
  }
}
void WorkerSubMenu(int id)
{
  string[] options = ["Usuń", "Zrestartuj hasło", "Dodaj notatke", "Usuń notatke"];
  PrintMenuOptions(options);
  string cStr = Console.ReadLine();
  switch (cStr)
  {
    case "0":
      wr.RemoveWorker(id);
      StartMenu("Usunieto pracownika");
      break;

    case "1":
      wr.RegeneratePassword(id);
      StartMenu("Zresetowano haslo");
      break;

    case "2":
      AddNote(id);
      break;

    case "3":
      RemoveNote();
      break;
  }
}

void AddNote(int id)
{
  Console.WriteLine("Podaj zawartosc notatki");
  string content = Console.ReadLine();

  wr.AddNote(id, content);
  StartMenu("Dodano notatke");
}

void RemoveNote()
{
  Console.WriteLine("podaj id notatki");
  if (int.TryParse(Console.ReadLine(), out int noteId))
  {
    wr.RemoveNote(noteId);
    StartMenu("Usunieto notatke");
  }
  else
  {
    Console.WriteLine("Usuwanie notatki nie powiodlo sie;");
  }
}

void PrintMenuOptions(string[] options, string? errorMsg = null, ConsoleColor color = ConsoleColor.White)
{
  if (!string.IsNullOrEmpty(errorMsg))
  {
    Console.ForegroundColor = color;
    Console.WriteLine(errorMsg);
    Console.ResetColor();
  }
  for (int i = 0; i < options.Length; i++)
    Console.WriteLine($"{i}. {options[i]}");
}