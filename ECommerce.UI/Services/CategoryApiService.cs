using Ecommerce.API.DTO.Pagination;
using ECommerce.UI.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Spectre.Console;
using System.Net.Http.Json;
namespace ECommerce.UI.Services;

public class CategoryApiService
{
    private const int DefaultPageSize = 10;
    private const int FetchAllPageSize = 50;

    private readonly HttpClient _httpClient;

    public CategoryApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IActionResult> GetAllCategoriesApi()
    {
        int pageNumber = ApiSeriveHelper.AskPositiveInt("Enter [green]Page Number[/]:", 1);
        int pageSize = ApiSeriveHelper.AskPositiveInt("Enter [green]Page Size[/]:", DefaultPageSize);
        PagedResponse<CategoryDto>? result;

        try
        {
            var response = await _httpClient.GetAsync($"api/Category?pageNumber={pageNumber}&pageSize={pageSize}");

            if (!response.IsSuccessStatusCode)
            {
                AnsiConsole.MarkupLine($"[red]Failed to fetch categories. Status code: {response.StatusCode}[/]");
                return new StatusCodeResult((int)response.StatusCode);
            }

            result = await response.Content.ReadFromJsonAsync<PagedResponse<CategoryDto>>();
        }
        catch (HttpRequestException ex)
        {
            AnsiConsole.MarkupLine($"[red]Network error while fetching categories: " + $"{Markup.Escape(ex.Message)}[/]");
            return new StatusCodeResult(StatusCodes.Status503ServiceUnavailable);
        }

        if (result == null || result.Data.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No categories found.[/]");
            return new OkObjectResult(result);
        }

        var table = new Table().AddColumns("Id", "Name", "Description");

        foreach (var category in result.Data)
        {
            table.AddRow(
                category.CategoryId.ToString(),
                Markup.Escape(category.Name ?? string.Empty),
                Markup.Escape(category.Description ?? string.Empty));
        }

        AnsiConsole.Write(table);

        AnsiConsole.MarkupLine($"Page [green]{result.PageNumber}[/] of size [green]{result.PageSize}[/] " +$"({result.TotalRecords} total)");
        return new OkObjectResult(result);
    }

    public async Task<IActionResult> CreateCategoryApi()
    {
        string name = ApiSeriveHelper.AskNonEmptyString("Enter [green]Category Name[/]:");
        string description = ApiSeriveHelper.AskNonEmptyString("Enter [green]Category Description[/]:");
        var request = new CategoryDto
        {
            Name = name,
            Description = description
        };

        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/Category",request);

            if (response.IsSuccessStatusCode)
            {
                AnsiConsole.MarkupLine("[green]Category created successfully![/]");
                return new OkResult();
            }

            AnsiConsole.MarkupLine($"[red]Failed to create category. " +$"Status Code: {response.StatusCode}[/]");
            return new StatusCodeResult((int)response.StatusCode);
        }
        catch (HttpRequestException ex)
        {
            AnsiConsole.MarkupLine($"[red]Network error while creating category: " +$"{Markup.Escape(ex.Message)}[/]");
            return new StatusCodeResult(StatusCodes.Status503ServiceUnavailable);
        }
    }

    public async Task<IActionResult> UpdateCategoryApi()
    {
        CategoryDto? category;

        try
        {
            category = await PickCategoryAsync("Select the [green]category[/] to update:");
        }
        catch (HttpRequestException ex)
        {
            AnsiConsole.MarkupLine($"[red]Network error while fetching categories: " +$"{Markup.Escape(ex.Message)}[/]");
            return new StatusCodeResult( StatusCodes.Status503ServiceUnavailable);
        }

        if (category == null)
        {
            AnsiConsole.MarkupLine("[yellow]No category selected. Update cancelled.[/]");
            return new OkResult();
        }

        string name = ApiSeriveHelper.AskNonEmptyString("Enter [green]Category Name[/]:",category.Name);

        string description = ApiSeriveHelper.AskNonEmptyString("Enter [green]Category Description[/]:",category.Description);

        AnsiConsole.MarkupLine("\n[bold]Review changes:[/]");
        AnsiConsole.MarkupLine($"Name: [green]{Markup.Escape(name)}[/]");
        AnsiConsole.MarkupLine($"Description: [green]{Markup.Escape(description)}[/]\n");

        if (!ApiSeriveHelper.Confirm(
                "Save these changes?",
                defaultValue: true))
        {
            AnsiConsole.MarkupLine("[yellow]Update cancelled.[/]");
            return new OkResult();
        }

        var request = new CategoryDto
        {
            CategoryId = category.CategoryId,
            Name = name,
            Description = description
        };

        try
        {
            var response = await _httpClient.PatchAsJsonAsync( $"api/Category/{category.CategoryId}",request);

            if (response.IsSuccessStatusCode)
            {
                AnsiConsole.MarkupLine($"[green]Updated {Markup.Escape(category.Name)} " +$"to {Markup.Escape(name)}.[/]");
                return new OkResult();
            }

            AnsiConsole.MarkupLine($"[red]Update failed: {(int)response.StatusCode} " + $"{response.ReasonPhrase}[/]");

            return new StatusCodeResult((int)response.StatusCode);
        }
        catch (HttpRequestException ex)
        {
            AnsiConsole.MarkupLine($"[red]Network error while updating category: " +$"{Markup.Escape(ex.Message)}[/]");

            return new StatusCodeResult(StatusCodes.Status503ServiceUnavailable);
        }
    }

    public async Task<IActionResult> DeleteCategoryApi()
    {
        CategoryDto? category;

        try
        {
            category = await PickCategoryAsync("Select the [green]category[/] to delete:");
        }
        catch (HttpRequestException ex)
        {
            AnsiConsole.MarkupLine( $"[red]Network error while fetching categories: " + $"{Markup.Escape(ex.Message)}[/]");

            return new StatusCodeResult(StatusCodes.Status503ServiceUnavailable);
        }

        if (category == null)
        {
            AnsiConsole.MarkupLine("[yellow]Delete cancelled.[/]");
            return new OkResult();
        }

        bool confirm = ApiSeriveHelper.Confirm( $"Are you sure you want to delete category " +  $"[red]{Markup.Escape(category.Name)}[/]?");

        if (!confirm)
        {
            AnsiConsole.MarkupLine( "[yellow]Delete cancelled.[/]");
            return new OkResult();
        }

        try
        {
            var response = await _httpClient.DeleteAsync($"api/Category/{category.CategoryId}");

            if (response.IsSuccessStatusCode)
            {
                AnsiConsole.MarkupLine( $"[green]Deleted {Markup.Escape(category.Name)}.[/]");
                return new OkResult();
            }

            AnsiConsole.MarkupLine($"[red]Delete failed: {(int)response.StatusCode} " + $"{response.ReasonPhrase}[/]");
            return new StatusCodeResult((int)response.StatusCode);
        }
        catch (HttpRequestException ex)
        {
            AnsiConsole.MarkupLine( $"[red]Network error while deleting category: " +$"{Markup.Escape(ex.Message)}[/]");

            return new StatusCodeResult(StatusCodes.Status503ServiceUnavailable);
        }
    }

    public async Task<List<CategoryDto>> FetchAllCategoriesAsync()
    {
        var all = new List<CategoryDto>();

        int page = 1;
        int totalPages = 0;

        do
        {
            var response = await _httpClient.GetAsync( $"api/Category?pageNumber={page}&pageSize={FetchAllPageSize}");

            response.EnsureSuccessStatusCode();

            var result =await response.Content.ReadFromJsonAsync<PagedResponse<CategoryDto>>();

            if (result == null || result.Data.Count == 0)
                break;

            all.AddRange(result.Data);

            totalPages = (int)Math.Ceiling(
                result.TotalRecords / (double)FetchAllPageSize);

            page++;

        } while (page <= totalPages);

        return all;
    }

    public async Task<CategoryDto?> PickCategoryAsync(string title)
    {
        List<CategoryDto> categories;

        try
        {
            categories = await FetchAllCategoriesAsync();
        }
        catch (HttpRequestException ex)
        {
            AnsiConsole.MarkupLine(
                $"[red]Network error while fetching categories: " +
                $"{Markup.Escape(ex.Message)}[/]");

            return null;
        }

        return ApiSeriveHelper.PickOrNone(
            title,
            categories,
            c => $"#{c.CategoryId} | {c.Name}",
            emptyMessage: "No categories found.");
    }
}