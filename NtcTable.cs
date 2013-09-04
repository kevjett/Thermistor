using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Thermistor
{
    public class NtcTable
    {
        public string Name { get; set; }
        public string Description { get; set; }
        /// <summary>
        /// Temperatur to resistance map. </summary>
        private SortedDictionary<double, double> map;

        /// <summary>
        /// Creates a new instance with name and given description. </summary>
        /// <param name="name"> NTC thermistor name. </param>
        /// <param name="description"> this table's NTC thermistor description. </param>
        public NtcTable(string name, string description)
        {
            this.Name = name;
            this.Description = description;
            map = new SortedDictionary<double, double>();
        }

        /// <summary>
        /// Adds a new temperature resistance pair to this table. </summary>
        /// <param name="tempString"> the temperature value. </param>
        /// <param name="resString"> the corresponding resistance value. </param>
        public virtual void add(string tempString, string resString)
        {
            double temperature = Convert.ToDouble(tempString);
            double resistance = Convert.ToDouble(resString);
            map.Add(temperature, resistance);
        }

        /// <summary>
        /// Gets the number of temperature value in this table. </summary>
        /// <returns> the number of temperature value in this table. </returns>
        public virtual int NumberOfTemperatures
        {
            get
            {
                return map.Count();
            }
        }

        /// <summary>
        /// Gets a Set with all temperatures in this table. </summary>
        /// <returns> a Set with all temperatures in this table. </returns>
        public virtual IList<double> Temperatures
        {
            get
            {
                return map.Keys.ToList();
            }
        }

        /// <summary>
        /// Gets the resistance for a given temperature value. </summary>
        /// <param name="temperature"> the temperature value. </param>
        /// <returns> the resistance value. </returns>
        /// <exception cref="NtcException"> in case no temperature-resistance pairs can be found. </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public double getResistance(double temperature) throws NtcException
        public virtual double getResistance(double temperature)
        {
            if (!map.ContainsKey(temperature))
            {
                throw new Exception("Temperature value " + temperature + " not found in Ntc table: " + Name);
            }
            return map[temperature];
        }
    }
}
