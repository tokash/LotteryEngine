﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotteryEngine
{
    public class Combination
    {
        private long n = 0;
        private long k = 0;
        private long[] data = null; 

        public Combination(long n, long k)
        {
            if (n < 0 || k < 0) // normally n >= k
                throw new Exception("Negative parameter in constructor");

            this.n = n;
            this.k = k;
            this.data = new long[k + 1];
            for (long i = 0; i < k + 1; ++i)
                this.data[i] = i + 1;
        } // Combination(n,k)

        public Combination(long n, long k, long[] a) // Combination from a[]
        {
            if (k != a.Length)
                throw new Exception("Array length does not equal k");

            this.n = n;
            this.k = k;
            this.data = new long[k];
            for (long i = 0; i < a.Length; ++i)
                this.data[i] = a[i];

            if (!this.IsValid())
                throw new Exception("Bad value from array");
        } // Combination(n,k,a)

        public bool IsValid()
        {
            if (this.data.Length != this.k)
                return false; // corrupted

            for (long i = 0; i < this.k; ++i)
            {
                if (this.data[i] < 0 || this.data[i] > this.n - 1)
                    return false; // value out of range

                for (long j = i + 1; j < this.k; ++j)
                    if (this.data[i] >= this.data[j])
                        return false; // duplicate or not lexicographic
            }

            return true;
        } // IsValid()

        public override string ToString()
        {
            string s = string.Empty;// = "{ ";
            for (long i = 0; i < this.k; ++i)
            {
                if (i + 1 < this.k)
                {
                    s += this.data[i].ToString() + ","; 
                }
                else
                {
                    s += this.data[i].ToString(); 
                }
            }
            //s += "}";
            return s;
        } // ToString()

        public Combination Successor()
        {
            if (this.data[0] == this.n - this.k)
                return null;

            Combination ans = new Combination(this.n, this.k);

            long i;
            for (i = 0; i < this.k; ++i)
                ans.data[i] = this.data[i];

            for (i = this.k - 1; i > 0 && ans.data[i] == this.n - this.k + i; --i)
                ;

            ++ans.data[i];

            for (long j = i; j < this.k - 1; ++j)
                ans.data[j + 1] = ans.data[j] + 1;

            return ans;
        } // Successor()

        public static long Choose(long n, long k)
        {
            if (n < 0 || k < 0)
                throw new Exception("Invalid negative parameter in Choose()");
            if (n < k)
                return 0;  // special case
            if (n == k)
                return 1;

            long delta, iMax;

            if (k < n - k) // ex: Choose(100,3)
            {
                delta = n - k;
                iMax = k;
            }
            else         // ex: Choose(100,97)
            {
                delta = k;
                iMax = n - k;
            }

            long ans = delta + 1;

            for (long i = 2; i <= iMax; ++i)
            {
                checked { ans = (ans * (delta + i)) / i; }
            }

            return ans;
        } // Choose()

        // return the mth lexicographic element of combination C(n,k)
        public Combination Element(long m)
        {
            long[] ans = new long[this.k];

            long a = this.n;
            long b = this.k;
            long x = (Choose(this.n, this.k) - 1) - m; // x is the "dual" of m

            for (long i = 0; i < this.k; ++i)
            {
                ans[i] = LargestV(a, b, x); // largest value v, where v < a and vCb < x    
                x = x - Choose(ans[i], b);
                a = ans[i];
                b = b - 1;
            }

            for (long i = 0; i < this.k; ++i)
            {
                ans[i] = (n - 1) - ans[i];
            }

            return new Combination(this.n, this.k, ans);
        } // Element()


        // return largest value v where v < a and  Choose(v,b) <= x
        private static long LargestV(long a, long b, long x)
        {
            long v = a - 1;

            while (Choose(v, b) > x)
                --v;

            return v;
        } // LargestV()


    } // Combination class
}
