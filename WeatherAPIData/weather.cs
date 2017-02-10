/*
 * Created by Richard Drexel
 * 
 * Returns JSON indicating current weather conditions for both home and work, or one location.
 * If home and work have identical values, only a single location is returned labeled "work"
 * 
 * 
 * To call the api issue a GET request: /weather?home=xx.xxx,yy.yyyy&work=xx.xxx,yy.yyyy
 * Where xx.xxx and yy.yyy are latitude and longitude respectively
 *  
  
 */

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Web;
using WeatherAPI.Library;

namespace WeatherAPIData

{
    public class getWeather : IHttpHandler //Using this interface to handle requests
    {
        //List holds information that will be serialized and transmitted

        public bool IsReusable
        {
            get
            {
                return true;
            }
        }

        public void ProcessRequest(HttpContext context)
        {
            List<dynamic> listOfDataForHTTPResponse = new List<dynamic>();

            try
            {

                //Using a switch/case here in case I want to add more functionality later
                switch (context.Request.HttpMethod)
                {
                    case "GET":

                        string home = context.Request.Params.Get("home");
                        string work = context.Request.Params.Get("work");
                        string zip = context.Request.Params.Get("zip");

                        if (home != null && home != work)
                        {
                            getWeatherWithLatAndLong(home, "home", listOfDataForHTTPResponse);
                        }

                        //already checked equality of values, if they are the same,
                        //this will take care of both home and work
                        if (zip != null)
                        {
                            getWeatherWithZip(zip, "zip", listOfDataForHTTPResponse);
                        }
                        if (work != null)
                        {
                            getWeatherWithLatAndLong(work, "work", listOfDataForHTTPResponse);
                        }
                        break;
                }

                sendResponse(listOfDataForHTTPResponse);

            }
            catch (Exception e)
            {
                //In the event of failure, return an empty list/json string
                dynamic failureObject = new ExpandoObject();
                HttpContext.Current.Response.ContentType = "application/json";
                HttpContext.Current.Response.Write(JsonConvert.SerializeObject(failureObject));
            }
        }

        private static void getWeatherWithZip(string zip, string label, List<dynamic> listOfDataForHTTPResponse)
        {
            int zipcode = int.Parse(zip);
            WeatherRetriever.setZip(zipcode);
            string data = WeatherRetriever.getWeatherTempAndDescripton();
            buildListOfWeatherInfo(label, data, listOfDataForHTTPResponse);
        }

        private static void sendResponse(List<dynamic> listOfDataForHTTPResponse)
        {
            string jsonReturnString = JsonConvert.SerializeObject(listOfDataForHTTPResponse);
            HttpContext.Current.Response.ContentType = "application/json";
            HttpContext.Current.Response.Write(jsonReturnString);
        }

        private static dynamic getDataFromJSONString(string jsonData)
        {
            //Convert into an object that can be parsed and data retrieved

            dynamic data = JsonConvert.DeserializeObject(jsonData);
            return data;
        }

        private static void getWeatherWithLatAndLong(string locationData, string label, List<dynamic> listOfDataForHTTPResponse)
        {
            string[] Coords = locationData.Split(',');

            float lat = float.Parse(Coords[0]);
            float lon = float.Parse(Coords[1]);

            WeatherRetriever.setCoordinates(lat, lon);
            string data = WeatherRetriever.getWeatherTempAndDescripton();
            buildListOfWeatherInfo(label, data, listOfDataForHTTPResponse);

        }

        private static void buildListOfWeatherInfo(string label, string data, List<dynamic> listOfDataForHTTPResponse)
        {
            //Get string returned from WeatherRetiever Library and convert it into a .NET Object

            dynamic dynData = getDataFromJSONString(data);

            //Needed information to send. May want to consider converting all info later on.
            string description = dynData["weather"][0]["description"];
            string temp = dynData["main"]["temp"];

            dynamic dynWeatherDataObject = new ExpandoObject();

            dynamic weatherData = new ExpandoObject();
            dynamic weatherLocation = new ExpandoObject();

            weatherLocation.label = label;
            weatherData.temp = temp;
            weatherData.description = description;

            dynWeatherDataObject = weatherLocation;
            dynWeatherDataObject.weather = weatherData;

            listOfDataForHTTPResponse.Add(dynWeatherDataObject);
        }
    }
}
