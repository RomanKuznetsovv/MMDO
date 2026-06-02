using System;

namespace Mmdo_coursework
{
    public struct Fraction : IComparable<Fraction>
    {
        public long Num { get; }
        public long Den { get; }

        public Fraction(long num, long den)
        {
            if (den == 0) throw new DivideByZeroException();
            long gcd = GCD(Math.Abs(num), Math.Abs(den));
            Num = (num * den < 0 ? -Math.Abs(num) : Math.Abs(num)) / gcd;
            Den = Math.Abs(den) / gcd;
        }

        public Fraction(long num) : this(num, 1) { }

        public static Fraction FromDouble(double val)
        {
            long sign = Math.Sign(val); val = Math.Abs(val);
            long whole = (long)val;
            double frac = val - whole;
            long num = (long)Math.Round(frac * 100000);
            long den = 100000;
            return new Fraction(sign * (whole * den + num), den);
        }

        private static long GCD(long a, long b)
        {
            while (b != 0) { long temp = b; b = a % b; a = temp; }
            return a;
        }

        public Fraction GetFractionalPart()
        {
            if (Den == 1 || Num == 0) return new Fraction(0);
            long floor = (Num >= 0) ? (Num / Den) : ((Num - Den + 1) / Den);
            return this - new Fraction(floor, 1);
        }

        public static Fraction operator +(Fraction a, Fraction b)
        {
            long gcdDen = GCD(a.Den, b.Den);
            long num = a.Num * (b.Den / gcdDen) + b.Num * (a.Den / gcdDen);
            long den = (a.Den / gcdDen) * b.Den;
            return new Fraction(num, den);
        }

        public static Fraction operator -(Fraction a, Fraction b)
        {
            long gcdDen = GCD(a.Den, b.Den);
            long num = a.Num * (b.Den / gcdDen) - b.Num * (a.Den / gcdDen);
            long den = (a.Den / gcdDen) * b.Den;
            return new Fraction(num, den);
        }

        public static Fraction operator *(Fraction a, Fraction b)
        {
            long gcd1 = GCD(Math.Abs(a.Num), b.Den);
            long gcd2 = GCD(Math.Abs(b.Num), a.Den);
            return new Fraction((a.Num / gcd1) * (b.Num / gcd2), (a.Den / gcd2) * (b.Den / gcd1));
        }

        public static Fraction operator /(Fraction a, Fraction b)
        {
            if (b.Num == 0) throw new DivideByZeroException();
            long gcd1 = GCD(Math.Abs(a.Num), Math.Abs(b.Num));
            long gcd2 = GCD(Math.Abs(b.Den), a.Den);
            long sign = Math.Sign(a.Num) * Math.Sign(b.Num);
            return new Fraction(sign * (Math.Abs(a.Num) / gcd1) * (Math.Abs(b.Den) / gcd2), (a.Den / gcd2) * (Math.Abs(b.Num) / gcd1));
        }

        public static Fraction operator -(Fraction a) => new Fraction(-a.Num, a.Den);
        public static bool operator <(Fraction a, Fraction b) => (a - b).Num < 0;
        public static bool operator >(Fraction a, Fraction b) => (a - b).Num > 0;
        public static bool operator <=(Fraction a, Fraction b) => (a - b).Num <= 0;
        public static bool operator >=(Fraction a, Fraction b) => (a - b).Num >= 0;

        public static bool operator ==(Fraction a, Fraction b) => a.Num == b.Num && a.Den == b.Den;
        public static bool operator !=(Fraction a, Fraction b) => !(a == b);

        public override bool Equals(object obj) => obj is Fraction f && this == f;
        public override int GetHashCode() => (Num, Den).GetHashCode();
        public int CompareTo(Fraction other) => (this - other).Num.CompareTo(0);
        public override string ToString() => Num == 0 ? "0" : Den == 1 ? Num.ToString() : $"{Num}/{Den}";
    }
}