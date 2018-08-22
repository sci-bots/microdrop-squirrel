using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using Squirrel;
using Splat;
using NuGet;


namespace MicroDrop
{
    public class FileLogger : Splat.ILogger, IDisposable
    {
        StreamWriter output;
        string UUID;

        public FileLogger(string logPath)
        {
            this.output = new StreamWriter(logPath, append: true);
            this.UUID = Guid.NewGuid().ToString();
        }
        public LogLevel Level { get; set; } = LogLevel.Debug;

        public void Write([Localizable(false)] string message, LogLevel logLevel)
        {
            output.WriteLine(UUID + " " + message);
        }

        public void Dispose()
        {
            // The implementation of this method not described here.
            // ... For now, just report the call.
            output.Close();
        }
    }

    static class Program
    {
        public static DirectoryInfo GetExecutingDirectory() 
        { 
            var location = new Uri(Assembly.GetEntryAssembly().GetName().CodeBase); 
            return new FileInfo(location.AbsolutePath).Directory; 
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            AssemblyTitleAttribute assemblyTitle = assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), false)[0] as AssemblyTitleAttribute;
            string appTitle = assemblyTitle.Title;

            bool firstRun = false;

            string cwd = GetExecutingDirectory().FullName;
            string logPath = System.IO.Path.Combine(cwd, appTitle + ".log");

            using (FileLogger logger = new FileLogger(logPath))
            {
                Locator.CurrentMutable.RegisterConstant(logger, typeof(Splat.ILogger));

                /* Update URL is resolved in the following order:
                 * 
                 *  1. `update-url.rc` file located in same directory as `.exe`
                 *  2. `MICRODROP_UPDATE_URL` environment variable.
                 *  3. Default of `https://sci-bots.com/microdrop-releases`.
                 */
                string updateUrlFilePath = System.IO.Path.Combine(cwd, @"update-url.rc");
                string updateUrl;

                if (System.IO.File.Exists(updateUrlFilePath))
                {
                    updateUrl = System.IO.File.ReadAllText(updateUrlFilePath).Trim();
                    LogHost.Default.Info(String.Format("Using update url from `{0}`: `{1}`",
                                                       updateUrlFilePath, updateUrl));
                } else if (Environment.GetEnvironmentVariable("MICRODROP_UPDATE_URL") != null)
                {
                    updateUrl = Environment.GetEnvironmentVariable("MICRODROP_UPDATE_URL");
                    LogHost.Default.Info(String.Format("Using update url from `MICRODROP_UPDATE_URL`: `{0}`",
                                                       updateUrl));
                } else
                {
                    updateUrl = "https://sci-bots.com/microdrop-releases";
                    LogHost.Default.Info(String.Format("Using default update url: `{0}`", updateUrl));
                }

                try
                {
                    using (var mgr = new UpdateManager(updateUrl))
                    {
                        // Note, in most of these scenarios, the app exits after this method
                        // completes!
                        SquirrelAwareApp.HandleEvents(
                          onInitialInstall: v =>
                          {
                              LogHost.Default.Info(String.Format("Installed {0} `{1}`.", appTitle, v));
                              mgr.CreateShortcutForThisExe();
                          },
                          onAppUpdate: v =>
                          {
                              MessageBox.Show(String.Format("Updated {0} `{1}` will " +
                                                            "load on next launch.", appTitle, v));
                              LogHost.Default.Info("onAppUpdate");
                              mgr.CreateShortcutForThisExe();
                          },
                          onAppUninstall: v =>
                          {
                              LogHost.Default.Info("onAppUninstall");
                              mgr.RemoveShortcutForThisExe();
                          },
                          onFirstRun: () =>
                          {
                              // First run after initial installation.
                              firstRun = true;
                              MessageBox.Show("onFirstRun");
                              LogHost.Default.Info("onFirstRun");
                          });
                    }
                } catch
                {
                    LogHost.Default.Error("Error handling updates.");
                }
                var updateTask = Task.Run(async () =>
                {
                    using (var mgr = new UpdateManager(updateUrl))
                    {
                        LogHost.Default.Info(String.Format("Checking updates from '{0}'",
                                                           updateUrl));

                        SemanticVersion version = mgr.CurrentlyInstalledVersion();
                        LogHost.Default.Info(String.Format("Version: '{0}'", version));

                        UpdateInfo updateInfo = await mgr.CheckForUpdate(ignoreDeltaUpdates: false);
                        LogHost.Default.Info(updateInfo);
                        LogHost.Default.Info(updateInfo.CurrentlyInstalledVersion.EntryAsString);
                        LogHost.Default.Info(updateInfo.FutureReleaseEntry.EntryAsString);
                        LogHost.Default.Info(updateInfo.PackageDirectory);
                        LogHost.Default.Info(String.Format("Number of releases to apply: {0}",
                                                           updateInfo.ReleasesToApply.Count));

                        ReleaseEntry entry = await mgr.UpdateApp();
                        LogHost.Default.Info(entry.Version);
                    }
                });

                // Launch IPython.
                var exe = Path.Combine(cwd, "app", "MicroDrop.exe");
                /* [Use `processInfo.EnvironmentVariables` as a dictionary][1] to set environment
                 * variables of launched process.
                 *
                 * [1]: https://stackoverflow.com/a/14582921/345236
                 */
                var processInfo = new ProcessStartInfo
                {
                    UseShellExecute = false,
                    FileName = exe
                };
                LogHost.Default.Info(String.Format("Executing: `{0} {1}`", processInfo.FileName,
                                                   processInfo.Arguments));
                using (var process = Process.Start(processInfo))
                {
                    process.WaitForExit();
                }

                // Explicitly wait for update task to complete before app closes.
                updateTask.Wait();
            }
        }
    }
}
