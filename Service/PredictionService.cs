public class PredictionService
{
    private readonly HttpClient _http;
    private readonly IConfiguration _config;

    public PredictionService(HttpClient http, IConfiguration config)
    {
        _http = http;
        _config = config;
    }

    public async Task<double> PredictAsync(string parkId, DateTime time)
{
    var response = await _http.PostAsJsonAsync(
        $"{_config["FastApi:BaseUrl"]}/predict",
        new
        {
            park_id = parkId,
            prediction_time = time.ToString("yyyy-MM-ddTHH:mm:ss")
        }
    );

    response.EnsureSuccessStatusCode();

    var result = await response.Content
        .ReadFromJsonAsync<PredictionResponse>();

    return result.predicted_occupancy_ratio;
}


}
