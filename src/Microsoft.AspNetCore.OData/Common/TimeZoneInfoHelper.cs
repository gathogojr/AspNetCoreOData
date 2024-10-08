//-----------------------------------------------------------------------------
// <copyright file="TimeZoneInfoHelper.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using System;

namespace Microsoft.AspNetCore.OData.Common;

internal class TimeZoneInfoHelper
{
    public static DateTimeOffset ConvertToDateTimeOffset(DateTime dateTime)
    {
        return ConvertToDateTimeOffset(dateTime, TimeZoneInfo.Local);
    }

    public static DateTimeOffset ConvertToDateTimeOffset(DateTime dateTime, TimeZoneInfo timeZone)
    {
        if (timeZone == null)
        {
            timeZone = TimeZoneInfo.Local;
        }

        TimeSpan utcOffset = timeZone.GetUtcOffset(dateTime);
        if (utcOffset >= TimeSpan.Zero)
        {
            if (dateTime <= DateTime.MinValue + utcOffset)
            {
                return DateTimeOffset.MinValue;
            }
        }
        else
        {
            if (dateTime >= DateTime.MaxValue + utcOffset)
            {
                return DateTimeOffset.MaxValue;
            }
        }

        if (dateTime.Kind == DateTimeKind.Local)
        {
            TimeZoneInfo localTimeZoneInfo = TimeZoneInfo.Local;
            TimeSpan localTimeSpan = localTimeZoneInfo.GetUtcOffset(dateTime);
            if (localTimeSpan < TimeSpan.Zero)
            {
                if (dateTime >= DateTime.MaxValue + localTimeSpan)
                {
                    return DateTimeOffset.MaxValue;
                }
            }
            else
            {
                if (dateTime <= DateTime.MinValue + localTimeSpan)
                {
                    return DateTimeOffset.MinValue;
                }
            }

            return TimeZoneInfo.ConvertTime(new DateTimeOffset(dateTime), timeZone);
        }

        if (dateTime.Kind == DateTimeKind.Utc)
        {
            return TimeZoneInfo.ConvertTime(new DateTimeOffset(dateTime), timeZone);
        }

        return new DateTimeOffset(dateTime, timeZone.GetUtcOffset(dateTime));
    }
}
