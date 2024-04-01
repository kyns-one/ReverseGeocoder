using System;
using System.Collections.Generic;
using System.Globalization;
using ReverseGeocoding.Interface;
using ReverseGeocoding.KdTree;

namespace ReverseGeocoding.Model
{
    internal class PlaceInfoEx : IKdComparator<PlaceInfoEx>, IPlaceInfo
    {
        private const char fieldsDelimiter = '\t';
        private const int fieldCount = 19;
        private const string modificationDateFormat = "yyyy-MM-dd";

        public int PlaceInfoId { get; init; }

        public string Name { get; init; }

        public string AsciiName { get; init; }

        public string AlternateNames { get; init; }

        public double Latitude { get; init; }

        public double Longitude { get; init; }

        public FeatureClass FeatureClass { get; init; }

        public string FeatureCode { get; init; }

        public string CountryCode { get; init; }

        public string AltCountryCodes { get; init; }

        public string Admin1Code { get; init; }

        public string Admin2Code { get; init; }

        public string Admin3Code { get; init; }

        public string Admin4Code { get; init; }

        public long Population { get; init; }

        public int? Elevation { get; init; }

        public int Dem { get; init; }

        public string TimeZone { get; init; }

        public DateTime ModificationDate { get; init; }

        public ICountryInfo CountryInfo { get; internal set; }

        //public PlaceInfoEx(string geoEntry)
        //{
        //    var fields = geoEntry.Split(fieldsDelimiter);
        //    if (fields.Length != fieldCount)
        //    {
        //        throw new ArgumentException("Invalid GeoName Record");
        //    }

        //    PlaceInfoId = int.Parse(fields.GetValue(PlaceInfoFields.PlaceInfoId), CultureInfo.InvariantCulture);
        //    Name = fields.GetValue(PlaceInfoFields.Name);
        //    AsciiName = fields.GetValue(PlaceInfoFields.AsciiName);
        //    AlternateNames = fields.GetValue(PlaceInfoFields.AlternateNames);
        //    Latitude = double.Parse(fields.GetValue(PlaceInfoFields.Latitude), CultureInfo.InvariantCulture);
        //    Longitude = double.Parse(fields.GetValue(PlaceInfoFields.Longitude), CultureInfo.InvariantCulture);
        //    FeatureClass = fields.GetValue(PlaceInfoFields.FeatureClass).ToFeatureClass();
        //    FeatureCode = fields.GetValue(PlaceInfoFields.FeatureCode);
        //    CountryCode = fields.GetValue(PlaceInfoFields.CountryCode);
        //    AltCountryCodes = fields.GetValue(PlaceInfoFields.AltCountryCodes);
        //    Admin1Code = fields.GetValue(PlaceInfoFields.Admin1Code);
        //    Admin2Code = fields.GetValue(PlaceInfoFields.Admin2Code);
        //    Admin3Code = fields.GetValue(PlaceInfoFields.Admin3Code);
        //    Admin4Code = fields.GetValue(PlaceInfoFields.Admin4Code);
        //    Population = long.Parse(fields.GetValue(PlaceInfoFields.Population), CultureInfo.InvariantCulture);
        //    Elevation = fields.GetValue(PlaceInfoFields.Elevation).GetIntOrNull();
        //    Dem = int.Parse(fields.GetValue(PlaceInfoFields.Dem), CultureInfo.InvariantCulture);
        //    TimeZone = fields.GetValue(PlaceInfoFields.TimeZone);
        //    ModificationDate = DateTime.ParseExact(fields.GetValue(PlaceInfoFields.ModificationDate), modificationDateFormat, CultureInfo.InvariantCulture);
        //}

        //public PlaceInfoEx(double latitude, double longitude)
        //{
        //    Latitude = latitude;
        //    Longitude = longitude;
        //}

        double IKdComparator<PlaceInfoEx>.AxisSquaredDistance(PlaceInfoEx location, Axis axis)
        {
            double distance;
            switch (axis)
            {
                case Axis.X:
                    distance = X - location.X;
                    break;
                case Axis.Y:
                    distance = Y - location.Y;
                    break;
                case Axis.Z:
                    distance = Z - location.Z;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(axis), axis, null);
            }

            return distance * distance;
        }

        IComparer<PlaceInfoEx> IKdComparator<PlaceInfoEx>.Comparator(Axis axis)
        {
            switch (axis)
            {
                case Axis.X: return new CompareX();
                case Axis.Y: return new CompareY();
                case Axis.Z: return new CompareZ();
                default: throw new ArgumentOutOfRangeException(nameof(axis), axis, "Invalid Axis");
            }
        }

        // The following methods are used purely for the KD-Tree
        // They don't convert lat/lon to any particular coordinate system
        private double X => Math.Cos(Deg2Rad(Latitude)) * Math.Cos(Deg2Rad(Longitude));
        private double Y => Math.Cos(Deg2Rad(Latitude)) * Math.Sin(Deg2Rad(Longitude));
        private double Z => Math.Sin(Deg2Rad(Latitude));

        double IKdComparator<PlaceInfoEx>.SquaredDistance(PlaceInfoEx location)
        {
            var x = X - location.X;
            var y = Y - location.Y;
            var z = Z - location.Z;
            return x * x + y * y + z * z;
        }

        public override string ToString()
        {
            return Name;
        }

        private static double Deg2Rad(double deg) => deg * Math.PI / 180.0;

        private class CompareX : IComparer<PlaceInfoEx>
        {
            public int Compare(PlaceInfoEx x, PlaceInfoEx y)
            {
                if (x == null && y == null)
                {
                    return 0;
                }

                if (x == null)
                {
                    return -1;
                }

                return y == null ? 1 : x.X.CompareTo(y.X);
            }
        }

        private class CompareY : IComparer<PlaceInfoEx>
        {
            public int Compare(PlaceInfoEx x, PlaceInfoEx y)
            {
                if (x == null && y == null)
                {
                    return 0;
                }

                if (x == null)
                {
                    return -1;
                }

                return y == null ? 1 : x.Y.CompareTo(y.Y);
            }
        }

        private class CompareZ : IComparer<PlaceInfoEx>
        {
            public int Compare(PlaceInfoEx x, PlaceInfoEx y)
            {
                if (x == null && y == null)
                {
                    return 0;
                }

                if (x == null)
                {
                    return -1;
                }

                return y == null ? 1 : x.Z.CompareTo(y.Z);
            }
        }
    }
}

