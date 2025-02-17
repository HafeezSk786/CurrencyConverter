using CurrencyConverter.Models;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyConverter.Services
{
    public interface IAuthProvider
    {
        Task<IActionResult> GetBearerToken(UserLogin userLogin);

        bool ValidateTkn(string token);
    }
}
