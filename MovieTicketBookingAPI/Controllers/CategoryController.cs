using BusinessObjects;
using BusinessObjects.Dtos.Category;
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
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<CategoryDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<CategoryDto>>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseModel<IEnumerable<CategoryDto>>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel<IEnumerable<CategoryDto>>>> GetAll()
        {
            try
            {
                var categories = await _categoryService.GetAll();
                if (categories == null || !categories.Any())
                    return NotFound(new ResponseModel<IEnumerable<CategoryDto>>()
                    {
                        Data = null,
                        Error = "Not found any categories!",
                        Success = false,
                        ErrorCode = 404
                    });

                var categoryDtos = categories.Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Type = c.Type
                }).ToList();

                return Ok(new ResponseModel<IEnumerable<CategoryDto>>()
                {
                    Data = categoryDtos,
                    Error = null,
                    Success = true,
                    ErrorCode = 200
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<IEnumerable<CategoryDto>>()
                {
                    Data = null,
                    Error = ex.Message,
                    Success = false,
                    ErrorCode = 500
                });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResponseModel<CategoryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<CategoryDto>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseModel<CategoryDto>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel<CategoryDto>>> GetById(int id)
        {
            try
            {
                var category = await _categoryService.GetById(id);
                if (category == null)
                    return NotFound(new ResponseModel<CategoryDto>()
                    {
                        Data = null,
                        Error = $"Not found category with id {id}",
                        Success = false,
                        ErrorCode = 404
                    });
                    
                var categoryDto = new CategoryDto
                {
                    Id = category.Id,
                    Type = category.Type
                };

                return Ok(new ResponseModel<CategoryDto>()
                {
                    Data = categoryDto,
                    Error = null,
                    Success = true,
                    ErrorCode = 200
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<CategoryDto>()
                {
                    Data = null,
                    Error = ex.Message,
                    Success = false,
                    ErrorCode = 500
                });
            }
        }

        [HttpGet("search/{name}")]
        [ProducesResponseType(typeof(ResponseModel<CategoryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<CategoryDto>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseModel<CategoryDto>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel<CategoryDto>>> GetByName(string name)
        {
            try
            {
                var category = await _categoryService.getByCateName(name);
                if (category == null) 
                    return NotFound(new ResponseModel<CategoryDto>()
                    {
                        Data = null,
                        Error = $"Not found category with name {name}",
                        Success = false,
                        ErrorCode = 404
                    });
                var categoryDto = new CategoryDto
                {
                    Id = category.Id,
                    Type = category.Type
                };
                return Ok(new ResponseModel<CategoryDto>()
                {
                    Data = categoryDto,
                    Error = null,
                    Success = true,
                    ErrorCode = 200
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<CategoryDto>()
                {
                    Data = null,
                    Error = ex.Message,
                    Success = false,
                    ErrorCode = 500
                });
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResponseModel<CategoryDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ResponseModel<CategoryDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseModel<CategoryDto>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel<CategoryDto>>> Create([FromBody] Category category)
        {
            try
            {
                var createdCategory = await _categoryService.Add(category);
                
                var categoryDto = new CategoryDto
                {
                    Id = createdCategory.Id,
                    Type = createdCategory.Type
                };
                
                return CreatedAtAction(nameof(GetById), new { id = createdCategory.Id },
                    new ResponseModel<CategoryDto>()
                    {
                        Data = categoryDto,
                        Error = null,
                        Success = true,
                        ErrorCode = 201
                    });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<CategoryDto>()
                {
                    Data = null,
                    Error = ex.Message,
                    Success = false,
                    ErrorCode = 500
                });
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ResponseModel<CategoryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<CategoryDto>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseModel<CategoryDto>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel<CategoryDto>>> Update(int id, [FromBody] Category category)
        {
            try
            {
                var existingCategory = await _categoryService.GetById(id);
                if (existingCategory == null)
                    return NotFound(new ResponseModel<CategoryDto>()
                    {
                        Data = null,
                        Error = $"Not found category with id {id}",
                        Success = false,
                        ErrorCode = 404
                    });

                category.Id = id;
                await _categoryService.Update(category);
                
                var categoryDto = new CategoryDto
                {
                    Id = category.Id,
                    Type = category.Type
                };
                
                return Ok(new ResponseModel<CategoryDto>()
                {
                    Data = categoryDto,
                    Error = null,
                    Success = true,
                    ErrorCode = 200
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<CategoryDto>()
                {
                    Data = null,
                    Error = ex.Message,
                    Success = false,
                    ErrorCode = 500
                });
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ResponseModel<CategoryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel<CategoryDto>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseModel<CategoryDto>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel<CategoryDto>>> Delete(int id)
        {
            try
            {
                var existingCategory = await _categoryService.GetById(id);
                if (existingCategory == null)
                    return NotFound(new ResponseModel<CategoryDto>()
                    {
                        Data = null,
                        Error = $"Not found category with id {id}",
                        Success = false,
                        ErrorCode = 404
                    });

                await _categoryService.Delete(id);
                
                var categoryDto = new CategoryDto
                {
                    Id = existingCategory.Id,
                    Type = existingCategory.Type
                };
                
                return Ok(new ResponseModel<CategoryDto>()
                {
                    Data = categoryDto,
                    Error = null,
                    Success = true,
                    ErrorCode = 200
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<CategoryDto>()
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
