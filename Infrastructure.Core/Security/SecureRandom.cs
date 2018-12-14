using System;
using System.Security.Cryptography;

namespace Infrastructure.Core.Security
{
    public class SecureRandom : Random
    {
        //private RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
        private byte[] buffer = new byte[4];

        public SecureRandom() { }
        public SecureRandom(Int32 ignoredSeed) { }

        public override Int32 Next()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(buffer);
            }
            return BitConverter.ToInt32(buffer, 0) & 0x7FFFFFFF;
        }

        public override Int32 Next(Int32 maxValue)
        {
            if (maxValue < 0)
                throw new ArgumentOutOfRangeException("maxValue");
            return Next(0, maxValue);
            
        }

        public override Int32 Next(Int32 minValue, Int32 maxValue)
        {
            if (minValue > maxValue)
                throw new ArgumentOutOfRangeException("minValue");
            if (minValue == maxValue) return minValue;
            Int64 diff = maxValue - minValue;
            using (var rng = new RNGCryptoServiceProvider())
            {
                while (true)
                {
                    rng.GetBytes(buffer);
                    UInt32 rand = BitConverter.ToUInt32(buffer, 0);

                    Int64 max = (1 + (Int64)UInt32.MaxValue);
                    Int64 remainder = max % diff;
                    if (rand < max - remainder)
                    {
                        return (Int32)(minValue + (rand % diff));
                    }
                }
            }
        }

        public override double NextDouble()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(buffer);
            }
            UInt32 rand = BitConverter.ToUInt32(buffer, 0);
            return rand / (1.0 + UInt32.MaxValue);
        }

        public override void NextBytes(byte[] b)
        {
            if (buffer == null) throw new ArgumentNullException("buffer");
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(buffer);
            }
        }
    }
}
