using System.Diagnostics;
using System.IO;
using System;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System.ComponentModel.Composition;
using ZinetExtension.Commands;

namespace ZinetExtension
{
    [Command(PackageIds.ZinetGenerateProject)]
    internal sealed class ProjectGenerator : BaseCommand<ProjectGenerator>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {

            ExecuteScript executeScript = new ExecuteScript();
            await executeScript.ExecuteScriptAsync("generate_project_files_AddressSanitizerOff.bat");
            await executeScript.ExecutePythonScriptAsync("build.py", "--BuildType Debug");
        }
    }
}
