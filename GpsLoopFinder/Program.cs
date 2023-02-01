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
                var speed = CalcualteSpeed(distance, 1d) * 3.6d;
                Console.WriteLine($"Longitude: {point.X}, Latitude: {point.Y}, Distance:{distance} M, Speed: {speed} KM/H");
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

double CalcualteSpeed(double distanceInMeter, double timeInSeconds)
{
    return distanceInMeter / timeInSeconds;
}