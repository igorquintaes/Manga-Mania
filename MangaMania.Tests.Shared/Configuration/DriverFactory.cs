using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace MangaMania.Tests.Shared.Configuration
{
    public static class DriverFactory
    {
        public static IWebDriver Create(string driverName)
        {
            switch (driverName)
            {
                case nameof(OpenQA.Selenium.Chrome.ChromeDriver):
                    return ChromeDriver();
                case nameof(OpenQA.Selenium.Firefox.FirefoxDriver):
                    return FirefoxDriver();
                case null:
                case "":
                    return null;
                default:
                    throw new ArgumentException();
            }
        }

        private static ChromeDriver ChromeDriver()
        {
            var seleniumFolder =
                RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "win32"
              : RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "linux64"
              : RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "mac64"
              : throw new ArgumentException("SO not supported");

            var driverLocation = GetNugetPackageDir("Selenium.WebDriver.ChromeDriver", "driver", seleniumFolder);
            var driverService = ChromeDriverService.CreateDefaultService(driverLocation);
            driverService.HideCommandPromptWindow = false;

            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("--headless");

            return new ChromeDriver(driverService, chromeOptions);
        }

        private static FirefoxDriver FirefoxDriver()
        {
            const string winX86Path = @"C:\Program Files (x86)\Mozilla Firefox\firefox.exe";
            const string winPath = @"C:\Program Files\Mozilla Firefox\firefox.exe";
            const string unixPath = @"/usr/bin/firefox/firefox-bin";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && File.Exists(winX86Path))
                SetUpDriverService(winX86Path, "win32");
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && File.Exists(winPath) && Architecture.X64 == RuntimeInformation.OSArchitecture)
                SetUpDriverService(winPath, "win64");
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && File.Exists(winPath) && Architecture.X86 == RuntimeInformation.OSArchitecture)
                SetUpDriverService(winPath, "win32");
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && File.Exists(unixPath))
                SetUpDriverService(unixPath, "linux64");
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) && File.Exists(unixPath))
                SetUpDriverService(unixPath, "mac64");
            else
                throw new Exception("Firefox not found installed or invalid SO");

            var firefoxOptions = new FirefoxOptions();
            firefoxOptions.AddArgument("--headless");

            FirefoxDriverService driverService;
            return new FirefoxDriver(driverService, firefoxOptions);

            void SetUpDriverService(string path, string driverFolder)
            {
                var driverLocation = GetNugetPackageDir("Selenium.WebDriver.GeckoDriver", "driver", driverFolder);
                driverService = FirefoxDriverService.CreateDefaultService(driverLocation);
                driverService.FirefoxBinaryPath = path;
            }
        }

        private static string GetNugetPackageDir(string packageName, params string[] dirs)
        {
            var expandedDirs = Environment.ExpandEnvironmentVariables(GetNugetPackageDir());
            var packageDirs = Path.Combine(expandedDirs, "packages", packageName);
            var lastVersion = Directory.EnumerateDirectories(packageDirs)
                    .Select(p => new Version(Path.GetFileName(p)))
                    .Max();

            return Path.Combine(
                packageDirs,
                lastVersion.ToString(),
                Path.Combine(dirs));
        }

        private static string GetNugetPackageDir() =>
            Path.Combine(Environment.GetEnvironmentVariable("USERPROFILE")
            ?? Environment.GetEnvironmentVariable("HOME"), ".nuget");
    }
}
