using Microsoft.OpenApi.Models;
using UsersService.Data;
using UsersService.DTO;
using UsersService.Model;
using System.Text;
using System.Security.Cryptography;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>{
        options.SwaggerDoc("v1", new OpenApiInfo { Title = "MINI API", Version = "v1" });
});
builder.Services.AddMvc();

var app = builder.Build();


using var db = new dbContext();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.UseHttpsRedirection();


// ====== get user ======
app.MapGet("/getAllUser", () =>
{
    var users = db.Users.ToList();
    return users is not null
        ? Results.Ok(users)
        : Results.NotFound(new { message = $"No users" });
})
.WithName("GetUsetInfo").WithOpenApi();

// ====== crate user ======
app.MapPost("/createUser", (RegisterRequest reg) =>
{
    if(db.Users.Any(user => user.Name == reg.Username)) {
      return Results.BadRequest("Username already exists.");
    }
    string hashedPassword;
    using (SHA256 sha256 = SHA256.Create()) {
      byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(reg.Password));
      hashedPassword = Convert.ToBase64String(hash);
    }
    var user = new User{
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
    if (user == null) {
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
    return Results.Ok("Login successful");

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
.WithName("FindUserId").WithOpenApi();

app.MapGet("/findUserName", (string Name) =>
{
  var user = db.Users.FirstOrDefault(user => user.Name == Name);
  return user is not null
        ? Results.Ok(user)
        : Results.NotFound(new { message = $"No user with given name {Name}" });

})
.WithName("FindUserName").WithOpenApi();

// ====== update user ======
app.MapPut("/updateUser", (User update,Guid id) =>
{
    var user = db.Users.FirstOrDefault(user => user.Id == id);
    if (user == null) {
      return Results.NotFound(new { message = $"user not found" });
    }

    user.Name = update.Name;
    user.Role = update.Role;
    user.Email = update.Email;

    db.SaveChanges();
    return Results.Ok(user);

})
.WithName("UpdateUser").WithOpenApi();

app.Run();

