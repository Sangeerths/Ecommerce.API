using Ecommerce.API.DTO.Pagination;
using Ecommerce.API.DTO.Sale;
using Ecommerce.UI.DTO.Sale;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Spectre.Console;
using System.Net.Http.Json;
namespace ECommerce.UI.Services;

public class SaleApiService
{
    private const int DefaultPageSize = 10;
    private const int FetchAllPageSize = 50;

    private readonly HttpClient _httpClient;
    private readonly ProductApiService _productapi;

    public SaleApiService(HttpClient httpClient, ProductApiService productapi)
    {
        _httpClient = httpClient;
        _productapi = productapi;
    }

    public async Task<IActionResult> GetAllSalesApi()
    {
        int pageNumber = ApiSeriveHelper.AskPositiveInt("Enter [green]Page Number[/]:", 1);
        int pageSize = ApiSeriveHelper.AskPositiveInt("Enter [green]Page Size[/]:", DefaultPageSize);

        PagedResponse<SaleDto>? result;
        try
        {
            var response = await _httpClient.GetAsync($"api/Sale?pageNumber={pageNumber}&pageSize={pageSize}");
            if (!response.IsSuccessStatusCode)
            {
                AnsiConsole.MarkupLine($"[red]Failed to fetch sales. Status code: {response.StatusCode}[/]");
                return new StatusCodeResult((int)response.StatusCode);
            }

            result = await response.Content.ReadFromJsonAsync<PagedResponse<SaleDto>>();
        }
        catch (HttpRequestException ex)
        {
            AnsiConsole.MarkupLine($"[red]Network error while fetching sales: {Markup.Escape(ex.Message)}[/]");
            return new StatusCodeResult(StatusCodes.Status503ServiceUnavailable);
        }

        if (result == null || result.Data.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No sales found.[/]");
            return new OkObjectResult(result);
        }

        var table = new Table().AddColumns("Id", "Items", "SaleDate","CustomerName","TotalPrice"); 
        foreach (var sale in result.Data)
        {
            table.AddRow(
                sale.SaleId.ToString(),
                sale.Items.Count.ToString(),
                sale.SaleDate.ToString("yyyy-MM-dd"),
                Markup.Escape(sale.CustomerName ?? string.Empty),
                sale.TotalPrice.ToString("C"));
        }
        AnsiConsole.Write(table);

        AnsiConsole.MarkupLine(
            $"Page [green]{result.PageNumber}[/] of size [green]{result.PageSize}[/] " +
            $"({result.TotalRecords} total)");

        return new OkObjectResult(result);
    }

    public async Task<IActionResult> CreateSaleApi()
    {
        string customerName = ApiSeriveHelper.AskNonEmptyString("Enter [green]Customer Name[/]:");
        var items = new List<SaleItemRequestDto>();

        while (true)
        {
            var pickedProduct = await _productapi.PickProductAsync("Select the [green]product[/]:");
            if (pickedProduct == null)
            {
                if (items.Count == 0)
                {
                    AnsiConsole.MarkupLine("[yellow]No product selected. Sale creation cancelled.[/]");
                    return new OkResult();
                }

                break;
            }

            AnsiConsole.MarkupLine($"Price: [green]{pickedProduct.Price:C}[/] | " +$"Stock: [green]{pickedProduct.StockQuantity}[/]");

            int quantity = ApiSeriveHelper.AskPositiveInt("Enter [green]Quantity[/]:");

            if (quantity > pickedProduct.StockQuantity)
            {
                AnsiConsole.MarkupLine($"[red]Only {pickedProduct.StockQuantity} units available.[/]");
                continue;
            }

            items.Add(new SaleItemRequestDto
            {
                ProductId = pickedProduct.ProductId,
                Quantity = quantity
            });

            bool addAnother = ApiSeriveHelper.Confirm("Add another product?",defaultValue: false);
            if (!addAnother)
            {
                break;
            }
        }

        var sale = new SaleRequestDto
        {
            CustomerName = customerName,
            Items = items
        };

        return await SendSaleRequestAsync(() => _httpClient.PostAsJsonAsync("api/Sale", sale),"created","create");
    }

    public async Task<IActionResult> UpdateSaleApi()
    {
        var selectedSale = await PickSalesAsync("Select the [green]sale[/] to update (or Back to cancel):");
        if (selectedSale == null)
        {
            AnsiConsole.MarkupLine("[yellow]Update cancelled.[/]");
            return new OkResult();
        }

        int saleId = selectedSale.SaleId;

        string customerName = ApiSeriveHelper.AskNonEmptyString("Enter [green]Customer Name[/]:",selectedSale.CustomerName);

        var items = new List<SaleItemRequestDto>();

        foreach (var existingItem in selectedSale.Items)
        {
            AnsiConsole.MarkupLine($"\n[bold]{Markup.Escape(existingItem.ProductName)}[/] " +$"(Current quantity: [green]{existingItem.Quantity}[/])");
            bool changeProduct = ApiSeriveHelper.Confirm("Change this product?",defaultValue: false);
            int productId = existingItem.ProductId;

            if (changeProduct)
            {
                var pickedProduct = await _productapi.PickProductAsync("Select the [green]new product[/]:");
                if (pickedProduct != null)
                {
                    productId = pickedProduct.ProductId;
                }
                else
                {
                    AnsiConsole.MarkupLine("[yellow]Keeping current product.[/]");
                }
            }

            int quantity = ApiSeriveHelper.AskPositiveInt("Enter [green]Quantity[/]:", existingItem.Quantity);

            items.Add(new SaleItemRequestDto
            {
                ProductId = productId,
                Quantity = quantity
            });
        }

        AnsiConsole.MarkupLine("\n[bold]Review changes:[/]");
        AnsiConsole.MarkupLine($"Customer Name: [green]{Markup.Escape(customerName)}[/]");
        foreach (var item in items)
        {
            AnsiConsole.MarkupLine($"Product ID: [green]{item.ProductId}[/] | " +$"Quantity: [green]{item.Quantity}[/]");
        }

        AnsiConsole.MarkupLine("");

        if (!ApiSeriveHelper.Confirm( "Save these changes?",defaultValue: true))
        {
            AnsiConsole.MarkupLine("[yellow]Update cancelled.[/]");
            return new OkResult();
        }

        var sale = new SaleRequestDto
        {
            CustomerName = customerName,
            Items = items
        };

        return await SendSaleRequestAsync(
            () => _httpClient.PutAsJsonAsync(
                $"api/Sale/{saleId}",
                sale),
            "updated",
            "update");
    }

    public async Task<IActionResult> DeleteSaleApi()
    {
        var selectedSale = await PickSalesAsync("Select the [green]sale[/] to delete (or Back to cancel):");
        if (selectedSale == null)
        {
            AnsiConsole.MarkupLine("[yellow]Delete cancelled.[/]");
            return new OkResult();
        }

        bool confirm = ApiSeriveHelper.Confirm(
            $"Are you sure you want to delete sale [red]#{selectedSale.SaleId}[/] for [red]{Markup.Escape(selectedSale.CustomerName)}[/]?");

        if (!confirm)
        {
            AnsiConsole.MarkupLine("[yellow]Delete cancelled.[/]");
            return new OkResult();
        }

        try
        {
            var response = await _httpClient.DeleteAsync($"api/Sale/{selectedSale.SaleId}");
            if (response.IsSuccessStatusCode)
            {
                AnsiConsole.MarkupLine("[green]Sale deleted successfully![/]");
                return new OkResult();
            }

            AnsiConsole.MarkupLine($"[red]Failed to delete sale. Status code: {response.StatusCode}[/]");
            return new StatusCodeResult((int)response.StatusCode);
        }
        catch (HttpRequestException ex)
        {
            AnsiConsole.MarkupLine($"[red]Network error while deleting sale: {Markup.Escape(ex.Message)}[/]");
            return new StatusCodeResult(StatusCodes.Status503ServiceUnavailable);
        }
    }

    public async Task<List<SaleDto>> FetchAllSalesAsync()
    {
        var all = new List<SaleDto>();
        int page = 1;
        int totalPages;

        do
        {
            var response = await _httpClient.GetAsync($"api/Sale?pageNumber={page}&pageSize={FetchAllPageSize}");
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<PagedResponse<SaleDto>>();
            if (result == null || result.Data.Count == 0) break;

            all.AddRange(result.Data);
            totalPages = (int)Math.Ceiling(result.TotalRecords / (double)FetchAllPageSize);
            page++;
        } while (page <= totalPages);

        return all;
    }

    public async Task<SaleDto?> PickSalesAsync(string title)
    {
        List<SaleDto> sales;
        try
        {
            sales = await FetchAllSalesAsync();
        }
        catch (HttpRequestException ex)
        {
            AnsiConsole.MarkupLine($"[red]Network error while fetching sales: {Markup.Escape(ex.Message)}[/]");
            return null;
        }

        return ApiSeriveHelper.PickOrNone(
         title,
         sales,
         s => $"#{s.SaleId} | {s.SaleDate:yyyy-MM-dd} | " +
              $"{s.CustomerName} | Items: {s.Items.Count} | {s.TotalPrice:C}",
         emptyMessage: "No sales found.");
    }

    // ---------- Shared helper ----------

    private static async Task<IActionResult> SendSaleRequestAsync(
        Func<Task<HttpResponseMessage>> requestFunc, string pastTenseVerb, string baseVerb)
    {
        try
        {
            var response = await requestFunc();
            if (response.IsSuccessStatusCode)
            {
                AnsiConsole.MarkupLine($"[green]Sale {pastTenseVerb} successfully![/]");
                return new OkResult();
            }

            AnsiConsole.MarkupLine($"[red]Failed to {baseVerb} sale. Status code: {response.StatusCode}[/]");
            return new StatusCodeResult((int)response.StatusCode);
        }
        catch (HttpRequestException ex)
        {
            AnsiConsole.MarkupLine($"[red]Network error: {Markup.Escape(ex.Message)}[/]");
            return new StatusCodeResult(StatusCodes.Status503ServiceUnavailable);
        }
    }
}