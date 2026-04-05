using BooksService.Models;
using System.Xml.Serialization;

namespace BooksService.Repository;

internal class Repository : IRepository
{
    private readonly List<Book> _books = new List<Book>();

    public List<Book> GetBooks()
    {
        return new List<Book>(_books);
    }

    public Book? GetBook(string title)
    {
        return _books.FirstOrDefault(b => b.Title.Contains(title, StringComparison.OrdinalIgnoreCase));
    }

    public List<Book> GetBooksFromXml(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be empty", nameof(filePath));

        if (!File.Exists(filePath))
            throw new FileNotFoundException("XML file not found", filePath);

        var serializer = new XmlSerializer(typeof(List<Book>));
        using var reader = new StreamReader(filePath);
        try
        {
            var books = (List<Book>) serializer.Deserialize(reader)!;
            var validBooks = new List<Book>();

            foreach (var book in books)
            {
                try
                {
                    book.Validate();
                    validBooks.Add(book);
                }
                catch (InvalidOperationException)
                {
                    Console.WriteLine("Skipping invalid book: {title}, {author}, {number}", book.Title, book.Author, book.NumberOfPages);
                }
            }

            return validBooks;
        }
        catch (InvalidOperationException ex)
        {
            throw new InvalidDataException("Invalid XML format or content", ex);
        }
    }

    public void AddBooks(List<Book> books)
    {
        if (books == null)
            throw new ArgumentNullException(nameof(books));
        
        foreach (var book in books)
        {
            AddBook(book);
        }
    }

    public void AddBook(Book book)
    {
        if (book == null)
            throw new ArgumentNullException(nameof(book));

        try
        {
            book.Validate();
            _books.Add(book);
        }
        catch (InvalidOperationException e)
        {
            throw new InvalidOperationException("Book does not pass validation!");
        }
    }

    public void SaveBooksToXml(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be empty", nameof(filePath));

        var serializer = new XmlSerializer(typeof(List<Book>));
        using var writer = new StreamWriter(filePath);
        serializer.Serialize(writer, _books);
    }
}

