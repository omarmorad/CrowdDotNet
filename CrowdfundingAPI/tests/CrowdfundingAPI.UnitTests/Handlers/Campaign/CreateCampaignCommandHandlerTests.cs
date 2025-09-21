using AutoMapper;
using CrowdfundingAPI.Application.Commands.Campaign;
using CrowdfundingAPI.Application.DTOs.Campaign;
using CrowdfundingAPI.Application.Handlers.Campaign;
using CrowdfundingAPI.Application.Mappings;
using CrowdfundingAPI.Domain.Entities;
using CrowdfundingAPI.Domain.Enums;
using CrowdfundingAPI.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace CrowdfundingAPI.UnitTests.Handlers.Campaign;

public class CreateCampaignCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly IMapper _mapper;
    private readonly CreateCampaignCommandHandler _handler;

    public CreateCampaignCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();
        
        _handler = new CreateCampaignCommandHandler(_unitOfWorkMock.Object, _mapper);
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldCreateCampaign()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            FirstName = "John",
            LastName = "Creator",
            Email = "creator@example.com",
            Role = UserRole.User
        };

        var command = new CreateCampaignCommand
        {
            Title = "Test Campaign",
            Description = "Test Description",
            ShortDescription = "Short Description",
            GoalAmount = 10000,
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(31),
            Category = CampaignCategory.Technology,
            UserId = userId,
            RewardTiers = new List<CreateRewardTierDto>
            {
                new()
                {
                    Title = "Early Bird",
                    Description = "Early bird reward",
                    MinimumAmount = 25
                }
            }
        };

        var mockUserRepo = new Mock<IRepository<User>>();
        var mockCampaignRepo = new Mock<IRepository<CrowdfundingAPI.Domain.Entities.Campaign>>();
        var mockRewardTierRepo = new Mock<IRepository<RewardTier>>();

        mockUserRepo.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);
        mockCampaignRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<CrowdfundingAPI.Domain.Entities.Campaign, bool>>>()))
            .ReturnsAsync(new CrowdfundingAPI.Domain.Entities.Campaign { Id = Guid.NewGuid(), Owner = user });

        _unitOfWorkMock.Setup(x => x.Users).Returns(mockUserRepo.Object);
        _unitOfWorkMock.Setup(x => x.Campaigns).Returns(mockCampaignRepo.Object);
        _unitOfWorkMock.Setup(x => x.RewardTiers).Returns(mockRewardTierRepo.Object);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Title.Should().Be(command.Title);
        result.Data.Status.Should().Be(CampaignStatus.Draft);
        
        mockCampaignRepo.Verify(x => x.AddAsync(It.IsAny<CrowdfundingAPI.Domain.Entities.Campaign>()), Times.Once);
        mockRewardTierRepo.Verify(x => x.AddRangeAsync(It.IsAny<IEnumerable<RewardTier>>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_UserNotFound_ShouldReturnError()
    {
        // Arrange
        var command = new CreateCampaignCommand
        {
            UserId = Guid.NewGuid(),
            Title = "Test Campaign"
        };

        var mockUserRepo = new Mock<IRepository<User>>();
        mockUserRepo.Setup(x => x.GetByIdAsync(command.UserId)).ReturnsAsync((User?)null);
        _unitOfWorkMock.Setup(x => x.Users).Returns(mockUserRepo.Object);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Be("User not found");
    }
}