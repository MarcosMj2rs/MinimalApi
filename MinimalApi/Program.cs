using MinimalApi.Model;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Usando a Injeção de dependência nativa
builder.Services.AddSingleton<SalutationService>(new SalutationService());

//Usando Entity Framework Core
builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("Clientes"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
	app.UseDeveloperExceptionPage();

app.UseSwagger();

#region[HttpClient]

//Assim vamos usar o HttpClient para acessar os dados do serviço REST em https://jsonplaceholder.typicode.com/ no formato JSON.
//app.MapGet("/albuns", async () => await new HttpClient().GetStringAsync("https://jsonplaceholder.typicode.com/albums"));

//app.MapGet("/welcome", (HttpContext context, SalutationService salutationService) =>
//						salutationService.Welcome(context.Request.Query["nome"].ToString()));

#endregion

#region[Operações Model]

app.MapGet("/Clientes/{id}", async (int id, AppDbContext dbContext) =>
	await dbContext.Clientes.FindAsync(id)
	is Cliente cliente
	? Results.Ok(cliente)
	: Results.NotFound());

app.MapGet("/Clientes", async (AppDbContext dbContext) =>
	await dbContext.Clientes.ToListAsync());

app.MapPost("/Clientes", async (Cliente cliente, AppDbContext dbContext) =>
{
	dbContext.Clientes.Add(cliente);
	await dbContext.SaveChangesAsync();

	return Results.Created($"/Clientes/{cliente.Id}", cliente);
});

app.MapPut("/Clientes/{id}", async (int id, Cliente cliente, AppDbContext dbContext) =>
{
	var clienteTmp = await dbContext.Clientes.FindAsync(id);

	if (clienteTmp is null)
		return Results.NotFound();

	clienteTmp.Nome = cliente.Nome;

	await dbContext.SaveChangesAsync();

	return Results.NoContent();
});

app.MapDelete("/Clientes/{id}", async (int id, AppDbContext dbContext) =>
{
	if (await dbContext.Clientes.FindAsync(id) is Cliente cliente)
	{
		dbContext.Clientes.Remove(cliente);
		await dbContext.SaveChangesAsync();
		return Results.NoContent();
	}

	return Results.NotFound();
});

#endregion

app.UseSwaggerUI();

app.Run();

#region[DbContext]
public class AppDbContext : DbContext
{
	public AppDbContext(DbContextOptions options) : base(options) { }

	public DbSet<Cliente>? Clientes => Set<Cliente>();
}
#endregion