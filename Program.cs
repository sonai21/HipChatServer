using HipChatServer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Db Connection
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

//integrating LM Studio with semantic kernel
var endpoint = new Uri("http://127.0.0.1:1234/v1");
var model = "qwen/qwen3-1.7b";
var apiKey = "guest";

var kernelBuilder = Kernel.CreateBuilder().AddOpenAIChatCompletion(model, endpoint, apiKey);
var kernel = kernelBuilder.Build();
builder.Services.AddSingleton(kernel);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
