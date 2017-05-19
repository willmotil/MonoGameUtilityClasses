﻿using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Microsoft.Xna.Framework //StringBuilder 4th major fix or change
{
    /// <summary> Change details
    /// No garbage stringbuilder
    /// The purpose of this class is to eliminate garbage collections. Performance is to be considered.
    /// While this is not for high precision, ill attempt to get it into reasonable shape over time.
    /// ...
    /// Change log 2017
    /// ...
    /// March 13 Added chained append funtionality that was getting annoying not having it.
    /// ...
    /// March 14 Cut out excess trailing float and double zeros. testing that for now.
    /// this appears to only be a partial fix for 1 case.
    /// ...
    /// March 17 Fixed a major floating point error 
    /// this fix 
    /// Shifted the remainder of floats doubles into the higher precision integer range.
    /// This is my second attempt to fix this. 
    /// Though it leaves some natural low bit error.
    /// It appears the higher order bit error is removed with this fix.
    /// ...
    /// March 17 Fixed the second case for trailing zeros. 
    /// (its starting to look a bit better) 
    /// ...
    /// Note ... Just realized as well i might be able to make this operate like a string after all.
    /// Even though all the guy on stack overflow and i myself initially thought this impossible.
    /// Im actually thinking the append with overloading can allow this to be done right.
    /// msg2 = 10 + "some" + "msg" + 1f; 
    /// ill take a shot at this later on its not that important.
    /// </summary>
    public sealed class MgStringBuilder
    {
        private static char decimalseperator = '.';
        private static char minus = '-';
        private static char plus = '+';

        // im considering pulling this, 
        // its a hack solution that can still fail 
        // for a edge case that is rare.
        private static StringBuilder last;
        
        private StringBuilder sb;
        // this is a property to ensure copy by value reference.
        // seen summary notes
        /// <summary>
        /// It is recommended you avoid this unless needed. 
        /// it is possible to create garbage with it.
        /// </summary>
        public StringBuilder StringBuilder
        {
            get 
            { 
                return sb; 
            }
            private set 
            { 
                sb = value; 
                /*last = sb;*/ 
            }
        }

        public int Length
        {
            get { return StringBuilder.Length; }
            set { StringBuilder.Length = value; }
        }
        public int Capacity
        {
            get { return StringBuilder.Capacity; }
            set { StringBuilder.Capacity = value; }
        }
        public void Clear()
        {
            Length = 0;
            sb.Length = 0;
        }

        // constructors
        public MgStringBuilder()
        {
            StringBuilder = StringBuilder;
            if (sb == null) { sb = new StringBuilder(); }
            //if (last == null) { last = new StringBuilder(); }
        }
        public MgStringBuilder(int capacity)
        {
            StringBuilder = new StringBuilder(capacity);
            if (sb == null) { sb = new StringBuilder(); }
            //if (last == null) { last = new StringBuilder(); }
        }
        public MgStringBuilder(StringBuilder sb)
        {
            StringBuilder = sb;
            if (sb == null) { sb = new StringBuilder(); }
            //if (last == null) { last = new StringBuilder(); }
        }
        public MgStringBuilder(string s)
        {
            StringBuilder = new StringBuilder(s);
            if (sb == null) { sb = new StringBuilder(); }
            //if (last == null) { last = new StringBuilder(); }
        }

        public static void CheckSeperator()
        {
            decimalseperator = Convert.ToChar(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
        }

        // operators
        public static implicit operator MgStringBuilder(StringBuilder sb)
        {
            return new MgStringBuilder(sb);
        }
        public static implicit operator StringBuilder(MgStringBuilder msb)
        {
            return msb.StringBuilder;
        }

        public static MgStringBuilder operator +(MgStringBuilder sbm, MgStringBuilder s)
        {
            sbm.StringBuilder.Append(s);
            return sbm;
        }
        //test
        public static MgStringBuilder operator +(MgStringBuilder sbm, string s)
        {
            sbm.StringBuilder.Append(s);
            return sbm;
        }

        public void AppendAt(int index, StringBuilder s)
        {
            int len = this.StringBuilder.Length;
            int reqcapacity = (index + s.Length + 1) - this.StringBuilder.Capacity;
            if (reqcapacity > 0)
                this.StringBuilder.Capacity += reqcapacity;

            int initialLength = StringBuilder.Length;
            //If we append near the end we can run out of space in the for loop. Make sure we are large enough
            if (StringBuilder.Length < index + s.Length)
            {
                StringBuilder.Length = index + s.Length;
            }

            //If our appendAt is outside the scope we need to add spaces until then
            if (index > initialLength - 1)
            {
                for (int j = initialLength - 1; j < index; j++)
                {
                    StringBuilder[j] = ' ';
                }
            }

            for (int i = 0; i < s.Length; i++)
            {
                this.StringBuilder[i + index] = (char)(s[i]);
            }
        }
        //public MgStringBuilder AppendAt(int index, StringBuilder s)
        //{
        //    int len = this.StringBuilder.Length;
        //    int reqcapacity = (index + s.Length + 1) - this.StringBuilder.Capacity;
        //    if (reqcapacity > 0)
        //        this.StringBuilder.Capacity += reqcapacity;

        //    for (int i = 0; i < s.Length; i++)
        //    {
        //        this.StringBuilder[i + index] = (char)(s[i]);
        //    }
        //    return this;
        //}

        public MgStringBuilder Append(StringBuilder s)
        {
            int len = this.StringBuilder.Length;
            int reqcapacity = (s.Length + len) - this.StringBuilder.Capacity;
            //int reqcapacity = (s.Length + len +1) - this.StringBuilder.Capacity;
            if (reqcapacity > 0)
                this.StringBuilder.Capacity += reqcapacity;

            this.StringBuilder.Length = len + s.Length;
            for(int i = 0;i< s.Length;i++)
            {
                this.StringBuilder[i + len] = (char)(s[i]);
            }
            return this;
        }
        public MgStringBuilder Append(string s)
        {
            this.StringBuilder.Append(s);
            return this;
        }
        public MgStringBuilder Append(bool value)
        {
            this.StringBuilder.Append(value);
            return this;
        }
        public MgStringBuilder Append(byte value)
        {
            // basics
            int num = value;
            if (num == 0)
            {
                sb.Append('0');
                return this;
            }
            int place = 100;
            if (num >= place * 10)
            {
                // just append it
                sb.Append(num);
                return this;
            }
            // part 1 pull integer digits
            bool addzeros = false;
            while (place > 0)
            {
                if (num >= place)
                {
                    addzeros = true;
                    int modulator = place * 10;
                    int val = num % modulator;
                    int dc = val / place;
                    sb.Append((char)(dc + 48));
                }
                else
                {
                    if (addzeros) { sb.Append('0'); }
                }
                place = (int)(place * .1);
            }
            return this;
        }
        public MgStringBuilder Append(short value)
        {
            int num = value;
            // basics
            if (num < 0)
            {
                // Negative.
                sb.Append(minus);
                num = -num;
            }
            if (value == 0)
            {
                sb.Append('0');
                return this;
            }

            int place = 10000;
            if (num >= place * 10)
            {
                // just append it, if its this big, this isn't a science calculator, its a edge case.
                sb.Append(num);
                return this;
            }
            // part 1 pull integer digits
            bool addzeros = false;
            while (place > 0)
            {
                if (num >= place)
                {
                    addzeros = true;
                    int modulator = place * 10;
                    int val = num % modulator;
                    int dc = val / place;
                    sb.Append((char)(dc + 48));
                }
                else
                {
                    if (addzeros) { sb.Append('0'); }
                }
                place = (int)(place * .1);
            }
            return this;
        }
        public MgStringBuilder Append(int value)
        {
            // basics
            if (value < 0)
            {
                // Negative.
                sb.Append(minus);
                value = -value;
            }
            if (value == 0)
            {
                sb.Append('0');
                return this;
            }

            int place = 1000000000;
            if (value >= place * 10)
            {
                // just append it
                sb.Append(value);
                return this;
            }
            // part 1 pull integer digits
            int n = (int)(value);
            bool addzeros = false;
            while (place > 0)
            {
                if (n >= place)
                {
                    addzeros = true;
                    int modulator = place * 10;
                    int val = n % modulator;
                    int dc = val / place;
                    sb.Append((char)(dc + 48));
                }
                else
                {
                    if (addzeros) { sb.Append('0'); }
                }
                place = (int)(place * .1);
            }
            return this;
        }
        public MgStringBuilder Append(long value)
        {
            // basics
            if (value < 0)
            {
                // Negative.
                sb.Append(minus);
                value = -value;
            }
            if (value == 0)
            {
                sb.Append('0');
                return this;
            }

            long place = 10000000000000000L;
            if (value >= place * 10)
            {
                // just append it,
                sb.Append(value);
                return this;
            }
            // part 1 pull integer digits
            long n = (long)(value);
            bool addzeros = false;
            while (place > 0)
            {
                if (n >= place)
                {
                    addzeros = true;
                    long modulator = place * 10L;
                    long val = n % modulator;
                    long dc = val / place;
                    sb.Append((char)(dc + 48));
                }
                else
                {
                    if (addzeros) { sb.Append('0'); }
                }
                place = (long)(place * .1);
            }
            return this;
        }

        public MgStringBuilder Append(float value)
        {
            // basics
            bool addZeros = false;
            int n = (int)(value);
            int place = 100000000;
            if (value < 0)
            {
                // Negative.
                sb.Append(minus);
                value = -value;
            }
            if (value == 0)
            {
                sb.Append('0');
                return this;
            }
            // fix march 18-17
            // values not zero value is at least a integer
            if (value <= -1f || value >= 1f)
            {

                place = 100000000;
                if (value >= place * 10)
                {
                    // just append it, if its this big its a edge case.
                    sb.Append(value);
                    return this;
                }
                // part 1 pull integer digits
                // int n =  // moved
                addZeros = false;
                while (place > 0)
                {
                    if (n >= place)
                    {
                        addZeros = true;
                        int modulator = place * 10;
                        int val = n % modulator;
                        int dc = val / place;
                        sb.Append((char)(dc + 48));
                    }
                    else
                    {
                        if (addZeros) { sb.Append('0'); }
                    }
                    place = (int)(place * .1);
                }
            }
            else
                sb.Append('0');

            sb.Append(decimalseperator);

            // part 2 

            // floating point part now it can have about 28 digits but uh ya.. nooo lol
            place = 1000000;
            // pull decimal to integer digits, based on the number of place digits
            int dn = (int)( (value - (float)(n) ) * place * 10);
            // ... march 17 testing... cut out extra zeros case 1
            if (dn == 0)
            {
                sb.Append('0');
                return this;
            }
            addZeros = true;
            while (place > 0)
            {
                if (dn >= place)
                {
                    //addzeros = true;
                    int modulator = place * 10;
                    int val = dn % modulator;
                    int dc = val / place;
                    sb.Append((char)(dc + 48));
                    if (val - dc * place == 0) // && trimEndZeros this would be a acstetic
                    {
                        return this;
                    }
                }
                else
                {
                    if (addZeros) { sb.Append('0'); }
                }
                place = (int)(place * .1);
            }
            return this;
        }

        public MgStringBuilder Append(double value)
        {
            // basics
            bool addZeros = false;
            long n = (long)(value);
            long place = 10000000000000000L;
            if (value < 0) // is Negative.
            {
                sb.Append(minus);
                value = -value;
            }
            if (value == 0) // is Zero
            {
                sb.Append('0');
                return this;
            }
            if (value <= -1d || value >= 1d) // is a Integer
            {
                if (value >= place * 10)
                {
                    sb.Append(value); // is big, just append its a edge case.
                    return this;
                }
                // part 1 pull integer digits
                addZeros = false;
                while (place > 0)
                {
                    if (n >= place)
                    {
                        addZeros = true;
                        long modulator = place * 10;
                        long val = n % modulator;
                        long dc = val / place;
                        sb.Append((char)(dc + 48));
                    }
                    else
                        if (addZeros) { sb.Append('0'); }

                    place = (long)(place * .1d);
                }
            }
            else
                sb.Append('0');

            sb.Append(decimalseperator);

            // part 2 
            // floating point part now it can have about 28 digits but uh ya.. nooo lol
            place = 1000000000000000L;
            // pull decimal to integer digits, based on the number of place digits
            long dn = (long)((value - (double)(n)) * place * 10);
            if (dn == 0)
            {
                sb.Append('0');
                return this;
            }
            addZeros = true;
            while (place > 0)
            {
                if (dn >= place)
                {
                    long modulator = place * 10;
                    long val = dn % modulator;
                    long dc = val / place;
                    sb.Append((char)(dc + 48));
                    if (val - dc * place == 0) // && trimEndZeros  aectetic
                    {
                        return this;
                    }
                }
                else
                    if (addZeros) { sb.Append('0'); }

                place = (long)(place * .1);
            }
            return this;
        }

        public MgStringBuilder Append(Vector2 value)
        {
            Append("(");
            Append(value.X);
            Append(",");
            Append(value.Y);
            Append(")");
            return this;
        }
        public MgStringBuilder Append(Vector3 value)
        {
            Append("(");
            Append(value.X);
            Append(",");
            Append(value.Y);
            Append(",");
            Append(value.Z);
            Append(")");
            return this;
        }
        public MgStringBuilder Append(Vector4 value)
        {
            Append("(");
            Append(value.X);
            Append(",");
            Append(value.Y);
            Append(",");
            Append(value.Z);
            Append(",");
            Append(value.W);
            Append(")");
            return this;
        }
        public MgStringBuilder Append(Color value)
        {
            Append("(");
            Append(value.R);
            Append(",");
            Append(value.G);
            Append(",");
            Append(value.B);
            Append(",");
            Append(value.A);
            Append(")");
            return this;
        }

        public MgStringBuilder AppendTrim(float value)
        {
            // basics
            bool addZeros = false;
            int n = (int)(value);
            int place = 100000000;
            if (value < 0)
            {
                // Negative.
                sb.Append(minus);
                value = -value;
            }
            if (value == 0)
            {
                sb.Append('0');
                return this;
            }
            // fix march 18-17
            // values not zero value is at least a integer
            if (value <= -1f || value >= 1f)
            {

                place = 100000000;
                if (value >= place * 10)
                {
                    // just append it, if its this big its a edge case.
                    sb.Append(value);
                    return this;
                }
                // part 1 pull integer digits
                // int n =  // moved
                addZeros = false;
                while (place > 0)
                {
                    if (n >= place)
                    {
                        addZeros = true;
                        int modulator = place * 10;
                        int val = n % modulator;
                        int dc = val / place;
                        sb.Append((char)(dc + 48));
                    }
                    else
                    {
                        if (addZeros) { sb.Append('0'); }
                    }
                    place = (int)(place * .1);
                }
            }
            else
                sb.Append('0');

            sb.Append(decimalseperator);

            // part 2 

            // floating point part now it can have about 28 digits but uh ya.. nooo lol
            place = 100;
            // pull decimal to integer digits, based on the number of place digits
            int dn = (int)((value - (float)(n)) * place * 10);
            // ... march 17 testing... cut out extra zeros case 1
            if (dn == 0)
            {
                sb.Append('0');
                return this;
            }
            addZeros = true;
            while (place > 0)
            {
                if (dn >= place)
                {
                    //addzeros = true;
                    int modulator = place * 10;
                    int val = dn % modulator;
                    int dc = val / place;
                    sb.Append((char)(dc + 48));
                    if (val - dc * place == 0) // && trimEndZeros this would be a acstetic
                    {
                        return this;
                    }
                }
                else
                {
                    if (addZeros) { sb.Append('0'); }
                }
                place = (int)(place * .1);
            }
            return this;
        }

        public MgStringBuilder AppendTrim(double value)
        {
            // basics
            bool addZeros = false;
            long n = (long)(value);
            long place = 10000000000000000L;
            if (value < 0) // is Negative.
            {
                sb.Append(minus);
                value = -value;
            }
            if (value == 0) // is Zero
            {
                sb.Append('0');
                return this;
            }
            if (value <= -1d || value >= 1d) // is a Integer
            {
                if (value >= place * 10)
                {
                    sb.Append(value); // is big, just append its a edge case.
                    return this;
                }
                // part 1 pull integer digits
                addZeros = false;
                while (place > 0)
                {
                    if (n >= place)
                    {
                        addZeros = true;
                        long modulator = place * 10;
                        long val = n % modulator;
                        long dc = val / place;
                        sb.Append((char)(dc + 48));
                    }
                    else
                        if (addZeros) { sb.Append('0'); }

                    place = (long)(place * .1);
                }
            }
            else
                sb.Append('0');

            sb.Append(decimalseperator);

            // part 2 
            // floating point part now it can have about 28 digits but uh ya.. nooo lol
            place = 100L;
            // pull decimal to integer digits, based on the number of place digits
            long dn = (long)((value - (double)(n)) * place * 10);
            if (dn == 0)
            {
                sb.Append('0');
                return this;
            }
            addZeros = true;
            while (place > 0)
            {
                if (dn >= place)
                {
                    long modulator = place * 10;
                    long val = dn % modulator;
                    long dc = val / place;
                    sb.Append((char)(dc + 48));
                    if (val - dc * place == 0) // && trimEndZeros  aectetic
                    {
                        return this;
                    }
                }
                else
                    if (addZeros) { sb.Append('0'); }

                place = (long)(place * .1);
            }
            return this;
        }
 
        public MgStringBuilder AppendTrim(Vector2 value)
        {
            Append("(");
            AppendTrim(value.X);
            Append(",");
            AppendTrim(value.Y);
            Append(")");
            return this;
        }
        public MgStringBuilder AppendTrim(Vector3 value)
        {
            Append("(");
            AppendTrim(value.X);
            Append(",");
            AppendTrim(value.Y);
            Append(",");
            AppendTrim(value.Z);
            Append(")");
            return this;
        }
        public MgStringBuilder AppendTrim(Vector4 value)
        {
            Append("(");
            AppendTrim(value.X);
            Append(",");
            AppendTrim(value.Y);
            Append(",");
            AppendTrim(value.Z);
            Append(",");
            AppendTrim(value.W);
            Append(")");
            return this;
        }

        public void AppendLine(StringBuilder s)
        {
            sb.AppendLine();
            Append(s);
        }
        public void AppendLine(string s)
        {
            sb.AppendLine();
            sb.Append(s);
        }
        public MgStringBuilder AppendLine()
        {
            sb.AppendLine();
            return this;
        }

        public MgStringBuilder Insert(int index, StringBuilder s)
        {
            this.StringBuilder.Insert(index, s);
            return this;
        }
        public MgStringBuilder Remove(int index, int length)
        {
            this.StringBuilder.Remove(index, length);
            return this;
        }

        public char[] ToCharArray()
        {
            char[] a = new char[sb.Length];
            sb.CopyTo(0, a, 0, sb.Length);
            return a;
        }
        public override string ToString()
        {
            return sb.ToString();
        }
    }
}


//public MgStringBuilder Append(double number)
//{

//    // basics
//    if (number < 0)
//    {
//        // Negative.
//        sb.Append(minus);
//        number = -number;
//    }
//    if (number == 0)
//    {
//        sb.Append('0');
//        return this;
//    }

//    long place = 10000000000000000L;
//    if (number >= place * 10)
//    {
//        // just append it, if its this big its a edge case.
//        sb.Append(number);
//        return this;
//    }
//    // part 1 pull integer digits
//    long n = (long)(number);
//    bool addzeros = false;
//    while (place > 0)
//    {
//        if (n >= place)
//        {
//            addzeros = true;
//            long modulator = place * 10L;
//            long val = n % modulator;
//            long dc = val / place;
//            sb.Append((char)(dc + 48));
//        }
//        else
//        {
//            if (addzeros) { sb.Append('0'); }
//        }
//        place = (long)(place * .1);
//    }

//    // the decimal part
//    double nd = number - (double)(n);
//    //
//    // ... march 17 testing... cut out extra zeros seems to work so far
//    //
//    if (nd == .000000000000000000000d)
//    {
//        return this;
//    }
//    if (nd > 0 && nd < 1)
//    {
//        sb.Append(decimalseperator);
//    }
//    addzeros = true;
//    //nd = number;
//    double placed = .1;
//    while (placed > 0.0000000000001)
//    {
//        if (nd > placed)
//        {
//            double modulator = placed * 10;
//            double val = nd % modulator;
//            double dc = val / placed;
//            sb.Append((char)(dc + 48));
//        }
//        else
//        {
//            if (addzeros) { sb.Append('0'); }
//        }
//        placed = placed * .1;
//    }
//    return this;
//}


//public MgStringBuilder AppendTrim(float value)
//{
//    // basics
//    if (value < 0)
//    {
//        // Negative.
//        sb.Append(minus);
//        value = -value;
//    }
//    if (value == 0)
//    {
//        sb.Append('0');
//        return this; 
//    }

//    int place = 100000000;
//    if (value >= place * 10)
//    {
//        // just append it, if its this big its a edge case.
//        sb.Append(value);
//        return this; 
//    }
//    // part 1 pull integer digits
//    int n = (int)(value);
//    bool addzeros = false;
//    while (place > 0)
//    {
//        if (n >= place)
//        {
//            addzeros = true;
//            int modulator = place * 10;
//            int val = n % modulator;
//            int dc = val / place;
//            sb.Append((char)(dc + 48));
//        }
//        else
//        {
//            if (addzeros) { sb.Append('0'); }
//        }
//        place = (int)(place * .1);
//    }

//    // ok lets try again
//    float nd = value - (float)(n);
//    sb.Append(decimalseperator);
//    addzeros = true;
//    //nd = value;
//    float placed = .1f;
//    while (placed > 0.001)
//    {
//        if (nd > placed)
//        {
//            float modulator = placed * 10;
//            float val = nd % modulator;
//            float dc = val / placed;
//            sb.Append((char)(dc + 48));
//        }
//        else
//        {
//            if (addzeros) { sb.Append('0'); }
//        }
//        placed = placed * .1f;
//    }
//    return this;
//}
//public MgStringBuilder AppendTrim(double number)
//{
//    // basics
//    if (number < 0)
//    {
//        // Negative.
//        sb.Append(minus);
//        number = -number;
//    }
//    if (number == 0)
//    {
//        sb.Append('0');
//        return this; ;
//    }
//    long place = 10000000000000000L;
//    if (number >= place * 10)
//    {
//        // just append it, if its this big its a edge case.
//        sb.Append(number);
//        return this; ;
//    }
//    // part 1 pull integer digits
//    long n = (long)(number);
//    bool addzeros = false;
//    while (place > 0)
//    {
//        if (n >= place)
//        {
//            addzeros = true;
//            long modulator = place * 10L;
//            long val = n % modulator;
//            long dc = val / place;
//            sb.Append((char)(dc + 48));
//        }
//        else
//        {
//            if (addzeros) { sb.Append('0'); }
//        }
//        place = (long)(place * .1);
//    }

//    // ok lets try again
//    double nd = number - (double)(n);
//    sb.Append(decimalseperator);
//    addzeros = true;
//    //nd = number;
//    double placed = .1;
//    while (placed > 0.001)
//    {
//        if (nd > placed)
//        {
//            double modulator = placed * 10;
//            double val = nd % modulator;
//            double dc = val / placed;
//            sb.Append((char)(dc + 48));
//        }
//        else
//        {
//            if (addzeros) { sb.Append('0'); }
//        }
//        placed = placed * .1;
//    }
//    return this;
//}