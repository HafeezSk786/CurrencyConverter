# CurrencyConverter
**Web API project: Currency Converter
Target Framework: .NET 8
Authentication Pattern: JWT Token**

**SetUp Instructions:**
Once the code is downloaded from the Github, Build the solution in Visual Studio 2022.
There will be two projects:
1. CurrencyConvert (Web API project)
2. AuthControllerTest (Test project)

**Testing can be done in the following ways:**
First run the Web API project in the visual studio 2022.
you can see the swagger url in the browser
The Swagger UI lists down all the api methods including the Login method.

For Login method, it expects Username and password. Please provide "test" as username and "test" as password.

**Assumptions Made:**

The username and password for the Login method is assumed as "test" and "test". Please provide the same to generate the token 
by calling the Login Method from the Auth Controller in the postman.

**Possible Future Enhancements:**

The project is designed on the basis of Factory pattern so it can be extended for any other authprovider and currencyprovider
in the future.


**Web API urls:**

1) https://localhost:7189/swagger/index.html   [Please note the port can be different when running in your machine. 
please change accordingly]

2)Login url to generate token:
METHOD: POST
URL: https://localhost:7189/api/Auth/login

Request:
{
  "username": "test",
  "password": "test"
}


3)Method to get the latest exchange rates. NOTE: pass the bearer token generated in the Login method under "Authorization"
tab if running through POSTMAN.
METHOD: GET
URL: https://localhost:7189/GetLatestExchangeRates?baseCurrency=USD

4)Method to get the currency conversion. NOTE: pass the bearer token generated in the Login method under "Authorization"
tab if running through POSTMAN.
METHOD: GET
URL: https://localhost:7189/GetCurrencyConversion?currencyFrom=eur&currencyTo=usd&amount=10


5)Method to get the Historic rates. NOTE: pass the bearer token generated in the Login method under "Authorization"
tab if running through POSTMAN.
METHOD: GET
URL: https://localhost:7189/GetHistoricExchangeRates?baseCurrency=eur&startDate=2023-06-01&endDate=2024-12-31
