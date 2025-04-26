using System;
using System.Security.Claims;
using API.DTOs;
using API.Extensions;
using API.Services;
using Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController: BaseApiController
{
    private readonly SignInManager<AppUser> _signInManager;
    private readonly ITokenService _tokenService;

    public AccountController(SignInManager<AppUser> signInManager, ITokenService tokenService)
    {
        _signInManager = signInManager;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
  public async Task<ActionResult> Register(RegisterDto registerDto){
    var user = new AppUser{
      FirstName = registerDto.FirstName,
      LastName = registerDto.LastName,
      Email = registerDto.Email,
      UserName = registerDto.Email

    };

    var result = await _signInManager.UserManager.CreateAsync(user, registerDto.Password);
    if(!result.Succeeded) {
      foreach(var error in result.Errors){
        ModelState.AddModelError(error.Code, error.Description);
      }
      return ValidationProblem();
    }
    return Ok();
  }
  [Authorize]
  [HttpPost("logout")]
  public async Task<ActionResult> Logout(){
    await _signInManager.SignOutAsync();
    return NoContent();
  }
  [HttpGet("auth-status")]
  public ActionResult GetAuthState(){
    return Ok(new{ IsAuthenticated =User.Identity?.IsAuthenticated ?? false});
  }

    [Authorize]
    [HttpGet("user-info")]
    public async Task<ActionResult> GetUserInfo()
    {
        var email = User.FindFirstValue(ClaimTypes.Email);
        if (email == null) return Unauthorized();

        var user = await _signInManager.UserManager.FindByEmailAsync(email);
        if (user == null) return NotFound();

        return Ok(new
        {
            user.FirstName,
            user.LastName,
            user.Email,
            Roles = User.FindFirstValue(ClaimTypes.Role)
        });
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user = await _signInManager.UserManager.FindByEmailAsync(loginDto.Email);
        if (user == null) return Unauthorized("Invalid email");

        var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
        if (!result.Succeeded) return Unauthorized("Invalid password");

        var token = _tokenService.CreateToken(user);  // Generate JWT token
        return Ok(new UserDto
        {
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Token = token
        });
    }
}
