using System;
using Services;
using Xunit;

public class SchedulerTests
{
    [Fact]
    public void NextOccurrence_Is_In_Future()
    {
        var tz = TimeZoneInfo.FindSystemTimeZoneById("America/Denver");
        var now = DateTimeOffset.UtcNow;
        var next = SchedulerService.NextOccurrence(tz, 8, 0, now);
        Assert.True(next > now);
        Assert.True((next - now).TotalHours <= 24.1);
    }
}
