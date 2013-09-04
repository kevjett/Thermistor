using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Thermistor
{
    public class NtcThermistorModel
    {
        protected internal const double TABS = -273.15;
	    /// <summary>
	    /// The NTC table used. </summary>
	    private readonly NtcTable table;
	    /// <summary>
	    /// x values derived from our NTC table. </summary>
	    private double[] x;
	    /// <summary>
	    /// y values derived from our NTC table. </summary>
	    private double[] y;
	    /// <summary>
	    /// The approximating Steinhart-Hart polynom. </summary>
	    private double[] steinhartHartPolynom;

	    /// <summary>
	    /// Constructs a new instance based on an NTC table. </summary>
	    /// <param name="table"> NTC table with T-R pairs. </param>
	    /// <exception cref="NtcException"> in case of missing temperature found. </exception>
    //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
    //ORIGINAL LINE: public NtcThermistorModel(NtcTable table) throws NtcException
	    public NtcThermistorModel(NtcTable table)
	    {
		    this.table = table;
		    int dimension = table.NumberOfTemperatures;
		    x = new double[dimension];
		    y = new double[dimension];
		    int i = 0;
		    foreach (double temperature in table.Temperatures)
		    {
			    x[i] = Math.Log(table.getResistance(temperature));
			    y[i] = 1.0 / (temperature - TABS);
			    i++;
		    }
		    double[][] @base = Base;
		    orthonormal(@base);
		    steinhartHartPolynom = approx(@base);
	    }

	    /// <summary>
	    /// Calculates temperature for a given resistance value. </summary>
	    /// <param name="resistance"> resistanc of NTC. </param>
	    /// <returns> calculated temperature value (in Celsius). </returns>
	    public virtual double calcTemperature(double resistance)
	    {
		    double ti;

		    ti = value(steinhartHartPolynom, Math.Log(resistance));
		    ti = 1.0 / ti + TABS;
		    return ti;
	    }

	    /// <summary>
	    /// Calculates resistance for a given temperature value. </summary>
	    /// <param name="temperature"> temperature (in Celsius). </param>
	    /// <returns> calculated resistance value. </returns>
	    public virtual double calcResistance(double temperature)
	    {
		    double r;
		    double u, v, p, q, b, c, d;

		    temperature = temperature - TABS;
            //System.out.println("temperature = " + temperature);
		    d = (steinhartHartPolynom[0] - 1.0 / temperature) / steinhartHartPolynom[3];
            //System.out.println("d = " + d);
		    c = steinhartHartPolynom[1] / steinhartHartPolynom[3];
            //System.out.println("c = " + c);
		    b = steinhartHartPolynom[2] / steinhartHartPolynom[3];
            //System.out.println("b = " + b);
		    q = 2.0 / 27.0 * b * b * b - 1.0 / 3.0 * b * c + d;
            //System.out.println("q = " +  q);
		    p = c - 1.0 / 3.0 * b * b;
            //System.out.println("p = " + p);
		    v = -Math.Pow(q / 2.0 + Math.Sqrt(q * q / 4.0 + p * p * p / 27.0), 1.0 / 3.0);
            //System.out.println("v = " + v);
		    u = Math.Pow(-q / 2.0 + Math.Sqrt(q * q / 4.0 + p * p * p / 27.0), 1.0 / 3.0);
            //System.out.println("u = " + u);
		    r = Math.Exp(u + v - b / 3.0);
            //System.out.println("r = " + r);
		    return r;
	    }

	    /// <summary>
	    /// Gets the name of the thermistor. </summary>
	    /// <returns> the name of the thermistor. </returns>
	    public virtual string Name
	    {
		    get
		    {
			    return table.Name;
		    }
	    }

	    /// <summary>
	    /// Gets the full name of the NTC thermistor. </summary>
	    /// <returns> the full name of the NTC thermistor. </returns>
	    public virtual string Description
	    {
		    get
		    {
			    return table.Description;
		    }
	    }

	    /// <summary>
	    /// Gets the underlying R-T table. </summary>
	    /// <returns> the underlying R-T table. </returns>
	    public virtual NtcTable Table
	    {
		    get
		    {
			    return table;
		    }
	    }

        /// <summary>
        /// Gets the Steinhart-Hart polynom of this thermistor. </summary>
        /// <returns> the Steinhart-Hart polynom of this thermistor. </returns>
        public virtual double[] SteinhartHartPolynom
        {
            get
            {
                return steinhartHartPolynom;
            }
        }

        /// <summary>
        /// Gets a base for subspace U. </summary>
        /// <returns> base for subspace U. </returns>
        internal virtual double[][] Base
        {
            get
            {
                double[][] @base = new double[][] { new double[] { 1.0, 0.0, 0.0, 0.0 }, new double[] { 0.0, 1.0, 0.0, 0.0 }, new double[] { 0.0, 0.0, 0.0, 1.0 } };
                return @base;
            }
        }

        /// <summary>
        /// Evaluates value of <code>p(x)</code> for a given polynom <code>p</code> from space U at <code>x</code>.
        /// Calculation is done using Horners scheme. </summary>
        /// <param name="p"> polynom of degree <code>subDimension</code> (given as array of doubles). </param>
        /// <param name="x"> value inserted into the polynom. </param>
        /// <returns> calculated value of <code>p(x)</code>. </returns>
        internal virtual double value(double[] p, double x)
        {
            int i;
            double retval = 0.0;
            for (i = p.Length - 1; i >= 0; i--)
            {
                retval = retval * x + p[i];
            }
            return retval;
        }

        /// <summary>
        /// Calculate scalar product <code>[p, q]<code> of polynoms <code>p</code> and <code>q</code> in space V.
        /// The scalar product is defined as
        /// <blockquote>
        /// <table border="0" cellpadding="0" cellspacing="0">
        /// <tr>
        /// <td>&nbsp;</td>
        /// <td><sub>&nbsp;dimension</sub></td>
        /// </tr>
        /// <tr>
        /// <td>[p, q] :=</td>
        /// <td><font size="+3">&Sigma;</font> p(x<sub>i</sub>) &middot; q(x<sub>i</sub>)</td>
        /// </tr>
        /// <tr>
        /// <td>&nbsp;</td>
        /// <td><sup>i=0</sup></td>
        /// </tr>
        /// </table>
        /// </blockquote> </summary>
        /// <param name="p"> first polynom (given as array of doubles). </param>
        /// <param name="q"> second polynom (given as array of doubles). </param>
        /// <returns> calculated scalar product. </returns>
        internal virtual double skalarpoly(double[] p, double[] q)
        {
            int i;
            double retval = 0.0;
            for (i = 0; i < x.Length; i++)
            {
                retval += value(p, x[i]) * value(q, x[i]);
            }
            return retval;
        }

        /// <summary>
        /// Evaluates <code>p *= fact</code>.
        /// Multiplies a polynom <code>p</code> from space U with a factor <code>fact</code>. </summary>
        /// <param name="p"> polynom (given as array of doubles). </param>
        /// <param name="fact"> factor used to multiply with. </param>
        internal virtual void mult(double[] p, double fact)
        {
            int i;
            for (i = 0; i < p.Length; i++)
            {
                p[i] *= fact;
            }
        }

        /// <summary>
        /// Evaluates <code>p += fact&middot;q</code>.
        /// Adds a polynom <code>q</code> multiplied with a factor <code>fact</code> to a given polynom <code>p</code>
        /// (both polynoms from U). </summary>
        /// <param name="p"> first polynom (given as array of doubles). </param>
        /// <param name="q"> second polynom (given as array of doubles). </param>
        /// <param name="fact"> factor used to multiply second polynom with. </param>
        internal virtual void linear(double[] p, double[] q, double fact)
        {
            int i;
            for (i = 0; i < p.Length; i++)
            {
                p[i] += q[i] * fact;
            }
        }


        /// <summary>
        /// Ortho-normalizes a given base of U. </summary>
        /// <param name="base"> base to be ortho-normalized (given as array of doubles). </param>
        internal virtual void orthonormal(double[][] @base)
        {
            int i, j;
            double fact, norm;
            for (i = 0; i < @base.Length; i++)
            {
                for (j = 0; j < i; j++)
                {
                    fact = skalarpoly(@base[i], @base[j]);
                    linear(@base[i], @base[j], -fact);
                }
                norm = skalarpoly(@base[i], @base[i]);
                mult(@base[i], 1.0 / Math.Sqrt(norm));
            }
        }

        /// <summary>
        /// Evaluates scalar product <code>[p, p<sub>f</sub>]</code> for a given
        /// polynom <code>p</code> from U with the solving polynom <code>p<sub>f</sub></code> in V. </summary>
        /// <param name="p"> polynom used to multipy with (given as array of doubles). </param>
        /// <returns> calculated scalar product. </returns>
        internal virtual double skalar(double[] p)
        {
            int i;
            double retval = 0.0;
            for (i = 0; i < x.Length; i++)
            {
                retval += y[i] * value(p, x[i]);
            }
            return retval;
        }

        /// <summary>
        /// Evaluate approximation polynom <code>u<sub>f</sub></code> in U.
        /// The evaluation is done using the formula
        /// <blockquote>
        /// <table border="0" cellpadding="0" cellspacing="0">
        /// <tr>
        /// <td>&nbsp;</td>
        /// <td><sub>&nbsp;m-1</sub></td>
        /// </tr>
        /// <tr>
        /// <td>u<sub>f</sub> =</td>
        /// <td><font size="+3">&Sigma;</font> [ u<sub>i</sub>, p<sub>f</sub> ] &middot; u<sub>i</sub></td>
        /// </tr>
        /// <tr>
        /// <td>&nbsp;</td>
        /// <td><sup>i=0</sup></td>
        /// </tr>
        /// </table>
        /// </blockquote>
        /// </summary>
        internal virtual double[] approx(double[][] orthoBasis)
        {
            int i;
            double fact;
            double[] solution = new double[orthoBasis[0].Length];
            for (i = 0; i < orthoBasis.Length; i++)
            {
                fact = skalar(orthoBasis[i]);
                linear(solution, orthoBasis[i], fact);
            }
            return solution;
        }
    }
}
