using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Brupper.AspNetCore.Identity.Services.Users.IdentityConstants;

namespace Brupper.AspNetCore.Identity.Controllers;

[Route("api/[area]/[controller]")]
[Authorize(Roles = Roles.SuperAdmin)]
public class RolesController(RoleManager<IdentityRole> roleManager) : BaseController
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<IdentityRole>>> GetAllAsync()
    {
        var roles = await roleManager.Roles.ToListAsync();
        return Ok(roles);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<IdentityRole>> GetByIdAsync(string id)
    {
        var role = await roleManager.FindByIdAsync(id);
        if (role == null)
        {
            return NotFound();
        }

        return Ok(role);
    }

    [HttpPost]
    public async Task<ActionResult<IdentityRole>> CreateAsync([FromBody] CreateRoleRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.RoleName))
        {
            return BadRequest("Role name is required");
        }

        var role = new IdentityRole(request.RoleName.Trim());
        var result = await roleManager.CreateAsync(role);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return CreatedAtAction(nameof(GetByIdAsync), new { id = role.Id }, role);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<IdentityRole>> UpdateAsync(string id, [FromBody] UpdateRoleRequest request)
    {
        var role = await roleManager.FindByIdAsync(id);
        if (role == null)
        {
            return NotFound();
        }

        role.Name = request.RoleName?.Trim();
        var result = await roleManager.UpdateAsync(role);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return Ok(role);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(string id)
    {
        var role = await roleManager.FindByIdAsync(id);
        if (role == null)
        {
            return NotFound();
        }

        var result = await roleManager.DeleteAsync(role);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return NoContent();
    }
}

public class CreateRoleRequest
{
    public string RoleName { get; set; } = string.Empty;
}

public class UpdateRoleRequest
{
    public string RoleName { get; set; } = string.Empty;
}
