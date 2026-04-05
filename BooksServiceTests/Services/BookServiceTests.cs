using BooksService.Models;
using BooksService.Repository;
using BooksService.Services;
using Moq;

namespace BooksServiceTests.Services;

[TestFixture]
public class BookServiceTests
{
    private Mock<IRepository> _mockRepository;

    [SetUp]
    public void SetUp()
    {
        _mockRepository = new Mock<IRepository>();
    }

    [Test]
    public void ctor_NullRepository_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
        {
            new BookService(null!);
        });
    }

    [Test]
    public void GetBooks_CallsRepository_ReturnsBooks()
    {
        // Arrange
        var expectedBooks = new List<Book>
        {
            new("Book 1", "Author 1", 100),
            new("Book 2", "Author 2", 200)
        };
        _mockRepository.Setup(r => r.GetBooks()).Returns(expectedBooks);
        var sut = new BookService(_mockRepository.Object);

        // Act
        var result = sut.GetBooks();

        // Assert
        Assert.That(result, Has.Count.EqualTo(2));
        _mockRepository.Verify(r => r.GetBooks(), Times.Once);
    }

    [Test]
    public void GetBookByTitle_ValidTitle_ReturnsBook()
    {
        // Arrange
        var expectedBook = new Book("The Hobbit", "Tolkien", 310);
        _mockRepository.Setup(r => r.GetBook("Hobbit")).Returns(expectedBook);
        var sut = new BookService(_mockRepository.Object);

        // Act
        var result = sut.GetBookByTitle("Hobbit");

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Title, Is.EqualTo("The Hobbit"));
        _mockRepository.Verify(r => r.GetBook("Hobbit"), Times.Once);
    }

    [TestCase("")]
    [TestCase(null)]
    public void GetBookByTitle_EmptyTitle_ReturnsNull(string? title)
    {
        // Arrange
        var sut = new BookService(_mockRepository.Object);

        // Act
        var result = sut.GetBookByTitle(title);

        // Assert
        Assert.That(result, Is.Null);
        _mockRepository.Verify(r => r.GetBook(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void AddBook_ValidBook_CallsRepository()
    {
        // Arrange
        var book = new Book("The Hobbit", "Tolkien", 310);
        var sut = new BookService(_mockRepository.Object);

        // Act
        sut.AddBook(book);

        // Assert
        _mockRepository.Verify(r => r.AddBook(book), Times.Once);
    }

    [Test]
    public void AddBooks_ValidList_CallsRepository()
    {
        // Arrange
        var books = new List<Book>
        {
            new("Book 1", "Author 1", 100)
        };
        var sut = new BookService(_mockRepository.Object);

        // Act
        sut.AddBooks(books);

        // Assert
        _mockRepository.Verify(r => r.AddBooks(books), Times.Once);
    }

    [Test]
    public void AddBooks_NullList_ThrowsArgumentNullException()
    {
        // Act & Assert
        var sut = new BookService(_mockRepository.Object);
        Assert.Throws<ArgumentNullException>(() => sut.AddBooks(null!));
        _mockRepository.Verify(r => r.AddBooks(It.IsAny<List<Book>>()), Times.Never);
    }

    [Test]
    public void AddBooks_EmptyList_ThrowsArgumentNullException()
    {
        // Arrange
        var books = new List<Book>();
        var sut = new BookService(_mockRepository.Object);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => sut.AddBooks(books));
        _mockRepository.Verify(r => r.AddBooks(It.IsAny<List<Book>>()), Times.Never);
    }

    [Test]
    public void SortBooks_MultipleBooks_SortsByAuthorThenTitle()
    {
        // Arrange
        var books = new List<Book>
        {
            new("The Ugly Duckling", "Andersen", 50),
            new("The Little Mermaid", "Andersen", 60),
            new("Carrie", "King", 400),
            new("The Shining", "King", 450)
        };
        _mockRepository.Setup(r => r.GetBooks()).Returns(books);
        var sut = new BookService(_mockRepository.Object);

        // Act
        var sorted = sut.SortBooks();

        // Assert
        Assert.That(sorted, Has.Count.EqualTo(4));
        Assert.That(sorted[0].Author, Is.EqualTo("Andersen"));
        Assert.That(sorted[0].Title, Is.EqualTo("The Little Mermaid"));
        Assert.That(sorted[1].Author, Is.EqualTo("Andersen"));
        Assert.That(sorted[1].Title, Is.EqualTo("The Ugly Duckling"));
        Assert.That(sorted[2].Author, Is.EqualTo("King"));
        Assert.That(sorted[2].Title, Is.EqualTo("Carrie"));
        Assert.That(sorted[3].Author, Is.EqualTo("King"));
        Assert.That(sorted[3].Title, Is.EqualTo("The Shining"));
        _mockRepository.Verify(r => r.GetBooks(), Times.Once);
    }

    [Test]
    public void SortBooks_EmptyList_ReturnsEmptyList()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetBooks()).Returns(new List<Book>());
        var sut = new BookService(_mockRepository.Object);

        // Act
        var sorted = sut.SortBooks();

        // Assert
        Assert.That(sorted, Is.Empty);
    }

    [Test]
    public void SortBooks_SingleBook_ReturnsSingleBook()
    {
        // Arrange
        var books = new List<Book> { new("The Hobbit", "Tolkien", 310) };
        _mockRepository.Setup(r => r.GetBooks()).Returns(books);
        var sut = new BookService(_mockRepository.Object);

        // Act
        var sorted = sut.SortBooks();

        // Assert
        Assert.That(sorted, Has.Count.EqualTo(1));
        Assert.That(sorted[0].Title, Is.EqualTo("The Hobbit"));
    }

    [Test]
    public async Task GetBooksFromXml_ValidPath_CallsRepository()
    {
        // Arrange
        var expectedBooks = new List<Book> { new("Test", "Author", 100) };
        _mockRepository.Setup(r => r.GetBooksFromXmlAsync("test.xml", It.IsAny<CancellationToken>())).ReturnsAsync(expectedBooks);
        var sut = new BookService(_mockRepository.Object);

        // Act
        var result = await sut.GetBooksFromXmlAsync("test.xml");

        // Assert
        Assert.That(result, Has.Count.EqualTo(1));
        _mockRepository.Verify(r => r.GetBooksFromXmlAsync("test.xml", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public void GetBooksFromXml_EmptyPath_ThrowsArgumentException()
    {
        // Act & Assert
        var sut = new BookService(_mockRepository.Object);
        Assert.ThrowsAsync<ArgumentException>(() => sut.GetBooksFromXmlAsync(""));
    }

    [Test]
    public async Task SaveAllBooksToXml_ValidPath_CallsRepository()
    {
        // Arrange
        var filePath = "test_output.xml";
        var sut = new BookService(_mockRepository.Object);

        // Act
        await sut.SaveAllBooksToXmlAsync(filePath);

        // Assert
        _mockRepository.Verify(r => r.SaveBooksToXmlAsync(filePath, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public void SaveAllBooksToXml_EmptyPath_ThrowsArgumentException()
    {
        // Act & Assert
        var sut = new BookService(_mockRepository.Object);
        Assert.ThrowsAsync<ArgumentException>(() =>
            sut.SaveAllBooksToXmlAsync(""));
    }

    [Test]
    public void SaveAllBooksToXml_NullPath_ThrowsArgumentException()
    {
        // Act & Assert
        var sut = new BookService(_mockRepository.Object);
        Assert.ThrowsAsync<ArgumentException>(() =>
            sut.SaveAllBooksToXmlAsync(null!));
    }
}