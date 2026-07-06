namespace ToddlerCoder;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
        bool kioskMode = !Environment.GetCommandLineArgs()
            .Skip(1)
            .Any(arg => string.Equals(arg, "--debug-windowed", StringComparison.OrdinalIgnoreCase));

        Application.Run(new Form1(kioskMode));
    }    
}
