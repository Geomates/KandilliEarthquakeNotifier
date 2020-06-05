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
using Amazon.Lambda.Core;

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

                var result = lines.Select(ParseLine).Where(l => l != null).ToList();

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
            var isValid = DateTime.TryParseExact(chunks[0] + ' ' + chunks[1], "yyyy.MM.dd HH:mm:ss", CultureInfo.InvariantCulture.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces, out DateTime date);
            if (!isValid)
            {
                LambdaLogger.Log("Date is not correct: " + chunks[0] + ' ' + chunks[1]);
                return null;
            }

            // Latitude
            isValid = double.TryParse(chunks[2], out var latitude);
            if (!isValid)
            {
                LambdaLogger.Log("Latitude is not correct: " + chunks[2]);
                return null;
            }

            // Longitude
            isValid = double.TryParse(chunks[3], out double longitude);
            if (!isValid)
            {
                LambdaLogger.Log("Longitude is not correct: " + chunks[3]);
                return null;
            }

            // Depth 
            isValid = double.TryParse(chunks[4], out double depth);
            if (!isValid)
            {
                LambdaLogger.Log("Depth is not correct: " + chunks[4]);
            }

            // Magnitude
            isValid = double.TryParse(chunks[6], out double magnitude);
            if (!isValid)
            {
                LambdaLogger.Log("Magnitude(ML) is not correct: " + chunks[6]);
                isValid = double.TryParse(chunks[7], out magnitude);
                if (!isValid)
                {
                    LambdaLogger.Log("Magnitude(Mw) is not correct: " + chunks[7]);
                    return null;
                }
            }

            // Location
            var location = string.Join(' ', chunks.Skip(8).Take(chunks.Length - 1 - 8));
            if (string.IsNullOrEmpty(location))
            {
                LambdaLogger.Log("Location is not correct");
                return null;
            }

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