using System.Security.Claims;

namespace API.Extensions
{
    public static class ClaimsPrincipleExtensions
    {
        public static string GetUsername(this ClaimsPrincipal user)
        {
            //var claimsIdentity = user.Identity as System.Security.Claims.ClaimsIdentity;
            //var username = claimsIdentity.Name;
            return user.FindFirst(ClaimTypes.Name)?.Value;
        }
        
    }
}