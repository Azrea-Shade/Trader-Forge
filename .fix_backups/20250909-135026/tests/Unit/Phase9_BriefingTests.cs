namespace Unit
{
    public class Phase9_BriefingTests
    {
        [Fact]
        public void Brief_service_type_and_generate_signature_exist()
        {
            // Allow either renamed or legacy type
            var svcType =
                Type.GetType("Services.BriefService, Services") ??
                Type.GetType("Services.BriefingService, Services");
            Assert.NotNull(svcType);

            // Accept either new Generate(DateOnly, string[]) or legacy/empty variants
            bool hasGenerate =
                svcType!.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
                    .Any(m =>
                    {
                        if (m.Name != "Generate") return false;
                        var p = m.GetParameters();
                        if (p.Length == 2)
                        {
                            // new form: (DateOnly, string[])
                            return p[0].ParameterType.Name == "DateOnly"
                                   && p[1].ParameterType == typeof(string[]);
                        }
                        // tolerate legacy no-arg form
                        return p.Length == 0;
                    });

            Assert.True(hasGenerate, $"Generate(...) method not found on {svcType.FullName}");
        }
    }
}
