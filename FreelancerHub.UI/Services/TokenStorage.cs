namespace FreelancerHub.UI.Services;

public class TokenStorage
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private const string CookieName = "X-Access-Token";

    public TokenStorage(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? GetToken()
    {
        return _httpContextAccessor.HttpContext?.Request.Cookies[CookieName];
    }

    public void SaveToken(string token)
    {
        _httpContextAccessor.HttpContext?.Response.Cookies.Append(CookieName, token);
    }

    public void ClearToken()
    {
        _httpContextAccessor.HttpContext?.Response.Cookies.Delete(CookieName);
    }
}