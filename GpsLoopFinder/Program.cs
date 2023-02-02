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

double ToDegrees(double radians)
{
    return radians * 180 / Math.PI;
}

double CalculateSpeed(double distanceInMeter, double timeInSeconds)
{
    return distanceInMeter / timeInSeconds;
}