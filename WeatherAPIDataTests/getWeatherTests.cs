using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web;

namespace WeatherAPIData.Tests
{
    [TestClass()]
    public class getWeatherTests
    {
        [TestMethod()]
        public void ProcessRequestTest()
        {
            var weatherTest = new getWeather();
            weatherTest.ProcessRequest(HttpContext.Current);

            Assert.Fail("{}");
        }
    }
}