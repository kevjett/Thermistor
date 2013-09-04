using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Thermistor
{
    public class NtcThermistorSimplifiedModel : NtcThermistorModel
    {
        /// <summary>
        /// Creates a new instance using the given T-R table. </summary>
        /// <param name="table"> the T-R table. </param>
        /// <exception cref="NtcException"> in case of errors </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public NtcThermistorSimplifiedModel(NtcTable table) throws NtcException
        public NtcThermistorSimplifiedModel(NtcTable table)
            : base(table)
        {
        }

        /// <summary>
        /// @inheritDoc
        /// </summary>
        internal override double[][] Base
        {
            get
            {
                double[][] @base = new double[][] { new double[] { 1.0, 0.0, 0.0, 0.0 }, new double[] { 0.0, 1.0, 0.0, 0.0 } };
                return @base;
            }
        }

        public override double calcResistance(double temperature)
        {
            temperature = temperature - TABS;
            double[] steinhartHartPolynom = SteinhartHartPolynom;
            double resistance = Math.Exp((1 / temperature - steinhartHartPolynom[0]) / steinhartHartPolynom[1]);
            return resistance;
        }
    }
}
