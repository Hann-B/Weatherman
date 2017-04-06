using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Weatherman
{
    class PastWeather
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public int Temperature { get; set; }
        public string CurrentConditions { get; set; }

        public PastWeather()
        {

        }
        public PastWeather(SqlDataReader reader)
        {
            Name = reader["Name"].ToString();
            Id = (int)reader["Id"];
            Temperature = (int)reader["Temperature"];
            CurrentConditions = reader["CurrentConditions"].ToString();
        }
    }
}
