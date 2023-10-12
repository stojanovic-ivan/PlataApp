using Newtonsoft.Json;

public class ExchangeRateHelper
{
    private readonly IHttpClientFactory _clientFactory;
    private decimal _rateEUR;
    private decimal _rateUSD;

    public ExchangeRateHelper(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public async Task<(decimal rateEUR, decimal rateUSD)> GetExchangeRatesAsync()
    {
        if (_rateEUR == 0 || _rateUSD == 0)
        {
            var apiUri = "https://api.fastforex.io/fetch-multi?from=RSD&to=EUR,USD&api_key=demo";
            var httpClient = _clientFactory.CreateClient();

            HttpResponseMessage apiResponse = await httpClient.GetAsync(new Uri(apiUri));

            if (apiResponse.IsSuccessStatusCode)
            {
                var content = await apiResponse.Content.ReadAsStringAsync();
                var konverzija = JsonConvert.DeserializeObject<Konverzija>(content);

                if (konverzija != null && konverzija.Results.ContainsKey("EUR") && konverzija.Results.ContainsKey("USD"))
                {
                    _rateEUR = konverzija.Results["EUR"];
                    _rateUSD = konverzija.Results["USD"];
                }
            }
        }

        return (_rateEUR, _rateUSD);
    }
}