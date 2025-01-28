using Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class BuggyController : BaseApiController
{
  [HttpGet("unauthorized")]
  public IActionResult GetUnauthorized()
  {
    return Unauthorized();
  }

  [HttpGet("badRequest")]
  public IActionResult GetBadRequest()
  {
    return BadRequest("not good request");
  }
  [HttpGet("notFound")]
  public IActionResult GetNotFound()
  {
    return NotFound();
  }
  [HttpGet("internalError")]
  public IActionResult GetInternalError()
  {
    throw new Exception("this is a test exceptioon");
  }
  [HttpPost("validationError")]
  public IActionResult GetValidationError(CreateCharityCaseDTO charityCase)
  {
    return Ok();
  }
}
