namespace E_Learner.Interfaces
{
    public interface IAuthenticationServices
    {
        bool IsAuthenticated(HttpContext httpContext);

    }
}
