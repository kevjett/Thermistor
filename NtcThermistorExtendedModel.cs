namespace Thermistor
{
    public class NtcThermistorExtendedModel : NtcThermistorModel
    {
        public NtcThermistorExtendedModel(NtcTable table)
            : base(table)
        {}

        /// <summary>
        /// @inheritDoc
        /// </summary>
        internal override double[][] Base
        {
            get
            {
                double[][] @base = new double[][] { new double[] { 1.0, 0.0, 0.0, 0.0 }, new double[] { 0.0, 1.0, 0.0, 0.0 }, new double[] { 0.0, 0.0, 1.0, 0.0 }, new double[] { 0.0, 0.0, 0.0, 1.0 } };
                return @base;
            }
        }
    }
}