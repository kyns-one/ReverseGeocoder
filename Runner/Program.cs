// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using nietras.SeparatedValues;
using ReverseGeocoding;
using System.Reflection;

//Console.WriteLine("Hello, World!");
//var geocoder = new ReverseGeocoder(@"DZ.txt");
//Console.WriteLine(geocoder.GetNearestPlaceName(36.7771311557237, 3.063545376062393));
//using var reader = Sep.Reader(op => op with { HasHeader = true }).FromFile("DZ.txt");
//var b = new Benchmarker();
//BenchmarkSwitcher.FromAssembly(Assembly.GetExecutingAssembly()).Run();


// sep method testing

//var sepReader = Sep.Reader(o => o with { HasHeader = false }).FromFile("DZ.txt");
//var geocoder = new ReverseGeocoderCoreEx("DZ.txt");
//Console.WriteLine(geocoder.GetNearestPlaceName(36.7771311557237, 3.063545376062393));

//var geocodere = new ReverseGeocoderCoreExo("DZ.txt");
//Console.WriteLine(geocoder.GetNearestPlaceName(36.7771311557237, 3.063545376062393));


BenchmarkRunner.Run<Benchmarker>();


[MemoryDiagnoser]
[ShortRunJob]
public class Benchmarker
{
    private const double lat = 36.7771311557237;
    private const double lng = 3.063545376062393;

    private double[,] pois = { { 36.78096348349715, 3.0612514168024063 },
        { 33.486307941536225, 6.80083516985178},
        { 33.48588878081414, 6.800719164311886},
        { 33.488076279715436, 6.800351031124592},
        { 36.77132254874999, 3.0640238150954247},
        { 36.7771311557237, 3.063545376062393},
    };

    [Params(10000)]
    public int itterations;

    //[Benchmark]
    //public void ReverseGeocoderSearch()
    //{
    //    var geocoder = new ReverseGeocoder(@"..\..\..\..\DZ.txt");
    //    geocoder.GetNearestPlaceName(lat, lng);
    //}

    //[Benchmark]
    //public void ReverseGeocoderCoreSearch()
    //{
    //    var geocoder = new ReverseGeocoderCore(@"..\..\..\..\DZ.txt", null);
    //    geocoder.GetNearestPlaceName(lat, lng);
    //}

    [Benchmark]
    public void ReverseGeocoderCoreExoSearch()
    {
        var geocoder = new ReverseGeocoderCoreExo(@"..\..\..\..\DZ.txt", null);

        geocoder.GetNearestPlaceName(pois[0,0], pois[0, 1]);
        geocoder.GetNearestPlaceName(pois[1,0], pois[1, 1]);
        geocoder.GetNearestPlaceName(pois[2,0], pois[2, 1]);
        geocoder.GetNearestPlaceName(pois[3,0], pois[3, 1]);
        geocoder.GetNearestPlaceName(pois[4,0], pois[4, 1]);
        geocoder.GetNearestPlaceName(pois[5,0], pois[5, 1]);
    }

    [Benchmark]
    public void ReverseGeocoderCoreSearch()
    {
        var geocoder = new ReverseGeocoderCore(@"..\..\..\..\DZ.txt", null);
        //geocoder.GetNearestPlaceName(lat, lng);
        geocoder.GetNearestPlaceName(pois[0, 0], pois[0, 1]);
        geocoder.GetNearestPlaceName(pois[1, 0], pois[1, 1]);
        geocoder.GetNearestPlaceName(pois[2, 0], pois[2, 1]);
        geocoder.GetNearestPlaceName(pois[3, 0], pois[3, 1]);
        geocoder.GetNearestPlaceName(pois[4, 0], pois[4, 1]);
        geocoder.GetNearestPlaceName(pois[5, 0], pois[5, 1]);
    }
    
    [Benchmark]
    public void ReverseGeocoderCoreExSearch()
    {
        var geocoder = new ReverseGeocoderCoreEx(@"..\..\..\..\DZ.txt", null);
        //geocoder.GetNearestPlaceName(lat, lng);
        geocoder.GetNearestPlaceName(pois[0, 0], pois[0, 1]);
        geocoder.GetNearestPlaceName(pois[1, 0], pois[1, 1]);
        geocoder.GetNearestPlaceName(pois[2, 0], pois[2, 1]);
        geocoder.GetNearestPlaceName(pois[3, 0], pois[3, 1]);
        geocoder.GetNearestPlaceName(pois[4, 0], pois[4, 1]);
        geocoder.GetNearestPlaceName(pois[5, 0], pois[5, 1]);
    }



    //[Benchmark]
    //public void SepLoading()
    //{
    //    Sep.Reader().FromFile(@"..\..\..\..\DZ.txt");
    //}
}