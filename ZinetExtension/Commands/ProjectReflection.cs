using System.Diagnostics;
using System.IO;
using System;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System.ComponentModel.Composition;

namespace ZinetExtension
{
    [Command(PackageIds.ToolbarGroup)]
    internal sealed class ProjectReflection : BaseCommand<ProjectReflection>
    {
        private string MessageTitle { get; } = "Zinet Extension";

        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            /// await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            /// Zinet root folder
            string projectFolderNameStartsWith = "Zinet";
            DirectoryInfo projectDirectory = FindProjectDirectory(projectFolderNameStartsWith);

            if (projectDirectory == null)
            {
                string errorMessage = String.Format("Can't find folder that starts with: {0}", projectFolderNameStartsWith);
                await VS.MessageBox.ShowErrorAsync(MessageTitle, errorMessage);
                return;
            }

            /// Generate reflection script path
            string reflectionScriptPath = projectDirectory.FullName + @"\Scripts\generate_reflection.bat";
            if (!File.Exists(reflectionScriptPath))
            {
                string errorMessage = String.Format("Reflection script {0} doesn't exist", reflectionScriptPath);
                await VS.MessageBox.ShowErrorAsync(MessageTitle, errorMessage);
                return;
            }

            // Run reflection script
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.EnableRaisingEvents = true;
            process.StartInfo.FileName = reflectionScriptPath;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = false;
            bool result = false;
            try
            {
                result = process.Start();
                process.WaitForExit();
            }
            catch (Exception Ex)
            {
                await VS.MessageBox.ShowErrorAsync(MessageTitle, Ex.Message);
            }
            string error = await process.StandardError.ReadToEndAsync();
            string output = await process.StandardOutput.ReadToEndAsync();

            if (process.ExitCode != 0)
            {
                string errorMessage = String.Format("Zinet Reflector returned error: {0}", error);
                await VS.MessageBox.ShowErrorAsync(MessageTitle, errorMessage);

                string outputMessage = String.Format("Zinet Reflector returned output: {0}", output);
                await VS.MessageBox.ShowErrorAsync(MessageTitle, outputMessage);

                string returnCodeMessage = String.Format("Zinet Reflector returned exit code: {0}", process.ExitCode);
                await VS.MessageBox.ShowErrorAsync(MessageTitle, returnCodeMessage);
            }
        }

        private DirectoryInfo FindProjectDirectory(string projectFolderNameStartsWith)
        {
            string rootPath = Path.GetFullPath(@"./");

            DirectoryInfo directoryInfo = new DirectoryInfo(rootPath);
            while (true)
            {
                directoryInfo = directoryInfo.Parent;
                if (directoryInfo == null)
                    return null;

                if (directoryInfo.Name.StartsWith(projectFolderNameStartsWith))
                    return directoryInfo;
            }
        }
    }
}
