using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PostRedisCRUD.Entity;
using PostRedisCRUD.Repositories;

namespace PostRedisCRUD.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController : ControllerBase
{
    #region Ctor
    private readonly IProductRepository _productRepository;

    public ProductController(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }
    #endregion


    #region Get Product By Id

    [HttpGet("[action]/{Id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Product>> GetProductBYId(int Id)
    {
        try
        {
            var product = await _productRepository.GetProductById(Id);

            return Ok(product);
        }
        catch (Exception ex)
        {

            return BadRequest(ex.Message);
        }

    }

    #endregion

    #region Get All Poducts

    [HttpGet("[action]")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<Product>>> GetListOfProducts()
    {
        try
        {
            var prods = await _productRepository.GetProducts();

            return (prods);
        }
        catch (Exception ex)
        {

            return BadRequest(ex.Message);
        }

    }

    #endregion

    #region Add New Product

    [HttpPost("[action]")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddNewProduct([FromBody] Product product)
    {
        try
        {
            await _productRepository.AddNewProduct(product);
            return Ok();
        }
        catch (Exception ex)
        {

            return BadRequest(ex.Message);
        }

    }

    #endregion

    #region Update Product

    [HttpPut("[action]")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateProduct([FromBody] Product product)
    {
        try
        {
            await _productRepository.UpdateProduct(product);
            return Ok();
        }
        catch (Exception ex)
        {

            return BadRequest(ex.Message);
        }

    }

    #endregion

    #region Delete Product

    [HttpDelete("[action]/{Id}")]
    public async Task<IActionResult> DeleteProduct(int Id)
    {
        try
        {
            await _productRepository.DeleteProduct(Id);

            return Ok();


        }
        catch (Exception ex)
        {

            return BadRequest(ex.Message);
        }
    }

    #endregion

}
