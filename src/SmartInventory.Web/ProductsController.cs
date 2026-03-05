using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SmartInventory.Core.Entities;
using SmartInventory.Core.Interfaces;
using SmartInventory.Services;
using SmartInventory.Services.DTOs;

namespace SmartInventory.Web.Controllers
{
    /// <summary>
    /// Controller for managing Products API endpoints.
    /// Supports dev-mode DTOs via FakeProductService and production EF Core service.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly FakeProductService? _fakeService; // Optional: DTO-only service for dev mode
        private readonly IMapper _mapper;
        private readonly IValidator<CreateProductDto> _createValidator;
        private readonly IValidator<UpdateProductDto> _updateValidator;

        /// <summary>
        /// Constructor for ProductsController.
        /// Injects product service, AutoMapper, validators, and optionally the fake DTO service.
        /// </summary>
        public ProductsController(
            IProductService productService,
            IMapper mapper,
            IValidator<CreateProductDto> createValidator,
            IValidator<UpdateProductDto> updateValidator,
            FakeProductService? fakeService = null)
        {
            _productService = productService;
            _mapper = mapper;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _fakeService = fakeService; // Injected only in dev mode
        }

        // ---------------------------
        // GET: /api/products
        // ---------------------------

        /// <summary>
        /// Retrieves all products.
        /// Uses DTO-aware methods in dev mode for frontend simulation.
        /// Maps entities to DTOs in production.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            if (_fakeService != null)
            {
                // Dev mode: return DTOs directly
                var dtos = await _fakeService.GetAllDtosAsync();
                return Ok(dtos);
            }

            // Production mode: map entity objects to DTOs for frontend
            var products = await _productService.GetAllAsync();
            var mapped = _mapper.Map<IEnumerable<ProductDto>>(products);
            return Ok(mapped);
        }

        // ---------------------------
        // GET: /api/products/top
        // ---------------------------

        /// <summary>
        /// Returns top N products for dashboard.
        /// Dev mode returns DTOs directly.
        /// </summary>
        [HttpGet("top")]
        public async Task<IActionResult> GetTop([FromQuery] int count = 5)
        {
            if (_fakeService != null)
            {
                var dtos = await _fakeService.GetTopDtosAsync(count);
                return Ok(dtos);
            }

            var products = await _productService.GetTopAsync(count);
            var mapped = _mapper.Map<IEnumerable<ProductDto>>(products);
            return Ok(mapped);
        }

        // ---------------------------
        // GET: /api/products/paged
        // ---------------------------

        /// <summary>
        /// Returns paginated and optionally filtered products.
        /// Frontend can request page, pageSize, categoryId, or search term.
        /// </summary>
        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] Guid? categoryId = null,
            [FromQuery] string? search = null)
        {
            if (_fakeService != null)
            {
                // Dev mode: use DTO-aware method
                var (items, totalCount) = await _fakeService.GetPagedDtosAsync(page, pageSize, categoryId, search);
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
                return Ok(new { data = items, totalCount, page, pageSize, totalPages });
            }

            // Production: map entities to DTOs
            var (products, totalCountProd) = await _productService.GetPagedAsync(page, pageSize, categoryId, search);
            var mapped = _mapper.Map<IEnumerable<ProductDto>>(products);
            var totalPagesMapped = (int)Math.Ceiling((double)totalCountProd / pageSize);
            return Ok(new { data = mapped, totalCount = totalCountProd, page, pageSize, totalPages = totalPagesMapped });
        }

        // ---------------------------
        // GET: /api/products/{id}
        // ---------------------------

        /// <summary>
        /// Retrieves a product by its ID.
        /// Returns NotFound if no matching product exists.
        /// </summary>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Get(Guid id)
        {
            if (_fakeService != null)
            {
                var dto = await _fakeService.GetByIdDtoAsync(id);
                return dto is null ? NotFound() : Ok(dto);
            }

            var product = await _productService.GetByIdAsync(id);
            return product is null ? NotFound() : Ok(_mapper.Map<ProductDto>(product));
        }

        // ---------------------------
        // POST: /api/products
        // ---------------------------

        /// <summary>
        /// Creates a new product from a DTO.
        /// Validates input using FluentValidation.
        /// Maps DTO to entity before storing in database.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
        {
            // Validate input
            var validation = await _createValidator.ValidateAsync(dto);
            if (!validation.IsValid)
                return BadRequest(validation.Errors);

            // Map DTO -> entity
            var productEntity = _mapper.Map<Product>(dto);

            // Create product in database
            var created = await _productService.CreateAsync(productEntity);

            // Map entity -> DTO for response
            return CreatedAtAction(nameof(Get), new { id = created.Id }, _mapper.Map<ProductDto>(created));
        }

        // ---------------------------
        // PUT: /api/products/{id}
        // ---------------------------

        /// <summary>
        /// Updates an existing product.
        /// Validates DTO input, maps to entity, and updates database.
        /// </summary>
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, UpdateProductDto dto)
        {
            var validation = await _updateValidator.ValidateAsync(dto);
            if (!validation.IsValid)
                return BadRequest(validation.Errors);

            if (id != dto.Id)
                return BadRequest();

            var productEntity = _mapper.Map<Product>(dto);
            await _productService.UpdateAsync(productEntity);

            return NoContent();
        }

        // ---------------------------
        // DELETE: /api/products/{id}
        // ---------------------------

        /// <summary>
        /// Deletes a product by ID.
        /// Returns NoContent on success.
        /// </summary>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _productService.DeleteAsync(id);
            return NoContent();
        }
    }
}