using DatingApp.Data;
using DatingApp.DTO;
using DatingApp.Entities;
using DatingApp.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DatingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly ITokenService _token;

        public AccountController(DataContext context, ITokenService token)
        {
            _context = context;
            _token = token;
        }

        [HttpPost("register")]
        public async Task<ActionResult> RegisterUser(RegisterDto register)
        {
            if (await UserExists(register.UserName))
            {
                return BadRequest("user is taken");
            }
            using var hmac = new HMACSHA512();
            var user = new AppUser
            {
                UserName = register.UserName,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(register.Password)),
                PasswordSalt = hmac.Key
            };

            await _context.Users.AddAsync(user).ConfigureAwait(false);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginUser)
        {
            var user = await _context.Users.FirstOrDefaultAsync(user => user.UserName.Equals(loginUser.UserName));

            if (user == null) return Unauthorized("invalid user name");

            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginUser.Password));
            var isCompletePassword = computedHash.AsEnumerable().All(hash => user.PasswordHash.AsEnumerable().Contains(hash));

            if (!isCompletePassword)
            {
                return Unauthorized("Password incorrect");
            }

            return new UserDto 
            {
                UserName = loginUser.UserName,
                Token = await _token.CreateToken(user).ConfigureAwait(false)
            };
        }

        private async Task<bool> UserExists(string userName)
        {
            return await _context.Users.AnyAsync(user => user.UserName.Equals(userName));
        }

    }
}
