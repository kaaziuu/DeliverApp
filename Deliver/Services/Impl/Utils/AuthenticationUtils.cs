using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Models;
using Models.Db;
using Repository.Repository.Interface;
using Services.Interface.Utils;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Services.Impl.Utils;

public class AuthenticationUtils : IAuthenticationUtils
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly JwtSettings _jwtSetting;

    public AuthenticationUtils(IRefreshTokenRepository refreshTokenRepository, IOptions<JwtSettings> jwtSetting)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _jwtSetting = jwtSetting.Value;
    }

    public string GenerateJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSetting.Secret);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()) }),
            Expires = DateTime.UtcNow.AddMinutes(6),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public async Task<RefreshToken> GenerateRefreshToken(User user, string ipAddress)
    {
        var refreshToken = new RefreshToken
        {
            Token = await getUniqueToken(),
            ExpireDate = DateTime.UtcNow.AddDays(7),
            CreatedBy = ipAddress,
            User = user
        };

        await _refreshTokenRepository.AddAsync(refreshToken);

        return refreshToken;

        async Task<string> getUniqueToken()
        {
            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

            var existToken = await _refreshTokenRepository.GetAll().FirstOrDefaultAsync(x => x.Token == token);

            if (existToken is not null)
            {
                return await getUniqueToken();
            }

            return token;
        }
    }

    public int? ValidateJwtToken(string? token)
    {
        if (token is null)
        {
            return null;
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSetting.Secret);

        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero,
                ValidateLifetime = true
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);

            return userId;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task RevokeAllRefreshTokenForUser(User user, string ipAddress)
    {
        var tokens = await _refreshTokenRepository
            .GetAll()
            .Where(x => x.UserId == user.Id && x.IsUsed == false && x.ExpireDate > DateTime.UtcNow)
            .ToListAsync();
        foreach (var token in tokens)
        {
            token.IsUsed = true;
            token.UsedBy = ipAddress;
            await _refreshTokenRepository.UpdateAsync(token);
        }
    }

    public async Task RevokeRefreshToken(RefreshToken refreshToken, string ipAddress)
    {
        if (refreshToken is not null)
        {
            refreshToken.IsUsed = true;
            refreshToken.UsedBy = ipAddress;
            await _refreshTokenRepository.UpdateAsync(refreshToken);
        }
    }

    public async Task<RefreshToken?> GetRefreshTokenByToken(string? token)
    {
        if (token is null)
        {
            return null;
        }

        var refreshToken = await _refreshTokenRepository.GetAll().FirstOrDefaultAsync(x => x.Token == token);

        if (refreshToken is null)
        {
            return null;
        }

        return refreshToken;
    }
}
