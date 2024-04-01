using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using nietras.SeparatedValues;
using ReverseGeocoding.Interface;
using ReverseGeocoding.Model;

namespace ReverseGeocoding
{
    /// <summary>
    /// All the geocoding data can be gotten from: http://download.geonames.org/export/dump/
    /// </summary>
    public class GeoDb
    {
        private const string commentMark = "#";

        public IReadOnlyList<IPlaceInfo> Places { get; }

        public IReadOnlyList<ICountryInfo> CountryInfos { get; }

        public GeoDb(Stream placesDb, Stream countryInfoDb = null)
        {
            if (placesDb == null)
            {
                throw new ArgumentNullException(nameof(placesDb));
            }

            IReadOnlyDictionary<string, ICountryInfo> countryInfos = null;

            if (countryInfoDb != null)
            {
                countryInfos = GetCountryInfos(countryInfoDb);
                CountryInfos = countryInfos.Values.ToList();
            }

            Places = GetPlaces(placesDb, countryInfos);
        }

        public GeoDb(string placesDbPath, string countryInfoDbPath = null)
        {
            if (string.IsNullOrWhiteSpace(placesDbPath))
            {
                throw new ArgumentNullException(nameof(placesDbPath));
            }

            IReadOnlyDictionary<string, ICountryInfo> countryInfos = null;

            if (!string.IsNullOrWhiteSpace(countryInfoDbPath))
            {
                countryInfos = GetCountryInfos(countryInfoDbPath);
                CountryInfos = countryInfos.Values.ToList();
            }

            Places =  GetPlaces(placesDbPath, countryInfos);
        }

        public GeoDb(string placesDbPath, string countryInfoDbPath = null, bool useExo = false)
        {
            if (string.IsNullOrWhiteSpace(placesDbPath))
            {
                throw new ArgumentNullException(nameof(placesDbPath));
            }

            IReadOnlyDictionary<string, ICountryInfo> countryInfos = null;

            if (!string.IsNullOrWhiteSpace(countryInfoDbPath))
            {
                countryInfos = GetCountryInfos(countryInfoDbPath);
                CountryInfos = countryInfos.Values.ToList();
            }

            Places = useExo ? GetPlacesExo(placesDbPath, countryInfos) : GetPlacesEx(placesDbPath, countryInfos);
        }

        private static IReadOnlyList<IPlaceInfo> GetPlaces(Stream input, IReadOnlyDictionary<string, ICountryInfo> countryInfos)
        {
            var places = new List<IPlaceInfo>();
            using (var db = new StreamReader(input))
            {
                string line;
                while (!db.EndOfStream && (line = db.ReadLine()) != null)
                {
                    if (line.StartsWith(commentMark))
                    {
                        continue;
                    }

                    var place = new PlaceInfo(line);
                    if (countryInfos != null && !string.IsNullOrWhiteSpace(place.CountryCode) && countryInfos.ContainsKey(place.CountryCode))
                    {
                        place.CountryInfo = countryInfos[place.CountryCode];
                    }

                    places.Add(place);
                }
            }

            return places;
        }

        private static IReadOnlyList<IPlaceInfo> GetPlacesEx(string placesDbPath, IReadOnlyDictionary<string, ICountryInfo> countryInfos)
        {

            using var reader = Sep.New('\t').Reader(o => o with { HasHeader = false, CultureInfo = CultureInfo.InvariantCulture }).FromFile(placesDbPath);
            var places = reader.Enumerate(row =>
            {
                var pi = new PlaceInfoEx
                {
                    PlaceInfoId = row[0].Parse<int>(), // int.Parse(fields.GetValue(PlaceInfoFields.PlaceInfoId), CultureInfo.InvariantCulture),
                    Name = row[1].ToString(), //fields.GetValue(PlaceInfoFields.Name),
                    AsciiName = row[2].ToString(), //fields.GetValue(PlaceInfoFields.AsciiName);
                    AlternateNames = row[3].ToString(), //fields.GetValue(PlaceInfoFields.AlternateNames);
                    Latitude = row[4].Parse<double>(), //double.Parse(fields.GetValue(PlaceInfoFields.Latitude), CultureInfo.InvariantCulture);
                    Longitude = row[5].Parse<double>(), //double.Parse(fields.GetValue(PlaceInfoFields.Longitude), CultureInfo.InvariantCulture);
                    FeatureClass = row[6].ToString().ToFeatureClass(), //fields.GetValue(PlaceInfoFields.FeatureClass).ToFeatureClass();
                    FeatureCode = row[7].ToString(), //fields.GetValue(PlaceInfoFields.FeatureCode);
                    CountryCode = row[8].ToString(), //fields.GetValue(PlaceInfoFields.CountryCode);
                    AltCountryCodes = row[9].ToString(), //fields.GetValue(PlaceInfoFields.AltCountryCodes);
                    Admin1Code = row[10].ToString(), //fields.GetValue(PlaceInfoFields.Admin1Code);
                    Admin2Code = row[11].ToString(), //fields.GetValue(PlaceInfoFields.Admin2Code);
                    Admin3Code = row[12].ToString(), //fields.GetValue(PlaceInfoFields.Admin3Code);
                    Admin4Code = row[13].ToString(), //fields.GetValue(PlaceInfoFields.Admin4Code);
                    Population = row[14].Parse<long>(), //long.Parse(fields.GetValue(PlaceInfoFields.Population), CultureInfo.InvariantCulture);
                    Elevation = row[15].TryParse<int>(), //fields.GetValue(PlaceInfoFields.Elevation).GetIntOrNull();
                    Dem = row[16].Parse<int>(), //int.Parse(fields.GetValue(PlaceInfoFields.Dem), CultureInfo.InvariantCulture);
                    TimeZone = row[17].ToString(), //fields.GetValue(PlaceInfoFields.TimeZone);
                    ModificationDate = row[18].Parse<DateTime>(), // //DateTime.ParseExact(fields.GetValue(PlaceInfoFields.ModificationDate), modificationDateFormat, CultureInfo.InvariantCulture);
                };

                if (countryInfos != null && !string.IsNullOrWhiteSpace(pi.CountryCode) && countryInfos.ContainsKey(pi.CountryCode))
                {
                    pi.CountryInfo = countryInfos[pi.CountryCode];
                }

                return pi;

            }).ToArray();

            return places;
        }

        private static IReadOnlyList<IPlaceInfo> GetPlacesExo(string placesDbPath, IReadOnlyDictionary<string, ICountryInfo> countryInfos)
        {

            using var reader = Sep.New('\t').Reader(o => o with { HasHeader = false, CultureInfo = CultureInfo.InvariantCulture }).FromFile(placesDbPath);
            var places = reader.Enumerate(row =>
            {
                var pi = new PlaceInfoExo
                {
                    PlaceInfoId = row[0].Parse<int>(), // int.Parse(fields.GetValue(PlaceInfoFields.PlaceInfoId), CultureInfo.InvariantCulture),
                    Name = row[1].ToString(), //fields.GetValue(PlaceInfoFields.Name),
                    AsciiName = row[2].ToString(), //fields.GetValue(PlaceInfoFields.AsciiName);
                    AlternateNames = row[3].ToString(), //fields.GetValue(PlaceInfoFields.AlternateNames);
                    Latitude = row[4].Parse<double>(), //double.Parse(fields.GetValue(PlaceInfoFields.Latitude), CultureInfo.InvariantCulture);
                    Longitude = row[5].Parse<double>(), //double.Parse(fields.GetValue(PlaceInfoFields.Longitude), CultureInfo.InvariantCulture);
                    FeatureClass = row[6].ToString().ToFeatureClass(), //fields.GetValue(PlaceInfoFields.FeatureClass).ToFeatureClass();
                    FeatureCode = row[7].ToString(), //fields.GetValue(PlaceInfoFields.FeatureCode);
                    CountryCode = row[8].ToString(), //fields.GetValue(PlaceInfoFields.CountryCode);
                    AltCountryCodes = row[9].ToString(), //fields.GetValue(PlaceInfoFields.AltCountryCodes);
                    Admin1Code = row[10].ToString(), //fields.GetValue(PlaceInfoFields.Admin1Code);
                    Admin2Code = row[11].ToString(), //fields.GetValue(PlaceInfoFields.Admin2Code);
                    Admin3Code = row[12].ToString(), //fields.GetValue(PlaceInfoFields.Admin3Code);
                    Admin4Code = row[13].ToString(), //fields.GetValue(PlaceInfoFields.Admin4Code);
                    Population = row[14].Parse<long>(), //long.Parse(fields.GetValue(PlaceInfoFields.Population), CultureInfo.InvariantCulture);
                    Elevation = row[15].TryParse<int>(), //fields.GetValue(PlaceInfoFields.Elevation).GetIntOrNull();
                    Dem = row[16].Parse<int>(), //int.Parse(fields.GetValue(PlaceInfoFields.Dem), CultureInfo.InvariantCulture);
                    TimeZone = row[17].ToString(), //fields.GetValue(PlaceInfoFields.TimeZone);
                    ModificationDate = row[18].Parse<DateTime>(), // //DateTime.ParseExact(fields.GetValue(PlaceInfoFields.ModificationDate), modificationDateFormat, CultureInfo.InvariantCulture);
                };

                if (countryInfos != null && !string.IsNullOrWhiteSpace(pi.CountryCode) && countryInfos.ContainsKey(pi.CountryCode))
                {
                    pi.CountryInfo = countryInfos[pi.CountryCode];
                }

                return pi;

            }).ToArray();

            return places;
        }

        private static IReadOnlyList<IPlaceInfo> GetPlaces(string placesDbPath, IReadOnlyDictionary<string, ICountryInfo> countryInfos)
        {
            using (var stream = new FileStream(placesDbPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return GetPlaces(stream, countryInfos);
            }
        }

        private static IReadOnlyDictionary<string, ICountryInfo> GetCountryInfos(Stream input)
        {
            var countryInfos = new Dictionary<string, ICountryInfo>();
            using (var db = new StreamReader(input))
            {
                string line;
                while (!db.EndOfStream && (line = db.ReadLine()) != null)
                {
                    if (line.StartsWith(commentMark))
                    {
                        continue;
                    }

                    var countryInfo = new CountryInfo(line);
                    countryInfos.Add(countryInfo.Iso, countryInfo);
                }
            }

            return countryInfos;
        }

        private static IReadOnlyDictionary<string, ICountryInfo> GetCountryInfos(string countryDatabase)
        {
            using (var stream = new FileStream(countryDatabase, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return GetCountryInfos(stream);
            }
        }
    }
}