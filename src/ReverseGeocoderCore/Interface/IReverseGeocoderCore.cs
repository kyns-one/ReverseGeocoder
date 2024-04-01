namespace ReverseGeocoding.Interface
{
    public interface IReverseGeocoderCore
    {
        IPlaceInfo GetNearestPlace(double latitude, double longitude);
        string GetNearestPlaceName(double latitude, double longitude);
    }
}
