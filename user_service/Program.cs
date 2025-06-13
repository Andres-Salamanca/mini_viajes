using Microsoft.OpenApi.Models;
using UsersService.Data;
using UsersService.DTO;
using UsersService.Model;
using UsersService.Services;
using System.Text;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "MINI API", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token in this format: Bearer <your-token>"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
builder.Services.AddMvc();

var conf = builder.Configuration;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
  .AddJwtBearer(options =>
  {
      var key = Encoding.UTF8.GetBytes(conf["Jwt:SecretKey"]!);

      options.TokenValidationParameters = new TokenValidationParameters
      {
          ValidateIssuer = true,
          ValidIssuer = conf["Jwt:Issuer"],

          ValidateAudience = true,
          ValidAudience = conf["Jwt:Audience"],

          ValidateLifetime = true,
          ValidateIssuerSigningKey = true,
          IssuerSigningKey = new SymmetricSecurityKey(key)
      };

  });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnlyPolicy", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserOnlyPolicy", policy => policy.RequireRole("User"));
});
builder.Services.AddAuthorization();
builder.Services.AddSingleton<JwtService>();
var app = builder.Build();


using var db = new dbContext();
var Jwtservice = new JwtService(conf);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}
app.UseAuthentication();
app.UseHttpsRedirection();
app.UseAuthorization();


// -------------------- ENDPOINTS ---------------------------- //
// ====== get user ======
app.MapGet("/getAllUser", () =>
{
    var users = db.Users.ToList();
    return users is not null
        ? Results.Ok(users)
        : Results.NotFound(new { message = $"No users" });
})
.WithName("GetUsetInfo").WithOpenApi().RequireAuthorization("AdminOnlyPolicy");

// ====== crate user ======
app.MapPost("/createUser", (RegisterRequest reg) =>
{
    if (db.Users.Any(user => user.Name == reg.Username))
    {
        return Results.BadRequest("Username already exists.");
    }
    string hashedPassword;
    using (SHA256 sha256 = SHA256.Create())
    {
        byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(reg.Password));
        hashedPassword = Convert.ToBase64String(hash);
    }
    var user = new User
    {
        Name = reg.Username,
        Email = reg.Email,
        Role = "User",
        PasswordHash = hashedPassword,
    };
    db.Add(user);
    db.SaveChanges();
    return Results.Ok();
})
.WithName("RegisterUser").WithOpenApi();

// ====== log user ======
app.MapPost("/logIn", (LoginRequest log) =>
{
    var user = db.Users.FirstOrDefault(user => user.Name == log.Username);
    if (user == null)
    {
        return Results.BadRequest("User not found");
    }
    using (SHA256 sha256 = SHA256.Create())
    {
        byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(log.Password));
        string hashedInputPassword = Convert.ToBase64String(hash);

        // Compare hashes
        if (user.PasswordHash != hashedInputPassword)
        {
            return Results.BadRequest("Invalid password");
        }
    }
    var token = Jwtservice.GenerateToken(user);
    return Results.Ok(new { user, token });

})
.WithName("LogIn").WithOpenApi();

// ====== find user ======
app.MapGet("/findUserId", (Guid id) =>
{
    var user = db.Users.Find(id);
    return user is not null
          ? Results.Ok(user)
          : Results.NotFound(new { message = $"No user with given ID {id}" });

})
.WithName("FindUserId").WithOpenApi().RequireAuthorization("AdminOnlyPolicy");

app.MapGet("/findUserName", (string Name) =>
{
    var user = db.Users.FirstOrDefault(user => user.Name == Name);
    return user is not null
          ? Results.Ok(user)
          : Results.NotFound(new { message = $"No user with given name {Name}" });

})
.WithName("FindUserName").WithOpenApi().RequireAuthorization("AdminOnlyPolicy");

// ====== update user ======
app.MapPut("/updateUser", (User update, Guid id) =>
{
    var user = db.Users.FirstOrDefault(user => user.Id == id);
    if (user == null)
    {
        return Results.NotFound(new { message = $"user not found" });
    }

    user.Name = update.Name;
    // if role changed we need new jwt
    user.Role = update.Role;
    user.Email = update.Email;

    db.SaveChanges();
    return Results.Ok(user);

})
.WithName("UpdateUser").WithOpenApi().RequireAuthorization();

app.Run();

