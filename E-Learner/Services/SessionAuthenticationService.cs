using E_Learner.Interfaces;

namespace E_Learner.Services
{
    public class SessionAuthenticationService : IAuthenticationServices
    {
        public bool IsAuthenticated(HttpContext httpContext)
        {
            // Check if the user is authenticated based on session
            return httpContext.Session.GetInt32("UserId") != null;
        }
    }
}
