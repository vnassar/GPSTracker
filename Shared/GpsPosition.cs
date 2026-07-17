namespace Shared;

/// <summary>
/// GPS position with timestamp
/// </summary>
public class GpsPosition
{
    public long Id {get;set;}
    public DateTime TimeStamp {get;set;}
    public double Latitude {get;set;}
    public double Longitude {get;set;}
}
