using System.Diagnostics;
using System.IO;
using System;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System.ComponentModel.Composition;
using ZinetExtension.Commands;

namespace ZinetExtension
{
    [Command(PackageIds.ZinetReflectCode)]
    internal sealed class ProjectReflection : BaseCommand<ProjectReflection>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            ExecuteScript executeScript = new ExecuteScript();
            await executeScript.ExecutePythonScriptAsync("reflection.py");
        }
    }
}
