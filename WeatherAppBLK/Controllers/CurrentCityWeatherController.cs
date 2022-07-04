﻿using System.Net;
using System.Text.Json;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace WeatherAppBLK.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CurrentCityWeatherController : ControllerBase
    {



        [HttpGet(Name = "CurrentCityWeatherController")]
        public async Task<HttpResponseMessage> Get(CancellationToken token)
        {
            var lastFetch = DateTime.Now;

            var client = new HttpClient();

            //var client = _httpClientFactory.CreateClient();

            //Parameters
            const string apiKey = "2a4d176f29584d73ae764117223006";
            var city = "London";
            var currentWeather = "yes"; //current weather conditions output
            var format = "json";
            var cityUrl = $"http://api.worldweatheronline.com/premium/v1/weather.ashx?key={apiKey}&q={city}&cc={currentWeather}&format={format}";

            #region XML maybe?
            //var response = await client.GetAsync(cityUrl);
            //var responseContent = response.Content.ReadAsStream(); //TODO: Async

            //var xml = new XmlSerializer(typeof(CityWeather));
            //var assignmentResult = new CityWeather();
            //xml.Deserialize(responseContent,);
            #endregion XML maybe?

            var jsonOptions = new JsonSerializerOptions();

            //var jsonResponse = await client.GetFromJsonAsync<object>(cityUrl, cancellationToken: token);
            var jsonResponse = await client.GetFromJsonAsync<Mapping.Rootobject>(cityUrl, cancellationToken: token);

            var tempC = jsonResponse?.data.current_condition[0].temp_C;
            var observationTime = jsonResponse?.data.current_condition[0].observation_time;

            //TODO: Create as middleware
            bool? WeatherBeenFetchLast15min;

            //TODO: Handle possible null ref
            var DateTimeObservationTime = DateTime.Parse(observationTime);


            if ((lastFetch - DateTimeObservationTime).TotalMinutes <= 15)
            { WeatherBeenFetchLast15min = true; }
            else
            { WeatherBeenFetchLast15min = false; }

            return WeatherBeenFetchLast15min switch
            {
                true => new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("Response OK") },
                false => new HttpResponseMessage(HttpStatusCode.NotModified) { Content = new StringContent("NotModified") },
                null => new HttpResponseMessage(HttpStatusCode.ExpectationFailed) { Content = new StringContent("Failed") }
            };
        }




    }
}