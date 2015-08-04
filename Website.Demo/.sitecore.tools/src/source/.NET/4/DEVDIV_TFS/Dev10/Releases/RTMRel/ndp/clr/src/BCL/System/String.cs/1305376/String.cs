// ==++== 
//
//   Copyright (c) Microsoft Corporation.  All rights reserved.
//
// ==--== 
/*============================================================
** 
** Class:  String 
**
** 
** Purpose: Your favorite String class.  Native methods
** are implemented in StringNative.cpp
**
** 
===========================================================*/
namespace System { 
    using System.Text; 
    using System;
    using System.Runtime; 
    using System.Runtime.ConstrainedExecution;
    using System.Globalization;
    using System.Threading;
    using System.Collections; 
    using System.Collections.Generic;
    using System.Runtime.CompilerServices; 
    using System.Runtime.InteropServices; 
    using System.Runtime.Versioning;
    using Microsoft.Win32; 
    using System.Diagnostics.Contracts;

    //
    // For Information on these methods, please see COMString.cpp 
    //
    // The String class represents a static string of characters.  Many of 
    // the String methods perform some type of transformation on the current 
    // instance and return the result as a new String. All comparison methods are
    // implemented as a part of String.  As with arrays, character positions 
    // (indices) are zero-based.
    //
    // When passing a null string into a constructor in VJ and VC, the null should be
    // explicitly type cast to a String. 
    // For Example:
    // String s = new String((String)null); 
    // Console.WriteLine(s); 
    //
    [ComVisible(true)] 
    [Serializable]
    public sealed class String : IComparable, ICloneable, IConvertible, IEnumerable
#if GENERICS_WORK
        , IComparable<String>, IEnumerable<char>, IEquatable<String> 
#endif
    { 
 
        //
        //NOTE NOTE NOTE NOTE 
        //These fields map directly onto the fields in an EE StringObject.  See object.h for the layout.
        //
        [NonSerialized]private int  m_stringLength;
 
        #if !FEATURE_CORECLR
        [System.Runtime.ForceTokenStabilization] 
        #endif //!FEATURE_CORECLR 
        [NonSerialized]private char m_firstChar;
 
        //private static readonly char FmtMsgMarkerChar='%';
        //private static readonly char FmtMsgFmtCodeChar='!';
        //These are defined in Com99/src/vm/COMStringCommon.h and must be kept in [....].
        private const int TrimHead = 0; 
        private const int TrimTail = 1;
        private const int TrimBoth = 2; 
 
        // The Empty constant holds the empty string value.
        //We need to call the String constructor so that the compiler doesn't mark this as a literal. 
        //Marking this as a literal would mean that it doesn't show up as a field which we can access
        //from native.
        public static readonly String Empty = "";
 
        //
        //Native Static Methods 
        // 

        // Joins an array of strings together as one string with a separator between each original string. 
        //
        public static String Join(String separator, params String[] value) {
            if (value==null)
                throw new ArgumentNullException("value"); 
            Contract.EndContractBlock();
            return Join(separator, value, 0, value.Length); 
        } 

        [ComVisible(false)] 
        public static String Join(String separator, params Object[] values) {
            if (values==null)
                throw new ArgumentNullException("values");
            Contract.EndContractBlock(); 

            if (values.Length == 0 || values[0] == null) 
                return String.Empty; 

            if (separator == null) 
                separator = String.Empty;

            StringBuilder result = new StringBuilder();
 
            String value = values[0].ToString();
            if (value != null) 
                result.Append(value); 

            for (int i = 1; i < values.Length; i++) { 
                result.Append(separator);
                if (values[i] != null) {
                    // handle the case where their ToString() override is broken
                    value = values[i].ToString(); 
                    if (value != null)
                        result.Append(value); 
                } 
            }
            return result.ToString(); 
        }

        [ComVisible(false)]
        public static String Join<T>(String separator, IEnumerable<T> values) { 
            if (values == null)
                throw new ArgumentNullException("values"); 
            Contract.Ensures(Contract.Result<String>() != null); 
            Contract.EndContractBlock();
 
            if (separator == null)
                separator = String.Empty;

            using(IEnumerator<T> en = values.GetEnumerator()) { 
                if (!en.MoveNext())
                    return String.Empty; 
 
                StringBuilder result = new StringBuilder();
                if (en.Current != null) { 
                    // handle the case that the enumeration has null entries
                    // and the case where their ToString() override is broken
                    string value = en.Current.ToString();
                    if (value != null) 
                        result.Append(value);
                } 
 
                while (en.MoveNext()) {
                    result.Append(separator); 
                    if (en.Current != null) {
                        // handle the case that the enumeration has null entries
                        // and the case where their ToString() override is broken
                        string value = en.Current.ToString(); 
                        if (value != null)
                            result.Append(value); 
                    } 
                }
                return result.ToString(); 
            }
        }

 

        [ComVisible(false)] 
        public static String Join(String separator, IEnumerable<String> values) { 
            if (values == null)
                throw new ArgumentNullException("values"); 
            Contract.Ensures(Contract.Result<String>() != null);
            Contract.EndContractBlock();

            if (separator == null) 
                separator = String.Empty;
 
 
            using(IEnumerator<String> en = values.GetEnumerator()) {
                if (!en.MoveNext()) 
                    return String.Empty;

                StringBuilder result = new StringBuilder();
                if (en.Current != null) { 
                    result.Append(en.Current);
                } 
 
                while (en.MoveNext()) {
                    result.Append(separator); 
                    if (en.Current != null) {
                        result.Append(en.Current);
                    }
                } 
                return result.ToString();
            } 
        } 

 
#if WIN64
        private const int charPtrAlignConst = 3;
        private const int alignConst        = 7;
#else 
        private const int charPtrAlignConst = 1;
        private const int alignConst        = 3; 
#endif 

        internal char FirstChar { get { return m_firstChar; } } 

        // Joins an array of strings together as one string with a separator between each original string.
        //
        [System.Security.SecuritySafeCritical]  // auto-generated 
        public unsafe static String Join(String separator, String[] value, int startIndex, int count) {
            //Range check the array 
            if (value == null) 
                throw new ArgumentNullException("value");
 
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException("startIndex", Environment.GetResourceString("ArgumentOutOfRange_StartIndex"));
            if (count < 0)
                throw new ArgumentOutOfRangeException("count", Environment.GetResourceString("ArgumentOutOfRange_NegativeCount")); 

            if (startIndex > value.Length - count) 
                throw new ArgumentOutOfRangeException("startIndex", Environment.GetResourceString("ArgumentOutOfRange_IndexCountBuffer")); 
            Contract.EndContractBlock();
 
            //Treat null as empty string.
            if (separator == null) {
                separator = String.Empty;
            } 

            //If count is 0, that skews a whole bunch of the calculations below, so just special case that. 
            if (count == 0) { 
                return String.Empty;
            } 

            int jointLength = 0;
            //Figure out the total length of the strings in value
            int endIndex = startIndex + count - 1; 
            for (int stringToJoinIndex = startIndex; stringToJoinIndex <= endIndex; stringToJoinIndex++) {
                if (value[stringToJoinIndex] != null) { 
                    jointLength += value[stringToJoinIndex].Length; 
                }
            } 

            //Add enough room for the separator.
            jointLength += (count - 1) * separator.Length;
 
            // Note that we may not catch all overflows with this check (since we could have wrapped around the 4gb range any number of times
            // and landed back in the positive range.) The input array might be modifed from other threads, 
            // so we have to do an overflow check before each append below anyway. Those overflows will get caught down there. 
            if ((jointLength < 0) || ((jointLength + 1) < 0) ) {
                throw new OutOfMemoryException(); 
            }

            //If this is an empty string, just return.
            if (jointLength == 0) { 
                return String.Empty;
            } 
 
            string jointString = FastAllocateString( jointLength );
            fixed (char * pointerToJointString = &jointString.m_firstChar) { 
                UnSafeCharBuffer charBuffer = new UnSafeCharBuffer( pointerToJointString, jointLength);

                // Append the first string first and then append each following string prefixed by the separator.
                charBuffer.AppendString( value[startIndex] ); 
                for (int stringToJoinIndex = startIndex + 1; stringToJoinIndex <= endIndex; stringToJoinIndex++) {
                    charBuffer.AppendString( separator ); 
                    charBuffer.AppendString( value[stringToJoinIndex] ); 
                }
                Contract.Assert(*(pointerToJointString + charBuffer.Length) == '\0', "String must be null-terminated!"); 
            }

            return jointString;
        } 

        [System.Security.SecuritySafeCritical]  // auto-generated 
        private unsafe static int CompareOrdinalIgnoreCaseHelper(String strA, String strB) 
        {
            Contract.Requires(strA != null); 
            Contract.Requires(strB != null);
            Contract.EndContractBlock();
            int length = Math.Min(strA.Length, strB.Length);
 
            fixed (char* ap = &strA.m_firstChar) fixed (char* bp = &strB.m_firstChar)
            { 
                char* a = ap; 
                char* b = bp;
 
                while (length != 0)
                {
                    int charA = *a;
                    int charB = *b; 

                    Contract.Assert((charA | charB) <= 0x7F, "strings have to be ASCII"); 
 
                    // uppercase both chars - notice that we need just one compare per char
                    if ((uint)(charA - 'a') <= (uint)('z' - 'a')) charA -= 0x20; 
                    if ((uint)(charB - 'a') <= (uint)('z' - 'a')) charB -= 0x20;

                    //Return the (case-insensitive) difference between them.
                    if (charA != charB) 
                        return charA - charB;
 
                    // Next char 
                    a++; b++;
                    length--; 
                }

                return strA.Length - strB.Length;
            } 
        }
 
        // native call to COMString::CompareOrdinalEx 
        [System.Security.SecurityCritical]  // auto-generated
        [ResourceExposure(ResourceScope.None)] 
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        internal static extern int nativeCompareOrdinalEx(String strA, int indexA, String strB, int indexB, int count);

        //This will not work in case-insensitive mode for any character greater than 0x80. 
        //We'll throw an ArgumentException.
        // 
        [System.Security.SecurityCritical]  // auto-generated 
        [ResourceExposure(ResourceScope.None)]
        [MethodImplAttribute(MethodImplOptions.InternalCall)] 
        unsafe internal static extern int nativeCompareOrdinalIgnoreCaseWC(String strA, char *strBChars);
        //
        // This is a helper method for the security team.  They need to uppercase some strings (guaranteed to be less
        // than 0x80) before security is fully initialized.  Without security initialized, we can't grab resources (the nlp's) 
        // from the assembly.  This provides a workaround for that problem and should NOT be used anywhere else.
        // 
        [System.Security.SecuritySafeCritical]  // auto-generated 
        internal unsafe static string SmallCharToUpper(string strIn) {
            Contract.Requires(strIn != null); 
            Contract.EndContractBlock();
            //
            // Get the length and pointers to each of the buffers.  Walk the length
            // of the string and copy the characters from the inBuffer to the outBuffer, 
            // capitalizing it if necessary.  We assert that all of our characters are
            // less than 0x80. 
            // 
            int length = strIn.Length;
            String strOut = FastAllocateString(length); 
            fixed (char * inBuff = &strIn.m_firstChar, outBuff = &strOut.m_firstChar) {

                for(int i = 0; i < length; i++) {
                    int c = inBuff[i]; 
                    Contract.Assert(c <= 0x7F, "string has to be ASCII");
 
                    // uppercase - notice that we need just one compare 
                    if ((uint)(c - 'a') <= (uint)('z' - 'a')) c -= 0x20;
 
                    outBuff[i] = (char)c;
                }

                Contract.Assert(outBuff[length]=='\0', "outBuff[length]=='\0'"); 
            }
            return strOut; 
        } 

        // 
        //
        // NATIVE INSTANCE METHODS
        //
        // 

        // 
        // Search/Query methods 
        //
 
        [System.Security.SecuritySafeCritical]  // auto-generated
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        private unsafe static bool EqualsHelper(String strA, String strB)
        { 
            Contract.Requires(strA != null);
            Contract.Requires(strB != null); 
            int length = strA.Length; 
            if (length != strB.Length) return false;
 
            fixed (char* ap = &strA.m_firstChar) fixed (char* bp = &strB.m_firstChar)
            {
                char* a = ap;
                char* b = bp; 

                // unroll the loop 
#if AMD64 
                // for AMD64 bit platform we unroll by 12 and
                // check 3 qword at a time. This is less code 
                // than the 32 bit case and is shorter
                // pathlength

                while (length >= 12) 
                {
                    if (*(long*)a     != *(long*)b) break; 
                    if (*(long*)(a+4) != *(long*)(b+4)) break; 
                    if (*(long*)(a+8) != *(long*)(b+8)) break;
                    a += 12; b += 12; length -= 12; 
                }
#else
                while (length >= 10)
                { 
                    if (*(int*)a != *(int*)b) break;
                    if (*(int*)(a+2) != *(int*)(b+2)) break; 
                    if (*(int*)(a+4) != *(int*)(b+4)) break; 
                    if (*(int*)(a+6) != *(int*)(b+6)) break;
                    if (*(int*)(a+8) != *(int*)(b+8)) break; 
                    a += 10; b += 10; length -= 10;
                }
#endif
 
                // This depends on the fact that the String objects are
                // always zero terminated and that the terminating zero is not included 
                // in the length. For odd string sizes, the last compare will include 
                // the zero terminator.
                while (length > 0) 
                {
                    if (*(int*)a != *(int*)b) break;
                    a += 2; b += 2; length -= 2;
                } 

                return (length <= 0); 
            } 
        }
 
        [System.Security.SecuritySafeCritical]  // auto-generated
        private unsafe static int CompareOrdinalHelper(String strA, String strB)
        {
            Contract.Requires(strA != null); 
            Contract.Requires(strB != null);
            int length = Math.Min(strA.Length, strB.Length); 
            int diffOffset = -1; 

            fixed (char* ap = &strA.m_firstChar) fixed (char* bp = &strB.m_firstChar) 
            {
                char* a = ap;
                char* b = bp;
 
                // unroll the loop
                while (length >= 10) 
                { 
                    if (*(int*)a != *(int*)b) {
                        diffOffset = 0; 
                        break;
                    }

                    if (*(int*)(a+2) != *(int*)(b+2)) { 
                        diffOffset = 2;
                        break; 
                    } 

                    if (*(int*)(a+4) != *(int*)(b+4)) { 
                        diffOffset = 4;
                        break;
                    }
 
                    if (*(int*)(a+6) != *(int*)(b+6)) {
                        diffOffset = 6; 
                        break; 
                    }
 
                    if (*(int*)(a+8) != *(int*)(b+8)) {
                        diffOffset = 8;
                        break;
                    } 
                    a += 10;
                    b += 10; 
                    length -= 10; 
                }
 
                if( diffOffset != -1) {
                    // we already see a difference in the unrolled loop above
                    a += diffOffset;
                    b += diffOffset; 
                    int order;
                    if ( (order = (int)*a - (int)*b) != 0) { 
                        return order; 
                    }
                    Contract.Assert( *(a+1) != *(b+1), "This byte must be different if we reach here!"); 
                    return ((int)*(a+1) - (int)*(b+1));
                }

                // now go back to slower code path and do comparison on 4 bytes one time. 
                // Following code also take advantage of the fact strings will
                // use even numbers of characters (runtime will have a extra zero at the end.) 
                // so even if length is 1 here, we can still do the comparsion. 
                while (length > 0) {
                    if (*(int*)a != *(int*)b) { 
                        break;
                    }
                    a += 2;
                    b += 2; 
                    length -= 2;
                } 
 
                if( length > 0) {
                    int c; 
                    // found a different int on above loop
                    if ( (c = (int)*a - (int)*b) != 0) {
                        return c;
                    } 
                    Contract.Assert( *(a+1) != *(b+1), "This byte must be different if we reach here!");
                    return ((int)*(a+1) - (int)*(b+1)); 
                } 

                // At this point, we have compared all the characters in at least one string. 
                // The longer string will be larger.
                return strA.Length - strB.Length;
            }
        } 

        // Determines whether two strings match. 
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)] 
        public override bool Equals(Object obj) {
            if (this == null)                        //this is necessary to guard against reverse-pinvokes and 
                throw new NullReferenceException();  //other callers who do not use the callvirt instruction

            String str = obj as String;
            if (str == null) 
                return false;
 
            if (Object.ReferenceEquals(this, obj)) 
                return true;
 
            return EqualsHelper(this, str);
        }

        // Determines whether two strings match. 
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
#if !FEATURE_CORECLR 
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")] 
#endif
        public bool Equals(String value) { 
            if (this == null)                        //this is necessary to guard against reverse-pinvokes and
                throw new NullReferenceException();  //other callers who do not use the callvirt instruction

            if (value == null) 
                return false;
 
            if (Object.ReferenceEquals(this, value)) 
                return true;
 
            return EqualsHelper(this, value);
        }

        [System.Security.SecuritySafeCritical]  // auto-generated 
        public bool Equals(String value, StringComparison comparisonType) {
            if (comparisonType < StringComparison.CurrentCulture || comparisonType > StringComparison.OrdinalIgnoreCase) 
                throw new ArgumentException(Environment.GetResourceString("NotSupported_StringComparison"), "comparisonType"); 
            Contract.EndContractBlock();
 
            if ((Object)this == (Object)value) {
                return true;
            }
 
            if ((Object)value == null) {
                return false; 
            } 

            switch (comparisonType) { 
                case StringComparison.CurrentCulture:
                    return (CultureInfo.CurrentCulture.CompareInfo.Compare(this, value, CompareOptions.None) == 0);

                case StringComparison.CurrentCultureIgnoreCase: 
                    return (CultureInfo.CurrentCulture.CompareInfo.Compare(this, value, CompareOptions.IgnoreCase) == 0);
 
                case StringComparison.InvariantCulture: 
                    return (CultureInfo.InvariantCulture.CompareInfo.Compare(this, value, CompareOptions.None) == 0);
 
                case StringComparison.InvariantCultureIgnoreCase:
                    return (CultureInfo.InvariantCulture.CompareInfo.Compare(this, value, CompareOptions.IgnoreCase) == 0);

                case StringComparison.Ordinal: 
                    return EqualsHelper(this, value);
 
                case StringComparison.OrdinalIgnoreCase: 
                    if( this.Length != value.Length)
                        return false; 
                    else {
                        // If both strings are ASCII strings, we can take the fast path.
                        if (this.IsAscii() && value.IsAscii()) {
                            return (CompareOrdinalIgnoreCaseHelper(this, value) == 0); 
                        }
                        // Take the slow path. 
                        return (TextInfo.CompareOrdinalIgnoreCase(this, value) == 0); 
                    }
 
                default:
                    throw new ArgumentException(Environment.GetResourceString("NotSupported_StringComparison"), "comparisonType");
            }
        } 

 
        // Determines whether two Strings match. 
#if !FEATURE_CORECLR
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")] 
#endif
        public static bool Equals(String a, String b) {
            if ((Object)a==(Object)b) {
                return true; 
            }
 
            if ((Object)a==null || (Object)b==null) { 
                return false;
            } 

            return EqualsHelper(a, b);
        }
 
        [System.Security.SecuritySafeCritical]  // auto-generated
        public static bool Equals(String a, String b, StringComparison comparisonType) { 
            if (comparisonType < StringComparison.CurrentCulture || comparisonType > StringComparison.OrdinalIgnoreCase) 
                throw new ArgumentException(Environment.GetResourceString("NotSupported_StringComparison"), "comparisonType");
            Contract.EndContractBlock(); 

            if ((Object)a==(Object)b) {
                return true;
            } 

            if ((Object)a==null || (Object)b==null) { 
                return false; 
            }
 
            switch (comparisonType) {
                case StringComparison.CurrentCulture:
                    return (CultureInfo.CurrentCulture.CompareInfo.Compare(a, b, CompareOptions.None) == 0);
 
                case StringComparison.CurrentCultureIgnoreCase:
                    return (CultureInfo.CurrentCulture.CompareInfo.Compare(a, b, CompareOptions.IgnoreCase) == 0); 
 
                case StringComparison.InvariantCulture:
                    return (CultureInfo.InvariantCulture.CompareInfo.Compare(a, b, CompareOptions.None) == 0); 

                case StringComparison.InvariantCultureIgnoreCase:
                    return (CultureInfo.InvariantCulture.CompareInfo.Compare(a, b, CompareOptions.IgnoreCase) == 0);
 
                case StringComparison.Ordinal:
                    return EqualsHelper(a, b); 
 
                case StringComparison.OrdinalIgnoreCase:
                    if( a.Length != b.Length) 
                        return false;
                    else {
                        // If both strings are ASCII strings, we can take the fast path.
                        if (a.IsAscii() && b.IsAscii()) { 
                            return (CompareOrdinalIgnoreCaseHelper(a, b) == 0);
                        } 
                        // Take the slow path. 

                        return (TextInfo.CompareOrdinalIgnoreCase(a, b) == 0); 
                    }
                default:
                    throw new ArgumentException(Environment.GetResourceString("NotSupported_StringComparison"), "comparisonType");
            } 
        }
 
#if !FEATURE_CORECLR 
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
#endif 
        public static bool operator == (String a, String b) {
           return String.Equals(a, b);
        }
 
#if !FEATURE_CORECLR
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")] 
#endif 
        public static bool operator != (String a, String b) {
           return !String.Equals(a, b); 
        }

        // Gets the character at a specified position.
        // 
        // Spec#: Apply the precondition here using a contract assembly.  Potential perf issue.
        [System.Runtime.CompilerServices.IndexerName("Chars")] 
        public extern char this[int index] { 
            [ResourceExposure(ResourceScope.None)]
            [MethodImpl(MethodImplOptions.InternalCall)] 
            [System.Security.SecuritySafeCritical] // public member
            get;
        }
 
        // Converts a substring of this string to an array of characters.  Copies the
        // characters of this string beginning at position startIndex and ending at 
        // startIndex + length - 1 to the character array buffer, beginning 
        // at bufferStartIndex.
        // 
        [System.Security.SecuritySafeCritical]  // auto-generated
        unsafe public void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count)
        {
            if (destination == null) 
                throw new ArgumentNullException("destination");
            if (count < 0) 
                throw new ArgumentOutOfRangeException("count", Environment.GetResourceString("ArgumentOutOfRange_NegativeCount")); 
            if (sourceIndex < 0)
                throw new ArgumentOutOfRangeException("sourceIndex", Environment.GetResourceString("ArgumentOutOfRange_Index")); 
            if (count > Length - sourceIndex)
                throw new ArgumentOutOfRangeException("sourceIndex", Environment.GetResourceString("ArgumentOutOfRange_IndexCount"));
            if (destinationIndex > destination.Length - count || destinationIndex < 0)
                throw new ArgumentOutOfRangeException("destinationIndex", Environment.GetResourceString("ArgumentOutOfRange_IndexCount")); 
            Contract.EndContractBlock();
 
            // Note: fixed does not like empty arrays 
            if (count > 0)
            { 
                fixed (char* src = &this.m_firstChar)
                    fixed (char* dest = destination)
                        wstrcpy(dest + destinationIndex, src + sourceIndex, count);
            } 
        }
 
        // Returns the entire string as an array of characters. 
        [System.Security.SecuritySafeCritical]  // auto-generated
        unsafe public char[] ToCharArray() { 
            // <STRIP> huge performance improvement for short strings by doing this </STRIP>
            int length = Length;
            char[] chars = new char[length];
            if (length > 0) 
            {
                fixed (char* src = &this.m_firstChar) 
                    fixed (char* dest = chars) { 
#if WIN32
                        wstrcpyPtrAligned(dest, src, length); 
#else
                        wstrcpy(dest, src, length);
#endif // WIN32
                    } 
            }
            return chars; 
        } 

        // Returns a substring of this string as an array of characters. 
        //
        [System.Security.SecuritySafeCritical]  // auto-generated
        unsafe public char[] ToCharArray(int startIndex, int length)
        { 
            // Range check everything.
            if (startIndex < 0 || startIndex > Length || startIndex > Length - length) 
                throw new ArgumentOutOfRangeException("startIndex", Environment.GetResourceString("ArgumentOutOfRange_Index")); 
            if (length < 0)
                throw new ArgumentOutOfRangeException("length", Environment.GetResourceString("ArgumentOutOfRange_Index")); 
            Contract.EndContractBlock();

            char[] chars = new char[length];
            if(length > 0) 
            {
                fixed (char* src = &this.m_firstChar) 
                    fixed (char* dest = chars) { 
                        wstrcpy(dest, src + startIndex, length);
                    } 
            }
            return chars;
        }
 
#if !FEATURE_CORECLR
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")] 
#endif 
        public static bool IsNullOrEmpty(String value) {
            return (value == null || value.Length == 0); 
        }

        public static bool IsNullOrWhiteSpace(String value) {
            if (value == null) return true; 

            for(int i = 0; i < value.Length; i++) { 
                if(!Char.IsWhiteSpace(value[i])) return false; 
            }
 
            return true;
        }

        // Gets a hash code for this string.  If strings A and B are such that A.Equals(B), then 
        // they will return the same hash code.
        [System.Security.SecuritySafeCritical]  // auto-generated 
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)] 
        public override int GetHashCode() {
            unsafe { 
                fixed (char *src = this) {
                    Contract.Assert(src[this.Length] == '\0', "src[this.Length] == '\\0'");
                    Contract.Assert( ((int)src)%4 == 0, "Managed string should start at 4 bytes boundary");
 
#if WIN32
                    int hash1 = (5381<<16) + 5381; 
#else 
                    int hash1 = 5381;
#endif 
                    int hash2 = hash1;

#if WIN32
                    // 32bit machines. 
                    int* pint = (int *)src;
                    int len = this.Length; 
                    while(len > 0) { 
                        hash1 = ((hash1 << 5) + hash1 + (hash1 >> 27)) ^ pint[0];
                        if( len <= 2) { 
                            break;
                        }
                        hash2 = ((hash2 << 5) + hash2 + (hash2 >> 27)) ^ pint[1];
                        pint += 2; 
                        len  -= 4;
                    } 
#else 
                    int     c;
                    char *s = src; 
                    while ((c = s[0]) != 0) {
                        hash1 = ((hash1 << 5) + hash1) ^ c;
                        c = s[1];
                        if (c == 0) 
                            break;
                        hash2 = ((hash2 << 5) + hash2) ^ c; 
                        s += 2; 
                    }
#endif 
#if DEBUG
                    // We want to ensure we can change our hash function daily.
                    // This is perfectly fine as long as you don't persist the
                    // value from GetHashCode to disk or count on String A 
                    // hashing before string B.  Those are bugs in your code.
                    hash1 ^= ThisAssembly.DailyBuildNumber; 
#endif 
                    return hash1 + (hash2 * 1566083941);
                } 
            }
        }

        // Gets the length of this string 
        //
        /// This is a EE implemented function so that the JIT can recognise is specially 
        /// and eliminate checks on character fetchs in a loop like: 
        ///        for(int I = 0; I < str.Length; i++) str[i]
        /// The actually code generated for this will be one instruction and will be inlined. 
        //
        // Spec#: Add postcondition in a contract assembly.  Potential perf problem.
        public extern int Length {
            [System.Security.SecuritySafeCritical]  // auto-generated 
            [ResourceExposure(ResourceScope.None)]
            [MethodImplAttribute(MethodImplOptions.InternalCall)] 
            get; 
        }
 
        // Creates an array of strings by splitting this string at each
        // occurence of a separator.  The separator is searched for, and if found,
        // the substring preceding the occurence is stored as the first element in
        // the array of strings.  We then continue in this manner by searching 
        // the substring that follows the occurence.  On the other hand, if the separator
        // is not found, the array of strings will contain this instance as its only element. 
        // If the separator is null 
        // whitespace (i.e., Character.IsWhitespace) is used as the separator.
        // 
        public String [] Split(params char [] separator) {
            Contract.Ensures(Contract.Result<String[]>() != null);
            return SplitInternal(separator, Int32.MaxValue, StringSplitOptions.None);
        } 

        // Creates an array of strings by splitting this string at each 
        // occurence of a separator.  The separator is searched for, and if found, 
        // the substring preceding the occurence is stored as the first element in
        // the array of strings.  We then continue in this manner by searching 
        // the substring that follows the occurence.  On the other hand, if the separator
        // is not found, the array of strings will contain this instance as its only element.
        // If the spearator is the empty string (i.e., String.Empty), then
        // whitespace (i.e., Character.IsWhitespace) is used as the separator. 
        // If there are more than count different strings, the last n-(count-1)
        // elements are concatenated and added as the last String. 
        // 
        public string[] Split(char[] separator, int count) {
            Contract.Ensures(Contract.Result<String[]>() != null); 
            return SplitInternal(separator, count, StringSplitOptions.None);
        }

        [ComVisible(false)] 
        public String[] Split(char[] separator, StringSplitOptions options) {
            Contract.Ensures(Contract.Result<String[]>() != null); 
            return SplitInternal(separator, Int32.MaxValue, options); 
        }
 
        [ComVisible(false)]
        public String[] Split(char[] separator, int count, StringSplitOptions options)
        {
            Contract.Ensures(Contract.Result<String[]>() != null); 
            return SplitInternal(separator, count, options);
        } 
 
        [ComVisible(false)]
        internal String[] SplitInternal(char[] separator, int count, StringSplitOptions options) 
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException("count",
                    Environment.GetResourceString("ArgumentOutOfRange_NegativeCount")); 

            if (options < StringSplitOptions.None || options > StringSplitOptions.RemoveEmptyEntries) 
                throw new ArgumentException(Environment.GetResourceString("Arg_EnumIllegalVal", options)); 
            Contract.Ensures(Contract.Result<String[]>() != null);
            Contract.EndContractBlock(); 

            bool omitEmptyEntries = (options == StringSplitOptions.RemoveEmptyEntries);

            if ((count == 0) || (omitEmptyEntries && this.Length == 0)) 
            {
                return new String[0]; 
            } 

            int[] sepList = new int[Length]; 
            int numReplaces = MakeSeparatorList(separator, ref sepList);

            //Handle the special case of no replaces and special count.
            if (0 == numReplaces || count == 1) { 
                String[] stringArray = new String[1];
                stringArray[0] = this; 
                return stringArray; 
            }
 
            if(omitEmptyEntries)
            {
                return InternalSplitOmitEmptyEntries(sepList, null, numReplaces, count);
            } 
            else
            { 
                return InternalSplitKeepEmptyEntries(sepList, null, numReplaces, count); 
            }
        } 

        [ComVisible(false)]
        public String [] Split(String[] separator, StringSplitOptions options) {
            Contract.Ensures(Contract.Result<String[]>() != null); 
            return Split(separator, Int32.MaxValue, options);
        } 
 
        [ComVisible(false)]
        public String[] Split(String[] separator, Int32 count, StringSplitOptions options) { 
            if (count < 0) {
                throw new ArgumentOutOfRangeException("count",
                    Environment.GetResourceString("ArgumentOutOfRange_NegativeCount"));
            } 

            if (options < StringSplitOptions.None || options > StringSplitOptions.RemoveEmptyEntries) { 
                throw new ArgumentException(Environment.GetResourceString("Arg_EnumIllegalVal", (int)options)); 
            }
            Contract.EndContractBlock(); 

            bool omitEmptyEntries = (options == StringSplitOptions.RemoveEmptyEntries);

            if (separator == null || separator.Length ==0) { 
                return SplitInternal((char[]) null, count, options);
            } 
 
            if ((count == 0) || (omitEmptyEntries && this.Length ==0)) {
                return new String[0]; 
            }

            int[] sepList = new int[Length];
            int[] lengthList = new int[Length]; 
            int numReplaces = MakeSeparatorList(separator, ref sepList, ref lengthList);
 
            //Handle the special case of no replaces and special count. 
            if (0 == numReplaces || count == 1) {
                String[] stringArray = new String[1]; 
                stringArray[0] = this;
                return stringArray;
            }
 
            if (omitEmptyEntries) {
                return InternalSplitOmitEmptyEntries(sepList, lengthList, numReplaces, count); 
            } 
            else {
                return InternalSplitKeepEmptyEntries(sepList, lengthList, numReplaces, count); 
            }
        }

        // Note a few special case in this function: 
        //     If there is no separator in the string, a string array which only contains
        //     the original string will be returned regardless of the count. 
        // 

        private String[] InternalSplitKeepEmptyEntries(Int32 [] sepList, Int32 [] lengthList, Int32 numReplaces, int count) { 
            Contract.Requires(numReplaces >= 0);
            Contract.Requires(count >= 2, "Count>=2");

            int currIndex = 0; 
            int arrIndex = 0;
 
            count--; 
            int numActualReplaces = (numReplaces < count) ? numReplaces : count;
 
            //Allocate space for the new array.
            //+1 for the string from the end of the last replace to the end of the String.
            String[] splitStrings = new String[numActualReplaces+1];
 
            for (int i = 0; i < numActualReplaces && currIndex < Length; i++) {
                splitStrings[arrIndex++] = Substring(currIndex, sepList[i]-currIndex ); 
                currIndex=sepList[i] + ((lengthList == null) ? 1 : lengthList[i]); 
            }
 
            //Handle the last string at the end of the array if there is one.
            if (currIndex < Length && numActualReplaces >= 0) {
                splitStrings[arrIndex] = Substring(currIndex);
            } 
            else if (arrIndex==numActualReplaces) {
                //We had a separator character at the end of a string.  Rather than just allowing 
                //a null character, we'll replace the last element in the array with an empty string. 
                splitStrings[arrIndex] = String.Empty;
 
            }

            return splitStrings;
        } 

 
        // This function will not keep the Empty String 
        private String[] InternalSplitOmitEmptyEntries(Int32[] sepList, Int32[] lengthList, Int32 numReplaces, int count) {
            Contract.Requires(numReplaces >= 0); 
            Contract.Requires(count >= 2, "Count>=2");

            // Allocate array to hold items. This array may not be
            // filled completely in this function, we will create a 
            // new array and copy string references to that new array.
 
            int maxItems = (numReplaces < count) ? (numReplaces+1): count ; 
            String[] splitStrings = new String[maxItems];
 
            int currIndex = 0;
            int arrIndex = 0;

            for(int i=0; i< numReplaces && currIndex < Length; i++) { 
                if( sepList[i]-currIndex > 0) {
                    splitStrings[arrIndex++] = Substring(currIndex, sepList[i]-currIndex ); 
                } 
                currIndex=sepList[i] + ((lengthList == null) ? 1 : lengthList[i]);
                if( arrIndex == count -1 )  { 
                    // If all the remaining entries at the end are empty, skip them
                    while( i < numReplaces - 1 && currIndex == sepList[++i]) {
                        currIndex += ((lengthList == null) ? 1 : lengthList[i]);
                    } 
                    break;
                } 
            } 

            // we must have at least one slot left to fill in the last string. 
            Contract.Assert( arrIndex < maxItems, "arrIndex < maxItems");

            //Handle the last string at the end of the array if there is one.
            if (currIndex< Length) { 
                splitStrings[arrIndex++] = Substring(currIndex);
            } 
 
            String[] stringArray = splitStrings;
            if( arrIndex!= maxItems) { 
                stringArray = new String[arrIndex];
                for( int j = 0; j < arrIndex; j++) {
                    stringArray[j] = splitStrings[j];
                } 
            }
            return stringArray; 
        } 

        //------------------------------------------------------------------- 
        // This function returns number of the places within baseString where
        // instances of characters in Separator occur.
        // Args: separator  -- A string containing all of the split characters.
        //       sepList    -- an array of ints for split char indicies. 
        //-------------------------------------------------------------------
        [System.Security.SecuritySafeCritical]  // auto-generated 
        private unsafe int MakeSeparatorList(char[] separator, ref int[] sepList) { 
            int foundCount=0;
 
            if (separator == null || separator.Length ==0) {
                fixed (char* pwzChars = &this.m_firstChar) {
                    //If they passed null or an empty string, look for whitespace.
                    for (int i=0; i < Length && foundCount < sepList.Length; i++) { 
                        if (Char.IsWhiteSpace(pwzChars[i])) {
                            sepList[foundCount++]=i; 
                        } 
                    }
                } 
            }
            else {
                int sepListCount = sepList.Length;
                int sepCount = separator.Length; 
                //If they passed in a string of chars, actually look for those chars.
                fixed (char* pwzChars = &this.m_firstChar, pSepChars = separator) { 
                    for (int i=0; i< Length && foundCount < sepListCount; i++) { 
                        char * pSep = pSepChars;
                        for( int j =0; j < sepCount; j++, pSep++) { 
                           if ( pwzChars[i] == *pSep) {
                               sepList[foundCount++]=i;
                               break;
                           } 
                        }
                    } 
                } 
            }
            return foundCount; 
        }

        //-------------------------------------------------------------------
        // This function returns number of the places within baseString where 
        // instances of separator strings occur.
        // Args: separators -- An array containing all of the split strings. 
        //       sepList    -- an array of ints for split string indicies. 
        //       lengthList -- an array of ints for split string lengths.
        //-------------------------------------------------------------------- 
        [System.Security.SecuritySafeCritical]  // auto-generated
        private unsafe int MakeSeparatorList(String[] separators, ref int[] sepList, ref int[] lengthList) {
            Contract.Assert(separators != null && separators.Length > 0, "separators != null && separators.Length > 0");
 
            int foundCount = 0;
            int sepListCount = sepList.Length; 
            int sepCount = separators.Length; 

            fixed (char* pwzChars = &this.m_firstChar) { 
                for (int i=0; i< Length && foundCount < sepListCount; i++) {
                    for( int j =0; j < separators.Length; j++) {
                        String separator = separators[j];
                        if (String.IsNullOrEmpty(separator)) { 
                            continue;
                        } 
                        Int32 currentSepLength = separator.Length; 
                        if ( pwzChars[i] == separator[0] && currentSepLength <= Length - i) {
                            if (currentSepLength == 1 
                                || String.CompareOrdinal(this, i, separator, 0, currentSepLength) == 0) {
                                sepList[foundCount] = i;
                                lengthList[foundCount] = currentSepLength;
                                foundCount++; 
                                i += currentSepLength - 1;
                                break; 
                            } 
                        }
                    } 
                }
            }
            return foundCount;
        } 

        // Returns a substring of this string. 
        // 
#if !FEATURE_CORECLR
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")] 
#endif
        public String Substring (int startIndex) {
            return this.Substring (startIndex, Length-startIndex);
        } 

        // Returns a substring of this string. 
        // 
        [System.Security.SecuritySafeCritical]  // auto-generated
#if !FEATURE_CORECLR 
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
#endif
        public String Substring(int startIndex, int length) {
            // okay to not enforce copying in the case of Substring(0, length), since we assume 
            // String instances are immutable.
            return InternalSubStringWithChecks(startIndex, length, false); 
        } 

 
        [System.Security.SecurityCritical]  // auto-generated
        internal String InternalSubStringWithChecks (int startIndex, int length, bool fAlwaysCopy) {

            //Bounds Checking. 
            if (startIndex < 0) {
                throw new ArgumentOutOfRangeException("startIndex", Environment.GetResourceString("ArgumentOutOfRange_StartIndex")); 
            } 

            if (startIndex > Length) { 
                throw new ArgumentOutOfRangeException("startIndex", Environment.GetResourceString("ArgumentOutOfRange_StartIndexLargerThanLength"));
            }

            if (length < 0) { 
                throw new ArgumentOutOfRangeException("length", Environment.GetResourceString("ArgumentOutOfRange_NegativeLength"));
            } 
 
            if (startIndex > Length - length) {
                throw new ArgumentOutOfRangeException("length", Environment.GetResourceString("ArgumentOutOfRange_IndexLength")); 
            }
            Contract.EndContractBlock();

            if( length == 0) { 
                return String.Empty;
            } 
            return InternalSubString(startIndex, length, fAlwaysCopy); 
        }
 
        [System.Security.SecurityCritical]  // auto-generated
        unsafe string InternalSubString(int startIndex, int length, bool fAlwaysCopy) {
            Contract.Assert( startIndex >= 0 && startIndex <= this.Length, "StartIndex is out of range!");
            Contract.Assert( length >= 0 && startIndex <= this.Length - length, "length is out of range!"); 

            if( startIndex == 0 && length == this.Length && !fAlwaysCopy)  { 
                return this; 
            }
 
            String result = FastAllocateString(length);

            fixed(char* dest = &result.m_firstChar)
                fixed(char* src = &this.m_firstChar) { 
                    wstrcpy(dest, src + startIndex, length);
                } 
 
            return result;
        } 


        // Removes a string of characters from the ends of this string.
        [Pure] 
#if !FEATURE_CORECLR
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")] 
#endif 
        public String Trim(params char[] trimChars) {
            if (null==trimChars || trimChars.Length == 0) { 
                return TrimHelper(TrimBoth);
            }
            return TrimHelper(trimChars,TrimBoth);
        } 

        // Removes a string of characters from the beginning of this string. 
#if !FEATURE_CORECLR 
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
#endif 
        public String TrimStart(params char[] trimChars) {
            if (null==trimChars || trimChars.Length == 0) {
                return TrimHelper(TrimHead);
            } 
            return TrimHelper(trimChars,TrimHead);
        } 
 

        // Removes a string of characters from the end of this string. 
#if !FEATURE_CORECLR
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
#endif
        public String TrimEnd(params char[] trimChars) { 
            if (null==trimChars || trimChars.Length == 0) {
                return TrimHelper(TrimTail); 
            } 
            return TrimHelper(trimChars,TrimTail);
        } 


        // Creates a new string with the characters copied in from ptr. If
        // ptr is null, a string initialized to ";<;No Object>;"; (i.e., 
        // String.NullString) is created.
        // 
        [System.Security.SecurityCritical]  // auto-generated 
        [ResourceExposure(ResourceScope.None)]
        [CLSCompliant(false), MethodImplAttribute(MethodImplOptions.InternalCall)] 
        unsafe public extern String(char *value);
        [System.Security.SecurityCritical]  // auto-generated
        [ResourceExposure(ResourceScope.None)]
        [CLSCompliant(false), MethodImplAttribute(MethodImplOptions.InternalCall)] 
        unsafe public extern String(char *value, int startIndex, int length);
 
        [System.Security.SecurityCritical]  // auto-generated 
        [ResourceExposure(ResourceScope.None)]
        [CLSCompliant(false), MethodImplAttribute(MethodImplOptions.InternalCall)] 
       unsafe public extern String(sbyte *value);
        [System.Security.SecurityCritical]  // auto-generated
        [ResourceExposure(ResourceScope.None)]
        [CLSCompliant(false), MethodImplAttribute(MethodImplOptions.InternalCall)] 
        unsafe public extern String(sbyte *value, int startIndex, int length);
 
        [System.Security.SecurityCritical]  // auto-generated 
        [ResourceExposure(ResourceScope.None)]
        [CLSCompliant(false), MethodImplAttribute(MethodImplOptions.InternalCall)] 
        unsafe public extern String(sbyte *value, int startIndex, int length, Encoding enc);

        [System.Security.SecurityCritical]  // auto-generated
        unsafe static private String CreateString(sbyte *value, int startIndex, int length, Encoding enc) { 
            if (enc == null)
                return new String(value, startIndex, length); // default to ANSI 
 
            if (length < 0)
                throw new ArgumentOutOfRangeException("length", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum")); 
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException("startIndex", Environment.GetResourceString("ArgumentOutOfRange_StartIndex"));
            if ((value + startIndex) < value) {
                // overflow check 
                throw new ArgumentOutOfRangeException("startIndex", Environment.GetResourceString("ArgumentOutOfRange_PartialWCHAR"));
            } 
 
            byte [] b = new byte[length];
 
            try {
                Buffer.memcpy((byte*)value, startIndex, b, 0, length);
            }
            catch(NullReferenceException) { 
                // If we got a NullReferencException. It means the pointer or
                // the index is out of range 
                throw new ArgumentOutOfRangeException("value", 
                        Environment.GetResourceString("ArgumentOutOfRange_PartialWCHAR"));
            } 

            return enc.GetString(b);
        }
 
        // Helper for encodings so they can talk to our buffer directly
        // stringLength must be the exact size we'll expect 
        [System.Security.SecurityCritical]  // auto-generated 
        unsafe static internal String CreateStringFromEncoding(
            byte* bytes, int byteLength, Encoding encoding) 
        {
            Contract.Requires(bytes != null);
            Contract.Requires(byteLength >= 0);
 
            // Get our string length
            int stringLength = encoding.GetCharCount(bytes, byteLength, null); 
            Contract.Assert(stringLength >= 0, "stringLength >= 0"); 

            // They gave us an empty string if they needed one 
            // 0 bytelength might be possible if there's something in an encoder
            if (stringLength == 0)
                return String.Empty;
 
            String s = FastAllocateString(stringLength);
            fixed(char* pTempChars = &s.m_firstChar) 
            { 
                int doubleCheck = encoding.GetChars(bytes, byteLength, pTempChars, stringLength, null);
                Contract.Assert(stringLength == doubleCheck, 
                    "Expected encoding.GetChars to return same length as encoding.GetCharCount");
            }

            return s; 
        }
 
        [System.Security.SecuritySafeCritical]  // auto-generated 
        unsafe internal byte[] ConvertToAnsi(int iMaxDBCSCharByteSize, bool fBestFit, bool fThrowOnUnmappableChar, out int cbLength)
        { 
            const uint CP_ACP = 0;
            int nb;

            int cbNativeBuffer = (Length + 3) * iMaxDBCSCharByteSize; 
            byte[] bytes = new byte[cbNativeBuffer];
 
            const uint WC_NO_BEST_FIT_CHARS = 0x00000400; 

            uint flgs = (fBestFit ? 0 : WC_NO_BEST_FIT_CHARS); 
            uint DefaultCharUsed = 0;

            fixed (byte* pbNativeBuffer = bytes)
            { 
                fixed (char* pwzChar = &this.m_firstChar)
                { 
                    nb = Win32Native.WideCharToMultiByte( 
                        CP_ACP,
                        flgs, 
                        pwzChar,
                        this.Length,
                        pbNativeBuffer,
                        cbNativeBuffer, 
                        IntPtr.Zero,
                        (fThrowOnUnmappableChar ? new IntPtr(&DefaultCharUsed) : IntPtr.Zero)); 
                } 
            }
 
            if (0 != DefaultCharUsed)
            {
                throw new ArgumentException(Environment.GetResourceString("Interop_Marshal_Unmappable_Char"));
            } 

            cbLength = nb; 
            bytes[nb] = 0; 
            return bytes;
        } 

        // Normalization Methods
        // These just wrap calls to Normalization class
        public bool IsNormalized() 
        {
#if !FEATURE_NORM_IDNA_ONLY 
            // Default to Form C 
            return IsNormalized(NormalizationForm.FormC);
#else 
            // Default to Form IDNA
            return IsNormalized((NormalizationForm)ExtendedNormalizationForms.FormIdna);
#endif
        } 

        [System.Security.SecuritySafeCritical]  // auto-generated 
        public bool IsNormalized(NormalizationForm normalizationForm) 
        {
#if !FEATURE_NORM_IDNA_ONLY 
            if (this.IsFastSort())
            {
                // If its FastSort && one of the 4 main forms, then its already normalized
                if( normalizationForm == NormalizationForm.FormC || 
                    normalizationForm == NormalizationForm.FormKC ||
                    normalizationForm == NormalizationForm.FormD || 
                    normalizationForm == NormalizationForm.FormKD ) 
                    return true;
            } 
#endif // !FEATURE_NORM_IDNA_ONLY
            return Normalization.IsNormalized(this, normalizationForm);
        }
 
        public String Normalize()
        { 
#if !FEATURE_NORM_IDNA_ONLY 
            // Default to Form C
            return Normalize(NormalizationForm.FormC); 
#else
            // Default to Form IDNA
            return Normalize((NormalizationForm)ExtendedNormalizationForms.FormIdna);
#endif 
        }
 
        [System.Security.SecuritySafeCritical]  // auto-generated 
        public String Normalize(NormalizationForm normalizationForm)
        { 
#if !FEATURE_NORM_IDNA_ONLY
            if (this.IsAscii())
            {
                // If its FastSort && one of the 4 main forms, then its already normalized 
                if( normalizationForm == NormalizationForm.FormC ||
                    normalizationForm == NormalizationForm.FormKC || 
                    normalizationForm == NormalizationForm.FormD || 
                    normalizationForm == NormalizationForm.FormKD )
                    return this; 
            }
#endif // !FEATURE_NORM_IDNA_ONLY
            return Normalization.Normalize(this, normalizationForm);
        } 

        [System.Security.SecurityCritical]  // auto-generated 
        [ResourceExposure(ResourceScope.None)] 
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        internal extern static String FastAllocateString(int length); 

        [System.Security.SecuritySafeCritical]  // auto-generated
        unsafe private static void FillStringChecked(String dest, int destPos, String src)
        { 
            if (src.Length > dest.Length - destPos) {
                throw new IndexOutOfRangeException(); 
            } 
            Contract.EndContractBlock();
 
            fixed(char *pDest = &dest.m_firstChar)
                fixed (char *pSrc = &src.m_firstChar) {
                    wstrcpy(pDest + destPos, pSrc, src.Length);
                } 
        }
 
        // Creates a new string from the characters in a subarray.  The new string will 
        // be created from the characters in value between startIndex and
        // startIndex + length - 1. 
        //
        [System.Security.SecuritySafeCritical]  // auto-generated
        [ResourceExposure(ResourceScope.None)]
        [MethodImplAttribute(MethodImplOptions.InternalCall)] 
        public extern String(char [] value, int startIndex, int length);
 
        // Creates a new string from the characters in a subarray.  The new string will be 
        // created from the characters in value.
        // 

        [System.Security.SecuritySafeCritical]  // auto-generated
        [ResourceExposure(ResourceScope.None)]
        [MethodImplAttribute(MethodImplOptions.InternalCall)] 
        public extern String(char [] value);
 
        // 
        // This handles the case where both smem and dmem pointers are
        //  aligned on a pointer boundary 
        //
        [System.Security.SecurityCritical]  // auto-generated
        private static unsafe void wstrcpyPtrAligned(char *dmem, char *smem, int charCount)
        { 
            Contract.Requires(((int)dmem & (IntPtr.Size-1)) == 0);
            Contract.Requires(((int)smem & (IntPtr.Size-1)) == 0); 
 
#if !WIN64
            while (charCount >= 8) 
            {
                ((uint *)dmem)[0] = ((uint *)smem)[0];
                ((uint *)dmem)[1] = ((uint *)smem)[1];
                ((uint *)dmem)[2] = ((uint *)smem)[2]; 
                ((uint *)dmem)[3] = ((uint *)smem)[3];
 
                dmem      += 8; 
                smem      += 8;
                charCount -= 8; 
            }
            if ((charCount & 4) != 0)
            {
                ((uint *)dmem)[0] = ((uint *)smem)[0]; 
                ((uint *)dmem)[1] = ((uint *)smem)[1];
                dmem += 4; 
                smem += 4; 
            }
#else 
            while (charCount >= 16)
            {
                ((ulong *)dmem)[0] = ((ulong *)smem)[0];
                ((ulong *)dmem)[1] = ((ulong *)smem)[1]; 
                ((ulong *)dmem)[2] = ((ulong *)smem)[2];
                ((ulong *)dmem)[3] = ((ulong *)smem)[3]; 
 
                dmem      += 16;
                smem      += 16; 
                charCount -= 16;
            }

            if ((charCount & 8) != 0) 
            {
                ((ulong *)dmem)[0] = ((ulong *)smem)[0]; 
                ((ulong *)dmem)[1] = ((ulong *)smem)[1]; 
                dmem += 8;
                smem += 8; 
            }
            if ((charCount & 4) != 0)
            {
                ((ulong *)dmem)[0] = ((ulong *)smem)[0]; 
                dmem += 4;
                smem += 4; 
            } 
#endif
            if ((charCount & 2) != 0) 
            {
                ((uint *)dmem)[0] = ((uint *)smem)[0];
                dmem += 2;
                smem += 2; 
            }
 
            if ((charCount & 1) != 0) 
            {
                dmem[0] = smem[0]; 
            }
        }

        [System.Security.SecurityCritical]  // auto-generated 
        internal static unsafe void wstrcpy(char *dmem, char *smem, int charCount)
        { 
            if (charCount > 0) { 
#if ALIGN_ACCESS
                if ((((int)dmem | (int)smem) & 1) == 0) { 
#endif
                    // First Align dmem to a pointer boundary
                    if (((int)dmem & 2) != 0)
                    { 
                        dmem[0] = smem[0];
                        dmem      += 1; 
                        smem      += 1; 
                        charCount -= 1;
                    } 
#if WIN64
                    if ((((int)dmem & 4) != 0) && (charCount >= 2))
                    {
#if IA64 
                        if (((int)smem & 2) != 0)
                        { 
                            dmem[0] = smem[0]; 
                            dmem[1] = smem[1];
                        } 
                        else
#endif
                        {
                            ((uint *)dmem)[0] = ((uint *)smem)[0]; 
                        }
                        dmem += 2; 
                        smem += 2; 
                        charCount -= 2;
                    } 
#endif
                    // Both x86 and AMD64 perform much faster if all writes are pointer aligned
                    // Unaligned reads perform better than 2-byte aligned reads and
                    //  better than pointer aligned reads with 16-bit shift and OR operation 
                    // So on x86 or AMD64 after aligning dmem to a pointer boundry
                    //  we just use standard mechanism 
#if !WIN64 
                    while (charCount >= 8)
                    { 
                        ((uint *)dmem)[0] = ((uint *)smem)[0];
                        ((uint *)dmem)[1] = ((uint *)smem)[1];
                        ((uint *)dmem)[2] = ((uint *)smem)[2];
                        ((uint *)dmem)[3] = ((uint *)smem)[3]; 
                        dmem      += 8;
                        smem      += 8; 
                        charCount -= 8; 
                    }
                    if ((charCount & 4) != 0) 
                    {
                        ((uint *)dmem)[0] = ((uint *)smem)[0];
                        ((uint *)dmem)[1] = ((uint *)smem)[1];
                        dmem += 4; 
                        smem += 4;
                    } 
                    if ((charCount & 2) != 0) 
                    {
                        ((uint *)dmem)[0] = ((uint *)smem)[0]; 
                        dmem += 2;
                        smem += 2;
                    }
#else 
#if AMD64
                    while (charCount >= 16) 
                    { 
                        ((ulong *)dmem)[0] = ((ulong *)smem)[0];
                        ((ulong *)dmem)[1] = ((ulong *)smem)[1]; 
                        ((ulong *)dmem)[2] = ((ulong *)smem)[2];
                        ((ulong *)dmem)[3] = ((ulong *)smem)[3];
                        dmem      += 16;
                        smem      += 16; 
                        charCount -= 16;
                    } 
                    if ((charCount & 8) != 0) 
                    {
                        ((ulong *)dmem)[0] = ((ulong *)smem)[0]; 
                        ((ulong *)dmem)[1] = ((ulong *)smem)[1];
                        dmem += 8;
                        smem += 8;
                    } 
                    if ((charCount & 4) != 0)
                    { 
                        ((ulong *)dmem)[0] = ((ulong *)smem)[0]; 
                        dmem += 4;
                        smem += 4; 
                    }
                    if ((charCount & 2) != 0)
                    {
                        ((uint *)dmem)[0] = ((uint *)smem)[0]; 
                        dmem += 2;
                        smem += 2; 
                    } 
#elif IA64
                    // On IA64 we MUST use aligned reads otherwise 
                    // we will fault
                    if (((int)smem & 2) == 0)
                    {
                        // align is 0 or 4 
                        if  (((int)smem & alignConst) == 0)
                        { 
                            while (charCount >= 16) 
                            {
                                ((ulong *)dmem)[0] = ((ulong *)smem)[0]; 
                                ((ulong *)dmem)[1] = ((ulong *)smem)[1];
                                ((ulong *)dmem)[2] = ((ulong *)smem)[2];
                                ((ulong *)dmem)[3] = ((ulong *)smem)[3];
                                dmem      += 16; 
                                smem      += 16;
                                charCount -= 16; 
                            } 

                            if ((charCount & 8) != 0) 
                            {
                                ((ulong *)dmem)[0] = ((ulong *)smem)[0];
                                ((ulong *)dmem)[1] = ((ulong *)smem)[1];
                                dmem += 8; 
                                smem += 8;
                            } 
 
                            if ((charCount & 4) != 0)
                            { 
                                ((ulong *)dmem)[0] = ((ulong *)smem)[0];
                                dmem += 4;
                                smem += 4;
                            } 
                        }
                        else // align is 4 
                        { 
                            while (charCount >= 8)
                            { 
                                ((uint *)dmem)[0] = ((uint *)smem)[0];
                                ((uint *)dmem)[1] = ((uint *)smem)[1];
                                ((uint *)dmem)[2] = ((uint *)smem)[2];
                                ((uint *)dmem)[3] = ((uint *)smem)[3]; 
                                dmem      += 8;
                                smem      += 8; 
                                charCount -= 8; 
                            }
 
                            if ((charCount & 4) != 0)
                            {
                                ((uint *)dmem)[0] = ((uint *)smem)[0];
                                ((uint *)dmem)[1] = ((uint *)smem)[1]; 
                                dmem += 4;
                                smem += 4; 
                            } 
                        }
                        if ((charCount & 2) != 0) 
                        {
                            ((uint *)dmem)[0] = ((uint *)smem)[0];
                            dmem += 2;
                            smem += 2; 
                        }
                    } 
                    else // align is 2 or 6 
                    {
                        while (charCount >= 8) 
                        {
                            dmem[0] = smem[0];
                            dmem[1] = smem[1];
                            dmem[2] = smem[2]; 
                            dmem[3] = smem[3];
                            dmem[4] = smem[4]; 
                            dmem[5] = smem[5]; 
                            dmem[6] = smem[6];
                            dmem[7] = smem[7]; 
                            dmem += 8;
                            smem += 8;
                            charCount -= 8;
                        } 

                        if ((charCount & 4) != 0) 
                        { 
                            dmem[0] = smem[0];
                            dmem[1] = smem[1]; 
                            dmem[2] = smem[2];
                            dmem[3] = smem[3];
                            dmem += 4;
                            smem += 4; 
                        }
                        if ((charCount & 2) != 0) 
                        { 
                            dmem[0] = smem[0];
                            dmem[1] = smem[1]; 
                            dmem += 2;
                            smem += 2;
                        }
                    } 
#endif
#endif 
                    if ((charCount & 1) != 0) 
                    {
                        dmem[0] = smem[0]; 
                    }
#if ALIGN_ACCESS
                }
                else 
                {
                    // This is rare case where at least one of the pointers is only byte aligned. 
                    do { 
                        ((byte *)dmem)[0] = ((byte *)smem)[0];
                        ((byte *)dmem)[1] = ((byte *)smem)[1]; 
                        charCount -= 1;
                        dmem += 1;
                        smem += 1;
                    } 
                    while (charCount > 0);
                } 
#endif 
            }
        } 

        [System.Security.SecuritySafeCritical]  // auto-generated
        private String CtorCharArray(char [] value)
        { 
            if (value != null && value.Length != 0) {
                String result = FastAllocateString(value.Length); 
 
                unsafe {
                    fixed (char * dest = result, source = value) { 
#if WIN32
                        wstrcpyPtrAligned(dest, source, value.Length);
#else
                        wstrcpy(dest, source, value.Length); 
#endif //WIN32
                    } 
                } 
                return result;
            } 
            else
                return String.Empty;
        }
 
        [System.Security.SecuritySafeCritical]  // auto-generated
        private String CtorCharArrayStartLength(char [] value, int startIndex, int length) 
        { 
            if (value == null)
                throw new ArgumentNullException("value"); 

            if (startIndex < 0)
                throw new ArgumentOutOfRangeException("startIndex", Environment.GetResourceString("ArgumentOutOfRange_StartIndex"));
 
            if (length < 0)
                throw new ArgumentOutOfRangeException("length", Environment.GetResourceString("ArgumentOutOfRange_NegativeLength")); 
 
            if (startIndex > value.Length - length)
                throw new ArgumentOutOfRangeException("startIndex", Environment.GetResourceString("ArgumentOutOfRange_Index")); 
            Contract.EndContractBlock();

            if (length > 0) {
                String result = FastAllocateString(length); 

                unsafe { 
                    fixed (char * dest = result, source = value) { 
                        wstrcpy(dest, source + startIndex, length);
                    } 
                }
                return result;
            }
            else 
                return String.Empty;
        } 
 
        [System.Security.SecuritySafeCritical]  // auto-generated
        private String CtorCharCount(char c, int count) 
        {
            if (count > 0) {
                String result = FastAllocateString(count);
                unsafe { 
                    fixed (char *dest = result) {
                        char *dmem = dest; 
                        while (((uint)dmem & 3) != 0 && count > 0) { 
                            *dmem++ = c;
                            count--; 
                        }
                        uint cc = (uint)((c << 16) | c);
                        if (count >= 4) {
                            count -= 4; 
                            do{
                                ((uint *)dmem)[0] = cc; 
                                ((uint *)dmem)[1] = cc; 
                                dmem += 4;
                                count -= 4; 
                            } while (count >= 0);
                        }
                        if ((count & 2) != 0) {
                            ((uint *)dmem)[0] = cc; 
                            dmem += 2;
                        } 
                        if ((count & 1) != 0) 
                            dmem[0] = c;
                    } 
                }
                return result;
            }
            else if (count == 0) 
                return String.Empty;
            else 
                throw new ArgumentOutOfRangeException("count", Environment.GetResourceString("ArgumentOutOfRange_MustBeNonNegNum", "count")); 
        }
 
        [System.Security.SecurityCritical]  // auto-generated
        #if !FEATURE_CORECLR
        [System.Runtime.ForceTokenStabilization]
        #endif //!FEATURE_CORECLR 
        private static unsafe int wcslen(char *ptr)
        { 
            char *end = ptr; 

            // The following code is (somewhat surprisingly!) significantly faster than a naive loop, 
            // at least on x86 and the current jit.

            // First make sure our pointer is aligned on a dword boundary
            while (((uint)end & 3) != 0 && *end != 0) 
                end++;
            if (*end != 0) { 
                // The loop condition below works because if "end[0] & end[1]" is non-zero, that means 
                // neither operand can have been zero. If is zero, we have to look at the operands individually,
                // but we hope this going to fairly rare. 

                // In general, it would be incorrect to access end[1] if we haven't made sure
                // end[0] is non-zero. However, we know the ptr has been aligned by the loop above
                // so end[0] and end[1] must be in the same page, so they're either both accessible, or both not. 

                while ((end[0] & end[1]) != 0 || (end[0] != 0 && end[1] != 0)) { 
                    end += 2; 
                }
            } 
            // finish up with the naive loop
            for ( ; *end != 0; end++)
                ;
 
            int count = (int)(end - ptr);
 
            return count; 
        }
 
        [System.Security.SecurityCritical]  // auto-generated
        private unsafe String CtorCharPtr(char *ptr)
        {
            if (ptr == null) 
                return String.Empty;
 
#if !FEATURE_PAL 
            if (ptr < (char*)64000)
                throw new ArgumentException(Environment.GetResourceString("Arg_MustBeStringPtrNotAtom")); 
#endif // FEATURE_PAL

            Contract.Assert(this == null, "this == null");        // this is the string constructor, we allocate it
 
            try {
                int count = wcslen(ptr); 
                String result = FastAllocateString(count); 
                fixed (char *dest = result)
                    wstrcpy(dest, ptr, count); 
                return result;
            }
            catch (NullReferenceException) {
                throw new ArgumentOutOfRangeException("ptr", Environment.GetResourceString("ArgumentOutOfRange_PartialWCHAR")); 
            }
        } 
 
        [System.Security.SecurityCritical]  // auto-generated
        #if !FEATURE_CORECLR 
        [System.Runtime.ForceTokenStabilization]
        #endif //!FEATURE_CORECLR
        private unsafe String CtorCharPtrStartLength(char *ptr, int startIndex, int length)
        { 
            if (length < 0) {
                throw new ArgumentOutOfRangeException("length", Environment.GetResourceString("ArgumentOutOfRange_NegativeLength")); 
            } 

            if (startIndex < 0) { 
                throw new ArgumentOutOfRangeException("startIndex", Environment.GetResourceString("ArgumentOutOfRange_StartIndex"));
            }
            Contract.EndContractBlock();
            Contract.Assert(this == null, "this == null");        // this is the string constructor, we allocate it 

            char *pFrom = ptr + startIndex; 
            if (pFrom < ptr) { 
                // This means that the pointer operation has had an overflow
                throw new ArgumentOutOfRangeException("startIndex", Environment.GetResourceString("ArgumentOutOfRange_PartialWCHAR")); 
            }

            String result = FastAllocateString(length);
 
            try {
                fixed(char *dest = result) 
                    wstrcpy(dest, pFrom, length); 
                return result;
            } 
            catch (NullReferenceException) {
                throw new ArgumentOutOfRangeException("ptr", Environment.GetResourceString("ArgumentOutOfRange_PartialWCHAR"));
            }
        } 

        [System.Security.SecuritySafeCritical]  // auto-generated 
        [ResourceExposure(ResourceScope.None)] 
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern String(char c, int count); 

        //
        //
        // INSTANCE METHODS 
        //
        // 
 
        // Provides a culture-correct string comparison. StrA is compared to StrB
        // to determine whether it is lexicographically less, equal, or greater, and then returns 
        // either a negative integer, 0, or a positive integer; respectively.
        //
#if !FEATURE_CORECLR
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")] 
#endif
        public static int Compare(String strA, String strB) { 
            return CultureInfo.CurrentCulture.CompareInfo.Compare(strA, strB, CompareOptions.None); 
        }
 

        // Provides a culture-correct string comparison. strA is compared to strB
        // to determine whether it is lexicographically less, equal, or greater, and then a
        // negative integer, 0, or a positive integer is returned; respectively. 
        // The case-sensitive option is set by ignoreCase
        // 
#if !FEATURE_CORECLR 
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
#endif 
#if false
        private void TrimSrcHack() {}    // workaround to avoid unclosed "#if !FEATURE_CORECLR"
#endif
        public static int Compare(String strA, String strB, bool ignoreCase) 
        {
            if (ignoreCase) { 
                return CultureInfo.CurrentCulture.CompareInfo.Compare(strA, strB, CompareOptions.IgnoreCase); 
            }
            return CultureInfo.CurrentCulture.CompareInfo.Compare(strA, strB, CompareOptions.None); 
        }


        // Provides a more flexible function for string comparision. See StringComparison 
        // for meaning of different comparisonType.
        [System.Security.SecuritySafeCritical]  // auto-generated 
        public static int Compare(String strA, String strB, StringComparison comparisonType) { 
            if (comparisonType < StringComparison.CurrentCulture || comparisonType > StringComparison.OrdinalIgnoreCase) {
                throw new ArgumentException(Environment.GetResourceString("NotSupported_StringComparison"), "comparisonType"); 
            }
            Contract.EndContractBlock();

            if ((Object)strA == (Object)strB) { 
                return 0;
            } 
 
            //they can't both be null;
            if (strA == null) { 
                return -1;
            }

            if (strB == null) { 
                return 1;
            } 
 

            switch (comparisonType) { 
                case StringComparison.CurrentCulture:
                    return CultureInfo.CurrentCulture.CompareInfo.Compare(strA, strB, CompareOptions.None);

                case StringComparison.CurrentCultureIgnoreCase: 
                    return CultureInfo.CurrentCulture.CompareInfo.Compare(strA, strB, CompareOptions.IgnoreCase);
 
                case StringComparison.InvariantCulture: 
                    return CultureInfo.InvariantCulture.CompareInfo.Compare(strA, strB, CompareOptions.None);
 
                case StringComparison.InvariantCultureIgnoreCase:
                    return CultureInfo.InvariantCulture.CompareInfo.Compare(strA, strB, CompareOptions.IgnoreCase);

                case StringComparison.Ordinal: 
                    return CompareOrdinalHelper(strA, strB);
 
                case StringComparison.OrdinalIgnoreCase: 
                    // If both strings are ASCII strings, we can take the fast path.
                    if (strA.IsAscii() && strB.IsAscii()) { 
                        return (CompareOrdinalIgnoreCaseHelper(strA, strB));
                    }
                    // Take the slow path.
                    return TextInfo.CompareOrdinalIgnoreCase(strA, strB); 

                default: 
                    throw new NotSupportedException(Environment.GetResourceString("NotSupported_StringComparison")); 
            }
        } 


        // Provides a culture-correct string comparison. strA is compared to strB
        // to determine whether it is lexicographically less, equal, or greater, and then a 
        // negative integer, 0, or a positive integer is returned; respectively.
        // 
#if !FEATURE_CORECLR 
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
#endif 
        public static int Compare(String strA, String strB, CultureInfo culture, CompareOptions options) {
            if (culture==null) {
                throw new ArgumentNullException("culture");
            } 
            Contract.EndContractBlock();
 
            return culture.CompareInfo.Compare(strA, strB, options); 
        }
 


        // Provides a culture-correct string comparison. strA is compared to strB
        // to determine whether it is lexicographically less, equal, or greater, and then a 
        // negative integer, 0, or a positive integer is returned; respectively.
        // The case-sensitive option is set by ignoreCase, and the culture is set 
        // by culture 
        //
#if !FEATURE_CORECLR 
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
#endif
#if false
      private void TrimSrcHack() {}    // workaround to avoid unclosed "#if !FEATURE_CORECLR" 
#endif
        public static int Compare(String strA, String strB, bool ignoreCase, CultureInfo culture) { 
            if (culture == null) { 
                throw new ArgumentNullException("culture");
            } 
            Contract.EndContractBlock();

            if (ignoreCase) {
                return culture.CompareInfo.Compare(strA, strB, CompareOptions.IgnoreCase); 
            }
            return culture.CompareInfo.Compare(strA, strB, CompareOptions.None); 
        } 

        // Determines whether two string regions match.  The substring of strA beginning 
        // at indexA of length count is compared with the substring of strB
        // beginning at indexB of the same length.
        //
        public static int Compare(String strA, int indexA, String strB, int indexB, int length) { 
            int lengthA = length;
            int lengthB = length; 
 
            if (strA!=null) {
                if (strA.Length - indexA < lengthA) { 
                  lengthA = (strA.Length - indexA);
                }
            }
 
            if (strB!=null) {
                if (strB.Length - indexB < lengthB) { 
                    lengthB = (strB.Length - indexB); 
                }
            } 
            return CultureInfo.CurrentCulture.CompareInfo.Compare(strA, indexA, lengthA, strB, indexB, lengthB, CompareOptions.None);
        }

 
        // Determines whether two string regions match.  The substring of strA beginning
        // at indexA of length count is compared with the substring of strB 
        // beginning at indexB of the same length.  Case sensitivity is determined by the ignoreCase boolean. 
        //
        public static int Compare(String strA, int indexA, String strB, int indexB, int length, bool ignoreCase) { 
            int lengthA = length;
            int lengthB = length;

            if (strA!=null) { 
                if (strA.Length - indexA < lengthA) {
                  lengthA = (strA.Length - indexA); 
                } 
            }
 
            if (strB!=null) {
                if (strB.Length - indexB < lengthB) {
                    lengthB = (strB.Length - indexB);
                } 
            }
 
            if (ignoreCase) { 
                return CultureInfo.CurrentCulture.CompareInfo.Compare(strA, indexA, lengthA, strB, indexB, lengthB, CompareOptions.IgnoreCase);
            } 
            return CultureInfo.CurrentCulture.CompareInfo.Compare(strA, indexA, lengthA, strB, indexB, lengthB, CompareOptions.None);
        }

        // Determines whether two string regions match.  The substring of strA beginning 
        // at indexA of length length is compared with the substring of strB
        // beginning at indexB of the same length.  Case sensitivity is determined by the ignoreCase boolean, 
        // and the culture is set by culture. 
        //
        public static int Compare(String strA, int indexA, String strB, int indexB, int length, bool ignoreCase, CultureInfo culture) { 
            if (culture == null) {
                throw new ArgumentNullException("culture");
            }
            Contract.EndContractBlock(); 

            int lengthA = length; 
            int lengthB = length; 

            if (strA!=null) { 
                if (strA.Length - indexA < lengthA) {
                  lengthA = (strA.Length - indexA);
                }
            } 

            if (strB!=null) { 
                if (strB.Length - indexB < lengthB) { 
                    lengthB = (strB.Length - indexB);
                } 
            }

            if (ignoreCase) {
                return culture.CompareInfo.Compare(strA,indexA,lengthA, strB, indexB, lengthB,CompareOptions.IgnoreCase); 
            } else {
                return culture.CompareInfo.Compare(strA,indexA,lengthA, strB, indexB, lengthB,CompareOptions.None); 
            } 
        }
 

        // Determines whether two string regions match.  The substring of strA beginning
        // at indexA of length length is compared with the substring of strB
        // beginning at indexB of the same length. 
        //
        public static int Compare(String strA, int indexA, String strB, int indexB, int length, CultureInfo culture, CompareOptions options) { 
            if (culture==null) { 
                throw new ArgumentNullException("culture");
            } 
            Contract.EndContractBlock();

            int lengthA = length;
            int lengthB = length; 

            if (strA!=null) { 
                if (strA.Length - indexA < lengthA) { 
                  lengthA = (strA.Length - indexA);
                } 
            }

            if (strB!=null) {
                if (strB.Length - indexB < lengthB) { 
                    lengthB = (strB.Length - indexB);
                } 
            } 

            return culture.CompareInfo.Compare(strA,indexA,lengthA, strB, indexB, lengthB, options); 
        }


        [System.Security.SecuritySafeCritical]  // auto-generated 
        public static int Compare(String strA, int indexA, String strB, int indexB, int length, StringComparison comparisonType) {
            if (comparisonType < StringComparison.CurrentCulture || comparisonType > StringComparison.OrdinalIgnoreCase) { 
                throw new ArgumentException(Environment.GetResourceString("NotSupported_StringComparison"), "comparisonType"); 
            }
            Contract.EndContractBlock(); 

            if (strA == null || strB == null) {
                 if ((Object)strA==(Object)strB) { //they're both null;
                     return 0; 
                 }
 
                 return (strA==null)? -1 : 1; //-1 if A is null, 1 if B is null. 
            }
 
            // @
            if (length < 0) {
                throw new ArgumentOutOfRangeException("length",
                                                      Environment.GetResourceString("ArgumentOutOfRange_NegativeLength")); 
            }
 
            if (indexA < 0) { 
                throw new ArgumentOutOfRangeException("indexA",
                                                     Environment.GetResourceString("ArgumentOutOfRange_Index")); 
            }

            if (indexB < 0) {
                throw new ArgumentOutOfRangeException("indexB", 
                                                     Environment.GetResourceString("ArgumentOutOfRange_Index"));
            } 
 
            if (strA.Length - indexA < 0) {
                throw new ArgumentOutOfRangeException("indexA", 
                                                      Environment.GetResourceString("ArgumentOutOfRange_Index"));
            }

            if (strB.Length - indexB < 0) { 
                throw new ArgumentOutOfRangeException("indexB",
                                                      Environment.GetResourceString("ArgumentOutOfRange_Index")); 
            } 

            if( ( length == 0 )  || 
                ((strA == strB) && (indexA == indexB)) ){
                return 0;
            }
 
            int lengthA = length;
            int lengthB = length; 
 
            if (strA!=null) {
                if (strA.Length - indexA < lengthA) { 
                  lengthA = (strA.Length - indexA);
                }
            }
 
            if (strB!=null) {
                if (strB.Length - indexB < lengthB) { 
                    lengthB = (strB.Length - indexB); 
                }
            } 

            switch (comparisonType) {
                case StringComparison.CurrentCulture:
                    return CultureInfo.CurrentCulture.CompareInfo.Compare(strA, indexA, lengthA, strB, indexB, lengthB, CompareOptions.None); 

                case StringComparison.CurrentCultureIgnoreCase: 
                    return CultureInfo.CurrentCulture.CompareInfo.Compare(strA, indexA, lengthA, strB, indexB, lengthB, CompareOptions.IgnoreCase); 

                case StringComparison.InvariantCulture: 
                    return CultureInfo.InvariantCulture.CompareInfo.Compare(strA, indexA, lengthA, strB, indexB, lengthB, CompareOptions.None);

                case StringComparison.InvariantCultureIgnoreCase:
                    return CultureInfo.InvariantCulture.CompareInfo.Compare(strA, indexA, lengthA, strB, indexB, lengthB, CompareOptions.IgnoreCase); 

                case StringComparison.Ordinal: 
                    // 
                    return nativeCompareOrdinalEx(strA, indexA, strB, indexB, length);
 
                case StringComparison.OrdinalIgnoreCase:
                    return (TextInfo.CompareOrdinalIgnoreCaseEx(strA, indexA, strB, indexB, lengthA, lengthB));

                default: 
                    throw new ArgumentException(Environment.GetResourceString("NotSupported_StringComparison"));
            } 
 
        }
 
        // Compares this object to another object, returning an integer that
        // indicates the relationship. This method returns a value less than 0 if this is less than value, 0
        // if this is equal to value, or a value greater than 0
        // if this is greater than value.  Strings are considered to be 
        // greater than all non-String objects.  Note that this means sorted
        // arrays would contain nulls, other objects, then Strings in that order. 
        // 
#if !FEATURE_CORECLR
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")] 
#endif
        [Pure]
        public int CompareTo(Object value) {
            if (value == null) { 
                return 1;
            } 
 
            if (!(value is String)) {
                throw new ArgumentException(Environment.GetResourceString("Arg_MustBeString")); 
            }

            return String.Compare(this,(String)value, StringComparison.CurrentCulture);
        } 

        // Determines the sorting relation of StrB to the current instance. 
        // 
#if !FEATURE_CORECLR
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")] 
#endif
        [Pure]
        public int CompareTo(String strB) {
            if (strB==null) { 
                return 1;
            } 
 
            return CultureInfo.CurrentCulture.CompareInfo.Compare(this, strB, 0);
        } 

        // Compares strA and strB using an ordinal (code-point) comparison.
        //
#if !FEATURE_CORECLR 
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
#endif 
        public static int CompareOrdinal(String strA, String strB) { 
            if ((Object)strA == (Object)strB) {
                return 0; 
            }

            //they can't both be null;
            if( strA == null) { 
                return -1;
            } 
 
            if( strB == null) {
                return 1; 
            }

            //
            return CompareOrdinalHelper(strA, strB); 
        }
 
 
        // Compares strA and strB using an ordinal (code-point) comparison.
        // 
        [System.Security.SecuritySafeCritical]  // auto-generated
#if !FEATURE_CORECLR
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
#endif 
        public static int CompareOrdinal(String strA, int indexA, String strB, int indexB, int length) {
           if (strA == null || strB == null) { 
                if ((Object)strA==(Object)strB) { //they're both null; 
                    return 0;
                } 

                return (strA==null)? -1 : 1; //-1 if A is null, 1 if B is null.
            }
 
            return nativeCompareOrdinalEx(strA, indexA, strB, indexB, length);
        } 
 

#if !FEATURE_CORECLR 
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
#endif
        public bool Contains( string value ) {
            return ( IndexOf(value, StringComparison.Ordinal) >=0 ); 
        }
 
 
        // Determines whether a specified string is a suffix of the the current instance.
        // 
        // The case-sensitive and culture-sensitive option is set by options,
        // and the default culture is used.
        //
        public Boolean EndsWith(String value) { 
#if FEATURE_CORECLR
            return EndsWith(value, StringComparison.Ordinal); 
#else 
            return EndsWith(value, StringComparison.CurrentCulture);
#endif 
        }

        [System.Security.SecuritySafeCritical]  // auto-generated
        [ComVisible(false)] 
        public Boolean EndsWith(String value, StringComparison comparisonType) {
            if( (Object)value == null) { 
                throw new ArgumentNullException("value"); 
            }
 
            if( comparisonType < StringComparison.CurrentCulture || comparisonType > StringComparison.OrdinalIgnoreCase) {
                throw new ArgumentException(Environment.GetResourceString("NotSupported_StringComparison"), "comparisonType");
            }
            Contract.EndContractBlock(); 

            if( (Object)this == (Object)value) { 
                return true; 
            }
 
            if( value.Length == 0) {
                return true;
            }
 
            switch (comparisonType) {
                case StringComparison.CurrentCulture: 
                    return CultureInfo.CurrentCulture.CompareInfo.IsSuffix(this, value, CompareOptions.None); 

                case StringComparison.CurrentCultureIgnoreCase: 
                    return CultureInfo.CurrentCulture.CompareInfo.IsSuffix(this, value, CompareOptions.IgnoreCase);

                case StringComparison.InvariantCulture:
                    return CultureInfo.InvariantCulture.CompareInfo.IsSuffix(this, value, CompareOptions.None); 

                case StringComparison.InvariantCultureIgnoreCase: 
                    return CultureInfo.InvariantCulture.CompareInfo.IsSuffix(this, value, CompareOptions.IgnoreCase); 

                case StringComparison.Ordinal: 
                    return this.Length < value.Length ? false : (nativeCompareOrdinalEx(this, this.Length -value.Length, value, 0, value.Length) == 0);

                case StringComparison.OrdinalIgnoreCase:
                    return this.Length < value.Length ? false : (TextInfo.CompareOrdinalIgnoreCaseEx(this, this.Length - value.Length, value, 0, value.Length, value.Length) == 0); 

                default: 
                    throw new ArgumentException(Environment.GetResourceString("NotSupported_StringComparison"), "comparisonType"); 
            }
        } 

        public Boolean EndsWith(String value, Boolean ignoreCase, CultureInfo culture) {
            if (null==value) {
                throw new ArgumentNullException("value"); 
            }
            Contract.EndContractBlock(); 
 
            if((object)this == (object)value) {
                return true; 
            }

            CultureInfo referenceCulture;
            if (culture == null) 
#if FEATURE_CORECLR
                referenceCulture = CultureInfo.InvariantCulture; 
#else 
                referenceCulture = CultureInfo.CurrentCulture;
#endif 
            else
                referenceCulture = culture;

            return referenceCulture.CompareInfo.IsSuffix(this, value, ignoreCase ? CompareOptions.IgnoreCase : CompareOptions.None); 
        }
 
#if !FEATURE_CORECLR 
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
#endif 
        internal bool EndsWith(char value) {
            int thisLen = this.Length;
            if (thisLen != 0) {
                if (this[thisLen - 1] == value) 
                    return true;
            } 
            return false; 
        }
 

        // Returns the index of the first occurance of value in the current instance.
        // The search starts at startIndex and runs thorough the next count characters.
        // 
#if !FEATURE_CORECLR
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")] 
#endif 
        public int IndexOf(char value) {
            return IndexOf(value, 0, this.Length); 
        }

#if !FEATURE_CORECLR
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")] 
#endif
        public int IndexOf(char value, int startIndex) { 
            return IndexOf(value, startIndex, this.Length - startIndex); 
        }
 
        [System.Security.SecuritySafeCritical]  // auto-generated
        [ResourceExposure(ResourceScope.None)]
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern int IndexOf(char value, int startIndex, int count); 

        // Returns the index of the first occurance of any character in value in the current instance. 
        // The search starts at startIndex and runs to endIndex-1. [startIndex,endIndex). 
        //
 
#if !FEATURE_CORECLR
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
#endif
        public int IndexOfAny(char [] anyOf) { 
            return IndexOfAny(anyOf,0, this.Length);
        } 
 
#if !FEATURE_CORECLR
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")] 
#endif
        public int IndexOfAny(char [] anyOf, int startIndex) {
            return IndexOfAny(anyOf, startIndex, this.Length - startIndex);
        } 

        [System.Security.SecuritySafeCritical]  // auto-generated 
        [ResourceExposure(ResourceScope.None)] 
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern int IndexOfAny(char [] anyOf, int startIndex, int count); 


        // Determines the position within this string of the first occurence of the specified
        // string, according to the specified search criteria.  The search begins at 
        // the first character of this string, it is case-sensitive and ordinal (code-point)
        // comparison is used. 
        // 
        public int IndexOf(String value) {
#if FEATURE_CORECLR 
            return IndexOf(value, StringComparison.Ordinal);
#else
            return IndexOf(value, StringComparison.CurrentCulture);
#endif 
        }
 
        // Determines the position within this string of the first occurence of the specified 
        // string, according to the specified search criteria.  The search begins at
        // startIndex, it is case-sensitive and ordinal (code-point) comparison is used. 
        //
        public int IndexOf(String value, int startIndex) {
#if FEATURE_CORECLR
            return IndexOf(value, startIndex, StringComparison.Ordinal); 
#else
            return IndexOf(value, startIndex, StringComparison.CurrentCulture); 
#endif 
        }
 
        // Determines the position within this string of the first occurence of the specified
        // string, according to the specified search criteria.  The search begins at
        // startIndex, ends at endIndex and ordinal (code-point) comparison is used.
        // 
        public int IndexOf(String value, int startIndex, int count) {
            if (startIndex < 0 || startIndex > this.Length) { 
                throw new ArgumentOutOfRangeException("startIndex", Environment.GetResourceString("ArgumentOutOfRange_Index")); 
            }
 
            if (count < 0 || count > this.Length - startIndex) {
                throw new ArgumentOutOfRangeException("count", Environment.GetResourceString("ArgumentOutOfRange_Count"));
            }
            Contract.EndContractBlock(); 

#if FEATURE_CORECLR 
            return IndexOf(value, startIndex, count, StringComparison.Ordinal); 
#else
            return IndexOf(value, startIndex, count, StringComparison.CurrentCulture); 
#endif
        }

        [System.Security.SecuritySafeCritical]  // auto-generated 
#if !FEATURE_CORECLR
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")] 
#endif 
        public int IndexOf(String value, StringComparison comparisonType) {
            return IndexOf(value, 0, this.Length, comparisonType); 
        }

#if !FEATURE_CORECLR
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")] 
#endif
        public int IndexOf(String value, int startIndex, StringComparison comparisonType) { 
            return IndexOf(value, startIndex, this.Length - startIndex, comparisonType); 
        }
 
        public int IndexOf(String value, int startIndex, int count, StringComparison comparisonType) {
            // Validate inputs
            if (value == null)
                throw new ArgumentNullException("value"); 

            if (startIndex < 0 || startIndex > this.Length) 
                throw new ArgumentOutOfRangeException("startIndex", Environment.GetResourceString("ArgumentOutOfRange_Index")); 

            if (count < 0 || startIndex > this.Length - count) 
                throw new ArgumentOutOfRangeException("count", Environment.GetResourceString("ArgumentOutOfRange_Count"));
            Contract.EndContractBlock();

            switch (comparisonType) { 
                case StringComparison.CurrentCulture:
                    return CultureInfo.CurrentCulture.CompareInfo.IndexOf(this, value, startIndex, count, CompareOptions.None); 
 
                case StringComparison.CurrentCultureIgnoreCase:
                    return CultureInfo.CurrentCulture.CompareInfo.IndexOf(this, value, startIndex, count, CompareOptions.IgnoreCase); 

                case StringComparison.InvariantCulture:
                    return CultureInfo.InvariantCulture.CompareInfo.IndexOf(this, value, startIndex, count, CompareOptions.None);
 
                case StringComparison.InvariantCultureIgnoreCase:
                    return CultureInfo.InvariantCulture.CompareInfo.IndexOf(this, value, startIndex, count, CompareOptions.IgnoreCase); 
 
                case StringComparison.Ordinal:
                    return CultureInfo.InvariantCulture.CompareInfo.IndexOf(this, value, startIndex, count, CompareOptions.Ordinal); 

                case StringComparison.OrdinalIgnoreCase:
                    return TextInfo.IndexOfStringOrdinalIgnoreCase(this, value, startIndex, count);
 
                default:
                    throw new ArgumentException(Environment.GetResourceString("NotSupported_StringComparison"), "comparisonType"); 
            } 
        }
 
        // Returns the index of the last occurance of value in the current instance.
        // The search starts at startIndex and runs to endIndex. [startIndex,endIndex].
        // The character at position startIndex is included in the search.  startIndex is the larger
        // index within the string. 
        //
#if !FEATURE_CORECLR 
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")] 
#endif
        public int LastIndexOf(char value) { 
            return LastIndexOf(value, this.Length-1, this.Length);
        }

        public int LastIndexOf(char value, int startIndex){ 
            return LastIndexOf(value,startIndex,startIndex + 1);
        } 
 
        [System.Security.SecuritySafeCritical]  // auto-generated
        [ResourceExposure(ResourceScope.None)] 
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern int LastIndexOf(char value, int startIndex, int count);

        // Returns the index of the last occurance of any character in value in the current instance. 
        // The search starts at startIndex and runs to endIndex. [startIndex,endIndex].
        // The character at position startIndex is included in the search.  startIndex is the larger 
        // index within the string. 
        //
 
        //ForceInline ... Jit can't recognize String.get_Length to determine that this is "fluff"
        public int LastIndexOfAny(char [] anyOf) {
            return LastIndexOfAny(anyOf,this.Length-1,this.Length);
        } 

        //ForceInline ... Jit can't recognize String.get_Length to determine that this is "fluff" 
        public int LastIndexOfAny(char [] anyOf, int startIndex) { 
            return LastIndexOfAny(anyOf,startIndex,startIndex + 1);
        } 

        [System.Security.SecuritySafeCritical]  // auto-generated
        [ResourceExposure(ResourceScope.None)]
        [MethodImplAttribute(MethodImplOptions.InternalCall)] 
        public extern int LastIndexOfAny(char [] anyOf, int startIndex, int count);
 
 
        // Returns the index of the last occurance of any character in value in the current instance.
        // The search starts at startIndex and runs to endIndex. [startIndex,endIndex]. 
        // The character at position startIndex is included in the search.  startIndex is the larger
        // index within the string.
        //
        [System.Security.SecuritySafeCritical]  // auto-generated 
        public int LastIndexOf(String value) {
            return LastIndexOf(value, this.Length-1,this.Length, 
#if FEATURE_CORECLR 
                               StringComparison.Ordinal);
#else 
                               StringComparison.CurrentCulture);
#endif
        }
 
        public int LastIndexOf(String value, int startIndex) {
            return LastIndexOf(value, startIndex, startIndex + 1, 
#if FEATURE_CORECLR 
                               StringComparison.Ordinal);
#else 
                               StringComparison.CurrentCulture);
#endif
        }
 
        public int LastIndexOf(String value, int startIndex, int count) {
            if (count<0) { 
                throw new ArgumentOutOfRangeException("count", Environment.GetResourceString("ArgumentOutOfRange_Count")); 
            }
            Contract.EndContractBlock(); 

            return LastIndexOf(value, startIndex, count,
#if FEATURE_CORECLR
                               StringComparison.Ordinal); 
#else
                               StringComparison.CurrentCulture); 
#endif 
        }
 
        [System.Security.SecuritySafeCritical]  // auto-generated
#if !FEATURE_CORECLR
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
#endif 
        public int LastIndexOf(String value, StringComparison comparisonType) {
            return LastIndexOf(value, this.Length-1, this.Length, comparisonType); 
        } 

        public int LastIndexOf(String value, int startIndex, StringComparison comparisonType) { 
            return LastIndexOf(value, startIndex, startIndex + 1, comparisonType);
        }

        public int LastIndexOf(String value, int startIndex, int count, StringComparison comparisonType) { 
            if (value == null)
                throw new ArgumentNullException("value"); 
            Contract.EndContractBlock(); 

            // Special case for 0 length input strings 
            if (this.Length == 0 && (startIndex == -1 || startIndex == 0))
                return (value.Length == 0) ? 0 : -1;

            // Now after handling empty strings, make sure we're not out of range 
            if (startIndex < 0 || startIndex > this.Length)
                throw new ArgumentOutOfRangeException("startIndex", Environment.GetResourceString("ArgumentOutOfRange_Index")); 
 
            // Make sure that we allow startIndex == this.Length
            if (startIndex == this.Length) 
            {
                startIndex--;
                if (count > 0)
                    count--; 

                // If we are looking for nothing, just return 0 
                if (value.Length == 0 && count >= 0 && startIndex - count + 1 >= 0) 
                    return startIndex;
            } 

            // 2nd half of this also catches when startIndex == MAXINT, so MAXINT - 0 + 1 == -1, which is < 0.
            if (count < 0 || startIndex - count + 1 < 0)
                throw new ArgumentOutOfRangeException("count", Environment.GetResourceString("ArgumentOutOfRange_Count")); 

 
            switch (comparisonType) { 
                case StringComparison.CurrentCulture:
                    return CultureInfo.CurrentCulture.CompareInfo.LastIndexOf(this, value, startIndex, count, CompareOptions.None); 

                case StringComparison.CurrentCultureIgnoreCase:
                    return CultureInfo.CurrentCulture.CompareInfo.LastIndexOf(this, value, startIndex, count, CompareOptions.IgnoreCase);
 
                case StringComparison.InvariantCulture:
                    return CultureInfo.InvariantCulture.CompareInfo.LastIndexOf(this, value, startIndex, count, CompareOptions.None); 
 
                case StringComparison.InvariantCultureIgnoreCase:
                    return CultureInfo.InvariantCulture.CompareInfo.LastIndexOf(this, value, startIndex, count, CompareOptions.IgnoreCase); 
                case StringComparison.Ordinal:
                    return CultureInfo.InvariantCulture.CompareInfo.LastIndexOf(this, value, startIndex, count, CompareOptions.Ordinal);

                case StringComparison.OrdinalIgnoreCase: 
                    return TextInfo.LastIndexOfStringOrdinalIgnoreCase(this, value, startIndex, count);
                default: 
                    throw new ArgumentException(Environment.GetResourceString("NotSupported_StringComparison"), "comparisonType"); 
            }
        } 

        //
        //
        public String PadLeft(int totalWidth) { 
            return PadHelper(totalWidth, ' ', false);
        } 
 
        public String PadLeft(int totalWidth, char paddingChar) {
            return PadHelper(totalWidth, paddingChar, false); 
        }

        public String PadRight(int totalWidth) {
            return PadHelper(totalWidth, ' ', true); 
        }
 
        public String PadRight(int totalWidth, char paddingChar) { 
            return PadHelper(totalWidth, paddingChar, true);
        } 


        [System.Security.SecuritySafeCritical]  // auto-generated
        [ResourceExposure(ResourceScope.None)] 
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        private extern String PadHelper(int totalWidth, char paddingChar, bool isRightPadded); 
 
        // Determines whether a specified string is a prefix of the current instance
        // 
#if !FEATURE_CORECLR
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
#endif
        public Boolean StartsWith(String value) { 
            if ((Object)value == null) {
                throw new ArgumentNullException("value"); 
            } 
            Contract.EndContractBlock();
            return StartsWith(value, 
#if FEATURE_CORECLR
                              StringComparison.Ordinal);
#else
                              StringComparison.CurrentCulture); 
#endif
        } 
 
        [System.Security.SecuritySafeCritical]  // auto-generated
        [ComVisible(false)] 
        public Boolean StartsWith(String value, StringComparison comparisonType) {
            if( (Object)value == null) {
                throw new ArgumentNullException("value");
            } 

            if( comparisonType < StringComparison.CurrentCulture || comparisonType > StringComparison.OrdinalIgnoreCase) { 
                throw new ArgumentException(Environment.GetResourceString("NotSupported_StringComparison"), "comparisonType"); 
            }
            Contract.EndContractBlock(); 

            if( (Object)this == (Object)value) {
                return true;
            } 

            if( value.Length == 0) { 
                return true; 
            }
 
            switch (comparisonType) {
                case StringComparison.CurrentCulture:
                    return CultureInfo.CurrentCulture.CompareInfo.IsPrefix(this, value, CompareOptions.None);
 
                case StringComparison.CurrentCultureIgnoreCase:
                    return CultureInfo.CurrentCulture.CompareInfo.IsPrefix(this, value, CompareOptions.IgnoreCase); 
 
                case StringComparison.InvariantCulture:
                    return CultureInfo.InvariantCulture.CompareInfo.IsPrefix(this, value, CompareOptions.None); 

                case StringComparison.InvariantCultureIgnoreCase:
                    return CultureInfo.InvariantCulture.CompareInfo.IsPrefix(this, value, CompareOptions.IgnoreCase);
 
                case StringComparison.Ordinal:
                    if( this.Length < value.Length) { 
                        return false; 
                    }
                    return (nativeCompareOrdinalEx(this, 0, value, 0, value.Length) == 0); 

                case StringComparison.OrdinalIgnoreCase:
                    if( this.Length < value.Length) {
                        return false; 
                    }
 
                    return (TextInfo.CompareOrdinalIgnoreCaseEx(this, 0, value, 0, value.Length, value.Length) == 0); 

                default: 
                    throw new ArgumentException(Environment.GetResourceString("NotSupported_StringComparison"), "comparisonType");
            }
        }
 
        public Boolean StartsWith(String value, Boolean ignoreCase, CultureInfo culture) {
            if (null==value) { 
                throw new ArgumentNullException("value"); 
            }
            Contract.EndContractBlock(); 

            if((object)this == (object)value) {
                return true;
            } 

            CultureInfo referenceCulture; 
            if (culture == null) 
#if FEATURE_CORECLR
                referenceCulture = CultureInfo.InvariantCulture; 
#else
                referenceCulture = CultureInfo.CurrentCulture;
#endif
            else 
                referenceCulture = culture;
 
            return referenceCulture.CompareInfo.IsPrefix(this, value, ignoreCase ? CompareOptions.IgnoreCase : CompareOptions.None); 
        }
 
        // Creates a copy of this string in lower case.
        public String ToLower() {
            Contract.Ensures(Contract.Result<String>() != null);
            Contract.EndContractBlock(); 
#if FEATURE_CORECLR
            return this.ToLower(CultureInfo.InvariantCulture); 
#else 
            return this.ToLower(CultureInfo.CurrentCulture);
#endif 
        }

        // Creates a copy of this string in lower case.  The culture is set by culture.
#if !FEATURE_CORECLR 
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
#endif 
        public String ToLower(CultureInfo culture) { 
            if (culture==null) {
                throw new ArgumentNullException("culture"); 
            }
            Contract.Ensures(Contract.Result<String>() != null);
            Contract.EndContractBlock();
            return culture.TextInfo.ToLower(this); 
        }
 
        // Creates a copy of this string in lower case based on invariant culture. 
#if !FEATURE_CORECLR
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")] 
#endif
#if false
        private void TrimSrcHack() {}    // workaround to avoid unclosed "#if !FEATURE_CORECLR"
#endif 
        public String ToLowerInvariant() {
            Contract.Ensures(Contract.Result<String>() != null); 
            Contract.EndContractBlock(); 
            return this.ToLower(CultureInfo.InvariantCulture);
        } 

        // Creates a copy of this string in upper case.
        public String ToUpper() {
            Contract.Ensures(Contract.Result<String>() != null); 
            Contract.EndContractBlock();
#if FEATURE_CORECLR 
            return this.ToUpper(CultureInfo.InvariantCulture); 
#else
            return this.ToUpper(CultureInfo.CurrentCulture); 
#endif
        }

 
        // Creates a copy of this string in upper case.  The culture is set by culture.
#if !FEATURE_CORECLR 
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")] 
#endif
        public String ToUpper(CultureInfo culture) { 
            if (culture==null) {
                throw new ArgumentNullException("culture");
            }
            Contract.Ensures(Contract.Result<String>() != null); 
            Contract.EndContractBlock();
            return culture.TextInfo.ToUpper(this); 
        } 

 
        //Creates a copy of this string in upper case based on invariant culture.
#if !FEATURE_CORECLR
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
#endif 
#if false
        private void TrimSrcHack() {}    // workaround to avoid unclosed "#if !FEATURE_CORECLR" 
#endif 
        public String ToUpperInvariant() {
            Contract.Ensures(Contract.Result<String>() != null); 
            Contract.EndContractBlock();
            return this.ToUpper(CultureInfo.InvariantCulture);
        }
 

        // Returns this string. 
#if !FEATURE_CORECLR 
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
#endif 
        public override String ToString() {
            Contract.Ensures(Contract.Result<String>() != null);
            Contract.EndContractBlock();
            return this; 
        }
 
#if !FEATURE_CORECLR 
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
#endif 
        public String ToString(IFormatProvider provider) {
            Contract.Ensures(Contract.Result<String>() != null);
            Contract.EndContractBlock();
            return this; 
        }
 
        // Method required for the ICloneable interface. 
        // There's no point in cloning a string since they're immutable, so we simply return this.
        public Object Clone() { 
            Contract.Ensures(Contract.Result<Object>() != null);
            Contract.EndContractBlock();
            return this;
        } 

 
        // Trims the whitespace from both ends of the string.  Whitespace is defined by 
        // Char.IsWhiteSpace.
        // 
        public String Trim() {
            Contract.Ensures(Contract.Result<String>() != null);
            Contract.EndContractBlock();
 
            return TrimHelper(TrimBoth);
        } 
 

        [System.Security.SecuritySafeCritical]  // auto-generated 
        private String TrimHelper(int trimType) {
            //end will point to the first non-trimmed character on the right
            //start will point to the first non-trimmed character on the Left
            int end = this.Length-1; 
            int start=0;
 
            //Trim specified characters. 
            if (trimType !=TrimTail)  {
                for (start=0; start < this.Length; start++) { 
                    if (!Char.IsWhiteSpace(this[start])) break;
                }
            }
 
            if (trimType !=TrimHead) {
                for (end= Length -1; end >= start;  end--) { 
                    if (!Char.IsWhiteSpace(this[end])) break; 
                }
            } 

            return CreateTrimmedString(start, end);
        }
 

        [System.Security.SecuritySafeCritical]  // auto-generated 
        private String TrimHelper(char[] trimChars, int trimType) { 
            //end will point to the first non-trimmed character on the right
            //start will point to the first non-trimmed character on the Left 
            int end = this.Length-1;
            int start=0;

            //Trim specified characters. 
            if (trimType !=TrimTail)  {
                for (start=0; start < this.Length; start++) { 
                    int i = 0; 
                    char ch = this[start];
                    for( i = 0; i < trimChars.Length; i++) { 
                        if( trimChars[i] == ch) break;
                    }
                    if( i == trimChars.Length) { // the character is not white space
                        break; 
                    }
                } 
            } 

            if (trimType !=TrimHead) { 
                for (end= Length -1; end >= start;  end--) {
                    int i = 0;
                    char ch = this[end];
                    for(i = 0; i < trimChars.Length; i++) { 
                        if( trimChars[i] == ch) break;
                    } 
                    if( i == trimChars.Length) { // the character is not white space 
                        break;
                    } 
                }
            }

            return CreateTrimmedString(start, end); 
        }
 
 
        [System.Security.SecurityCritical]  // auto-generated
        private String CreateTrimmedString(int start, int end) { 
            //Create a new STRINGREF and initialize it from the range determined above.
            int len = end -start + 1;
            if (len == this.Length) {
                // Don't allocate a new string as the trimmed string has not changed. 
                return this;
            } 
            else { 
                if( len == 0) {
                    return String.Empty; 
                }
                return InternalSubString(start, len, false);
            }
        } 

        // 
        // 
        [System.Security.SecurityCritical]  // auto-generated
        [ResourceExposure(ResourceScope.None)] 
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        private extern String InsertInternal(int startIndex, String value);

        [System.Security.SecuritySafeCritical]  // auto-generated 
        public String Insert(int startIndex, String value)
        { 
            if (value == null) 
                throw new ArgumentNullException("value");
            if (startIndex < 0 || startIndex > this.Length) 
                throw new ArgumentOutOfRangeException("startIndex");
            Contract.Ensures(Contract.Result<String>() != null);
            Contract.Ensures(Contract.Result<String>().Length == this.Length + value.Length);
            Contract.EndContractBlock(); 

            return InsertInternal(startIndex, value); 
        } 

        // Replaces all instances of oldChar with newChar. 
        //
        [System.Security.SecuritySafeCritical]  // auto-generated
        [ResourceExposure(ResourceScope.None)]
        [MethodImplAttribute(MethodImplOptions.InternalCall)] 
        private extern String ReplaceInternal(char oldChar, char newChar);
 
        [System.Security.SecuritySafeCritical]  // auto-generated 
        public String Replace(char oldChar, char newChar)
        { 
            Contract.Ensures(Contract.Result<String>() != null);
            Contract.Ensures(Contract.Result<String>().Length == this.Length);
            Contract.EndContractBlock();
 
            return ReplaceInternal(oldChar, newChar);
        } 
 
        // This method contains the same functionality as StringBuilder Replace. The only difference is that
        // a new String has to be allocated since Strings are immutable 
        [System.Security.SecuritySafeCritical]  // auto-generated
        [ResourceExposure(ResourceScope.None)]
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        private extern String ReplaceInternal(String oldValue, String newValue); 

        [System.Security.SecuritySafeCritical]  // auto-generated 
#if !FEATURE_CORECLR 
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
#endif 
        public String Replace(String oldValue, String newValue)
        {
            if (oldValue == null)
                throw new ArgumentNullException("oldValue"); 
            // Note that if newValue is null, we treat it like String.Empty.
            Contract.Ensures(Contract.Result<String>() != null); 
            Contract.EndContractBlock(); 

            return ReplaceInternal(oldValue, newValue); 
        }

        //
        // 
        [System.Security.SecurityCritical]  // auto-generated
        [ResourceExposure(ResourceScope.None)] 
        [MethodImplAttribute(MethodImplOptions.InternalCall)] 
        private extern String RemoveInternal(int startIndex, int count);
 
        [System.Security.SecuritySafeCritical]  // auto-generated
        public String Remove(int startIndex, int count)
        {
            if (startIndex < 0) 
                throw new ArgumentOutOfRangeException("startIndex",
                    Environment.GetResourceString("ArgumentOutOfRange_StartIndex")); 
            Contract.Ensures(Contract.Result<String>() != null); 
            Contract.Ensures(Contract.Result<String>().Length == this.Length - count);
            Contract.EndContractBlock(); 
            return RemoveInternal(startIndex, count);
        }

        // a remove that just takes a startindex. 
        public string Remove( int startIndex ) {
            if (startIndex < 0) { 
                throw new ArgumentOutOfRangeException("startIndex", 
                        Environment.GetResourceString("ArgumentOutOfRange_StartIndex"));
            } 

            if (startIndex >= Length) {
                throw new ArgumentOutOfRangeException("startIndex",
                        Environment.GetResourceString("ArgumentOutOfRange_StartIndexLessThanLength")); 
            }
 
            Contract.Ensures(Contract.Result<String>() != null); 
            Contract.EndContractBlock();
 
            return Substring(0, startIndex);
        }

        public static String Format(String format, Object arg0) { 
            if (format == null)
                throw new ArgumentNullException("format"); 
            Contract.Ensures(Contract.Result<String>() != null); 
            Contract.EndContractBlock();
            return Format(null, format, new Object[] {arg0}); 
        }

        public static String Format(String format, Object arg0, Object arg1) {
            if (format == null) 
                throw new ArgumentNullException("format");
            Contract.Ensures(Contract.Result<String>() != null); 
            Contract.EndContractBlock(); 
            return Format(null, format, new Object[] {arg0, arg1});
        } 

        public static String Format(String format, Object arg0, Object arg1, Object arg2) {
            if (format == null)
                throw new ArgumentNullException("format"); 
            Contract.Ensures(Contract.Result<String>() != null);
            Contract.EndContractBlock(); 
 
            return Format(null, format, new Object[] {arg0, arg1, arg2});
        } 


        public static String Format(String format, params Object[] args) {
            if (format == null || args == null) 
                throw new ArgumentNullException((format == null) ? "format" : "args");
            Contract.Ensures(Contract.Result<String>() != null); 
            Contract.EndContractBlock(); 

            return Format(null, format, args); 
        }

        [System.Security.SecuritySafeCritical]  // auto-generated
        public static String Format( IFormatProvider provider, String format, params Object[] args) { 
            if (format == null || args == null)
                throw new ArgumentNullException((format == null) ? "format" : "args"); 
            Contract.Ensures(Contract.Result<String>() != null); 
            Contract.EndContractBlock();
 
            StringBuilder sb = new StringBuilder(format.Length + args.Length * 8);
            sb.AppendFormat(provider,format,args);
            return sb.ToString();
        } 

        [System.Security.SecuritySafeCritical]  // auto-generated 
        unsafe public static String Copy (String str) { 
            if (str==null) {
                throw new ArgumentNullException("str"); 
            }
            Contract.Ensures(Contract.Result<String>() != null);
            Contract.EndContractBlock();
 
            int length = str.Length;
 
            String result = FastAllocateString(length); 

            fixed(char* dest = &result.m_firstChar) 
                fixed(char* src = &str.m_firstChar) {
#if WIN32
                     wstrcpyPtrAligned(dest, src, length);
#else 
                     wstrcpy(dest, src, length);
#endif //WIN32 
             	 } 
             return result;
        } 

        public static String Concat(Object arg0) {
            Contract.Ensures(Contract.Result<String>() != null);
            Contract.EndContractBlock(); 

            if (arg0 == null) 
            { 
                return String.Empty;
            } 
            return arg0.ToString();
        }

        public static String Concat(Object arg0, Object arg1) { 
            Contract.Ensures(Contract.Result<String>() != null);
            Contract.EndContractBlock(); 
 
            if (arg0 == null)
            { 
                arg0 = String.Empty;
            }

            if (arg1==null) { 
                arg1 = String.Empty;
            } 
            return Concat(arg0.ToString(), arg1.ToString()); 
        }
 
        public static String Concat(Object arg0, Object arg1, Object arg2) {
            Contract.Ensures(Contract.Result<String>() != null);
            Contract.EndContractBlock();
 
            if (arg0 == null)
            { 
                arg0 = String.Empty; 
            }
 
            if (arg1==null) {
                arg1 = String.Empty;
            }
 
            if (arg2==null) {
                arg2 = String.Empty; 
            } 

            return Concat(arg0.ToString(), arg1.ToString(), arg2.ToString()); 
        }

        [System.Security.SecuritySafeCritical]  // auto-generated
        [CLSCompliant(false)] 
        public static String Concat(Object arg0, Object arg1, Object arg2, Object arg3, __arglist)
        { 
            Contract.Ensures(Contract.Result<String>() != null); 
            Contract.EndContractBlock();
 
            Object[]   objArgs;
            int        argCount;

            ArgIterator args = new ArgIterator(__arglist); 

            //+4 to account for the 4 hard-coded arguments at the beginning of the list. 
            argCount = args.GetRemainingCount() + 4; 

            objArgs = new Object[argCount]; 

            //Handle the hard-coded arguments
            objArgs[0] = arg0;
            objArgs[1] = arg1; 
            objArgs[2] = arg2;
            objArgs[3] = arg3; 
 
            //Walk all of the args in the variable part of the argument list.
            for (int i=4; i<argCount; i++) { 
                objArgs[i] = TypedReference.ToObject(args.GetNextArg());
            }

            return Concat(objArgs); 
        }
 
        public static String Concat(params Object[] args) { 
            if (args==null) {
                throw new ArgumentNullException("args"); 
            }
            Contract.Ensures(Contract.Result<String>() != null);
            Contract.EndContractBlock();
 
            String[] sArgs = new String[args.Length];
            int totalLength=0; 
 
            for (int i=0; i<args.Length; i++) {
                object value = args[i]; 
                sArgs[i] = ((value==null)?(String.Empty):(value.ToString()));
                if (sArgs[i] == null) sArgs[i] = String.Empty; // value.ToString() above could have returned null
                totalLength += sArgs[i].Length;
                // check for overflow 
                if (totalLength < 0) {
                    throw new OutOfMemoryException(); 
                } 
            }
            return ConcatArray(sArgs, totalLength); 
        }

        [ComVisible(false)]
        public static String Concat<T>(IEnumerable<T> values) { 
            if (values == null)
                throw new ArgumentNullException("values"); 
            Contract.Ensures(Contract.Result<String>() != null); 
            Contract.EndContractBlock();
 
            StringBuilder result = new StringBuilder();
            using(IEnumerator<T> en = values.GetEnumerator()) {
                while (en.MoveNext()) {
                    if (en.Current != null) { 
                        // handle the case that the enumeration has null entries
                        // and the case where their ToString() override is broken 
                        string value = en.Current.ToString(); 
                        if (value != null)
                            result.Append(value); 
                    }
                }
            }
            return result.ToString(); 
        }
 
 
        [ComVisible(false)]
        public static String Concat(IEnumerable<String> values) { 
            if (values == null)
                throw new ArgumentNullException("values");
            Contract.Ensures(Contract.Result<String>() != null);
            Contract.EndContractBlock(); 

            StringBuilder result = new StringBuilder(); 
            using(IEnumerator<String> en = values.GetEnumerator()) { 
                while (en.MoveNext()) {
                    if (en.Current != null) { 
                        result.Append(en.Current);
                    }
                }
            } 
            return result.ToString();
        } 
 

        [System.Security.SecuritySafeCritical]  // auto-generated 
        public static String Concat(String str0, String str1) {
            Contract.Ensures(Contract.Result<String>() != null);
            Contract.Ensures(Contract.Result<String>().Length ==
                (str0 == null ? 0 : str0.Length) + 
                (str1 == null ? 0 : str1.Length));
            Contract.EndContractBlock(); 
 
            if (IsNullOrEmpty(str0)) {
                if (IsNullOrEmpty(str1)) { 
                    return String.Empty;
                }
                return str1;
            } 

            if (IsNullOrEmpty(str1)) { 
                return str0; 
            }
 
            int str0Length = str0.Length;

            String result = FastAllocateString(str0Length + str1.Length);
 
            FillStringChecked(result, 0,        str0);
            FillStringChecked(result, str0Length, str1); 
 
            return result;
        } 

        [System.Security.SecuritySafeCritical]  // auto-generated
        public static String Concat(String str0, String str1, String str2) {
            Contract.Ensures(Contract.Result<String>() != null); 
            Contract.Ensures(Contract.Result<String>().Length ==
                (str0 == null ? 0 : str0.Length) + 
                (str1 == null ? 0 : str1.Length) + 
                (str2 == null ? 0 : str2.Length));
            Contract.EndContractBlock(); 

            if (str0==null && str1==null && str2==null) {
                return String.Empty;
            } 

            if (str0==null) { 
                str0 = String.Empty; 
            }
 
            if (str1==null) {
                str1 = String.Empty;
            }
 
            if (str2 == null) {
                str2 = String.Empty; 
            } 

            int totalLength = str0.Length + str1.Length + str2.Length; 

            String result = FastAllocateString(totalLength);
            FillStringChecked(result, 0, str0);
            FillStringChecked(result, str0.Length, str1); 
            FillStringChecked(result, str0.Length + str1.Length, str2);
 
            return result; 
        }
 
        [System.Security.SecuritySafeCritical]  // auto-generated
        public static String Concat(String str0, String str1, String str2, String str3) {
            Contract.Ensures(Contract.Result<String>() != null);
            Contract.Ensures(Contract.Result<String>().Length == 
                (str0 == null ? 0 : str0.Length) +
                (str1 == null ? 0 : str1.Length) + 
                (str2 == null ? 0 : str2.Length) + 
                (str3 == null ? 0 : str3.Length));
            Contract.EndContractBlock(); 

            if (str0==null && str1==null && str2==null && str3==null) {
                return String.Empty;
            } 

            if (str0==null) { 
                str0 = String.Empty; 
            }
 
            if (str1==null) {
                str1 = String.Empty;
            }
 
            if (str2 == null) {
                str2 = String.Empty; 
            } 

            if (str3 == null) { 
                str3 = String.Empty;
            }

            int totalLength = str0.Length + str1.Length + str2.Length + str3.Length; 

            String result = FastAllocateString(totalLength); 
            FillStringChecked(result, 0, str0); 
            FillStringChecked(result, str0.Length, str1);
            FillStringChecked(result, str0.Length + str1.Length, str2); 
            FillStringChecked(result, str0.Length + str1.Length + str2.Length, str3);

            return result;
        } 

        [System.Security.SecuritySafeCritical]  // auto-generated 
        private static String ConcatArray(String[] values, int totalLength) { 
            String result =  FastAllocateString(totalLength);
            int currPos=0; 

            for (int i=0; i<values.Length; i++) {
                Contract.Assert((currPos <= totalLength - values[i].Length),
                                "[String.ConcatArray](currPos <= totalLength - values[i].Length)"); 

                FillStringChecked(result, currPos, values[i]); 
                currPos+=values[i].Length; 
            }
 
            return result;
        }

        public static String Concat(params String[] values) { 
            if (values == null)
                throw new ArgumentNullException("values"); 
            Contract.Ensures(Contract.Result<String>() != null); 
            // Spec#: Consider a postcondition saying the length of this string == the sum of each string in array
            Contract.EndContractBlock(); 
            int totalLength=0;

            // Always make a copy to prevent changing the array on another thread.
            String[] internalValues = new String[values.Length]; 

            for (int i=0; i<values.Length; i++) { 
                string value = values[i]; 
                internalValues[i] = ((value==null)?(String.Empty):(value));
                totalLength += internalValues[i].Length; 
                // check for overflow
                if (totalLength < 0) {
                    throw new OutOfMemoryException();
                } 
            }
 
            return ConcatArray(internalValues, totalLength); 
        }
 
        [System.Security.SecuritySafeCritical]  // auto-generated
#if !FEATURE_CORECLR
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
#endif 
        public static String Intern(String str) {
            if (str==null) { 
                throw new ArgumentNullException("str"); 
            }
            Contract.Ensures(Contract.Result<String>().Length == str.Length); 
            Contract.Ensures(str.Equals(Contract.Result<String>()));
            Contract.EndContractBlock();

            return Thread.GetDomain().GetOrInternString(str); 
        }
 
        [System.Security.SecuritySafeCritical]  // auto-generated 
        public static String IsInterned(String str) {
            if (str==null) { 
                throw new ArgumentNullException("str");
            }
            Contract.Ensures(Contract.Result<String>() == null || Contract.Result<String>().Length == str.Length);
            Contract.EndContractBlock(); 

            return Thread.GetDomain().IsStringInterned(str); 
        } 

 
        //
        // IConvertible implementation
        //
 
#if !FEATURE_CORECLR
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")] 
#endif 
        public TypeCode GetTypeCode() {
            return TypeCode.String; 
        }

        /// <internalonly/>
        bool IConvertible.ToBoolean(IFormatProvider provider) { 
            return Convert.ToBoolean(this, provider);
        } 
 
        /// <internalonly/>
        char IConvertible.ToChar(IFormatProvider provider) { 
            return Convert.ToChar(this, provider);
        }

        /// <internalonly/> 
        [System.Security.SecuritySafeCritical]  // auto-generated
        sbyte IConvertible.ToSByte(IFormatProvider provider) { 
            return Convert.ToSByte(this, provider); 
        }
 
        /// <internalonly/>
        [System.Security.SecuritySafeCritical]  // auto-generated
        byte IConvertible.ToByte(IFormatProvider provider) {
            return Convert.ToByte(this, provider); 
        }
 
        /// <internalonly/> 
        [System.Security.SecuritySafeCritical]  // auto-generated
        short IConvertible.ToInt16(IFormatProvider provider) { 
            return Convert.ToInt16(this, provider);
        }

        /// <internalonly/> 
        [System.Security.SecuritySafeCritical]  // auto-generated
        ushort IConvertible.ToUInt16(IFormatProvider provider) { 
            return Convert.ToUInt16(this, provider); 
        }
 
        /// <internalonly/>
        [System.Security.SecuritySafeCritical]  // auto-generated
        int IConvertible.ToInt32(IFormatProvider provider) {
            return Convert.ToInt32(this, provider); 
        }
 
        /// <internalonly/> 
        [System.Security.SecuritySafeCritical]  // auto-generated
        uint IConvertible.ToUInt32(IFormatProvider provider) { 
            return Convert.ToUInt32(this, provider);
        }

        /// <internalonly/> 
        [System.Security.SecuritySafeCritical]  // auto-generated
        long IConvertible.ToInt64(IFormatProvider provider) { 
            return Convert.ToInt64(this, provider); 
        }
 
        /// <internalonly/>
        [System.Security.SecuritySafeCritical]  // auto-generated
        ulong IConvertible.ToUInt64(IFormatProvider provider) {
            return Convert.ToUInt64(this, provider); 
        }
 
        /// <internalonly/> 
        [System.Security.SecuritySafeCritical]  // auto-generated
        float IConvertible.ToSingle(IFormatProvider provider) { 
            return Convert.ToSingle(this, provider);
        }

        /// <internalonly/> 
        [System.Security.SecuritySafeCritical]  // auto-generated
        double IConvertible.ToDouble(IFormatProvider provider) { 
            return Convert.ToDouble(this, provider); 
        }
 
        /// <internalonly/>
        [System.Security.SecuritySafeCritical]  // auto-generated
        Decimal IConvertible.ToDecimal(IFormatProvider provider) {
            return Convert.ToDecimal(this, provider); 
        }
 
        /// <internalonly/> 
        [System.Security.SecuritySafeCritical]  // auto-generated
        DateTime IConvertible.ToDateTime(IFormatProvider provider) { 
            return Convert.ToDateTime(this, provider);
        }

        /// <internalonly/> 
        Object IConvertible.ToType(Type type, IFormatProvider provider) {
            return Convert.DefaultToType((IConvertible)this, type, provider); 
        } 

 
        // Is this a string that can be compared quickly (that is it has only characters > 0x80
        // and not a - or '
        [System.Security.SecurityCritical]  // auto-generated
        [ResourceExposure(ResourceScope.None)] 
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        internal extern bool IsFastSort(); 
        // Is this a string that only contains characters < 0x80. 
        [System.Security.SecurityCritical]  // auto-generated
        [ResourceExposure(ResourceScope.None)] 
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        internal extern bool IsAscii();

        // Set extra byte for odd-sized strings that came from interop as BSTR. 
        [System.Security.SecurityCritical]
        [ResourceExposure(ResourceScope.None)] 
        [MethodImplAttribute(MethodImplOptions.InternalCall)] 
        internal extern void SetTrailByte(byte data);
        // Try to retrieve the extra byte - returns false if not present. 
        [System.Security.SecurityCritical]
        [ResourceExposure(ResourceScope.None)]
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        internal extern bool TryGetTrailByte(out byte data); 

#if !FEATURE_CORECLR 
        public CharEnumerator GetEnumerator() { 
            Contract.Ensures(Contract.Result<CharEnumerator>() != null);
            Contract.EndContractBlock(); 
            BCLDebug.Perf(false, "Avoid using String's CharEnumerator until C# special cases foreach on String - use the indexed property on String instead.");
            return new CharEnumerator(this);
        }
#endif // !FEATURE_CORECLR 

        IEnumerator<char> IEnumerable<char>.GetEnumerator() { 
            Contract.Ensures(Contract.Result<IEnumerator<char>>() != null); 
            Contract.EndContractBlock();
            BCLDebug.Perf(false, "Avoid using String's CharEnumerator until C# special cases foreach on String - use the indexed property on String instead."); 
            return new CharEnumerator(this);
        }

        /// <internalonly/> 
        IEnumerator IEnumerable.GetEnumerator() {
            Contract.Ensures(Contract.Result<IEnumerator>() != null); 
            Contract.EndContractBlock(); 
            BCLDebug.Perf(false, "Avoid using String's CharEnumerator until C# special cases foreach on String - use the indexed property on String instead.");
            return new CharEnumerator(this); 
        }

         // Copies the source String (byte buffer) to the destination IntPtr memory allocated with len bytes.
        [System.Security.SecurityCritical]  // auto-generated 
        #if !FEATURE_CORECLR
        [System.Runtime.ForceTokenStabilization] 
        #endif //!FEATURE_CORECLR 
        internal unsafe static void InternalCopy(String src, IntPtr dest,int len)
        { 
            if (len == 0)
                return;
            fixed(char* charPtr = &src.m_firstChar) {
                byte* srcPtr = (byte*) charPtr; 
                byte* dstPtr = (byte*) dest.ToPointer();
                Buffer.memcpyimpl(srcPtr, dstPtr, len); 
            } 
        }
    } 

    [ComVisible(false)]
    [Flags]
    public enum StringSplitOptions { 
        None = 0,
        RemoveEmptyEntries = 1 
    } 
}

// File provided for Reference Use Only by Microsoft Corporation (c) 2007.
