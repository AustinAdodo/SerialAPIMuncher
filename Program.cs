var builder = WebApplication.CreateBuilder(args);

//By default, it creates HttpClient instances as Transient, 
// but with a key difference: it reuses the underlying HttpMessageHandler, 
// which manages the HTTP connections, effectively giving you the benefits of a Singleton 
// HttpClient (connection pooling) without its drawbacks (DNS changes not respected)

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();  //added to Pipeline.

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
