using System;
using System.Threading;
using Services;
using Xunit;

namespace Unit
{
    /// <summary>
    /// Minimal manual clock for tests. Matches Services.IClock contract.
    /// </summary>
    public sealed class ManualClock : IClock
    {
        public DateTimeOffset Now { get; private set; }

        public ManualClock(DateTimeOffset now) => Now = now;

        public void Set(DateTimeOffset now) => Now = now;

        public void Advance(TimeSpan delta) => Now = Now + delta;
    }

    public class Phase6_SchedulingTests
    {
        [Fact]
        public void ManualClock_Exposes_Now_As_DateTimeOffset()
        {
            var t0 = new DateTimeOffset(2025, 1, 1, 9, 0, 0, TimeSpan.Zero);
            var clock = new ManualClock(t0);
            Assert.Equal(t0, clock.Now);

            clock.Advance(TimeSpan.FromMinutes(5));
            Assert.Equal(t0 + TimeSpan.FromMinutes(5), clock.Now);
        }

        // Add more scheduler tests later; this keeps CI compiling.
        [Fact]
        public void Placeholder_Passes()
        {
            Assert.True(true);
        }
    }
}
