using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using LumenWorks.Framework.IO.Csv;

namespace Thermistor
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Thermistor library version 1.0");
            Console.WriteLine("Copyright (C) 2007, 2013 - SoftQuadrat GmbH, Germany");
            Console.WriteLine();
            var line = 0;
            NtcTable table = new NtcTable("Simu", "Simulated NTC");
            try
            {
                using (var csv = new CsvReader(new StreamReader("simu.txt"), false))
                {
                    while (csv.ReadNextRecord())
                    {
                        line += 1;
                        //if (csv.Count() != 2)
                        //{
                        //    throw new Exception("Wrong number of tokens encountered in line:\n" + line);
                        //}
                        var temperature = csv[0];
                        var resistance = csv[1];
                        table.add(temperature, resistance);
                    }
                }
            }
            catch (IOException e)
            {
                throw new Exception(e.Message, e);
            }

            NtcThermistorModel ntc = new NtcThermistorSimplifiedModel(table);
            Console.WriteLine("Simplified Model");
            doCalculation(table, ntc);
            ntc = new NtcThermistorModel(table);
            Console.WriteLine("Standard Model");
            doCalculation(table, ntc);
            ntc = new NtcThermistorExtendedModel(table);
            Console.WriteLine("Extended Model");
            doCalculation(table, ntc);

            Console.Read();
        }

        private static void doCalculation(NtcTable table, NtcThermistorModel ntc) {
		    Console.WriteLine("------------------------------------------------------------------------");
		    double[] steinhartHartPolynom = ntc.SteinhartHartPolynom;
		    Console.Write("Steinhart-Hart polynom: ");
		    for (int i = steinhartHartPolynom.Length - 1; i >= 0; i--)
		    {
			    if (steinhartHartPolynom[i] == 0.0)
			    {
				    continue;
			    }
			    string fmt;
			    if (i == 0)
			    {
                    fmt = " {0:E}";
			    }
			    else if (i == 1)
			    {
				    fmt = " {0:E} * x";
			    }
			    else
			    {
                    fmt = " {0:E} * x^" + i;
			    }
			    if (i < steinhartHartPolynom.Length - 1)
			    {
				    fmt = " + " + fmt;
			    }
                Console.Write(fmt, steinhartHartPolynom[i]);
		    }
		    Console.WriteLine("\n");
		    double maxErr = 0.0;
		    double errTemp = 0.0;
		    IList<double> temperatures = table.Temperatures;
		    foreach (double t in temperatures)
		    {
			    double r = table.getResistance(t);
			    double calcT = ntc.calcTemperature(r);
			    double calcR = ntc.calcResistance(t);
			    double err = Math.Abs(calcT - t);
			    if (err > maxErr)
			    {
				    maxErr = err;
				    errTemp = t;
			    }
			    Console.Write("temperature={0,4:F0}\tresistance={1,8:F1}\tcalculated temperature={2,9:F2}\tcalculated resistance={3,9:F2}\n", t, r, calcT, calcR);
		    }
		    Console.WriteLine();
		    Console.Write("Maximal error={0,7:F5} at temperature={1,5:F1}\n", maxErr, errTemp);
		    Console.WriteLine("========================================================================\n");
        }   
    }
}
