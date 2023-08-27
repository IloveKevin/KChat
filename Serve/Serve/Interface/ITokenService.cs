using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface ITokenService
    {

        public void OnConfigration(JwtBearerOptions options);
        public string IssuaToken(long expires, Claim[] claims);
        public JwtSecurityToken? Get(string token);
    }
}
