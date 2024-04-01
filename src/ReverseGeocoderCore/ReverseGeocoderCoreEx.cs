using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ReverseGeocoding.Interface;
using ReverseGeocoding.KdTree;
using ReverseGeocoding.Model;

namespace ReverseGeocoding
{
    public class ReverseGeocoderCoreEx : IReverseGeocoderCore
    {
        private readonly KdTree<PlaceInfoEx> _tree;

        private ReverseGeocoderCoreEx(IEnumerable<IPlaceInfo> geoNames) => _tree = new KdTree<PlaceInfoEx>(geoNames.Cast<PlaceInfoEx>().ToArray());

        public ReverseGeocoderCoreEx(GeoDb geoDb) : this(geoDb.Places)
        {
            if (geoDb == null)
            {
                throw new ArgumentNullException(nameof(geoDb));
            }
        }

        public ReverseGeocoderCoreEx(Stream placesDb, Stream countryInfoDb = null) : this(new GeoDb(placesDb, countryInfoDb))
        {
            if (placesDb == null)
            {
                throw new ArgumentNullException(nameof(placesDb));
            }
        }

        public ReverseGeocoderCoreEx(string placesDbPath, string countryInfoDbPath = null) : this(new GeoDb(placesDbPath, countryInfoDbPath , false))
        {
            if (string.IsNullOrWhiteSpace(placesDbPath))
            {
                throw new ArgumentNullException(nameof(placesDbPath));
            }
        }

        public IPlaceInfo GetNearestPlace(double latitude, double longitude) => _tree.FindNearest(new PlaceInfoEx { Latitude = latitude, Longitude= longitude });

        public string GetNearestPlaceName(double latitude, double longitude) => _tree.FindNearest(new PlaceInfoEx{ Latitude = latitude, Longitude = longitude })?.Name;
    }
}
