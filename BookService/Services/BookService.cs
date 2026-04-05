using BooksService.Models;
using BooksService.Repository;

namespace BooksService.Services;

public class BookService
{
    private readonly IRepository _repository;

    public BookService(IRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }
    
    public List<Book> GetBooks()
    {
        return _repository.GetBooks();
    }

    public Book? GetBookByTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return null;
        return _repository.GetBook(title);
    }

    public async Task<List<Book>> GetBooksFromXmlAsync(string filePath, CancellationToken token = default)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be empty", nameof(filePath));

        return await _repository.GetBooksFromXmlAsync(filePath, token);
    }

    public void AddBooks(List<Book> books)
    {
        if (books == null || !books.Any())
            throw new ArgumentNullException(nameof(books));

        _repository.AddBooks(books);
    }

    public void AddBook(Book book)
    {
        _repository.AddBook(book);
    }

    public async Task SaveAllBooksToXmlAsync(string filePath, CancellationToken token = default)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be empty", nameof(filePath));

        await _repository.SaveBooksToXmlAsync(filePath, token);
    }

    public List<Book> SortBooks()
    {
        var books = _repository.GetBooks();

        if (books.Any())
        {
            return books.OrderBy(b => b.Author).ThenBy(b => b.Title).ToList();
        }
        return new List<Book>();
    }
}
