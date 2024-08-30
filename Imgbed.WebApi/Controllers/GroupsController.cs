using Imgbed.WebApi.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Imgbed.WebApi.Controllers;

[Route("[controller]")]
public class GroupsController : ControllerBase
{
    public GroupsController()
    {

    }

    [HttpGet("")]
    public async Task<ActionResult<IList<Group>>> GetAll()
    {
        return Ok();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> Get([FromRoute] Guid id)
    {
        return Ok();
    }

    [HttpPost("")]
    public ActionResult<Group> Create([FromBody] Group group)
    {
        return Created();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete([FromRoute] Guid id)
    {
        return BadRequest();
    }
}
