using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Linq;

namespace Microsoft.NET.Build.Tasks
{
    public sealed class GenerateGatherAnalyzerByPackageIdTarget : Task
    {
        [Required]
        public string PackageId { get; set; }
        [Required]
        public string TargetFrameworks { get; set; }

        [Output]
        public ITaskItem[] TargetLines { get; private set; }
        [Output]
        public ITaskItem TargetName { get; private set; }

        public override bool Execute()
        {
            var targetName = $"_GatherAnalyzerByPackageId_{PackageId}";
            TargetName = new TaskItem(targetName);
            var tfms = string.Join(Environment.NewLine,
                TargetFrameworks.Split(';')
                .Select(x => x.Trim())
                .Select(tfm => $"  <Import Project=\"{PackageId}.{tfm}.targets\""));
            var targetLines = $@"<Project xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
  <Target Name=""{targetName}"">
    <ItemGroup>
      <_{PackageId}Analyzers Include=""@(Analyzer)"" Condition=""'%(Analyzer.NuGetPackageId)' == '{PackageId}'"" />
      <Analyzer Remove=""@(_{PackageId}Analyzers)""/>
    </ItemGroup>
  </Target>
{tfms}
</Project>
";
            TargetLines = targetLines.AsLines();
            return TargetLines.Any();
        }
    }
}
