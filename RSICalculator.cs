using Binance.Net.Interfaces;

namespace TrendLib.RSI
{
    public class RSICalculator
    {
        public int Period { get; }

        public RSICalculator(int period)
        {
            if (period <= 0)
                throw new ArgumentException("Period must be greater than zero.", nameof(period));
            Period = period;
        }

        public decimal CalculateRSI(IEnumerable<IBinanceKline> klines)
        {
            var closingPrices = klines.Select(k => k.ClosePrice).Reverse().ToArray();
            if (closingPrices.Length < Period)
                throw new ArgumentException($"Insufficient data points. At least {Period} closing prices are required.");

            decimal[] rsiGains = new decimal[200];
            decimal[] rsiLosses = new decimal[200];

            for (int a = 197; a >= 1; a--)
            {
                decimal change = closingPrices[a] - closingPrices[a + 1];
                if (change > 0M)
                    rsiGains[a] = change;
                else if (change < 0M)
                    rsiLosses[a] = closingPrices[a + 1] - closingPrices[a];
            }

            Decimal RS = 0M;
            Decimal RSI = 0M;
            Decimal alpha1 = 1M;
            Decimal alpha2 = 14M;
            Decimal alpha = alpha1 / alpha2;
            Decimal averageGain = 0M;
            Decimal averageLoss = 0M;


            for (int a = 196; a > 196 - 14; a--)
            {
                averageGain += rsiGains[a];
                averageLoss += rsiLosses[a];
            }
            averageGain /= 14M;
            averageLoss /= 14M;

            
            Decimal[] gain = new Decimal[200];
            Decimal[] loss = new Decimal[200];

            gain[183] = rsiGains[183] * alpha + (1M - alpha) * averageGain;
            loss[183] = rsiLosses[183] * alpha + (1M - alpha) * averageLoss;
            
            for (int a = 182; a >= 1; a--)
            {
                gain[a] = rsiGains[a] * alpha + (1M - alpha) * gain[a + 1];
                loss[a] = rsiLosses[a] * alpha + (1M - alpha) * loss[a + 1];
            }

            RS = gain[1] / loss[1];
            RSI = 100M - 100M / (1M + RS);       


            
            return Math.Round(RSI, 4);
        }
    }
}
