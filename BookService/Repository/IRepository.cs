using BooksService.Models;

namespace BooksService.Repository;

public interface IRepository
{
    public List<Book> GetBooks();
    public Book? GetBook(string title);
    public void AddBooks(List<Book> books);
    public void AddBook(Book book);
    public Task SaveBooksToXmlAsync(string filePath, CancellationToken cancellationToken = default);
    public Task<List<Book>> GetBooksFromXmlAsync(string filePath, CancellationToken cancellationToken = default);
}

