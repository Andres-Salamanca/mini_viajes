using Microsoft.OpenApi.Models;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using TripServices.DTO;
using TripServices.Model;
using TripServices.Data;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "MINI API Clients", Version = "v1" });
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

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnlyPolicy", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserOnlyPolicy", policy => policy.RequireRole("User"));
});
builder.Services.AddAuthorization();

using var db = new dbContext();
if (!tripDb.Trips.Any())
{
    tripDb.Trips.AddRange(
        new Trip
        {
            Id = Guid.NewGuid(),
            Destination = "Paris",
            DepartureDate = DateTime.UtcNow.AddDays(10),
            Price = 999.99m,
            IsAvailable = true
        },
        new Trip
        {
            Id = Guid.NewGuid(),
            Destination = "Tokyo",
            DepartureDate = DateTime.UtcNow.AddDays(30),
            Price = 1200.50m,
            IsAvailable = true
        },
        new Trip
        {
            Id = Guid.NewGuid(),
            Destination = "New York",
            DepartureDate = DateTime.UtcNow.AddDays(5),
            Price = 850.00m,
            IsAvailable = false
        }
    );
    tripDb.SaveChanges();
}
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.UseCors("AllowAngularApp");
app.UseAuthentication();
app.UseHttpsRedirection();
app.UseAuthorization();


// -------------------- ENDPOINTS ---------------------------- //
app.MapGet("/getDestinations", () =>
{
    var trips = db.Trips.ToList();
    return trips.Any()
        ? Results.Ok(trips)
        : Results.NotFound(new { message = "No trips found." });
})
.WithName("GetDestinations")
.WithOpenApi()
.RequireAuthorization();

app.MapPost("/createDestination", async (CreateTripRequest newTrip) =>
{
    var trip = new Trip
    {
        Id = Guid.NewGuid(),
        Destination = newTrip.Destination,
        DepartureDate = newTrip.DepartureDate,
        Price = newTrip.Price,
        IsAvailable = true
    };

    db.Trips.Add(trip);
    await db.SaveChangesAsync();

    return Results.Created($"/getDestinations/{trip.Id}", trip);
})
.WithName("CreateDestination")
.WithOpenApi()
.RequireAuthorization("AdminOnlyPolicy");

app.MapPut("/updateDestination/{id}", async (Guid id, CreateTripRequest updateTrip) =>
{
    var trip = await db.Trips.FindAsync(id);
    if (trip is null)
        return Results.NotFound(new { message = "Trip not found" });

    trip.Destination = updateTrip.Destination;
    trip.DepartureDate = updateTrip.DepartureDate;
    trip.Price = updateTrip.Price;

    await db.SaveChangesAsync();
    return Results.Ok(trip);
})
.WithName("UpdateDestination")
.WithOpenApi()
.RequireAuthorization("AdminOnlyPolicy");

app.MapDelete("/deleteDestination/{id}", async (Guid id) =>
{
    var trip = await db.Trips.FindAsync(id);
    if (trip is null)
        return Results.NotFound(new { message = "Trip not found" });

    db.Trips.Remove(trip);
    await db.SaveChangesAsync();
    return Results.Ok(new { message = "Trip deleted" });
})
.WithName("DeleteDestination")
.WithOpenApi()
.RequireAuthorization("AdminOnlyPolicy");

app.Run();


