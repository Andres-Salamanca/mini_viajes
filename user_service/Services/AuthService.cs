namespace UsersService.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using UsersService.Model;
using System.Text;

public class JwtService
{
    private readonly IConfiguration conf;
    public JwtService(IConfiguration configuratios)
    {
        conf = configuratios;
    }

    public string GenerateToken(User user)
    {
        var claims = new[]
        {
          new Claim(ClaimTypes.Name, user.Name),
          new Claim(ClaimTypes.Role, user.Role)
      };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(conf["Jwt:SecretKey"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: conf["Jwt:Issuer"],
            audience: conf["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
