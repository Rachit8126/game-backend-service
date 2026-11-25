using GameBackendAPI.Entities;
using GameBackendAPI.Security.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;

namespace GameBackendAPI.Tests
{
    public class JWTAuthTests
    {
        private readonly AuthTokenService _authtokenservice = new(Options.Create<AuthSettings>(new()
        {
            Audience = "players",
            Issuer = "game_backend",
            Key = "test1234567890qwertyuiopasdfghjk",
            LifeTimeInSeconds = 300
        }));

        private readonly Player _player = new()
        {
            Id = 12345,
            Email = "test@mail.com",
            HashedPassword = "testHash",
            Username = "Test",
            Rating = 10,
            CreatedAt = DateTime.UtcNow
        };

        [Fact]
        public void TestTokenService()
        {
            string token = _authtokenservice.CreateToken(_player);

            var result = new JsonWebTokenHandler().ReadJsonWebToken(token);

            Assert.Equal(_player.Id.ToString(), result.Subject);
            Assert.Equal(["players"], result.Audiences);
            Assert.Equal("game_backend", result.Issuer);
            Assert.True(result.ValidTo > result.IssuedAt);
        }
    }
}
