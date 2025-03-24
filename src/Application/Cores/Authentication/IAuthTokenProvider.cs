namespace Application.Cores.Authentication;

public interface IAuthTokenProvider
{
    Task<string?> GetAccessToken();
}