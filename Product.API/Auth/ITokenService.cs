namespace Product.API.Auth
{
    public interface ITokenService
    {
        string GenerateAccessToken(string username);
        string GenerateRefreshToken();
        DateTime GetAccessTokenExpiry();
    }
}
