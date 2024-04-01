using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ReverseGeocoding.Interface;
using ReverseGeocoding.KdTree;
using ReverseGeocoding.Model;

namespace ReverseGeocoding
{
    public class ReverseGeocoderCore : IReverseGeocoderCore
    {
        private readonly KdTree<PlaceInfo> _tree;

        private ReverseGeocoderCore(IEnumerable<IPlaceInfo> geoNames) => _tree = new KdTree<PlaceInfo>(geoNames.Cast<PlaceInfo>().ToArray());

        public ReverseGeocoderCore(GeoDb geoDb) : this(geoDb.Places)
        {
            if (geoDb == null)
            {
                throw new ArgumentNullException(nameof(geoDb));
            }
        }

        public ReverseGeocoderCore(Stream placesDb, Stream countryInfoDb = null) : this(new GeoDb(placesDb, countryInfoDb))
        {
            if (placesDb == null)
            {
                throw new ArgumentNullException(nameof(placesDb));
            }
        }

        public ReverseGeocoderCore(string placesDbPath, string countryInfoDbPath = null) : this(new GeoDb(placesDbPath, countryInfoDbPath))
        {
            if (string.IsNullOrWhiteSpace(placesDbPath))
            {
                throw new ArgumentNullException(nameof(placesDbPath));
            }
        }

        public IPlaceInfo GetNearestPlace(double latitude, double longitude) => _tree.FindNearest(new PlaceInfo(latitude, longitude));

        public string GetNearestPlaceName(double latitude, double longitude) => _tree.FindNearest(new PlaceInfo(latitude, longitude))?.Name;

    }
}
