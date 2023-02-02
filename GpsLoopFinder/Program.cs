using System.Security.Cryptography.X509Certificates;
using Aspose.Gis;
using Aspose.Gis.Geometries;

var layer = Drivers.Gpx.OpenLayer(@"../../../../../../Downloads/example.gpx");

foreach (var feature in layer)
{
    // Check for MultiLineString geometry
    if (feature.Geometry.GeometryType == GeometryType.MultiLineString)
    {
        // Read track
        var lines = (MultiLineString)feature.Geometry;
        foreach(var line in lines)
        {
            var points = (LineString)line.ToLinearGeometry();
            var lastPoint = points[0];
            foreach (var point in points)
            {
                var distance = CalculateDistance(lastPoint.X, lastPoint.Y, point.X, point.Y);
                var speed = CalculateSpeed(distance, 1d) * 3.6d;
                var bearing = CalculateBearing(lastPoint.X, lastPoint.Y, point.X, point.Y);
                Console.WriteLine($"Longitude: {point.X}, Latitude: {point.Y}, Distance:{distance} M, Speed: {speed} KM/H, Bearing: {bearing}");
                lastPoint = point;
            }
        }
    }
}

// https://www.movable-type.co.uk/scripts/latlong.html
double CalculateDistance(double startLon, double startLat, double endLon, double endLat)
{
    var result = 0d;
    var earthRadius = 6_371_000d;
    var angleConvertValue = Math.PI / 180d;

    var lat1Angle = startLat * angleConvertValue;
    var lat2Angle = endLat * angleConvertValue;
    var latDeltaAngle = (endLat - startLat) * angleConvertValue;
    var lonDeltaAngle = (endLon - startLon) * angleConvertValue;

    var a = Math.Sin(latDeltaAngle / 2f) * Math.Sin(latDeltaAngle / 2f) +
            Math.Cos(lat1Angle) * Math.Cos(lat2Angle) *
            Math.Sin(lonDeltaAngle / 2f) * Math.Sin(lonDeltaAngle / 2f);
    var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

    result = earthRadius * c;

    return result;
}

/*
47.88189778116764, 20.37680305248897
47.88189180427088, 20.37681142192834
Distance:0,9117285828999017 M, 
Speed: 3,282222898439646 KM/H, 
Bearing: 136,79807491415391
*/

double CalculateBearing(double startLon, double startLat, double endLon, double endLat)
{
    double dLon = ToRadians(endLon - startLon);
    var startLatRad = ToRadians(startLat);
    var endLatRad = ToRadians(endLat);

    double y = Math.Sin(dLon) * Math.Cos(endLatRad);
    double x = Math.Cos(startLatRad) * Math.Sin(endLatRad) -
               Math.Sin(startLatRad) * Math.Cos(endLatRad) * Math.Cos(dLon);
    double bearing = Math.Atan2(y, x);
    bearing = ToDegrees(bearing);
    bearing = (bearing + 360) % 360;

    return bearing;
}

double ToRadians(double angle)
{
    return Math.PI * angle / 180.0;
}


double EarthRadius = 6371.0;
double Radian = Math.PI / 180.0;


public class Coordinate
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public Coordinate(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }
}

double Distance(Coordinate coord1, Coordinate coord2)
{
    double latitude1 = coord1.Latitude * Radian;
    double latitude2 = coord2.Latitude * Radian;
    double longitude1 = coord1.Longitude * Radian;
    double longitude2 = coord2.Longitude * Radian;
    double deltaLatitude = latitude2 - latitude1;
    double deltaLongitude = longitude2 - longitude1;
    double a = Math.Sin(deltaLatitude / 2) * Math.Sin(deltaLatitude / 2) +
               Math.Cos(latitude1) * Math.Cos(latitude2) *
               Math.Sin(deltaLongitude / 2) * Math.Sin(deltaLongitude / 2);
    double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
    double distance = EarthRadius * c;

    return distance;
}

static Coordinate Midpoint(Coordinate coord1, Coordinate coord2)
{
    double latitude1 = coord1.Latitude * Radian;
    double latitude2 = coord2.Latitude * Radian;
    double longitude1 = coord1.Longitude * Radian;
    double longitude2 = coord2.Longitude * Radian;
    double deltaLongitude = longitude2 - longitude1;

    double x = Math.Cos(latitude2) * Math.Cos(deltaLongitude);
    double y = Math.Cos(latitude2) * Math.Sin(deltaLongitude);

    double latitude3 = Math.Atan2(
        Math.Sin(latitude1) + Math.Sin(latitude2),
        Math.Sqrt(
            (Math.Cos(latitude1) + x) *
            (Math.Cos(latitude1) + x) + y * y));
    double longitude3 = longitude1 + Math.Atan2(y, Math.Cos(latitude1) + x);

    return new Coordinate(latitude3 / Radian, longitude3 / Radian);
}

double ToDegrees(double radians)
{
    return radians * 180 / Math.PI;
}

double CalculateSpeed(double distanceInMeter, double timeInSeconds)
{
    return distanceInMeter / timeInSeconds;
}