using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.NET.Build.Tasks
{

    public sealed class GenerateAnalyzerTargetLinesForGivenTargetFramework : Task
    {
        [Required]
        public string PackageId { get; set; }

        [Required]
        public string DependsOnTargets { get; set; }

        [Required]
        public string TargetFramework { get; set; }
 
        [Required]
        public string TargetFrameworkIdentifier { get; set; }

        [Output]
        public ITaskItem[] TargetLines { get; private set; }
        [Output]
        public ITaskItem TargetName { get; private set; }

        public override bool Execute()
        {
            var targetName = $"_AddAnalyzer{PackageId}For{TargetFramework}";
            TargetName = new TaskItem(targetName);

            string conditionString = string.Equals(TargetFrameworkIdentifier, ".NETCoreApp", System.StringComparison.OrdinalIgnoreCase)
                ? "$(MSBuildRuntimeType) == 'Core'"
                : "$(MSBuildRuntimeType) != 'Core'";
            
            var targetLines = $@"<Project xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
  <Target Name=""{targetName}""
          AfterTargets=""ResolvePackageDependenciesForBuild;ResolveNuGetPackageAssets""
          Condition=""{conditionString}""
          DependsOnTargets=""{DependsOnTargets}"">
    <ItemGroup>
      <Analyzer Include=""@(_{PackageId}Analyzers)"" Condition=""$([System.String]::Copy(%(_{PackageId}Analyzers.FullPath)).Contains('{TargetFramework}'))"" />
    </ItemGroup>
  </Target>           
</Project>
";
            TargetLines = targetLines.AsLines();
            return TargetLines.Any();
        }
    }
}
