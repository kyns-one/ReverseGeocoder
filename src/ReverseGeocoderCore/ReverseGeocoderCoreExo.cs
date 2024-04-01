using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ReverseGeocoding.Interface;
using ReverseGeocoding.KdTree;
using ReverseGeocoding.Model;

namespace ReverseGeocoding
{
    public class ReverseGeocoderCoreExo : IReverseGeocoderCore
    {
        private readonly KdTree<PlaceInfoExo> _tree;

        private ReverseGeocoderCoreExo(IEnumerable<IPlaceInfo> geoNames) => _tree = new KdTree<PlaceInfoExo>(geoNames.Cast<PlaceInfoExo>().ToArray());

        public ReverseGeocoderCoreExo(GeoDb geoDb) : this(geoDb.Places)
        {
            if (geoDb == null)
            {
                throw new ArgumentNullException(nameof(geoDb));
            }
        }

        public ReverseGeocoderCoreExo(Stream placesDb, Stream countryInfoDb = null) : this(new GeoDb(placesDb, countryInfoDb))
        {
            if (placesDb == null)
            {
                throw new ArgumentNullException(nameof(placesDb));
            }
        }

        public ReverseGeocoderCoreExo(string placesDbPath, string countryInfoDbPath = null) : this(new GeoDb(placesDbPath, countryInfoDbPath , true))
        {
            if (string.IsNullOrWhiteSpace(placesDbPath))
            {
                throw new ArgumentNullException(nameof(placesDbPath));
            }
        }

        public IPlaceInfo GetNearestPlace(double latitude, double longitude) => _tree.FindNearest(new PlaceInfoExo { Latitude = latitude, Longitude= longitude });

        public string GetNearestPlaceName(double latitude, double longitude) => _tree.FindNearest(new PlaceInfoExo{ Latitude = latitude, Longitude = longitude })?.Name;
    }
}
