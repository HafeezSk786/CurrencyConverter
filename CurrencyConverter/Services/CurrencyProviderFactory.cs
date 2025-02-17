using Polly.Registry;

namespace CurrencyConverter.Services
{
    public class CurrencyProviderFactory
    {
        public static ICurrencyProvider GetCurrencyProvider(string provider)
        {
            ICurrencyProvider? _currencyProvider = null;

            try
            {
                if (provider == "Frankfurter")
                {
                    _currencyProvider = new FrankfurterService();
                }

                return _currencyProvider!;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
