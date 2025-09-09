public class Phase2_SchedulerCoreTests
{
    [Fact]
    public void Computes_next_times_today_or_tomorrow()
    {
        var tz = TimeZoneInfo.Local;
        var now = new DateTime(2025, 1, 1, 7, 0, 0);

        var (gen, noti) = SchedulerCore.NextTimes("07:30", "08:00", now, tz);

        gen.Should().Be(new DateTime(2025, 1, 1, 7, 30, 0));
        noti.Should().Be(new DateTime(2025, 1, 1, 8, 0, 0));
    }

    [Fact]
    public void Rolls_to_tomorrow_if_past_time()
    {
        var tz = TimeZoneInfo.Local;
        var now = new DateTime(2025, 1, 1, 9, 0, 0);

        var (gen, noti) = SchedulerCore.NextTimes("07:30", "08:00", now, tz);

        gen.Should().Be(new DateTime(2025, 1, 2, 7, 30, 0));
        noti.Should().Be(new DateTime(2025, 1, 2, 8, 0, 0));
    }
}
