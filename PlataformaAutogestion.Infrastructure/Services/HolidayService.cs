using System.Text.Json;
using Microsoft.Extensions.Logging;
using PlataformaAutogestion.Application.DTOs;
using PlataformaAutogestion.Application.Interfaces;

namespace PlataformaAutogestion.Infrastructure.Services
{
    public class HolidayService : IHolidayService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<HolidayService> _logger;

        public HolidayService(HttpClient httpClient, ILogger<HolidayService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<HolidayDTO>> GetHolidaysByYearAsync(int year)
        {
            if (year < 2000 || year > 2100)
            {
                _logger.LogWarning(
                    "Se intentó consultar un año fuera de rango: {Year}",
                    year);

                return Enumerable.Empty<HolidayDTO>();
            }

            try
            {
                var response = await _httpClient.GetAsync($"?anio={year}");

                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();

                var holidays = JsonSerializer.Deserialize<IEnumerable<HolidayDTO>>(
                    json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                return holidays ?? Enumerable.Empty<HolidayDTO>();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(
                    ex,
                    "Error de red al consultar la API de feriados.");
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(
                    ex,
                    "Timeout: la API de feriados tardó demasiado.");
            }
            catch (JsonException ex)
            {
                _logger.LogError(
                    ex,
                    "Error parseando el JSON de feriados.");
            }
            catch (Exception ex)
            {
                _logger.LogCritical(
                    ex,
                    "Error crítico inesperado en HolidayService.");
            }

            return Enumerable.Empty<HolidayDTO>();
        }

        public async Task<bool> IsHolidayAsync(DateTime date)
        {
            var holidays = await GetHolidaysByYearAsync(date.Year);

            return holidays.Any(h => h.Date.Date == date.Date);
        }
    }
}