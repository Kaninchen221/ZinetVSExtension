using System.IO;

namespace ZinetExtension.Commands
{
    internal class ExecuteScript
    {
        private string MessageTitle { get; } = "Zinet Extension";
        private string ProjectFolderNameStartsWith { get; } = "Zinet";
        private string ScriptsFolderNameSubPath { get; } = @"\Scripts\";


        public async Task ExecuteScriptAsync(string ScriptName)
        {
            /// Get zinet root folder
            DirectoryInfo ProjectDirectory = FindProjectRootDirectory();

            if (ProjectDirectory == null)
            {
                string ErrorMessage = String.Format("Can't find folder that starts with: {0}", ProjectFolderNameStartsWith);
                await VS.MessageBox.ShowErrorAsync(MessageTitle, ErrorMessage);
                return;
            }

            /// Get absolute path to a script
            string ScriptAbsolutePath = ProjectDirectory.FullName + ScriptsFolderNameSubPath + ScriptName;
            if (!File.Exists(ScriptAbsolutePath))
            {
                string ErrorMessage = String.Format("Script path doesn't exist. Path: {0}", ScriptAbsolutePath);
                await VS.MessageBox.ShowErrorAsync(MessageTitle, ErrorMessage);
                return;
            }

            // Run script
            System.Diagnostics.Process ProcessObject = new System.Diagnostics.Process();
            ProcessObject.EnableRaisingEvents = true;
            ProcessObject.StartInfo.FileName = ScriptAbsolutePath;
            ProcessObject.StartInfo.RedirectStandardError = true;
            ProcessObject.StartInfo.RedirectStandardOutput = true;
            ProcessObject.StartInfo.UseShellExecute = false;
            ProcessObject.StartInfo.CreateNoWindow = false;
            bool Result = false;
            try
            {
                Result = ProcessObject.Start();
                ProcessObject.WaitForExit();
            }
            catch (Exception Ex)
            {
                await VS.MessageBox.ShowErrorAsync(MessageTitle, Ex.Message);
            }
            string Error = await ProcessObject.StandardError.ReadToEndAsync();
            string Output = await ProcessObject.StandardOutput.ReadToEndAsync();

            ///if (String.IsNullOrEmpty(Error))
            if (ProcessObject.ExitCode != 0 || !Result)
            {
                string ErrorMessage = String.Format("Script returned error: {0}", Error);
                await VS.MessageBox.ShowErrorAsync(MessageTitle, ErrorMessage);

                string OutputMessage = String.Format("Script returned output: {0}", Output);
                await VS.MessageBox.ShowErrorAsync(MessageTitle, OutputMessage);

                string ReturnCodeMessage = String.Format("Script returned exit code: {0}", ProcessObject.ExitCode);
                await VS.MessageBox.ShowErrorAsync(MessageTitle, ReturnCodeMessage);
            }
        }
        private DirectoryInfo FindProjectRootDirectory()
        {
            string RootPath = Path.GetFullPath(@"./");

            DirectoryInfo RootDirectoryInfo = new DirectoryInfo(RootPath);
            while (true)
            {
                RootDirectoryInfo = RootDirectoryInfo.Parent;
                if (RootDirectoryInfo == null)
                    return null;

                if (RootDirectoryInfo.Name.StartsWith(ProjectFolderNameStartsWith))
                    return RootDirectoryInfo;
            }
        }
    }
}
