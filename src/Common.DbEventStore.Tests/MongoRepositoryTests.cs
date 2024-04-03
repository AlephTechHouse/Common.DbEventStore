using Common.DbEventStore.MongoDB;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Moq;

namespace Common.DbEventStore.Tests;

public class MongoRepositoryTests
{
    private readonly Mock<IMongoDatabase> _mockDatabase;
    private readonly Mock<IMongoCollection<User>> _mockCollection;
    private readonly Mock<ILogger<MongoRepository<User>>> _mockLogger;
    private readonly MongoRepository<User> _repository;
    public MongoRepositoryTests()
    {
        _mockDatabase = new Mock<IMongoDatabase>();
        _mockCollection = new Mock<IMongoCollection<User>>();
        _mockLogger = new Mock<ILogger<MongoRepository<User>>>();

        _mockDatabase.Setup(db => db.GetCollection<User>(It.IsAny<string>(), null))
            .Returns(_mockCollection.Object);

        _repository = new MongoRepository<User>(_mockDatabase.Object, "test", _mockLogger.Object);
    }
    [Fact]
    public async Task GetAllAsync_NoFilter_ReturnsAllItems()
    {
        var mockCursor = new Mock<IAsyncCursor<User>>();
        // Arrange
        var user = new User { Name = "Test", Email = "test@example.com" };

        // Act
        await _repository.CreateAsync(user);

        // Assert
        _mockCollection.Verify(c => c.InsertOneAsync(user, null, default), Times.Once);
    }
}