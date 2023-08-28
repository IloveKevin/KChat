using EFCore.Entity;
using Model.HttpModel.Response;
using Model.JWT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface IAccountService
    {
		Token? Login(string account, string password);
        Task<Token?> LoginAsync(string account, string password);
        User? Register(string account, string password);
        Task<User?> RegisterAsync(string account, string password);
        Token? RefreshToken(string refreshToken);
        Task<Token?> RefreshTokenAsync(string refreshToken);
        Task<bool> UpdateNickName(long userId,string nickName);
        Task<List<UserInfo>> GetUserInfo(List<long> usersId);
    }
}
