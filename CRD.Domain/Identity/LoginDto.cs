using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRD.Domain.Identity
{
    public class LoginDto
    {
        public LoginDto(bool succeed, string message, string token, UserDto user)
        {
            Succeed = succeed;
            Message = message;
            Token = token;
            User = user;
        }

        public bool Succeed { get; set; }
        public string Message { get; set; }
        public string Token { get; set; }
        public UserDto User { get; set; }
    }
    public class UserDto
    {

        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public IEnumerable<string> Roles { get; set; }=new List<string>();
    }
}
