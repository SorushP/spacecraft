using Weather.Data;

namespace Weather.Services
{
    public class WeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly WeatherRepository _repository;
        public WeatherService(HttpClient httpClient, WeatherRepository repository)
        {
            _httpClient = httpClient;
            _repository = repository;
        }

        public async Task<string?> FetchWeatherDataAsync()
        {
            try
            {
                // NOTE: Use this code to simulate failures easily.
                // SimulateFailure(0.1);

                var response = await _httpClient.GetAsync("https://api.open-meteo.com/v1/forecast?latitude=52.52&longitude=13.41&hourly=temperature_2m,relativehumidity_2m,windspeed_10m");
                response.EnsureSuccessStatusCode();
                var res = await response.Content.ReadAsStringAsync();

                if (res is not null)
                {
                    // Ignore awaining for data update process to speed up responding.
                    _repository.SaveWeatherDataAsync(res);
                }
            }
            catch
            {
            }

            return _repository.Cache;
        }

        static void SimulateFailure(double prob)
        {
            bool shouldFail = new Random().NextDouble() < prob;

            if (shouldFail)
            {
                throw new Exception("Random failure occurred.");
            }
        }
    }
}