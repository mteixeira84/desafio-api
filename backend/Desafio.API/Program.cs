using Desafio.API.Configuration;

var builder = WebApplication.CreateBuilder(args);

// SERVICES
builder.Services.AddApiConfiguration(builder.Configuration);

var app = builder.Build();

// PIPELINE
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseResponseCompression();

app.UseApiConfiguration(app.Environment);

app.MapControllers();

app.Run();
