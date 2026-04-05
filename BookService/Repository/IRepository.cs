using BooksService.Models;

namespace BooksService.Repository;

public interface IRepository
{
    public List<Book> GetBooks();
    public Book? GetBook(string title);
    public void AddBooks(List<Book> books);
    public void AddBook(Book book);
    public void SaveBooksToXml(string filePath);
    public List<Book> GetBooksFromXml(string filePath);
}

