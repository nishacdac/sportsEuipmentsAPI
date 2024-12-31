using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SportsEquipment.Interfaces;
using SportsEquipment.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddSignalR(); // SignalR service

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure database connection string
string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new Exception("Database connection string is not configured in appsettings.json");
}

// Register services and inject connection string for those that need it
builder.Services.AddSingleton<ICategoryService>(new CategoryService(connectionString));
builder.Services.AddScoped<IEquipmentService, EquipmentService>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IAdminSellerService>(provider => new AdminSellerService(connectionString));
builder.Services.AddScoped<ISellerService>(provider => new SellerService(connectionString));

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", corsBuilder =>
        corsBuilder.WithOrigins("http://localhost:4200") // Update with your frontend's URL
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .AllowCredentials());
});

// Configure JWT authentication
string? jwtSecret = builder.Configuration["Jwt:Secret"];
if (string.IsNullOrEmpty(jwtSecret))
{
    throw new Exception("JWT Secret is not configured in appsettings.json");
}

var key = Encoding.ASCII.GetBytes(jwtSecret);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false, // Set to true and configure Issuer if needed
        ValidateAudience = false // Set to true and configure Audience if needed
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection(); // Ensure HTTPS redirection is used
app.UseCors("AllowSpecificOrigins");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// SignalR configuration
app.MapHub<ChatHub>("/chathub");

app.Run();
