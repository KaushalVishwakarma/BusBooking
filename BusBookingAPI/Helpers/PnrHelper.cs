namespace BusBookingAPI.Helpers;

public static class PnrHelper
{
    public static string Generate() =>
        "PNR" + DateTime.UtcNow.ToString("yyyyMMdd") + Random.Shared.Next(1000, 9999);
}
