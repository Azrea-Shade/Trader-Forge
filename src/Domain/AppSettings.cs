namespace Domain
{
    public class AppSettings
    {
        public string PrimaryEmail { get; set; } = "shade.azrea@gmail.com";
        public string SecondaryEmail { get; set; } = "roccilhowe@gmail.com";
        public string TimeZoneId { get; set; } = "America/Denver";
        public int CompileHour { get; set; } = 7;
        public int CompileMinute { get; set; } = 30;
        public int DeliverHour { get; set; } = 8;
        public int DeliverMinute { get; set; } = 0;
        public bool AutostartOnLogin { get; set; } = true;
    }
}
