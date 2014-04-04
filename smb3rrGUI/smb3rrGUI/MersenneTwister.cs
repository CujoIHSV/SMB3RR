using System;
using System.Collections.Generic;

namespace PRNG
{

    class MersenneTwister
    {

        private uint seed;
        private int w, n, m, r;
        private uint a, b, c;
        private int s, t, u, l;
        private int stateIndex;
        private uint[] stateArray;

        // Default = standard MT19937 implementation
        public MersenneTwister(uint seed_ = 0, int w_ = 32, int n_ = 624, int m_ = 397, int r_ = 31,
            uint a_ = 0x9908b0df, uint b_ = 0x9d2c5680, uint c_ = 0xefc60000,
            int s_ = 7, int t_ = 15, int u_ = 11, int l_ = 18)
        {

            seed = ((seed_ == 0) ? ((uint)DateTime.Now.Ticks) : seed_);
            w = w_;
            n = n_;
            m = m_;
            r = r_;
            a = a_;
            b = b_;
            c = c_;
            s = s_;
            t = t_;
            u = u_;
            l = l_;

            stateIndex = 0;
            stateArray = new uint[n];
            stateArray[0] = seed;
            for (int i = 1; i < n; ++i)
            {
                stateArray[i] = 0x6c078965 * (stateArray[i - 1] ^ (stateArray[i - 1] >> 30)) + (uint)i;
            }

        }

        // Get the original PRNG seed
        public uint Seed
        {
            get
            {
                return seed;
            }
        }

        // Generate a pseudo-random uint
        public uint Next
        {
            get
            {

                if (stateIndex == 0)
                {
                    RegenerateArray();
                }

                uint output = stateArray[stateIndex];
                output ^= output >> u;
                output ^= (output << s) & b;
                output ^= (output << t) & c;
                output ^= output >> l;

                stateIndex = (stateIndex + 1) % n;

                return output;

            }
        }

        // Regenerate the base array used to generate random numbers
        private void RegenerateArray()
        {

            for (int i = 0; i < n; ++i)
            {

                uint newState = (stateArray[i] & ~(0xffffffff >> (w - r))) + (stateArray[(i + 1) % n] & (0xffffffff >> (w - r)));
                stateArray[i] = stateArray[(i + m) % n] ^ (newState >> 1);
                if (newState % 2 == 1)
                {
                    stateArray[i] ^= a;
                }

            }

        }

        // Randomly rearrange a generic collection
        public void Shuffle<T>(IList<T> collection)
        {

            for (int i = collection.Count - 1; i > 1; --i)
            {

                uint j = this.Next % ((uint)i + 1);
                T swap = collection[i];
                collection[i] = collection[(int)j];
                collection[(int)j] = swap;

            }

        }

    }

}
