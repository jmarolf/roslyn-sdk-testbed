using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.NET.Build.Tasks
{
    public static class StringExtensions
    {
        public static ITaskItem[] AsLines(this string @string)
        {
            using (var reader = new StringReader(@string))
            {
                var lines = new List<TaskItem>();
                string line;
                do
                {
                    line = reader.ReadLine();
                    if (line != null)
                    {
                        lines.Add(new TaskItem(line));
                    }
                } while (line != null);
                return lines.ToArray();
            }
        }
    }
}
