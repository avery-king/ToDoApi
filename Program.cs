using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using ToDoApi.Data;
using ToDoApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(opt => 
    opt.UseSqlite(builder.Configuration.GetConnectionString("SqliteConnection")));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.MapGet("api/todo", async (AppDbContext context) =>
{
    var items = await context.ToDoItems.ToListAsync();
    return Results.Ok(items);
});

app.MapPost("api/todo", async (AppDbContext context, ToDo toDo) =>
{
    await context.ToDoItems.AddAsync(toDo);
    await context.SaveChangesAsync();

    return Results.Created($"api/todo/{toDo.Id}", toDo);
});

app.MapPut("api/toDo/{id}", async (AppDbContext context, int id, ToDo toDo) =>
{
    var item = await context.ToDoItems.FirstOrDefaultAsync(i => i.Id == id);

    if (item == null)
    {
        return Results.NotFound();
    }
    
    item.Name = toDo.Name;
    await context.SaveChangesAsync();

    return Results.NoContent();
});

app.MapDelete("api/toDo/{id}", async (AppDbContext context, int id) =>
{
    var item = await context.ToDoItems.FirstOrDefaultAsync(i => i.Id == id);

    if (item == null)
    {
        return Results.NotFound();
    }

    context.ToDoItems.Remove(item);
    await context.SaveChangesAsync();

    return Results.NoContent();
});
app.Run();
