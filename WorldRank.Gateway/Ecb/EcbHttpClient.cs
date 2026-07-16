using System.Globalization;
using System.Xml.Linq;

namespace WorldRank.Gateway.Ecb
{
    public class EcbHttpClient : IEcbRatesClient
    {
        private const string DailyPath = "/stats/eurofxref/eurofxref-daily.xml";
        private static readonly XNamespace Ns = "http://www.ecb.int/vocabulary/2002-08-01/eurofxref";

        private readonly HttpClient _http;

        public EcbHttpClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<EcbRatesResult> GetLatestRatesAsync(CancellationToken ct = default)
        {
            var xml = await _http.GetStringAsync(DailyPath, ct);
            var doc = XDocument.Parse(xml);

            var dateCube = doc.Descendants(Ns + "Cube")
                .First(cube => cube.Attribute("time") is not null);

            var date = DateTime.Parse(dateCube.Attribute("time")!.Value, CultureInfo.InvariantCulture);

            var rates = dateCube.Elements(Ns + "Cube")
                .Select(cube => new EcbRate(
                    cube.Attribute("currency")!.Value,
                    decimal.Parse(cube.Attribute("rate")!.Value, CultureInfo.InvariantCulture)))
                .ToList();

            return new EcbRatesResult(date, rates);
        }
    }
}
