using BooksService.Models;

namespace BooksServiceTests.Models;

[TestFixture]
public class BookTests
{
    [Test]
    public void ctor_ValidParameters_BookInitialized()
    {
        // Arrange & Act
        var book = new Book("The Little Mermaid", "Andersen", 310);

        // Assert
        Assert.That(book.Title, Is.EqualTo("The Little Mermaid"));
        Assert.That(book.Author, Is.EqualTo("Andersen"));
        Assert.That(book.NumberOfPages, Is.EqualTo(310));
    }

    [TestCase("", "Tolkien", 310)]
    [TestCase(null, "Tolkien", 310)]
    public void ctor_EmptyOrNullTitle_ThrowsArgumentException(string? title, string author, int pages)
    {
        // Arrange & Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new Book(title, author, pages));
        Assert.That(ex.ParamName, Is.EqualTo("Title"));
    }

    [TestCase("The Hobbit", "", 310)]
    [TestCase("The Hobbit", null, 310)]
    public void ctor_EmptyOrNullAuthor_ThrowsArgumentException(string title, string? author, int pages)
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new Book(title, author, pages));
        Assert.That(ex.ParamName, Is.EqualTo("Author"));
    }

    [TestCase("The Hobbit", "Tolkien", 0)]
    [TestCase("The Hobbit", "Tolkien", -1)]
    public void ctor_ZeroPages_ThrowsArgumentException(string title, string author, int pages)
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new Book(title, author, pages));
        Assert.That(ex.ParamName, Is.EqualTo("NumberOfPages"));
    }

    [Test]
    public void Title_SetToEmpty_ThrowsArgumentException()
    {
        // Arrange
        var book = new Book("The Hobbit", "Tolkien", 310);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => book.Title = "");
    }

    [Test]
    public void Author_SetToEmpty_ThrowsArgumentException()
    {
        // Arrange
        var book = new Book("The Hobbit", "Tolkien", 310);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => book.Author = "");
    }

    [Test]
    public void NumberOfPages_SetToZero_ThrowsArgumentException()
    {
        // Arrange
        var book = new Book("The Hobbit", "Tolkien", 310);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => book.NumberOfPages = 0);
    }

    [Test]
    public void Validate_ValidBook_DoesNotThrow()
    {
        // Arrange
        var book = new Book("The Hobbit", "Tolkien", 310);

        // Act & Assert
        Assert.DoesNotThrow(() => book.Validate());
    }

    [Test]
    public void Validate_EmptyTitle_ThrowsInvalidOperationException()
    {
        // Arrange
        var book = new Book { Author = "Tolkien", NumberOfPages = 310 };

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => book.Validate());
    }

    [Test]
    public void Validate_EmptyAuthor_ThrowsInvalidOperationException()
    {
        // Arrange
        var book = new Book { Title = "The Hobbit", NumberOfPages = 310 };

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => book.Validate());
    }

    [Test]
    public void Validate_EmptyPages_ThrowsInvalidOperationException()
    {
        // Arrange
        var book = new Book { Author = "Tolkien", Title = "The Hobbit" };

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => book.Validate());
    }

    [Test]
    public void Validate_EmptyBook_ThrowsInvalidOperationException()
    {
        // Arrange
        var book = new Book();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => book.Validate());
    }
}
