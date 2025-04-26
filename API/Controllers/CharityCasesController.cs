using Core;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;


public class CharityCasesController(IGenericRepository<CharityCase> repo) : BaseApiController
{

  [HttpGet]
  public async Task<ActionResult<IReadOnlyCollection<CharityCase>>> GetCharityCases([FromQuery]CharityCaseSpecParams specParams )
  {

    var spec = new CharityCaseSpecification(specParams);
    return await CreatePagedResult(repo, spec,  specParams.PageIndex,specParams.PageSize);
  }
  [HttpGet("{id}")] // api/cases/1
  public async Task<ActionResult<CharityCase>> GetCharityCase(int id)
  {
    return await repo.GetByIdAsync(id);
  }
  [HttpGet("categories")]
  public async Task<ActionResult<IReadOnlyList<string>>> GetCategoriess(){
    var spec = new CategoryListSpecification();

    return Ok(await repo.ListAsync(spec));
  }
[HttpGet("urgencyLevels")]
  public async Task<ActionResult<IReadOnlyList<string>>> GetLevels(){
    var spec = new LevelListSpecification();
    return Ok(await repo.ListAsync(spec));
  }

  [Authorize]
  [HttpPost]
  public async Task<ActionResult<CharityCase>> CreateCharityCase(CharityCase charityCase)
  {
    repo.Add(charityCase);
    if (await repo.SaveAlllAsync())
    {
      return CreatedAtAction("GetCharityCase", new { id = charityCase.Id }, charityCase);
    }

    return BadRequest("Problem creating charity case");
  }
  [Authorize]

  [HttpPut("{id:int}")]
  public async Task<ActionResult> UpdateCharityCase(int id, CharityCase charityCase)
  {
    if (charityCase.Id != id || !CharityCaseExits(id))
      return BadRequest("Cannot update this request");

    repo.Update(charityCase);
    if (await repo.SaveAlllAsync())
    {
      return NoContent();
    }
    return BadRequest("Problem updating this case");

  }

  [Authorize(Roles="Admin")]

  [HttpDelete("{id:int}")]
  public async Task<ActionResult> DeleteCharityCase(int id)
  {
    var charityCase = await repo.GetByIdAsync(id);
    if (charityCase == null) return NotFound();

    repo.Remove(charityCase);
    if (await repo.SaveAlllAsync())
    {
      return NoContent();
    }
    return BadRequest("Problem deleting  this case");

  }
  private bool CharityCaseExits(int id)
  {
    return repo.Exists(id);
  }

}
