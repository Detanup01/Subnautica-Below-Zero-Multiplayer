﻿namespace Subnautica.API.Features.Compressors
{
    using Subnautica.Network.Structures;
    using System;
    using UnityEngine;

    public class UnityCompressor_Long
    {
        public enum Metadata
        {
            None = 0x0000000,
            X = 0x0000001,
            Y = 0x0000002,
            Z = 0x0000004
        }

        private static long BigNumber = 1000000L * 1000000L * 1000000L;

        private const float FLOAT_PRECISION_MULT = 10000f;

        public static long Vector3Compress(float x, float y, float z)
        {
            var qData = Metadata.None;
            if (x < 0)
            {
                qData |= Metadata.X;
            }

            if (y < 0)
            {
                qData |= Metadata.Y;
            }

            if (z < 0)
            {
                qData |= Metadata.Z;
            }

            var xData = (long)(Math.Abs(x) * 100);
            var yData = (long)(Math.Abs(y) * 100) * 1000000;
            var zData = (long)(Math.Abs(z) * 100) * 1000000 * 1000000;

            return (1000000000000000000 * (long)qData) + (xData + yData + zData);
        }

        public static Vector3 Vector3Decompress(long longNumber)
        {
            var flag = (byte)(longNumber / BigNumber);
            longNumber -= BigNumber * flag;

            var zData = longNumber / (1000000L * 1000000L);
            longNumber -= (1000000L * 1000000L) * zData;

            var yData = longNumber / (1000000L);
            longNumber -= (1000000L) * yData;

            if ((flag & 0x0000001) == 0x0000001)
            {
                longNumber *= -1;
            }

            if ((flag & 0x0000002) == 0x0000002)
            {
                yData *= -1;
            }

            if ((flag & 0x0000004) == 0x0000004)
            {
                zData *= -1;
            }

            return new Vector3(longNumber / 100f, yData / 100f, zData / 100f);
        }

        public static ZeroVector3 ZeroVector3Decompress(long longNumber)
        {
            var flag = (byte)(longNumber / BigNumber);
            longNumber -= BigNumber * flag;

            var zData = longNumber / (1000000L * 1000000L);
            longNumber -= (1000000L * 1000000L) * zData;

            var yData = longNumber / (1000000L);
            longNumber -= (1000000L) * yData;

            if ((flag & 0x0000001) == 0x0000001)
            {
                longNumber *= -1;
            }

            if ((flag & 0x0000002) == 0x0000002)
            {
                yData *= -1;
            }

            if ((flag & 0x0000004) == 0x0000004)
            {
                zData *= -1;
            }

            return new ZeroVector3(longNumber / 100f, yData / 100f, zData / 100f);
        }

        public static long QuaternionCompress(Quaternion rotation)
        {
            var maxIndex = (byte)0;
            var maxValue = float.MinValue;
            var sign = 1f;

            for (int i = 0; i < 4; i++)
            {
                var element = rotation[i];
                if (element > maxValue)
                {
                    maxIndex = (byte)i;
                    maxValue = element;
                }
            }

            var a = (short)0;
            var b = (short)0;
            var c = (short)0;

            if (maxIndex == 0)
            {
                a = (short)(rotation.y * sign * FLOAT_PRECISION_MULT);
                b = (short)(rotation.z * sign * FLOAT_PRECISION_MULT);
                c = (short)(rotation.w * sign * FLOAT_PRECISION_MULT);
            }
            else if (maxIndex == 1)
            {
                a = (short)(rotation.x * sign * FLOAT_PRECISION_MULT);
                b = (short)(rotation.z * sign * FLOAT_PRECISION_MULT);
                c = (short)(rotation.w * sign * FLOAT_PRECISION_MULT);
            }
            else if (maxIndex == 2)
            {
                a = (short)(rotation.x * sign * FLOAT_PRECISION_MULT);
                b = (short)(rotation.y * sign * FLOAT_PRECISION_MULT);
                c = (short)(rotation.w * sign * FLOAT_PRECISION_MULT);
            }
            else
            {
                a = (short)(rotation.x * sign * FLOAT_PRECISION_MULT);
                b = (short)(rotation.y * sign * FLOAT_PRECISION_MULT);
                c = (short)(rotation.z * sign * FLOAT_PRECISION_MULT);
            }

            var xData = (long)Math.Abs(a);
            var yData = (long)Math.Abs(b) * 100000;
            var zData = (long)Math.Abs(c) * 100000 * 100000;
            var mData = (long)Math.Abs(maxIndex) * 100000 * 100000 * 100000;

            var qData = Metadata.None;
            if (a < 0)
            {
                qData |= Metadata.X;
            }

            if (b < 0)
            {
                qData |= Metadata.Y;
            }

            if (c < 0)
            {
                qData |= Metadata.Z;
            }

            return (1000000000000000000 * (long)qData) + (xData + yData + zData + mData);
        }

        public static Quaternion QuaternionDecompress(long longNumber)
        {
            var flag = (byte)(longNumber / BigNumber);
            longNumber -= BigNumber * flag;

            var qData = longNumber / (100000L * 100000L * 100000L);
            longNumber -= (100000L * 100000L * 100000L) * qData;

            var zData = longNumber / (100000L * 100000L);
            longNumber -= (100000L * 100000L) * zData;

            var yData = longNumber / (100000L);
            longNumber -= (100000L) * yData;

            if ((flag & 0x0000001) == 0x0000001)
            {
                longNumber *= -1;
            }

            if ((flag & 0x0000002) == 0x0000002)
            {
                yData *= -1;
            }

            if ((flag & 0x0000004) == 0x0000004)
            {
                zData *= -1;
            }

            return ReadCompressedRotation((byte)qData, (short)longNumber, (short)yData, (short)zData);
        }

        private static Quaternion ReadCompressedRotation(byte maxIndex, short readerA, short readerB, short readerC)
        {
            if (maxIndex >= 4 && maxIndex <= 7)
            {
                var x = (maxIndex == 4) ? 1f : 0f;
                var y = (maxIndex == 5) ? 1f : 0f;
                var z = (maxIndex == 6) ? 1f : 0f;
                var w = (maxIndex == 7) ? 1f : 0f;

                return new Quaternion(x, y, z, w);
            }

            var a = (float)readerA / FLOAT_PRECISION_MULT;
            var b = (float)readerB / FLOAT_PRECISION_MULT;
            var c = (float)readerC / FLOAT_PRECISION_MULT;
            var d = (float)Math.Sqrt(1f - (a * a + b * b + c * c));

            if (maxIndex == 0)
                return new Quaternion(d, a, b, c);
            else if (maxIndex == 1)
                return new Quaternion(a, d, b, c);
            else if (maxIndex == 2)
                return new Quaternion(a, b, d, c);

            return new Quaternion(a, b, c, d);
        }
    }
}

