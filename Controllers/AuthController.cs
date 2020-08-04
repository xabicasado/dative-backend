using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using DativeBackend.Models;

namespace DativeBackend.Controllers {
    [Route("Api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase {
        private readonly IConfiguration _configuration;
        private readonly CustomerContext _context;

        public AuthController(
            CustomerContext context, IConfiguration configuration) {
            _context = context;
            _configuration = configuration;
        }

        // POST: api/Auth
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Login(CustomerLoginDTO customerLoginDTO) {
            // https://code-maze.com/using-refresh-tokens-in-asp-net-core-authentication/
            if (customerLoginDTO == null) {
                return BadRequest();
            }

            var customer = _context.Customer.FirstOrDefault(c => (
                c.Username == customerLoginDTO.Username) && 
                (c.Password == customerLoginDTO.Password)
            );

            if (customer == null) {
                return Unauthorized();
            }
    
            return BuildToken(customerLoginDTO);
        }
        
        private ActionResult BuildToken(CustomerLoginDTO customerLoginDTO) {
            // Get the secret key from appsettings
            var secretKey = _configuration.GetValue<string>("SecretKey");
            var key = Encoding.UTF8.GetBytes(secretKey);
            var creds = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim> {
                new Claim(JwtRegisteredClaimNames.Sub, customerLoginDTO.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            
            var expiration = DateTime.UtcNow.AddHours(1);
            
            var token = new JwtSecurityToken(
                claims: claims,
                expires: expiration,
                signingCredentials: creds
            );

            return Ok(new {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }
    }
}
