namespace CurrencyConverter.Services
{
    public class AuthProviderFactory
    {
        public static IAuthProvider GetAuthProvider(string provider)
        {
            IAuthProvider? _authProvider = null;
            try
            {
                if (provider == "JWT")
                {
                    _authProvider = new JWTTokenService();
                }

                return _authProvider!;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
