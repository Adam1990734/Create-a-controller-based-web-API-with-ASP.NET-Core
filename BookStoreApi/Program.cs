using BookStoreApi.Models;
using BookStoreApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

builder.Services.Configure<BookStoreDatabaseSettings>(
    builder.Configuration.GetSection("BookStoreDatabase")    
);
builder.Services.AddSingleton<BooksService>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//Book végpontok:
var booksGroup = app.MapGroup("/books");

booksGroup.MapGet("/", async (BooksService booksService) =>
{
    var books = await booksService.GetAsync();
    return Results.Ok(books);
});

booksGroup.MapGet("/{id:length(24)}", async (string id, BooksService booksService) =>
{
    var book = await booksService.GetAsync(id);

    return book is null ? Results.NotFound() : Results.Ok(book);
});

booksGroup.MapPost("/", async (Book newBook, BooksService booksService) =>
{
    await booksService.CreateAsync(newBook);

    return Results.Created($"/books/{newBook.Id}", newBook);
});

booksGroup.MapPut("/{id:length(24)}", async (string id, Book updatedBook, BooksService booksService) =>
{
    var book = await booksService.GetAsync(id);

    if (book is null)
    {
        return Results.NotFound();
    }

    updatedBook.Id = book.Id;

    await booksService.UpdateAsync(id, updatedBook);

    return Results.NoContent();
});

booksGroup.MapDelete("/{id:length(24)}", async (string id, BooksService booksService) =>
{
    var book = await booksService.GetAsync(id);

    if (book is null)
    {
        return Results.NotFound();
    }

    await booksService.RemoveAsync(id);

    return Results.NoContent();
});

app.UseHttpsRedirection();

app.UseAuthorization();

//app.MapControllers();

app.Run();
