using SimpleRateLimiter;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<BucketAlgorithmService>();


var app = builder.Build();
app.UseMiddleware<RateLimiterMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/unlimited", () => "Hello unlimited world!")
    .WithName("GetUnlimited")
    .WithOpenApi();

app.MapGet("/limited", () => "Hello limited world!")
    .WithName("GetLimited")
    .WithOpenApi();

app.Run();