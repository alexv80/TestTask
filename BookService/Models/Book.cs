namespace BooksService.Models;

public class Book
{
    private string _title;
    private string _author;
    private int _numberOfPages;

    public string Title 
    { 
        get => _title;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Title cannot be empty", nameof(Title));
            _title = value;
        }
    }

    public string Author 
    { 
        get => _author;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Author cannot be empty", nameof(Author));
            _author = value;
        }
    }

    public int NumberOfPages 
    { 
        get => _numberOfPages;
        set
        {
            if (value <= 0)
                throw new ArgumentException("Number of pages must be positive", nameof(NumberOfPages));
            _numberOfPages = value;
        }
    }

    public Book(string title, string author, int numberOfPages)
    {
        Title = title;
        Author = author;
        NumberOfPages = numberOfPages;
    }

    public Book() { }

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(_title))
            throw new InvalidOperationException("Title cannot be empty");
        if (string.IsNullOrWhiteSpace(_author))
            throw new InvalidOperationException("Author cannot be empty");
        if (_numberOfPages <= 0)
            throw new InvalidOperationException("Number of pages must be positive");
    }
}
