using Common.Services;
using HtmlAgilityPack;
using KandilliEarthquakePuller.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace KandilliEarthquakePuller.Services
{
    public interface IKandilliService
    {
        Task<IEnumerable<Earthquake>> GetEarthquakes();
    }

    public class KandilliService : IKandilliService
    {
        private const string KANDILLI_PAGE_URL = "KANDILLI_PAGE_URL";
        private readonly string _pageURL;
        public KandilliService(IEnvironmentService environmentService)
        {
            _pageURL = environmentService.GetEnvironmentValue(KANDILLI_PAGE_URL);
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        public async Task<IEnumerable<Earthquake>> GetEarthquakes()
        {
            var url = new Uri(_pageURL);

            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(_pageURL))
            {
                HtmlDocument htmlDocument = new HtmlDocument();
                htmlDocument.Load(await response.Content.ReadAsStreamAsync(), Encoding.GetEncoding(1254));

                var lines = ReadLines(htmlDocument);

                var result = lines.Select(l => ParseLine(l)).ToList();

                return result;
            }
        }

        private string[] ReadLines(HtmlDocument htmlDocument)
        {
            HtmlNodeCollection headers = htmlDocument.DocumentNode.SelectNodes("//pre");

            string result = headers[0].InnerHtml;
            int startIndex = result.IndexOf("<pre>") + 5;

            result = result.Substring(startIndex).Trim();

            result = result.Substring(result.LastIndexOf("------") + 6).Trim();
            var lines = result.Split('\n');

            return lines;
        }

        private Earthquake ParseLine(string line)
        {
            var chunks = line.Split(' ').Where(l => l.Trim().Length > 0).ToArray();
            // Date & Time
            DateTime date = DateTime.ParseExact(chunks[0] + ' ' + chunks[1], "yyyy.MM.dd HH:mm:ss", CultureInfo.InvariantCulture);

            // Latitude
            double latitude = double.Parse(chunks[2]);

            // Longitude
            double longitude = double.Parse(chunks[3]);

            // Depth 
            double depth = double.Parse(chunks[4]);

            // Magnitude
            double magnitude = double.Parse(chunks[6]);

            // Location
            string location = string.Join(' ', chunks.Skip(8).Take(chunks.Length - 1 - 8));

            Earthquake model = new Earthquake
            {
                Date = date,
                Latitude = latitude,
                Longitude = longitude,
                Depth = depth,
                Magnitude = magnitude,
                Location = location.Trim()
            };

            return model;
        }
    }
}