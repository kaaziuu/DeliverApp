using FluentAssertions;
using Microsoft.Extensions.Options;
using MockQueryable.NSubstitute;
using Models;
using Models.Db;
using NSubstitute;
using Repository.Repository.Interface;
using Services.Impl.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Utils;

public class AuthenticationUtilsTest
{
    private readonly IQueryable<RefreshToken> _refreshTokens = new List<RefreshToken>
    {
        new RefreshToken
        {
            Id = 1,
            Token =  "test-token",
            UserId = 10,
            IsUsed = false,
            ExpireDate = System.DateTime.UtcNow.AddDays(2),
        },
        new RefreshToken
        {
            Id = 2,
            Token = "test",
            UserId = 30,
            ExpireDate = System.DateTime.UtcNow.AddDays(-3),
        },
        new RefreshToken
        {
            Id = 3,
            Token = "test-token-ss",
            UserId = 10,
            IsUsed = false,
            ExpireDate = System.DateTime.UtcNow.AddDays(2),
        }
    }.BuildMock();

    private readonly JwtSettings _jwtSettings = new()
    {
        Secret = "secret key for mock tests"
    };

    private readonly IRefreshTokenRepository _refreshTokenRepositoryMock;
    private readonly IOptions<JwtSettings> _jwtSettingsMock;
    private readonly AuthenticationUtils _service;

    public AuthenticationUtilsTest()
    {
        _refreshTokenRepositoryMock = Substitute.For<IRefreshTokenRepository>();
        _refreshTokenRepositoryMock.AddAsync(Arg.Any<RefreshToken>()).Returns(true);
        _refreshTokenRepositoryMock.UpdateAsync(Arg.Any<RefreshToken>()).Returns(true);
        _refreshTokenRepositoryMock.GetAll().Returns(_refreshTokens);

        _jwtSettingsMock = Substitute.For<IOptions<JwtSettings>>();
        _jwtSettingsMock.Value.Returns(_jwtSettings);
        
        _service = new AuthenticationUtils(_refreshTokenRepositoryMock, _jwtSettingsMock);
    }

    [Fact]
    public void GenerateJwtToken_WhenPassUser_ThenReturnToken()
    {
        // arragne
        var user = new User { Id = 10 };

        // act
        var response = _service.GenerateJwtToken(user);

        // asset
        response.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GenerateRefreshToken_WhenPassUser_ThenReturnToken()
    {
        // arrange
        var user = new User
        {
            Id = 10
        };

        // act
        var response = await _service.GenerateRefreshToken(user, "local");

        // assert
        response.User.Should().BeOfType<User>();
        response.CreatedBy.Should().Be("local");
    }

    [Fact]
    public void ValidateJwtToken_WhenTokenIsNullOrInvalid_ReturnNull()
    {
        // act
        var responseNotNullToken = _service.ValidateJwtToken("asdasd");
        var responseNullToken = _service.ValidateJwtToken(null);

        // assert
        responseNotNullToken.Should().BeNull();
        responseNullToken.Should().BeNull();
    }

    [Fact]
    public void ValidateJwtToken_WhenTokenIsValid_ReturnsUserId()
    {
        // arrange
        var user = new User { Id = 10 };
        var token = _service.GenerateJwtToken(user);

        // act
        var response = _service.ValidateJwtToken(token);

        // assert
        response.Should().Be(10);
    }

    [Fact]
    public void ValidateJwtToken_WhenTokenExpire_ReturnNull()
    {
        // act
        var token = _service.ValidateJwtToken("test");

        // assert
        token.Should().BeNull();
    }

    [Fact]
    public async Task RevokeAllRefreshTokenForUser_WhenUserHaveToken_ThenRevokeAllRefreshToken()
    {
        // arrange
        var user = new User { Id = 10 };

        // act
        await _service.RevokeAllRefreshTokenForUser(user, "local");

        // assert
        await _refreshTokenRepositoryMock.Received(2).UpdateAsync(Arg.Is<RefreshToken>(x => x.IsUsed && x.UsedBy == "local"));
    }

    [Fact]
    public async Task RevokeRefreshToken_WhenTokenIsNotNull_ThenRevokeToken()
    {
        // arrange
        var refreshToken = new RefreshToken { Id = 1 };

        // act
        await _service.RevokeRefreshToken(refreshToken, "local");

        // assert
        await _refreshTokenRepositoryMock.Received(1).UpdateAsync(Arg.Is<RefreshToken>(x => x.IsUsed && x.UsedBy == "local"));
    }

    [Fact]
    public async Task GetRefreshTokenByToken_WhenTokenExists_ThenReturnRefreshToken()
    {
        // act
        var response = await _service.GetRefreshTokenByToken("test-token");

        // assert
        response.Should().NotBeNull();
        response.Should().BeOfType<RefreshToken>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("notExists")]
    public async Task GetRefreshTokenByToken_WhenTokenDoesntExists_ThenReturnNull(string token)
    {
        // act
        var response = await _service.GetRefreshTokenByToken(token);

        // asset
        response.Should().BeNull();
    }
}
