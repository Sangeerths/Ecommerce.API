using ECommerce.UI.Services;
using Spectre.Console;
namespace ECommerce.UI.Menu;

public class MenuUI
{
     public enum MenuOptions
    {
        Category ,
        Product,
        Sale,
        Exit
    }
    private enum ProductOptions
    {
        AddProduct,
        UpdateProduct,
        DeleteProduct,
        ListProducts,
        Back
    }

    private enum CategoryOptions
    {
        AddCategory,
        UpdateCategory,
        DeleteCategory,
        ListCategories,
        Back
    }

    private enum SaleOptions
    {
        AddSale,
        UpdateSale,
        DeleteSale,
        ListSales,
        Back
    }
    private readonly ProductApiService _productService;
    private readonly SaleApiService _saleApiService;
    private readonly CategoryApiService _categoryApiService;
    private readonly ConsoleHelper _ui;
    public MenuUI(ProductApiService productService, ConsoleHelper ui, SaleApiService saleApiService, CategoryApiService categoryApiService)
    {
        _productService = productService;
        _ui = ui;
        _saleApiService = saleApiService;
        _categoryApiService = categoryApiService;
    }

    public async Task OnStart()
    {
        bool isRunning = true;
        while(isRunning)
        {
            _ui.ShowHeader();
            var choice = AnsiConsole.Prompt(new SelectionPrompt<MenuOptions>().Title("Choose your operation").AddChoices(
            MenuOptions.Product,
            MenuOptions.Category,
            MenuOptions.Sale,
            MenuOptions.Exit));

            switch(choice)
            {
                case MenuOptions.Product:
                    await ProductApiManagement();
                    break;
                case MenuOptions.Category:
                    await CategoryApiManagment();
                    break;
                case MenuOptions.Sale:
                    await SaleApiManagment();
                    break;
                case MenuOptions.Exit:
                    isRunning = false;
                    break;

            }
        }
        _ui.ShowGoodbye();
        _ui.Pause();
    }

    public async Task ProductApiManagement()
    {
        bool isRunning = true;
        while (isRunning)
        {
            _ui.ShowHeader();
            var choice = AnsiConsole.Prompt(new SelectionPrompt<ProductOptions>().Title("Choose your operation").AddChoices(
            ProductOptions.AddProduct,
            ProductOptions.UpdateProduct,
            ProductOptions.DeleteProduct,
            ProductOptions.ListProducts,
            ProductOptions.Back));
              switch (choice)
            {
                case ProductOptions.AddProduct:
                  await  _productService.CreateProductApi();
                    break;
                case ProductOptions.UpdateProduct:
                   await _productService.UpdateProductApi();
                    break;
                case ProductOptions.DeleteProduct:
                    await _productService.DeleteProductApi();
                    break;
                case ProductOptions.ListProducts:
                    await _productService.GetAllProductsApi();
                    break;
                case ProductOptions.Back:
                    isRunning = false;
                    break;
            }
            if (choice != ProductOptions.Back)
                _ui.Pause();
        }
    }

    public async Task CategoryApiManagment()
    {
        bool isRunning = true;
        while (isRunning)
        {
            _ui.ShowHeader();
            var choice = AnsiConsole.Prompt(new SelectionPrompt<CategoryOptions>().Title("Choose your operation").AddChoices(
            CategoryOptions.AddCategory,
            CategoryOptions.UpdateCategory,
            CategoryOptions.DeleteCategory,
            CategoryOptions.ListCategories,
            CategoryOptions.Back));
            switch (choice)
            {
                case CategoryOptions.AddCategory:
                   await _categoryApiService.CreateCategoryApi();
                    break;
                case CategoryOptions.UpdateCategory:
                   await _categoryApiService.UpdateCategoryApi();
                    break;
                case CategoryOptions.DeleteCategory:
                   await _categoryApiService.DeleteCategoryApi();
                    break;
                case CategoryOptions.ListCategories:
                    await _categoryApiService.GetAllCategoriesApi();
                    break;
                case CategoryOptions.Back:
                    isRunning = false;
                    break;
            }
            if(choice != CategoryOptions.Back)
            _ui.Pause();
        }
    }
    public async Task SaleApiManagment()
    {
        bool isRunning = true;
        while (isRunning)
        {
            _ui.ShowHeader();
            var choice = AnsiConsole.Prompt(new SelectionPrompt<SaleOptions>().Title("Choose your operation").AddChoices(
            SaleOptions.AddSale,
            SaleOptions.UpdateSale,
            SaleOptions.DeleteSale,
            SaleOptions.ListSales,
            SaleOptions.Back));
            switch (choice)
            {
                case SaleOptions.AddSale:
                    await _saleApiService.CreateSaleApi();
                    break;
                case SaleOptions.UpdateSale:
                    await _saleApiService.UpdateSaleApi();
                    break;
                case SaleOptions.DeleteSale:
                    await _saleApiService.DeleteSaleApi();
                    break;
                case SaleOptions.ListSales:
                    await _saleApiService.GetAllSalesApi();
                    break;
                case SaleOptions.Back:
                    isRunning = false;
                    break;
            }
            if (choice != SaleOptions.Back)
                _ui.Pause();
        }
    }
}
