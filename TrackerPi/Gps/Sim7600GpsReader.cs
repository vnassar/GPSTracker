using System;
using System.IO.Ports;
using Microsoft.Extensions.Logging;
using Shared;

namespace TrackerPi.Gps
{
    public class Sim7600GpsReader : IDisposable
    {
        
        private readonly SerialPort _serialPort;
        private readonly ILogger<Sim7600GpsReader> _logger;

        private readonly object _lock = new();


        public Sim7600GpsReader(string portName, int baudRate = 4800, ILogger<Sim7600GpsReader>? logger = null)
        {
            _serialPort = new SerialPort(portName, baudRate)
            {
                ReadTimeout = 500,
                WriteTimeout = 500,
                NewLine = "\r\n"
            };
            _logger = logger ?? LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<Sim7600GpsReader>();
        }

        /// <summary>
        /// Open serial port and read gps data 
        /// </summary>
        public void Open()
        {
            try 
            {
                _serialPort.Open();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "failed to open gps port {port}", _serialPort.PortName);
            }

        }

        public string? ReadNextSentence()
        {
            lock (_lock)
            {
                try
                {
                    if (!_serialPort.IsOpen)
                    {
                        _logger.LogWarning("Serial port is not open");
                        return null;
                    }

                    var message = _serialPort.ReadLine();

                    if (string.IsNullOrWhiteSpace(message)) return null;

                    if (message.StartsWith("$GPPA")) return message;

                    return null;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "error reading gps data");
                    return null;
                }
            }
        }


        public GpsPosition? ParseGpgga(string message)
        {
            var parts = message.Split(',');

            if (parts.Length < 15)
            {
                _logger.LogWarning("invalid gpgga sentence {Message}", message);
                return null;
            }

            var latitude = ConvertNmeaToDecimal(parts[2], parts[3]);
            var longitude = ConvertNmeaToDecimal(parts[4], parts[5]);

            var fixIndicator = parts[6];

            if (fixIndicator != "1" && fixIndicator != "2")
            {
                _logger.LogWarning("bad gps fix, quality: {Quality}", fixIndicator);
                return null;
            }

            var position = new GpsPosition
            {
              TimeStamp = DateTime.Now,
              Latitude = latitude,
              Longitude = longitude
            };

            return position;
        }

        private double ConvertNmeaToDecimal(string coordinate, string direction)
        {
            var degrees = double.Parse(coordinate.Substring(0, 2));
            var minutes = double.Parse(coordinate.Substring(2));

            var decimalDegrees = degrees + (minutes / 60);

            if (direction == "S" || direction == "W") decimalDegrees = -decimalDegrees;

            return decimalDegrees;
        }

        public void Close()
        {
            lock (_lock)
            {
                if (_serialPort.IsOpen)
                {
                    _serialPort.Close();
                }
            }
        }
        public void Dispose()
        {
            Close();
            _serialPort.Dispose();
        }
    }
}