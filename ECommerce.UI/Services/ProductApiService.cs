using Ecommerce.API.DTO.Pagination;
using ECommerce.UI.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Spectre.Console;
using System.Net.Http.Json;
namespace ECommerce.UI.Services;

public class ProductApiService
{
    private const int DefaultPageSize = 10;
    private const int FetchAllPageSize = 50;

    private readonly HttpClient _httpClient;
    private readonly CategoryApiService _categoryApiService;

    public ProductApiService(HttpClient httpClient, CategoryApiService categoryApiService)
    {
        _httpClient = httpClient;
        _categoryApiService = categoryApiService;
    }

    public async Task<IActionResult> GetAllProductsApi()
    {
        int pageNumber = ApiSeriveHelper.AskPositiveInt("Enter [green]Page Number[/]:", 1);
        int pageSize = ApiSeriveHelper.AskPositiveInt("Enter [green]Page Size[/]:", DefaultPageSize);

        PagedResponse<ProductDto>? result;

        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync( $"api/Product?pageNumber={pageNumber}&pageSize={pageSize}");

            if (!response.IsSuccessStatusCode)
            {
                AnsiConsole.MarkupLine( $"[red]Failed to fetch products. Status code: {response.StatusCode}[/]");
                return new StatusCodeResult((int)response.StatusCode);
            }

            result = await response.Content.ReadFromJsonAsync<PagedResponse<ProductDto>>();
        }
        catch (HttpRequestException ex)
        {
            AnsiConsole.MarkupLine($"[red]Network error while fetching products: {Markup.Escape(ex.Message)}[/]");

            return new StatusCodeResult(StatusCodes.Status503ServiceUnavailable);
        }

        if (result == null || result.Data.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No products found.[/]");
            return new OkObjectResult(result);
        }

        Table table = new Table().AddColumns( "Id", "Name", "Description","Price", "Stock Quantity");

        foreach (ProductDto product in result.Data)
        {
            table.AddRow(
                product.ProductId.ToString(),
                Markup.Escape(product.Name ?? string.Empty),
                Markup.Escape(product.Description ?? string.Empty),
                product.Price.ToString("C"),
                product.StockQuantity.ToString());
        }

        AnsiConsole.Write(table);
        AnsiConsole.MarkupLine( $"Page [green]{result.PageNumber}[/] of size [green]{result.PageSize}[/] " + $"({result.TotalRecords} total)");
        return new OkObjectResult(result);
    }

    public async Task<IActionResult> CreateProductApi()
    {
        string name = ApiSeriveHelper.AskNonEmptyString("Enter [green]Product Name[/]:");
        string description = ApiSeriveHelper.AskNonEmptyString("Enter [green]Product Description[/]:");
        decimal price = ApiSeriveHelper.AskPositiveDecimal("Enter [green]Product Price[/]:");
        int stockQuantity = ApiSeriveHelper.AskPositiveInt("Enter [green]Product Stock Quantity[/]:");
        var category = await _categoryApiService.PickCategoryAsync("Select the [green]category[/]:");

        if (category == null)
        {
            AnsiConsole.MarkupLine("[yellow]No category selected. Product creation cancelled.[/]");
            return new OkResult();
        }

        var product = new ProductRequestDto
        {
            Name = name,
            Description = description,
            Price = price,
            StockQuantity = stockQuantity,
            CategoryId = category.CategoryId
        };

        try
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("api/Product",product);

            if (response.IsSuccessStatusCode)
            {
                AnsiConsole.MarkupLine("[green]Product created successfully.[/]");
                ProductDto? createdProduct =await response.Content.ReadFromJsonAsync<ProductDto>();
                return new OkObjectResult(createdProduct);
            }

            AnsiConsole.MarkupLine($"[red]Create failed: {(int)response.StatusCode} " + $"{response.ReasonPhrase}[/]");
            return new StatusCodeResult((int)response.StatusCode);
        }
        catch (HttpRequestException ex)
        {
            AnsiConsole.MarkupLine( $"[red]Network error while creating product: " +$"{Markup.Escape(ex.Message)}[/]");
            return new StatusCodeResult(StatusCodes.Status503ServiceUnavailable);
        }
    }

    public async Task<IActionResult> UpdateProductApi()
    {
        ProductDto? product;

        try
        {
            product = await PickProductAsync("Select the [green]product[/] to update:");
        }
        catch (HttpRequestException ex)
        {
            AnsiConsole.MarkupLine( $"[red]Network error while fetching products: " +$"{Markup.Escape(ex.Message)}[/]");
            return new StatusCodeResult(StatusCodes.Status503ServiceUnavailable);
        }

        if (product == null)
        {
            AnsiConsole.MarkupLine("[yellow]Update cancelled.[/]");
            return new OkResult();
        }

        string name = ApiSeriveHelper.AskNonEmptyString("Enter [green]Product Name[/]:",product.Name);
        string description = ApiSeriveHelper.AskNonEmptyString("Enter [green]Product Description[/]:",product.Description);
        decimal price = ApiSeriveHelper.AskPositiveDecimal("Enter [green]Product Price[/]:",product.Price);
        int stockQuantity = ApiSeriveHelper.AskPositiveInt("Enter [green]Product Stock Quantity[/]:",product.StockQuantity);
        CategoryDto? category = await _categoryApiService.PickCategoryAsync("Select the [green]category[/] to update:");
        int categoryId = category?.CategoryId ?? product.CategoryId;

        AnsiConsole.MarkupLine("\n[bold]Review changes:[/]");
        AnsiConsole.MarkupLine($"Name: [green]{Markup.Escape(name)}[/]");
        AnsiConsole.MarkupLine($"Description: [green]{Markup.Escape(description)}[/]");
        AnsiConsole.MarkupLine($"Price: [green]{price:C}[/]");
        AnsiConsole.MarkupLine($"Stock Quantity: [green]{stockQuantity}[/]");
        AnsiConsole.MarkupLine($"Category ID: [green]{categoryId}[/]\n");

        if (!ApiSeriveHelper.Confirm("Save these changes?",defaultValue: true))
        {
            AnsiConsole.MarkupLine("[yellow]Update cancelled.[/]");
            return new OkResult();
        }

        var updatedProduct = new ProductDto
        {
            ProductId = product.ProductId,
            Name = name,
            Description = description,
            Price = price,
            StockQuantity = stockQuantity,
            CategoryId = categoryId
        };

        try
        {
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"api/Product/{product.ProductId}",updatedProduct);

            if (response.IsSuccessStatusCode)
            {
                AnsiConsole.MarkupLine("[green]Product updated successfully.[/]");
                var update =await response.Content.ReadFromJsonAsync<ProductDto>();
                return new OkObjectResult(update);
            }

            AnsiConsole.MarkupLine( $"[red]Update failed: {(int)response.StatusCode} " +$"{response.ReasonPhrase}[/]");
            return new StatusCodeResult((int)response.StatusCode);
        }
        catch (HttpRequestException ex)
        {
            AnsiConsole.MarkupLine($"[red]Network error while updating product: " +$"{Markup.Escape(ex.Message)}[/]");

            return new StatusCodeResult(StatusCodes.Status503ServiceUnavailable);
        }
    }

    public async Task<IActionResult> DeleteProductApi()
    {
        ProductDto? product;

        try
        {
            product = await PickProductAsync( "Select the [green]product[/] to delete:");
        }
        catch (HttpRequestException ex)
        {
            AnsiConsole.MarkupLine($"[red]Network error while fetching products: " +$"{Markup.Escape(ex.Message)}[/]");
            return new StatusCodeResult(StatusCodes.Status503ServiceUnavailable);
        }

        if (product == null)
        {
            AnsiConsole.MarkupLine("[yellow]Delete cancelled.[/]");
            return new OkResult();
        }

        bool confirm = ApiSeriveHelper.Confirm($"Are you sure you want to delete product " +
            $"[red]{Markup.Escape(product.Name)}[/]?");

        if (!confirm)
        {
            AnsiConsole.MarkupLine("[yellow]Delete cancelled.[/]");
            return new OkResult();
        }

        try
        {
            var response = await _httpClient.DeleteAsync($"api/Product/{product.ProductId}");

            if (response.IsSuccessStatusCode)
            {
                AnsiConsole.MarkupLine($"[green]Deleted {Markup.Escape(product.Name)} successfully.[/]");
                return new OkResult();
            }

            AnsiConsole.MarkupLine($"[red]Delete failed: {(int)response.StatusCode} " +$"{response.ReasonPhrase}[/]");
            return new StatusCodeResult((int)response.StatusCode);
        }
        catch (HttpRequestException ex)
        {
            AnsiConsole.MarkupLine( $"[red]Network error while deleting product: " +$"{Markup.Escape(ex.Message)}[/]");

            return new StatusCodeResult(StatusCodes.Status503ServiceUnavailable);
        }
    }

    public async Task<List<ProductDto>> FetchAllProductsAsync()
    {
        var all = new List<ProductDto>();
        int page = 1;
        int totalPages = 0;

        do
        {
            var response = await _httpClient.GetAsync($"api/Product?pageNumber={page}&pageSize={FetchAllPageSize}");

            response.EnsureSuccessStatusCode();

            var result =await response.Content.ReadFromJsonAsync<PagedResponse<ProductDto>>();

            if (result == null || result.Data.Count == 0)
                break;

            all.AddRange(result.Data);

            totalPages = (int)Math.Ceiling(
                result.TotalRecords / (double)FetchAllPageSize);

            page++;

        } while (page <= totalPages);

        return all;
    }

    public async Task<ProductDto?> PickProductAsync(string title)
    {
        List<ProductDto> products;

        try
        {
            products = await FetchAllProductsAsync();
        }
        catch (HttpRequestException)
        {
            throw;
        }

        return ApiSeriveHelper.PickOrNone(
    title,
    products,
    p => $"{p.Name} | {p.Price:C} | Stock: {p.StockQuantity}");
    }
}