using BooksService.Models;

namespace BooksServiceTests.Repository;

[TestFixture]
public class RepositoryTests
{
    private string _testXmlPath;

    [SetUp]
    public void SetUp()
    {
        _testXmlPath = Path.Combine(Path.GetTempPath(), $"test_books_{Guid.NewGuid()}.xml");
    }

    [TearDown]
    public void TearDown()
    {
        if (File.Exists(_testXmlPath))
        {
            File.Delete(_testXmlPath);
        }
    }

    [Test]
    public void AddBook_ValidBook_AddsSuccessfully()
    {
        // Arrange
        var book = new Book("The Hobbit", "Tolkien", 310);
        var sut = new BooksService.Repository.Repository();

        // Act
        sut.AddBook(book);

        // Assert
        var books = sut.GetBooks();
        Assert.That(books, Has.Count.EqualTo(1));
        Assert.IsTrue(AreBooksEqual(book, books[0]));
    }

    [Test]
    public void AddBook_NullBook_ThrowsArgumentNullException()
    {
        // Arrange
        var sut = new BooksService.Repository.Repository();


        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => sut.AddBook(null!));
    }

    [Test]
    public void AddBook_InvalidBook_ThrowsInvalidOperationException()
    {
        // Arrange
        var book1 = new Book { Author = "Tolkien", Title = "The Hobbit" };
        var book2 = new Book { Title = "The Little Mermaid", NumberOfPages = 60 };
        var book3 = new Book { Title = "Carrie", Author = "King" };
        var book4 = new Book {  };
        var books = new List<Book>() {book1, book2, book3, book4};
        var sut = new BooksService.Repository.Repository();

        // Act && Assert
        foreach(var book in books) 
            Assert.Throws<InvalidOperationException>(() => sut.AddBook(book));
    }

    [Test]
    public void AddBooks_ValidList_AddsAllBooks()
    {
        // Arrange
        var books = new List<Book>
        {
            new("Book 1", "Author 1", 100),
            new("Book 2", "Author 2", 200)
        };
        var sut = new BooksService.Repository.Repository();

        // Act
        sut.AddBooks(books);
        var result = sut.GetBooks();

        // Assert
        Assert.That(result, Has.Count.EqualTo(2));
        foreach (var book in result)
        {
            Assert.IsTrue(AreBooksEqual(books.First(b => b.Title == book.Title), book));
        }
    }

    [Test]
    public void AddBooks_NullList_ThrowsArgumentNullException()
    {
        // Arrange
        var sut = new BooksService.Repository.Repository();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => sut.AddBooks(null!));
    }

    [Test]
    public void AddBooks_EmptyList_DoesNotThrow()
    {
        // Arrange
        var books = new List<Book>();
        var sut = new BooksService.Repository.Repository();

        // Act & Assert
        Assert.DoesNotThrow(() => sut.AddBooks(books));
        Assert.That(sut.GetBooks(), Is.Empty);
    }

    [Test]
    public void GetBooks_EmptyRepository_ReturnsEmptyList()
    {
        // Arrange
        var sut = new BooksService.Repository.Repository();

        // Act
        var books = sut.GetBooks();

        // Assert
        Assert.That(books, Is.Empty);
    }

    [Test]
    public void GetBooks_AddNewBook_RepositoryUpdated()
    {
        // Arrange
        var sut = new BooksService.Repository.Repository();
        var book1 = new Book("book1", "Author", 150);
        sut.AddBook(book1);

        // Act and Assert
        var books1 = sut.GetBooks();
        Assert.That(books1, Has.Count.EqualTo(1));
        Assert.IsTrue(AreBooksEqual(books1[0], book1));
        var book2 = new Book("book2", "Author2", 200);
        sut.AddBook(book2);
        var books2 = sut.GetBooks();
        Assert.That(books2, Has.Count.EqualTo(2));
        foreach(var book in books2)
            Assert.IsTrue(AreBooksEqual(book.Title == "book1" ? book1 : book2, book));
    }

    [Test]
    public void GetBook_ExactMatch_ReturnsBook()
    {
        // Arrange
        var sut = new BooksService.Repository.Repository();
        var expectedBook = new Book("The Hobbit", "Tolkien", 310);
        sut.AddBook(expectedBook);

        // Act
        var book = sut.GetBook("The Hobbit");

        // Assert
        Assert.That(book, Is.Not.Null);
        Assert.IsTrue(AreBooksEqual(expectedBook, book));
    }

    [TestCase("little")]
    [TestCase("little mermaid")]
    [TestCase("the lit")]
    [TestCase("LITTLE")]
    [TestCase("LITTLE MERMAID")]
    [TestCase("THE LIT")]
    public void GetBook_PartialMatch_ReturnsBook(string search)
    {
        // Arrange
        var sut = new BooksService.Repository.Repository();
        var expectedBook = new Book("The Little Mermaid", "Andersen", 60);
        sut.AddBook(expectedBook);

        // Act
        var book = sut.GetBook(search);

        // Assert
        Assert.That(book, Is.Not.Null);
        Assert.IsTrue(AreBooksEqual(expectedBook, book));
    }

    [Test]
    public void GetBook_NotFound_ReturnsNull()
    {
        // Arrange
        var sut = new BooksService.Repository.Repository();
        sut.AddBook(new Book("The Hobbit", "Tolkien", 310));

        // Act
        var book = sut.GetBook("NonExistent");

        // Assert
        Assert.That(book, Is.Null);
    }

    [Test]
    public void GetBook_EmptyString_ReturnsNull()
    {
        // Arrange
        var sut = new BooksService.Repository.Repository();

        // Act
        var book = sut.GetBook("");

        // Assert
        Assert.That(book, Is.Null);
    }

    [Test]
    public async Task SaveBooksToXml_ValidBooks_CreatesFile()
    {
        // Arrange
        var sut = new BooksService.Repository.Repository();
        var book1 = new Book("The Hobbit", "Tolkien", 310);
        var book2 = new Book("The Little Mermaid", "Andersen", 60);
        var book3 = new Book("The Lord of the Rings", "Tolkien", 1178);
        var books = new List<Book>(){book1, book2, book3};
        sut.AddBooks(books);

        // Act
        await sut.SaveBooksToXmlAsync(_testXmlPath);

        // Assert
        Assert.That(File.Exists(_testXmlPath), Is.True);
        var actualBooks = await sut.GetBooksFromXmlAsync(_testXmlPath);
        Assert.That(actualBooks, Has.Count.EqualTo(3));
        foreach (var book in books)
        {
            Assert.IsTrue(AreBooksEqual(book, actualBooks.FirstOrDefault(a => a.Title == book.Title)));
        }
    }

    [Test]
    public void SaveBooksToXml_EmptyPath_ThrowsArgumentException()
    {
        // Arrange
        var sut = new BooksService.Repository.Repository();

        // Act & Assert
        Assert.ThrowsAsync<ArgumentException>(() => sut.SaveBooksToXmlAsync(""));
    }

    [Test]
    public async Task GetBooksFromXml_ValidFile_LoadsBooks()
    {
        // Arrange
        var sut = new BooksService.Repository.Repository();
        var book1 = new Book("The Hobbit", "Tolkien", 310);
        var book2 = new Book("Carrie", "King", 400);
        var expectedBooks = new List<Book>() {book1, book2};
        sut.AddBooks(expectedBooks);
        await sut.SaveBooksToXmlAsync(_testXmlPath);

        // Act
        var newRepo = new BooksService.Repository.Repository();
        var books = await newRepo.GetBooksFromXmlAsync(_testXmlPath);

        // Assert
        Assert.That(books, Has.Count.EqualTo(2));
        foreach(var book in expectedBooks)
        {
            Assert.IsTrue(AreBooksEqual(book, books.FirstOrDefault(b => b.Title == book.Title)));
        }
    }

    [Test]
    public void GetBooksFromXml_NonExistentFile_ThrowsFileNotFoundException()
    {
        // Arrange
        var sut = new BooksService.Repository.Repository();

        // Act & Assert
        Assert.ThrowsAsync<FileNotFoundException>(() =>
            sut.GetBooksFromXmlAsync("nonexistent.xml"));
    }

    [Test]
    public void GetBooksFromXml_EmptyPath_ThrowsArgumentException()
    {
        // Arrange
        var sut = new BooksService.Repository.Repository();

        // Act & Assert
        Assert.ThrowsAsync<ArgumentException>(() => sut.GetBooksFromXmlAsync(""));
    }

    [Test]
    public void GetBooksFromXml_InvalidXml_ThrowsInvalidDataException()
    {
        // Arrange
        File.WriteAllText(_testXmlPath, "<Invalid>XML</Content>");
        var sut = new BooksService.Repository.Repository();

        // Act & Assert
        Assert.ThrowsAsync<InvalidDataException>(() =>
            sut.GetBooksFromXmlAsync(_testXmlPath));
    }
    
    private bool AreBooksEqual(Book expected, Book? actual)
    {
        if(actual == null) return false;
        return expected.Title == actual.Title &&
               expected.Author == actual.Author &&
               expected.NumberOfPages == actual.NumberOfPages;
    }
}