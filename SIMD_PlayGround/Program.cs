﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace SIMD_PlayGround
{
    class Program
    {
        static void Main(string[] args)
        {
            var r = new Random();
            int[] lhs = new int[int.MaxValue / 12];
            int[] rhs = new int[int.MaxValue / 12];

            var t1 = Task.Run(() =>
            {
                for (int i = 0; i < lhs.Length; i++)
                {
                    lhs[i] = r.Next();
                }
            });


            var t2 = Task.Run(() =>
            {
                for (int i = 0; i < rhs.Length; i++)
                {
                    rhs[i] = r.Next();
                }
            });

            Task.WhenAll(t1, t2).Wait();

            Console.WriteLine("done data preparing step");

            var st1 = Stopwatch.StartNew();

            for (int i = 0; i < 10; i++)
            {
                SIMDArrayAddition(lhs, rhs);
            }

            st1.Stop();

            var st2 = Stopwatch.StartNew();
            for (int i = 0; i < 10; i++)
            {
                ArrayAddition(lhs, rhs);
            }

            st2.Stop();

            Console.WriteLine(st1.Elapsed);
            Console.WriteLine(st2.Elapsed);
            Console.ReadLine();
        }

        public static int[] ArrayAddition(int[] lhs, int[] rhs)
        {
            var result = new int[lhs.Length];
            for (int i = 0; i < lhs.Length; i++)
            {
                result[i] = lhs[i] + rhs[i];
            }
            return result;
        }

        public static int[] SIMDArrayAddition(int[] lhs, int[] rhs)
        {
            var simdLength = Vector<int>.Count;
            var result = new int[lhs.Length];
            var i = 0;
            for (i = 0; i <= lhs.Length - simdLength; i += simdLength)
            {
                var va = new Vector<int>(lhs, i);
                var vb = new Vector<int>(rhs, i);
                (va + vb).CopyTo(result, i);
            }

            for (; i < lhs.Length; ++i)
            {
                result[i] = lhs[i] + rhs[i];
            }

            return result;
        }
    }
}