using System;
using System.Collections.Generic;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared;
using TrackerPi.Gps;

namespace TrackerPi.Services;

/// <summary>
/// Background service collecting gps position and sending it to the server in intervals
/// </summary>
public class GpsCollectionService : BackgroundService
{
    private readonly ILogger<GpsCollectionService> _logger;
    private readonly Sim7600GpsReader _gpsReader;
    private readonly HttpClient _httpClient;

    public GpsCollectionService(ILogger<GpsCollectionService> logger, Sim7600GpsReader gpsReader, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _gpsReader = gpsReader;
        _httpClient = httpClientFactory.CreateClient("GpsTracker");
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation($"Background gps collection service started");

        try
        {
            _gpsReader.Open();
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var position = new GpsPosition();
                    var maxAttempts = 10;
                    for (int i = 0 ; i <maxAttempts; i++ )
                    {
                        var line = _gpsReader.ReadNextSentence();
                        if (line != null)
                        {
                            position = _gpsReader.ParseGpgga(line);
                            if (position != null)
                            {
                                var content = new StringContent(
                                    System.Text.Json.JsonSerializer.Serialize(position),
                                    System.Text.Encoding.UTF8,
                                    "application/json"
                                );

                                using var response = await _httpClient.PostAsync("api/gpsposition", content, stoppingToken);

                                if (response.IsSuccessStatusCode)
                                {
                                    _logger.LogInformation($"Gps position sent successfully: Lat={position.Latitude}, Lon={position.Longitude}");
                                }
                                else
                                {
                                    _logger.LogInformation($"Gps position failed to send. Status: {response.StatusCode}");

                                }
                            }
                        }
                    }

                    await Task.Delay(1000, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error collecting gps position: {ex.Message}");
                    throw;
                }
                await Task.Delay(30000, stoppingToken);
            }
        }
        finally
        {
            _gpsReader.Close();
            _logger.LogInformation($"gps collection shutting down...");
        }
    }
}