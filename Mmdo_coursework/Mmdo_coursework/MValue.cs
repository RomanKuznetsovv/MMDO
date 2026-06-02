using System;

namespace Mmdo_coursework
{
    public struct MValue : IComparable<MValue>
    {
        public Fraction Real;
        public Fraction M;

        public MValue(Fraction real, Fraction m) { Real = real; M = m; }
        public MValue(Fraction real) { Real = real; M = new Fraction(0); }

        public static MValue operator +(MValue a, MValue b) => new MValue(a.Real + b.Real, a.M + b.M);
        public static MValue operator -(MValue a, MValue b) => new MValue(a.Real - b.Real, a.M - b.M);
        public static MValue operator *(MValue a, Fraction scalar) => new MValue(a.Real * scalar, a.M * scalar);
        public static MValue operator /(MValue a, Fraction scalar) => new MValue(a.Real / scalar, a.M / scalar);

        public int CompareTo(MValue other)
        {
            int mCmp = M.CompareTo(other.M);
            return mCmp != 0 ? mCmp : Real.CompareTo(other.Real);
        }

        public static bool operator <(MValue a, MValue b) => a.CompareTo(b) < 0;
        public static bool operator >(MValue a, MValue b) => a.CompareTo(b) > 0;
        public static bool operator ==(MValue a, MValue b) => a.Real == b.Real && a.M == b.M;
        public static bool operator !=(MValue a, MValue b) => !(a == b);

        public override bool Equals(object obj) => obj is MValue m && this == m;
        public override int GetHashCode() => (Real, M).GetHashCode();

        public override string ToString()
        {
            if (M.Num == 0) return Real.ToString();
            string mStr = M.Num == M.Den ? "M" : (M.Num == -M.Den ? "-M" : $"{M}M");
            if (Real.Num == 0) return mStr;
            if (M.Num > 0) return $"{Real} + {mStr}";
            return $"{Real} - {mStr.Substring(1)}";
        }
    }
}