using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Microsoft.Xna.Framework
{
    /// <summary>
    /// No garbage stringbuilder William Motill 2017, last fix or change October 16, 2018.
    /// 
    /// The purpose of this class is to eliminate garbage collections. 
    /// Primarily bypassing numeric conversions to string.
    /// While this is not for high precision, Performance is to be considered.
    /// This class can be used in place of stringbuilder it was primarily designed for use with monogame.
    /// 
    ///  Notes
    ///  To use this as a regular c# class simply remove the vector and color overloads.
    ///  It's more like a string now ability wise, since over time ive slowly changed things to add overloads properly.
    ///  So while you can use it with the + "" + operators id avoid that when adding dynamic number variable and stick with .append()
    ///   
    /// ...
    /// Change log 2017
    /// ...
    /// March to December
    /// Added chained append funtionality that was getting annoying not having it.
    /// Cut out excess trailing float and double zeros. This was a partial fix. 
    /// Fixed a major floating point error.  Shifted the remainder of floats doubles into the higher integer range.
    /// Fixed a second edge case for trailing zeros.  
    /// Fixed n = -n;  when values were negative in float double appends.
    /// that would lead to a bug were - integer portions didn't get returned.
    /// yanked some redundant stuff for a un-needed reference swap hack.
    /// ...
    ///  Change log  2018
    /// ...
    /// March 18
    /// Added a Indexer to index into the underlying stringbuilder to directly access chars.
    /// March 20
    /// Added a insert char overload.
    /// Octob 13 
    /// Appendline was adding the new line to the beginning not the end.
    /// Added a method to directly link via reference to the internal string builder this will probably stay in.
    /// Octob 16
    /// The original AppendAt was fixed and renamed to OverWriteAt, The new AppendAt's works as a Insert.
    /// Multiple overloads were added and tested in relation.
    /// Nov 16 
    /// Standardized capacity and length checks to a method.
    /// ...
    /// </summary>
    public sealed class MgStringBuilder
    {
        private static char decimalseperator = '.';
        private static char minus = '-';
        private static char plus = '+';
        private StringBuilder stringbuilder;

        /// <summary>
        /// This was sort of a iffy thing to add i was superstitious it might make garbage, after all this time i guess its pretty safe.
        /// </summary>
        public StringBuilder StringBuilder
        {
            get { return stringbuilder; }
            private set { if (stringbuilder == null) { stringbuilder = value; } else { stringbuilder.Clear(); stringbuilder.Append(value); } }
        }
        /// <summary>
        /// Clears the length without clearing the capacity to prevent garbage deallocation.
        /// </summary>
        public int Length
        {
            get { return stringbuilder.Length; }
            set { stringbuilder.Length = value; }
        }
        /// <summary>
        /// Clears the length without clearing the capacity to prevent garbage deallocation.
        /// </summary>
        public void Clear()
        {
            stringbuilder.Length = 0;
        }
        /// <summary>
        /// Typically you only increase this setting it to say zero would cause a garbage collection to occur.
        /// </summary>
        public int Capacity
        {
            get { return stringbuilder.Capacity; }
            set { stringbuilder.Capacity = value; }
        }

        public static void CheckSeperator()
        {
            decimalseperator = Convert.ToChar(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
        }

        /// <summary>
        /// Indexer to chars in the underlying array
        /// </summary>
        public char this[int i]
        {
            get { return stringbuilder[i]; }
            set { stringbuilder[i] = value; }
        }

        // constructors

        public MgStringBuilder()
        {
            StringBuilder = StringBuilder;
            if (stringbuilder == null) { stringbuilder = new StringBuilder(); }
        }
        public MgStringBuilder(int capacity)
        {
            StringBuilder = new StringBuilder(capacity);
            if (stringbuilder == null) { stringbuilder = new StringBuilder(); }
        }
        public MgStringBuilder(StringBuilder sb)
        {
            StringBuilder = sb;
            if (sb == null) { sb = new StringBuilder(); }
        }
        public MgStringBuilder(string s)
        {
            StringBuilder = new StringBuilder(s);
            if (stringbuilder == null) { stringbuilder = new StringBuilder(); }
        }

        // operators

        public static implicit operator MgStringBuilder(String s)
        {
            return new MgStringBuilder(s);
        }
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
        public static MgStringBuilder operator +(MgStringBuilder sbm, string s)
        {
            sbm.StringBuilder.Append(s);
            return sbm;
        }

        // Methods all the appends are unrolled to squeeze out speed.

        public MgStringBuilder Append(StringBuilder s)
        {
            int len = this.StringBuilder.Length;
            CheckAppendCapacityAndLength(stringbuilder.Length, s.Length);
            for (int i = 0; i < s.Length; i++)
            {
                this.StringBuilder[i + len] = (char)(s[i]);
            }
            return this;
        }
        public MgStringBuilder Append(string s)
        {
            stringbuilder.Append(s);
            return this;
        }
        public MgStringBuilder Append(char value)
        {
            stringbuilder.Append(value);
            return this;
        }
        public MgStringBuilder Append(bool value)
        {
            stringbuilder.Append(value);
            return this;
        }
        public MgStringBuilder Append(byte value)
        {
            // basics
            int num = value;
            if (num == 0)
            {
                stringbuilder.Append('0');
                return this;
            }
            int place = 100;
            if (num >= place * 10)
            {
                // just append it
                stringbuilder.Append(num);
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
                    stringbuilder.Append((char)(dc + 48));
                }
                else
                {
                    if (addzeros) { stringbuilder.Append('0'); }
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
                stringbuilder.Append(minus);
                num = -num;
            }
            if (value == 0)
            {
                stringbuilder.Append('0');
                return this;
            }

            int place = 10000;
            if (num >= place * 10)
            {
                // just append it, if its this big, this isn't a science calculator, its a edge case.
                stringbuilder.Append(num);
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
                    stringbuilder.Append((char)(dc + 48));
                }
                else
                {
                    if (addzeros) { stringbuilder.Append('0'); }
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
                stringbuilder.Append(minus);
                value = -value;
            }
            if (value == 0)
            {
                stringbuilder.Append('0');
                return this;
            }

            int place = 1000000000;
            if (value >= place * 10)
            {
                // just append it
                stringbuilder.Append(value);
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
                    stringbuilder.Append((char)(dc + 48));
                }
                else
                {
                    if (addzeros) { stringbuilder.Append('0'); }
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
                stringbuilder.Append(minus);
                value = -value;
            }
            if (value == 0)
            {
                stringbuilder.Append('0');
                return this;
            }

            long place = 10000000000000000L;
            if (value >= place * 10)
            {
                // just append it,
                stringbuilder.Append(value);
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
                    stringbuilder.Append((char)(dc + 48));
                }
                else
                {
                    if (addzeros) { stringbuilder.Append('0'); }
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
                stringbuilder.Append(minus);
                value = -value;
                n = -n;
            }
            if (value == 0)
            {
                stringbuilder.Append('0');
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
                    stringbuilder.Append(value);
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
                        stringbuilder.Append((char)(dc + 48));
                    }
                    else
                    {
                        if (addZeros) { stringbuilder.Append('0'); }
                    }
                    place = (int)(place * .1);
                }
            }
            else
                stringbuilder.Append('0');

            stringbuilder.Append(decimalseperator);

            // part 2 

            // floating point part now it can have about 28 digits but uh ya.. nooo lol
            place = 1000000;
            // pull decimal to integer digits, based on the number of place digits
            int dn = (int)((value - (float)(n)) * place * 10);
            // ... march 17 testing... cut out extra zeros case 1
            if (dn == 0)
            {
                stringbuilder.Append('0');
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
                    stringbuilder.Append((char)(dc + 48));
                    if (val - dc * place == 0) // && trimEndZeros this would be a acstetic
                    {
                        return this;
                    }
                }
                else
                {
                    if (addZeros) { stringbuilder.Append('0'); }
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
                stringbuilder.Append(minus);
                value = -value;
                n = -n;
            }
            if (value == 0) // is Zero
            {
                stringbuilder.Append('0');
                return this;
            }
            if (value <= -1d || value >= 1d) // is a Integer
            {
                if (value >= place * 10)
                {
                    stringbuilder.Append(value); // is big, just append its a edge case.
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
                        stringbuilder.Append((char)(dc + 48));
                    }
                    else
                        if (addZeros) { stringbuilder.Append('0'); }

                    place = (long)(place * .1d);
                }
            }
            else
                stringbuilder.Append('0');

            stringbuilder.Append(decimalseperator);

            // part 2 
            // floating point part now it can have about 28 digits but uh ya.. nooo lol
            place = 1000000000000000L;
            // pull decimal to integer digits, based on the number of place digits
            long dn = (long)((value - (double)(n)) * place * 10);
            if (dn == 0)
            {
                stringbuilder.Append('0');
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
                    stringbuilder.Append((char)(dc + 48));
                    if (val - dc * place == 0) // && trimEndZeros  aectetic
                    {
                        return this;
                    }
                }
                else
                    if (addZeros) { stringbuilder.Append('0'); }

                place = (long)(place * .1);
            }
            return this;
        }

        public MgStringBuilder Append(Point value)
        {
            Append("(");
            Append(value.X);
            Append(", ");
            Append(value.Y);
            Append(")");
            return this;
        }
        public MgStringBuilder Append(Vector2 value)
        {
            Append("(");
            Append(value.X);
            Append(", ");
            Append(value.Y);
            Append(")");
            return this;
        }
        public MgStringBuilder Append(Vector3 value)
        {
            Append("(");
            Append(value.X);
            Append(", ");
            Append(value.Y);
            Append(", ");
            Append(value.Z);
            Append(")");
            return this;
        }
        public MgStringBuilder Append(Vector4 value)
        {
            Append("(");
            Append(value.X);
            Append(", ");
            Append(value.Y);
            Append(", ");
            Append(value.Z);
            Append(", ");
            Append(value.W);
            Append(")");
            return this;
        }
        public MgStringBuilder Append(Color value)
        {
            Append("(");
            Append(value.R);
            Append(", ");
            Append(value.G);
            Append(", ");
            Append(value.B);
            Append(", ");
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
                stringbuilder.Append(minus);
                value = -value;
                n = -n;
            }
            if (value == 0)
            {
                stringbuilder.Append('0');
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
                    stringbuilder.Append(value);
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
                        stringbuilder.Append((char)(dc + 48));
                    }
                    else
                    {
                        if (addZeros) { stringbuilder.Append('0'); }
                    }
                    place = (int)(place * .1);
                }
            }
            else
                stringbuilder.Append('0');

            stringbuilder.Append(decimalseperator);

            // part 2 

            // floating point part now it can have about 28 digits but uh ya.. nooo lol
            place = 100;
            // pull decimal to integer digits, based on the number of place digits
            int dn = (int)((value - (float)(n)) * place * 10);
            // ... march 17 testing... cut out extra zeros case 1
            if (dn == 0)
            {
                stringbuilder.Append('0');
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
                    stringbuilder.Append((char)(dc + 48));
                    if (val - dc * place == 0) // && trimEndZeros this would be a acstetic
                    {
                        return this;
                    }
                }
                else
                {
                    if (addZeros) { stringbuilder.Append('0'); }
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
                stringbuilder.Append(minus);
                value = -value;
                n = -n;
            }
            if (value == 0) // is Zero
            {
                stringbuilder.Append('0');
                return this;
            }
            if (value <= -1d || value >= 1d) // is a Integer
            {
                if (value >= place * 10)
                {
                    stringbuilder.Append(value); // is big, just append its a edge case.
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
                        stringbuilder.Append((char)(dc + 48));
                    }
                    else
                        if (addZeros) { stringbuilder.Append('0'); }

                    place = (long)(place * .1);
                }
            }
            else
                stringbuilder.Append('0');

            stringbuilder.Append(decimalseperator);

            // part 2 
            // floating point part now it can have about 28 digits but uh ya.. nooo lol
            place = 100L;
            // pull decimal to integer digits, based on the number of place digits
            long dn = (long)((value - (double)(n)) * place * 10);
            if (dn == 0)
            {
                stringbuilder.Append('0');
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
                    stringbuilder.Append((char)(dc + 48));
                    if (val - dc * place == 0) // && trimEndZeros  aectetic
                    {
                        return this;
                    }
                }
                else
                    if (addZeros) { stringbuilder.Append('0'); }

                place = (long)(place * .1);
            }
            return this;
        }

        public MgStringBuilder AppendTrim(Vector2 value)
        {
            Append("(");
            AppendTrim(value.X);
            Append(", ");
            AppendTrim(value.Y);
            Append(")");
            return this;
        }
        public MgStringBuilder AppendTrim(Vector3 value)
        {
            Append("(");
            AppendTrim(value.X);
            Append(", ");
            AppendTrim(value.Y);
            Append(", ");
            AppendTrim(value.Z);
            Append(")");
            return this;
        }
        public MgStringBuilder AppendTrim(Vector4 value)
        {
            Append("(");
            AppendTrim(value.X);
            Append(", ");
            AppendTrim(value.Y);
            Append(", ");
            AppendTrim(value.Z);
            Append(", ");
            AppendTrim(value.W);
            Append(")");
            return this;
        }

        public MgStringBuilder AppendLine(StringBuilder s)
        {
            Append(s);
            stringbuilder.AppendLine();
            return this;
        }
        public MgStringBuilder AppendLine(string s)
        {
            stringbuilder.Append(s);
            stringbuilder.AppendLine();
            return this;
        }
        public MgStringBuilder AppendLine()
        {
            stringbuilder.AppendLine();
            return this;
        }

        /// <summary>
        /// Functions just like a indexer.
        /// </summary>
        public void OverWriteAt(int index, Char s)
        {
            CheckOverWriteCapacityAndLength(index, 1);
                this.StringBuilder[index] = (char)(s);
        }
        /// <summary>
        /// Functions to overwrite data at the index on
        /// </summary>
        public void OverWriteAt(int index, StringBuilder s)
        {
            CheckOverWriteCapacityAndLength(index, s.Length);
            for (int i = 0; i < s.Length; i++)
                this.StringBuilder[i + index] = (char)(s[i]);
        }
        /// <summary>
        /// Functions to overwrite data at the index on
        /// </summary>
        public void OverWriteAt(int index, String s)
        {
            CheckAppendCapacityAndLength(index, s.Length);
            for (int i = 0; i < s.Length; i++)
                this.StringBuilder[i + index] = (char)(s[i]);
        }

        /// <summary>
        /// Functions as a insert, existing text will be moved over.
        /// </summary>
        public void AppendAt(int index, Char s)
        {
            CheckAppendCapacityAndLength(index, 1);
            for (int j = StringBuilder.Length - 1; j >= index + 1; j--)
                stringbuilder[j] = stringbuilder[j - 1];
            for (int i = 0; i < 1; i++)
                stringbuilder[i + index] = (char)(s);
        }
        /// <summary>
        /// Functions as a insert, existing text will be moved over.
        /// </summary>
        public void AppendAt(int index, StringBuilder s)
        {
            CheckAppendCapacityAndLength(index, s.Length);
            int insertedsbLength = s.Length;
            for (int j = stringbuilder.Length - 1; j >= index + insertedsbLength; j--)
                stringbuilder[j] = stringbuilder[j - insertedsbLength];
            for (int i = 0; i < insertedsbLength; i++)
            {
                stringbuilder[index + i] = s[i];
            }
        }
        /// <summary>
        /// Functions as a insert, existing text will be moved over. Notes are left in this method overload.
        /// </summary>
        public void AppendAt(int index, String s)
        {
            CheckAppendCapacityAndLength(index, s.Length);
            // Now we will wind from back to front the current characters in the stringbuilder to make room for this append.
            // Yes this will be a expensive operation however this must be done if we want a proper AppendAt.
            // Chunks or no chunks stringbuilders insert is piss poor.
            int insertedsbLength = s.Length;
            for (int j = stringbuilder.Length - 1; j >= index + insertedsbLength; j--)
                stringbuilder[j] = stringbuilder[j - insertedsbLength];
            // perform the append
            for (int i = 0; i < insertedsbLength; i++)
            {
                stringbuilder[index + i] = s[i];
            }
        }

        /// <summary>
        /// This uses AppendAt to get around problems with garbage collections.
        /// </summary>
        public MgStringBuilder Insert(int index, char c)
        {
            AppendAt(index, c);
            return this;
        }
        /// <summary>
        /// This uses AppendAt to get around problems with garbage collections.
        /// </summary>
        public MgStringBuilder Insert(int index, StringBuilder s)
        {
            AppendAt(index, s);
            return this;
        }
        /// <summary>
        /// This uses AppendAt to get around problems with garbage collections.
        /// </summary>
        public MgStringBuilder Insert(int index, string s)
        {
            AppendAt(index, s);
            return this;
        }

        public MgStringBuilder Remove(int index, int length)
        {
            stringbuilder.Remove(index, length);
            return this;
        }

        private void CheckAppendCapacityAndLength(int index, int lengthOfAddition)
        {
            int newLength = lengthOfAddition + stringbuilder.Length;
            int reqcapacity = (newLength + 1) - (stringbuilder.Capacity);
            if (reqcapacity >= 0)
                stringbuilder.Capacity = (stringbuilder.Capacity + reqcapacity + 64);
            stringbuilder.Length = newLength;
        }
        private void CheckOverWriteCapacityAndLength(int index, int lengthOfOverWrite)
        {
            int dist = index + lengthOfOverWrite;
            if (dist >= stringbuilder.Length)
            {
                int newLength = lengthOfOverWrite + stringbuilder.Length;
                int reqcapacity = (newLength + 1) - (stringbuilder.Capacity);
                if (reqcapacity >= 0)
                    stringbuilder.Capacity = (stringbuilder.Capacity + reqcapacity + 64);
                stringbuilder.Length = newLength;
            }
        }

        /// <summary>
        /// Use with caution this solves a rare edge case.
        /// Be careful using this you should understand c# references before doing so. 
        /// This creates a direct secondary reference to the internal stringbuilder via out.
        /// Declare a StringBuilder reference such as StringBuilder sb; don't call new on it,  then pass sb to this function.
        /// When you are done with it unlink it by declaring new on it like so, sb = new StringBuilder();
        /// This allows a way to link a reference to the internal stringbuilder without creating deallocation garbage.
        /// </summary>
        public void LinkReferenceToTheInnerStringBuilder(out StringBuilder rsb)
        {
            rsb = stringbuilder;
        }

        public char[] ToCharArray()
        {
            char[] a = new char[stringbuilder.Length];
            stringbuilder.CopyTo(0, a, 0, stringbuilder.Length);
            return a;
        }

        public override string ToString()
        {
            return stringbuilder.ToString();
        }
    }
}
