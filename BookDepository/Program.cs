using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace BookDepository
{
    class Program
    {
        static void Main(string[] args)
        {
            Librarian librarian = new Librarian();
            librarian.doJob();
        }
    }

    class Librarian
    {
        Depository depository;

        private const string FileName = "books";

        public Librarian()
        {
            depository = new Depository();

            depository.LoadBooks("books");
        }

        public void doJob()
        {
            bool doService = true;

            while (doService)
            {
                Console.WriteLine($"------------------------------------");
                Console.WriteLine($"Добро пожаловать в книжное хранилище");
                Console.WriteLine($"------------------------------------");
                ShowMainMenu();

                Console.Write($"\nВыберите действие: ");
                string userInput = Console.ReadLine();
                Console.Clear();

                switch (userInput)
                {
                    case "1":
                        SortByTitleSubMenu();
                        break;
                    case "2":
                        SortByAuthorSubMenu();
                        break;
                    case "3":
                        SortByYearSubMenu();
                        break;
                    case "4":
                        AddBookSubMenu();
                        break;
                    case "5":
                        DeleteBookSubMenu();
                        break;
                    case "6":
                        depository.ShowInfo();
                        break;
                    case "9":
                        doService = false;
                        break;
                    default:
                        break;
                }
            }
        }

        private void ShowMainMenu()
        {
            Console.WriteLine();
            Console.WriteLine($"1 - список книг по названию");
            Console.WriteLine($"2 - список книг по автору");
            Console.WriteLine($"3 - список книг по годам");
            Console.WriteLine($"4 - добавить книгу");
            Console.WriteLine($"5 - удалить книгу");
            Console.WriteLine($"6 - показать все книги");
            Console.WriteLine($"9 - выход");
        }

        private void SortByTitleSubMenu()
        {
            Console.WriteLine($"СОРТИРОВКА ПО НАЗВАНИЮ\n");
            Console.WriteLine($"1 - по возрастанию\n9 - по убыванию\nслово - фильтрация по слову/части слова");
            string title = Console.ReadLine();

            depository.SortByTitle(title);
        }

        private void SortByAuthorSubMenu()
        {
            Console.WriteLine($"СОРТИРОВКА ПО АВТОРУ\n");
            Console.WriteLine($"1 - по возрастанию\n9 - по убыванию\nслово - фильтрация по слову/части слова");
            string author = Console.ReadLine();

            depository.SortByAuthor(author);
        }

        private void SortByYearSubMenu()
        {
            Console.WriteLine($"СОРТИРОВКА ПО ГОДАМ\n");
            Console.WriteLine($"1 - по возрастанию\n9 - по убыванию\nточное число - фильтрация по году");
            int.TryParse(Console.ReadLine(), out int number);

            depository.SortByYear(number);
        }

        private void AddBookSubMenu()
        {
            Console.WriteLine($"ДОБАВЛЕНИЕ КНИГИ\n");
            Console.WriteLine($"Введите название:");
            string title = Console.ReadLine();

            Console.WriteLine($"Введите автора:");
            string author = Console.ReadLine();

            Console.WriteLine($"Введите год выпуска:");
            int.TryParse(Console.ReadLine(), out int year);

            depository.AddBook(author, title, year);
            depository.SaveBooks(FileName);
        }

        private void DeleteBookSubMenu()
        {
            Console.WriteLine($"УДАЛЕНИЕ КНИГИ\n");
            Console.Write($"Введите номер книги для удаления: ");
            int.TryParse(Console.ReadLine(), out int number);

            if (number > 0 && number <= depository.Books.Count)
            {
                int index = number - 1;
                Console.Write($"удалена книга ");
                depository.Books[index].ToDisplay();

                depository.DeleteBook(index);
            }
            else
            {
                Console.WriteLine($"несоответствие номера книги");
            }

            depository.SaveBooks(FileName);
        }
    }

    class Depository
    {
        private List<Book> _books;

        public IReadOnlyList<Book> Books => _books;

        public Depository()
        {
            _books = new List<Book>();
        }

        public void ShowInfo()
        {
            for (int i = 0; i < _books.Count; i++)
            {
                Console.Write($"{i + 1}. ");
                _books[i].ToDisplay();
            }

            Console.WriteLine();
        }

        public void LoadBooks(string filename)
        {
            _books.Clear();

            string[] booksArray = File.ReadAllLines($"Depository/{filename}.txt");

            for (int i = 0; i < booksArray.Length; i++)
            {
                string line = booksArray[i];
                string[] split = line.Split(';');

                string title = split[0];
                string author = split[1];
                int.TryParse(split[2], out int year);

                _books.Add(new Book(title, author, year));
            }
        }

        public void SaveBooks(string filename)
        {
            int length = _books.Count;
            string[] booksForSave = new string[length];

            for (int i = 0; i < booksForSave.Length; i++)
            {
                string line = $"{_books[i].Title};{_books[i].Author};{_books[i].Published}";

                booksForSave[i] = line;
            }

            File.WriteAllLines($"Depository/{filename}.txt", booksForSave);
        }

        public void AddBook(string author, string title, int year)
        {
            author = ReplaceSymbol(author);
            title = ReplaceSymbol(title);

            _books.Add(new Book(title, author, year));
        }

        public void DeleteBook(int index)
        {
            _books.RemoveAt(index);
        }

        public void SortByTitle(string text)
        {
            var filteredTitle = _books.Where(book => book.Title.ToLower().Contains(text.ToLower()));

            switch (text)
            {
                case "1":
                    filteredTitle = _books.OrderBy(book => book.Title);
                    break;
                case "9":
                    filteredTitle = _books.OrderByDescending(book => book.Title);
                    break;
            }

            foreach (Book book in filteredTitle)
            {
                book.ToDisplay();
            }
        }

        public void SortByAuthor(string text)
        {
            var filteredAuthor = _books.Where(author => author.Author.ToLower().Contains(text.ToLower()));

            switch (text)
            {
                case "1":
                    filteredAuthor = _books.OrderBy(author => author.Author);
                    break;
                case "9":
                    filteredAuthor = _books.OrderByDescending(author => author.Author);
                    break;
            }

            foreach (Book author in filteredAuthor)
            {
                author.ToDisplay();
            }
        }

        public void SortByYear(int number)
        {
            var filteredYear = _books.Where(year => year.Published.Equals(number));

            switch (number)
            {
                case 1:
                    filteredYear = _books.OrderBy(year => year.Published);
                    break;
                case 9:
                    filteredYear = _books.OrderByDescending(year => year.Published);
                    break;
            }

            foreach (Book year in filteredYear)
            {
                year.ToDisplay();
            }
        }

        private string ReplaceSymbol(string line)
        {
            char oldChar = ';';
            char newChar = '.';

            line.Replace(oldChar, newChar);            

            return line;
        }
    }

    class Book
    {
        public string Title { get; private set; }
        public string Author { get; private set; }
        public int Published { get; private set; }

        public Book(string title, string author, int year)
        {
            Title = title;
            Author = author;
            Published = year;
        }

        public void ToDisplay()
        {
            Console.WriteLine($"{Author}, {Title}, {Published}");
        }
    }
}
