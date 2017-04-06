using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using MathNet;
using System.Data.SqlClient;

namespace Weatherman
{
    class Program
    {
        static public string GetUserName()
        {
            //Asks the user for their name
            Console.WriteLine("What is your Name?");

            //store name to variable
            var userName = Console.ReadLine();

            return userName;
        }
        static public string GetUserZipCode(string userName)
        {
            //Asks the user for their zip code
            Console.WriteLine($"{userName}, enter a zip code to see the weather");
            var zip = Console.ReadLine();
            return zip;
        }
        static public RootObject GetWeatherFromAPI(string zip)
        {
            //Get weather for queried zip code
            var url = "http://api.openweathermap.org/data/2.5/weather?zip=" + zip + ",us&units=imperial&id=524901&APPID=ca27a9311ef37c705b0e639ecdfb29a6";
            var request = WebRequest.Create(url);
            var response = request.GetResponse();
            var rawResponse = String.Empty;
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                rawResponse = reader.ReadToEnd();
            }
            var weatherNow = JsonConvert.DeserializeObject<RootObject>(rawResponse);
            return weatherNow;
        }
        static public void DisplayCurrentWeather(RootObject weatherNow)
        {
            Console.WriteLine("Currently weather is: ");
            Console.WriteLine($"{weatherNow.main.temp}°F");//Current temp 
            Console.WriteLine($"{weatherNow.weather.First().description}");//Current weather conditions
        }
        static public void AddCurrentWeather(RootObject WeatherToAdd, string userName)
        {
            const string connectionString =
                @"Server=localhost\SQLEXPRESS;Database=Weatherman;Trusted_Connection=True;";
            using (var connection = new SqlConnection(connectionString))
            {
                var text = @"INSERT INTO WeatherQuery (Name, Temperature, CurrentConditions)" +
                        "Values (@Name, @Temperature, @CurrentConditions)";

                var cmd = new SqlCommand(text, connection);

                cmd.Parameters.AddWithValue("@Name", userName);
                cmd.Parameters.AddWithValue("@Temperature", WeatherToAdd.main.temp);
                cmd.Parameters.AddWithValue("@CurrentConditions", WeatherToAdd.weather.First().description);
                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
            }
        }
        public static List<PastWeather> ReadQueriesFromFile(SqlConnection connection)
        {
            //Read database 
            var weather = new List<PastWeather>();
            var sqlCommand = new SqlCommand(
                @"SELECT*
                FROM WeatherQuery", connection);
            connection.Open();
            var reader = sqlCommand.ExecuteReader();
            while (reader.Read())
                {
                var weatherPast = new PastWeather(reader);
                weather.Add(new PastWeather());
                }
            connection.Close();
            return weather;
        }
        public static void CompareNames(List<PastWeather>weather)
        {
            //compare names
            Console.WriteLine();
        }

        static void Main(string[] args)
        {
            //Where is the Database
            const string connectionString =
                 @"Server=localhost\SQLEXPRESS;Database=Weatherman;Trusted_Connection=True;";
            var connection = new SqlConnection(connectionString);

            string userName = GetUserName();

            string zip = GetUserZipCode(userName);

            RootObject weatherNow = GetWeatherFromAPI(zip);

            AddCurrentWeather(weatherNow, userName);

            DisplayCurrentWeather(weatherNow);

            List<PastWeather>weather= ReadQueriesFromFile(connection);

            CompareNames(weather);

            //if user returns display past queries

            //when ID!=ID but Name=Name 
            //Display past queries 
            //WHERE Name=Name

            //display if applicable
        }
    }
}
