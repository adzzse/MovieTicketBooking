using BusinessObjects;
using BusinessObjects.Dtos.Schema_Response;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;
using Services.Service;

namespace MovieTicketBookingAPI.Controllers
{
    [Route("api/categories")]
    [ApiController]
    public class CategoryController(ICategoryService categoryService) : ControllerBase
    {
        private readonly ICategoryService _categoryService = categoryService;

        [HttpGet]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<Category>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<Category>>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<Category>>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel<IEnumerable<Category>>>> GetAll()
        {
            try
            {
                var categories = await _categoryService.GetAll();
                
                // Check if categories list is empty and return 404 if it is
                if (categories == null || !categories.Any())
                {
                    return NotFound(new ResponseModel<IEnumerable<Category>>()
                    {
                        Data = null,
                        Error = "No categories found",
                        Success = false,
                        ErrorCode = 404
                    });
                }
                
                return Ok(new ResponseModel<IEnumerable<Category>>()
                {
                    Data = categories,
                    Error = null,
                    Success = true,
                    ErrorCode = 200
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<IEnumerable<Category>>()
                {
                    Data = null,
                    Error = ex.Message,
                    Success = false,
                    ErrorCode = 500
                });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResponseModel<Category>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<Category>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseModel<Category>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel<Category>>> GetById(int id)
        {
            try
            {
                var category = await _categoryService.GetById(id);
                if (category == null) 
                    return NotFound(new ResponseModel<Category>()
                    {
                        Data = null,
                        Error = $"Not found category with id {id}",
                        Success = false,
                        ErrorCode = 404
                    });
                return Ok(new ResponseModel<Category>()
                {
                    Data = category,
                    Error = null,
                    Success = true,
                    ErrorCode = 200
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<Category>()
                {
                    Data = null,
                    Error = ex.Message,
                    Success = false,
                    ErrorCode = 500
                });
            }
        }

        [HttpGet("search/{name}")]
        [ProducesResponseType(typeof(ResponseModel<Category>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<Category>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseModel<Category>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel<Category>>> GetByName(string name)
        {
            try
            {
                var category = await _categoryService.getByCateName(name);
                if (category == null) 
                    return NotFound(new ResponseModel<Category>()
                    {
                        Data = null,
                        Error = $"Not found category with name {name}",
                        Success = false,
                        ErrorCode = 404
                    });
                return Ok(new ResponseModel<Category>()
                {
                    Data = category,
                    Error = null,
                    Success = true,
                    ErrorCode = 200
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<Category>()
                {
                    Data = null,
                    Error = ex.Message,
                    Success = false,
                    ErrorCode = 500
                });
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResponseModel<Category>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ResponseModel<Category>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseModel<Category>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel<Category>>> Create([FromBody] Category category)
        {
            try
            {
                var createdCategory = await _categoryService.Add(category);
                return CreatedAtAction(nameof(GetById), new { id = createdCategory.Id }, 
                    new ResponseModel<Category>()
                    {
                        Data = createdCategory,
                        Error = null,
                        Success = true,
                        ErrorCode = 201
                    });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<Category>()
                {
                    Data = null,
                    Error = ex.Message,
                    Success = false,
                    ErrorCode = 500
                });
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ResponseModel<Category>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<Category>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseModel<Category>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel<Category>>> Update(int id, [FromBody] Category category)
        {
            try
            {
                var existingCategory = await _categoryService.GetById(id);
                if (existingCategory == null) 
                    return NotFound(new ResponseModel<Category>()
                    {
                        Data = null,
                        Error = $"Not found category with id {id}",
                        Success = false,
                        ErrorCode = 404
                    });

                category.Id = id;
                await _categoryService.Update(category);
                return Ok(new ResponseModel<Category>()
                {
                    Data = category,
                    Error = null,
                    Success = true,
                    ErrorCode = 200
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<Category>()
                {
                    Data = null,
                    Error = ex.Message,
                    Success = false,
                    ErrorCode = 500
                });
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ResponseModel<Category>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<Category>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseModel<Category>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel<Category>>> Delete(int id)
        {
            try
            {
                var existingCategory = await _categoryService.GetById(id);
                if (existingCategory == null) 
                    return NotFound(new ResponseModel<Category>()
                    {
                        Data = null,
                        Error = $"Not found category with id {id}",
                        Success = false,
                        ErrorCode = 404
                    });

                await _categoryService.Delete(id);
                return Ok(new ResponseModel<Category>()
                {
                    Data = existingCategory,
                    Error = null,
                    Success = true,
                    ErrorCode = 200
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<Category>()
                {
                    Data = null,
                    Error = ex.Message,
                    Success = false,
                    ErrorCode = 500
                });
            }
        }
    }
}
