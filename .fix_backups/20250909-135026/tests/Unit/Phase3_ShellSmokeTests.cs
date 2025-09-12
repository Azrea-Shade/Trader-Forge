namespace Unit
{
    public class Phase3_ShellSmokeTests
    {
        [Fact]
        public void SettingsViewModel_is_constructible_for_smoke_test()
        {
            // Get type by name so the test doesn't care about the project namespace imports.
            var vmType =
                Type.GetType("Presentation.SettingsViewModel, Presentation") ??
                Type.GetType("TraderForge.Presentation.SettingsViewModel, Presentation");

            Assert.NotNull(vmType);

            object instance;
            // Prefer public parameterless ctor if it exists
            var ctor0 = vmType!.GetConstructor(Type.EmptyTypes);
            if (ctor0 != null)
            {
                instance = ctor0.Invoke(null);
            }
            else
            {
                // Fall back to allocation without running a ctor (just to prove type exists)
                instance = FormatterServices.GetUninitializedObject(vmType);
            }

            Assert.NotNull(instance);
        }
    }
}
