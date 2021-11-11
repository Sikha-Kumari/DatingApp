using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenService tokenService, IMapper mapper)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _tokenService = tokenService;
            _mapper = mapper;

        }
        [HttpPost("register")]

        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (await UserExists(registerDto.Username))
            {
                return BadRequest("Username already exists");

            }
            var user = _mapper.Map<AppUser>(registerDto);
            user.UserName = registerDto.Username.ToLower();

            var result = await _userManager.CreateAsync(user,registerDto.Password);

            if(!result.Succeeded) return BadRequest(result.Errors);
            
            var roleResult = await _userManager.AddToRoleAsync(user,"Member");

            if(!roleResult.Succeeded) return BadRequest(result.Errors);

            return new UserDto
            {
                UserName = user.UserName,
                Token = await _tokenService.CreateToken(user),
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };


        }
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {

            var user = await _userManager.Users
            .Include(p => p.Photos).
            SingleOrDefaultAsync(x => x.UserName.ToLower() == loginDto.UserName.ToLower());

            if (user == null) return Unauthorized("Invalid User Name");

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if(!result.Succeeded) return Unauthorized();


            return new UserDto
            {
                UserName = user.UserName,
                Token = await _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };

        }

        private async Task<bool> UserExists(string UserName)
        {
            return await _userManager.Users.AnyAsync(x => x.UserName == UserName.ToLower());

        }
    }
}