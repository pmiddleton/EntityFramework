// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    /// Provides common language runtime (CLR) methods that expose database functions
    /// for use in <see cref="DbContext" /> LINQ to Entities queries.
    /// </summary>
    public static class DbFunctionsExtensions
    {
        /// <summary>
        /// When used as part of a LINQ to Entities query, this method returns a given
        /// number of the leftmost characters in a string.
        /// </summary>
        /// <remarks>
        /// You cannot call this function directly. This function can only appear within a LINQ to Entities query.
        /// This function is translated to a corresponding function in the database.
        /// </remarks>
        /// <param name="func">The dbFunction class</param>
        /// <param name="stringArgument"> The input string. </param>
        /// <param name="length"> The number of characters to return </param>
        /// <returns> A string containing the number of characters asked for from the left of the input string. </returns>
        public static String Left([NotNull] this DbFunctions func, [CanBeNull] String stringArgument, long? length)
        {
            return length == null ? null : stringArgument?.Substring(0, (int)length.Value);
        }

        /// <summary>
        /// When used as part of a LINQ to Entities query, this method returns a given
        /// number of the rightmost characters in a string.
        /// </summary>
        /// <remarks>
        /// You cannot call this function directly. This function can only appear within a LINQ to Entities query.
        /// This function is translated to a corresponding function in the database.
        /// </remarks>
        /// <param name="func"> The dbFunction class </param>
        /// <param name="stringArgument"> The input string. </param>
        /// <param name="length"> The number of characters to return </param>
        /// <returns> A string containing the number of characters asked for from the right of the input string. </returns>
        public static String Right([NotNull] this DbFunctions func, [CanBeNull] String stringArgument, long? length)
        {
            return length == null ? null : stringArgument?.Substring(stringArgument.Length - (int)length.Value);
        }

        /// <summary>
        /// When used as part of a LINQ to Entities query, this method returns a given
        /// string with the order of the characters reversed.
        /// </summary>
        /// <remarks>
        /// You cannot call this function directly. This function can only appear within a LINQ to Entities query.
        /// This function is translated to a corresponding function in the database.
        /// </remarks>
        /// <param name="func"> The dbFunction class </param>
        /// <param name="stringArgument"> The input string. </param>
        /// <returns> The input string with the order of the characters reversed. </returns>
        public static String Reverse([NotNull] this DbFunctions func, [CanBeNull] String stringArgument)
        {
            var chars = stringArgument.ToCharArray();
            Array.Reverse(chars);
            return new string(chars);
        }

        /// <summary>
        /// When used as part of a LINQ to Entities query, this method returns
        /// the given date with the time portion cleared.
        /// </summary>
        /// <remarks>
        /// You cannot call this function directly. This function can only appear within a LINQ to Entities query.
        /// This function is translated to a corresponding function in the database.
        /// </remarks>
        /// <param name="func"> The dbFunction class </param>
        /// <param name="dateValue"> The date/time value to use. </param>
        /// <returns> The input date with the time portion cleared. </returns>
        public static DateTime? TruncateTime([NotNull] this DbFunctions func, DateTime? dateValue)
        {
            return dateValue?.Date;
        }

        /// <summary>
        /// When used as part of a LINQ to Entities query, this method 
        /// adds the given number of years to a date/time.
        /// </summary>
        /// <remarks>
        /// You cannot call this function directly. This function can only appear within a LINQ to Entities query.
        /// This function is translated to a corresponding function in the database.
        /// </remarks>
        /// <param name="func"> The dbFunction class </param>
        /// <param name="dateValue"> The input date/time. </param>
        /// <param name="addValue"> The number of years to add. </param>
        /// <returns> A resulting date/time. </returns>
        public static DateTime? AddYears([NotNull] this DbFunctions func, DateTime? dateValue, int? addValue)
        {
            return addValue == null ? null : dateValue?.AddYears(addValue.Value);
        }

        /// <summary>
        /// When used as part of a LINQ to Entities query, this method 
        /// adds the given number of months to a date/time.
        /// </summary>
        /// <remarks>
        /// You cannot call this function directly. This function can only appear within a LINQ to Entities query.
        /// This function is translated to a corresponding function in the database.
        /// </remarks>
        /// <param name="func"> The dbFunction class </param>
        /// <param name="dateValue"> The input date/time. </param>
        /// <param name="addValue"> The number of months to add. </param>
        /// <returns> A resulting date/time. </returns>
        public static DateTime? AddMonths([NotNull] this DbFunctions func, DateTime? dateValue, int? addValue)
        {
            return addValue == null ? null : dateValue?.AddMonths(addValue.Value);
        }

        /// <summary>
        /// When used as part of a LINQ to Entities query, this method
        /// adds the given number of days to a date/time.
        /// </summary>
        /// <remarks>
        /// You cannot call this function directly. This function can only appear within a LINQ to Entities query.
        /// This function is translated to a corresponding function in the database.
        /// </remarks>
        /// <param name="func"> The dbFunction class </param>
        /// <param name="dateValue"> The input date/time. </param>
        /// <param name="addValue"> The number of days to add. </param>
        /// <returns> A resulting date/time. </returns>
        public static DateTime? AddDays([NotNull] this DbFunctions func, DateTime? dateValue, int? addValue)
        {
            return addValue == null ? null : dateValue?.AddDays((double)addValue);
        }

        /// <summary>
        /// When used as part of a LINQ to Entities query, this method
        /// adds the given number of hours to a date/time.
        /// </summary>
        /// <remarks>
        /// You cannot call this function directly. This function can only appear within a LINQ to Entities query.
        /// This function is translated to a corresponding function in the database.
        /// </remarks>
        /// <param name="func"> The dbFunction class </param>
        /// <param name="timeValue"> The input date/time. </param>
        /// <param name="addValue"> The number of hours to add. </param>
        /// <returns> A resulting date/time. </returns>
        public static DateTime? AddHours([NotNull] this DbFunctions func, DateTime? timeValue, int? addValue)
        {
            return addValue == null ? null : timeValue?.AddHours((double)addValue);
        }

        /// <summary>
        /// When used as part of a LINQ to Entities query, this method 
        /// adds the given number of minutes to a date/time.
        /// </summary>
        /// <remarks>
        /// You cannot call this function directly. This function can only appear within a LINQ to Entities query.
        /// This function is translated to a corresponding function in the database.
        /// </remarks>
        /// <param name="func"> The dbFunction class </param>
        /// <param name="timeValue"> The input date/time. </param>
        /// <param name="addValue"> The number of minutes to add. </param>
        /// <returns> A resulting date/time. </returns>
        public static DateTime? AddMinutes([NotNull] this DbFunctions func, DateTime? timeValue, int? addValue)
        {
            return addValue == null ? null : timeValue?.AddMinutes((double)addValue);
        }

        /// <summary>
        /// When used as part of a LINQ to Entities query, this method 
        /// adds the given number of seconds to a date/time.
        /// </summary>
        /// <remarks>
        /// You cannot call this function directly. This function can only appear within a LINQ to Entities query.
        /// This function is translated to a corresponding function in the database.
        /// </remarks>
        /// <param name="func"> The dbFunction class </param>
        /// <param name="timeValue"> The input date/time. </param>
        /// <param name="addValue"> The number of seconds to add. </param>
        /// <returns> A resulting date/time. </returns>
        public static DateTime? AddSeconds([NotNull] this DbFunctions func, DateTime? timeValue, int? addValue)
        {
            return addValue == null ? null : timeValue?.AddSeconds((double)addValue);
        }

        /// <summary>
        /// When used as part of a LINQ to Entities query, this method 
        /// adds the given number of milliseconds to a date/time.
        /// </summary>
        /// <remarks>
        /// You cannot call this function directly. This function can only appear within a LINQ to Entities query.
        /// This function is translated to a corresponding function in the database.
        /// </remarks>
        /// <param name="func"> The dbFunction class </param>
        /// <param name="timeValue"> The input date/time. </param>
        /// <param name="addValue"> The number of milliseconds to add. </param>
        /// <returns> A resulting date/time. </returns>
        public static DateTime? AddMilliseconds([NotNull] this DbFunctions func, DateTime? timeValue, int? addValue)
        {
            return addValue == null ? null : timeValue?.AddMilliseconds((double)addValue);
        }

        /// <summary>
        /// When used as part of a LINQ to Entities query, this method 
        /// calculates the number of years between two date/times.
        /// </summary>
        /// <remarks>
        /// You cannot call this function directly. This function can only appear within a LINQ to Entities query.
        /// This function is translated to a corresponding function in the database.
        /// </remarks>
        /// <param name="func"> The dbFunction class </param>
        /// <param name="dateValue1"> The first date/time. </param>
        /// <param name="dateValue2"> The second date/time. </param>
        /// <returns> The number of years between the first and second date/times. </returns>
        public static int? DiffYears([NotNull] this DbFunctions func, DateTime? dateValue1, DateTime? dateValue2)
        {
            return (int?)(dateValue2?.Year - dateValue1?.Year);
        }

        /// <summary>
        /// When used as part of a LINQ to Entities query, this method 
        /// calculates the number of months between two date/times.
        /// </summary>
        /// <remarks>
        /// You cannot call this function directly. This function can only appear within a LINQ to Entities query.
        /// This function is translated to a corresponding function in the database.
        /// </remarks>
        /// <param name="func"> The dbFunction class </param>
        /// <param name="dateValue1"> The first date/time. </param>
        /// <param name="dateValue2"> The second date/time. </param>
        /// <returns> The number of months between the first and second date/times. </returns>
        public static int? DiffMonths([NotNull] this DbFunctions func, DateTime? dateValue1, DateTime? dateValue2)
        {
            return (dateValue2?.Month - dateValue1?.Month) + 12 * (dateValue2?.Year - dateValue1?.Year);
        }

        /// <summary>
        /// When used as part of a LINQ to Entities query, this method 
        /// calculates the number of days between two date/times.
        /// </summary>
        /// <remarks>
        /// You cannot call this function directly. This function can only appear within a LINQ to Entities query.
        /// This function is translated to a corresponding function in the database.
        /// </remarks>
        /// <param name="func"> The dbFunction class </param>
        /// <param name="dateValue1"> The first date/time. </param>
        /// <param name="dateValue2"> The second date/time. </param>
        /// <returns> The number of days between the first and second date/times. </returns>
        public static int? DiffDays([NotNull] this DbFunctions func, DateTime? dateValue1, DateTime? dateValue2)
        {
            return (int?)(dateValue2 - dateValue1)?.TotalDays;
        }

        /// <summary>
        /// When used as part of a LINQ to Entities query, this method 
        /// calculates the number of hours between two date/times.
        /// </summary>
        /// <remarks>
        /// You cannot call this function directly. This function can only appear within a LINQ to Entities query.
        /// This function is translated to a corresponding function in the database.
        /// </remarks>
        /// <param name="func"> The dbFunction class </param>
        /// <param name="timeValue1"> The first date/time. </param>
        /// <param name="timeValue2"> The second date/time. </param>
        /// <returns> The number of hours between the first and second date/times. </returns>
        public static int? DiffHours([NotNull] this DbFunctions func, DateTime? timeValue1, DateTime? timeValue2)
        {
            return (int?)(timeValue2 - timeValue1)?.TotalHours;
        }

        /// <summary>
        /// When used as part of a LINQ to Entities query, this method 
        /// calculates the number of minutes between two date/times.
        /// </summary>
        /// <remarks>
        /// You cannot call this function directly. This function can only appear within a LINQ to Entities query.
        /// This function is translated to a corresponding function in the database.
        /// </remarks>
        /// <param name="func"> The dbFunction class </param>
        /// <param name="timeValue1"> The first date/time. </param>
        /// <param name="timeValue2"> The second date/time. </param>
        /// <returns> The number of minutes between the first and second date/times. </returns>
        public static int? DiffMinutes([NotNull] this DbFunctions func, DateTime? timeValue1, DateTime? timeValue2)
        {
            return (int?)(timeValue2 - timeValue1)?.TotalMinutes;
        }

        /// <summary>
        /// When used as part of a LINQ to Entities query, this method 
        /// calculates the number of seconds between two date/times.
        /// </summary>
        /// <remarks>
        /// You cannot call this function directly. This function can only appear within a LINQ to Entities query.
        /// This function is translated to a corresponding function in the database.
        /// </remarks>
        /// <param name="func"> The dbFunction class </param>
        /// <param name="timeValue1"> The first date/time. </param>
        /// <param name="timeValue2"> The second date/time. </param>
        /// <returns> The number of seconds between the first and second date/times. </returns>
        public static int? DiffSeconds([NotNull] this DbFunctions func, DateTime? timeValue1, DateTime? timeValue2)
        {
            return (int?)(timeValue2 - timeValue1)?.TotalSeconds;
        }

        /// <summary>
        /// When used as part of a LINQ to Entities query, this method 
        /// calculates the number of milliseconds between two date/times.
        /// </summary>
        /// <remarks>
        /// You cannot call this function directly. This function can only appear within a LINQ to Entities query.
        /// This function is translated to a corresponding function in the database.
        /// </remarks>
        /// <param name="func"> The dbFunction class </param>
        /// <param name="timeValue1"> The first date/time. </param>
        /// <param name="timeValue2"> The second date/time. </param>
        /// <returns> The number of milliseconds between the first and second date/times. </returns>
        public static int? DiffMilliseconds([NotNull] this DbFunctions func, DateTime? timeValue1, DateTime? timeValue2)
        {
            return (int?)(timeValue2 - timeValue1)?.TotalMilliseconds;
        }

        /// <summary>
        /// When used as part of a LINQ to Entities query, this method 
        /// calculates the number of microseconds between two date/times.
        /// </summary>
        /// <remarks>
        /// You cannot call this function directly. This function can only appear within a LINQ to Entities query.
        /// This function is translated to a corresponding function in the database.
        /// </remarks>
        /// <param name="func"> The dbFunction class </param>
        /// <param name="timeValue1"> The first date/time. </param>
        /// <param name="timeValue2"> The second date/time. </param>
        /// <returns> The number of microseconds between the first and second date/times. </returns>
        public static int? DiffMicroseconds([NotNull] this DbFunctions func, DateTime? timeValue1, DateTime? timeValue2)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// When used as part of a LINQ to Entities query, this method 
        /// calculates the number of nanoseconds between two date/times.
        /// </summary>
        /// <remarks>
        /// You cannot call this function directly. This function can only appear within a LINQ to Entities query.
        /// This function is translated to a corresponding function in the database.
        /// </remarks>
        /// <param name="func"> The dbFunction class </param>
        /// <param name="timeValue1"> The first date/time. </param>
        /// <param name="timeValue2"> The second date/time. </param>
        /// <returns> The number of nanoseconds between the first and second date/times. </returns>
        public static int? DiffNanoseconds([NotNull] this DbFunctions func, DateTime? timeValue1, DateTime? timeValue2)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// When used as part of a LINQ to Entities query, this method 
        /// truncates the given value to the number of specified digits.
        /// </summary>
        /// <remarks>
        /// You cannot call this function directly. This function can only appear within a LINQ to Entities query.
        /// This function is translated to a corresponding function in the database.
        /// </remarks>
        /// <param name="func"> The dbFunction class </param>
        /// <param name="value"> The value to truncate. </param>
        /// <param name="digits"> The number of digits to preserve. </param>
        /// <returns> The truncated value. </returns>
        public static double? Truncate([NotNull] this DbFunctions func, Double? value, int? digits)
        {
            if (value == null || digits == null)
                return null;

            var places = Math.Pow(10, digits.Value);

            return Math.Truncate(value.Value * places) / places;
        }

        /// <summary>
        /// When used as part of a LINQ to Entities query, this method 
        /// truncates the given value to the number of specified digits.
        /// </summary>
        /// <remarks>
        /// You cannot call this function directly. This function can only appear within a LINQ to Entities query.
        /// This function is translated to a corresponding function in the database.
        /// </remarks>
        /// <param name="func"> The dbFunction class </param>
        /// <param name="value"> The value to truncate. </param>
        /// <param name="digits"> The number of digits to preserve. </param>
        /// <returns> The truncated value. </returns>
        public static decimal? Truncate([NotNull] this DbFunctions func, Decimal? value, int? digits)
        {
            if (value == null || digits == null)
                return null;

            var places = Math.Pow(10, digits.Value);

            return Math.Truncate(value.Value * (int)places) / (int)places;
        }
    }
}
