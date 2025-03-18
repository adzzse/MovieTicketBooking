using BusinessObjects;
using BusinessObjects.Dtos.Role;
using BusinessObjects.Dtos.Schema_Response;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;
using Services.Service;

namespace MovieTicketBookingAPI.Controllers
{
    [Route("api/roles")]
    [ApiController]
    public class RoleController(IRoleService roleService) : ControllerBase
    {
        private readonly IRoleService _roleService = roleService;

        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseModel<RoleDto>>> GetRoleById(int id)
        {
            try
            {
                var role = await _roleService.GetById(id);
                if (role == null)
                {
                    return NotFound(new ResponseModel<RoleDto>
                    {
                        Success = false,
                        Error = "Role not found",
                        ErrorCode = 404
                    });
                }
                var roleDto = new RoleDto
                {
                    Id = role.Id,
                    Name = role.Name,
                };
                return Ok(new ResponseModel<RoleDto>
                {
                    Success = true,
                    Data = roleDto
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<RoleDto> { Success = false, Error = ex.Message, ErrorCode = 500 });
            }
        }

        [HttpGet]
        public async Task<ActionResult<ResponseModel<RoleDto>>> GetAllRoles()
        {
            try
            {
                var roles = await _roleService.GetAll();
                var roleResponses = roles.Select(role => new RoleDto
                {
                    Id = role.Id,
                    Name = role.Name,
                });
                return Ok(new ResponseModel<IEnumerable<RoleDto>>
                {
                    Success = true,
                    Data = roleResponses
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<IEnumerable<RoleDto>>
                {
                    Success = false,
                    Error = ex.Message,
                    ErrorCode = 500
                });
            }
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(ResponseModel<RoleDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ResponseModel<RoleDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseModel<RoleDto>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel<RoleDto>>> AddRole([FromForm] CreateRoleDto role)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ResponseModel<RoleDto>
                    {
                        Success = false,
                        Error = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)),
                        ErrorCode = 400
                    });
                }

                var roleEntity = new Role
                {
                    Name = role.Name
                };

                var result = await _roleService.Add(roleEntity);
                var roleDto = new RoleDto
                {
                    Id = result.Id,
                    Name = result.Name
                };

                return CreatedAtAction(nameof(GetRoleById), new { id = result.Id }, new ResponseModel<RoleDto>
                {
                    Success = true,
                    Data = roleDto
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<RoleDto>
                {
                    Success = false,
                    Error = ex.Message,
                    ErrorCode = 500
                });
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ResponseModel<RoleDto>), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ResponseModel<RoleDto>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseModel<RoleDto>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel<RoleDto>>> UpdateRole(int id, [FromBody] CreateRoleDto role)
        {
            try
            {
                var existingRole = await _roleService.GetById(id);
                if (existingRole == null)
                {
                    return NotFound(new ResponseModel<RoleDto>
                    {
                        Success = false,
                        Error = "Role not found",
                        ErrorCode = 404
                    });
                }

                existingRole.Name = role.Name;
                await _roleService.Update(existingRole);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<RoleDto>
                {
                    Success = false,
                    Error = ex.Message,
                    ErrorCode = 500
                });
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ResponseModel<RoleDto>), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ResponseModel<RoleDto>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseModel<RoleDto>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseModel<RoleDto>>> DeleteRole(int id)
        {
            try
            {
                var existingRole = await _roleService.GetById(id);
                if (existingRole == null)
                {
                    return NotFound(new ResponseModel<RoleDto>
                    {
                        Success = false,
                        Error = "Role not found",
                        ErrorCode = 404
                    });
                }

                await _roleService.Delete(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<RoleDto>
                {
                    Success = false,
                    Error = ex.Message,
                    ErrorCode = 500
                });
            }
        }
    }
}
