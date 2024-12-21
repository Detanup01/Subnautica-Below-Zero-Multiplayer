namespace Subnautica.Network.Structures
{
    using MessagePack;
    using System;
    using UnityEngine;

    [MessagePackObject]
    public class ZeroVector3 : IEquatable<ZeroVector3>
    {
        [Key(0)]
        public float X;

        [Key(1)]
        public float Y;

        [Key(2)]
        public float Z;

        public ZeroVector3()
        {
        }

        public ZeroVector3(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public static bool operator ==(ZeroVector3 u, ZeroVector3 v)
        {
            if (u is null && v is null)
            {
                return true;
            }

            return u is ZeroVector3 && u.Equals(v);
        }

        public static bool operator !=(ZeroVector3 u, ZeroVector3 v)
        {
            return !(u == v);
        }

        public bool Equals(ZeroVector3 other)
        {
            if (other is null)
            {
                return false;
            }

            return other.X == this.X && other.Y == this.Y && other.Z == this.Z;
        }

        public override bool Equals(object obj)
        {
            return obj is ZeroVector3 other && this.Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 0;
                hash = (hash * 397) ^ this.X.GetHashCode();
                hash = (hash * 397) ^ this.Y.GetHashCode();
                hash = (hash * 397) ^ this.Z.GetHashCode();

                return hash;
            }
        }

        public override string ToString()
        {
            return $"[ZeroVector3: {this.X}, {this.Y}, {this.Z}]";
        }

        public static ZeroVector3 Lerp(ZeroVector3 a, ZeroVector3 b, float t)
        {
            t = Mathf.Clamp01(t);
            return new ZeroVector3(a.X + (b.X - a.X) * t, a.Y + (b.Y - a.Y) * t, a.Z + (b.Z - a.Z) * t);
        }

        public float Distance(ZeroVector3 target)
        {
            float diffX = target.X - this.X;
            float diffY = target.Y - this.Y;
            float diffZ = target.Z - this.Z;

            return diffX * diffX + diffY * diffY + diffZ * diffZ;
        }

        public float Distance(Vector3 target)
        {
            float diffX = target.x - this.X;
            float diffY = target.y - this.Y;
            float diffZ = target.z - this.Z;

            return diffX * diffX + diffY * diffY + diffZ * diffZ;
        }

        public static float Distance(Vector3 from, Vector3 target)
        {
            float diffX = target.x - from.x;
            float diffY = target.y - from.y;
            float diffZ = target.z - from.z;

            return diffX * diffX + diffY * diffY + diffZ * diffZ;
        }
    }
}