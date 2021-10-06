using API.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using API.Entities;
using System.Security.Cryptography;
using System.Text;
using API.DTOs;
using Microsoft.EntityFrameworkCore;
using API.Interfaces;
using System.Linq;
using AutoMapper;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AccountController(DataContext context, ITokenService tokenService, IMapper mapper)
        {
            _tokenService = tokenService;
            _mapper = mapper;
            _context = context;

        }
        [HttpPost("register")]

        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (await UserExists(registerDto.Username))
            {
                return BadRequest("Username already exists");

            }
            var user = _mapper.Map<AppUser>(registerDto);
            using var hmac = new HMACSHA512();


                user.UserName = registerDto.Username.ToLower();
                user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
                user.PasswordSalt = hmac.Key;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return new UserDto {
                UserName = user.UserName,
                Token = _tokenService.CreateToken(user),
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };


        }
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {

            var user = await _context.Users
            .Include(p=> p.Photos).
            SingleOrDefaultAsync(x => x.UserName.ToLower() == loginDto.UserName.ToLower());

            if (user == null) return Unauthorized("Invalid User Name");

            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password");


            }
             return new UserDto {
                UserName = user.UserName,
                Token = _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x=> x.IsMain).Url,
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };;

        }

        private async Task<bool> UserExists(string UserName)
        {
            return await _context.Users.AnyAsync(x => x.UserName == UserName.ToLower());

        }
    }
}