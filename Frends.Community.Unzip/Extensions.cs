using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Frends.Community.Unzip
{
    class Extensions
    {
        internal static string GetNewFilename(string fullPath, string name, CancellationToken cancellationToken)
        {
            int index = 0;
            string newPath = null;
            do
            {
                cancellationToken.ThrowIfCancellationRequested();
                string new_Filename = $"{Path.GetFileNameWithoutExtension(name)}({index}){Path.GetExtension(name)}";
                newPath = Path.Combine(Path.GetDirectoryName(fullPath), new_Filename);
                index++;
            } while (File.Exists(newPath));
            return newPath;
        }
    }
}
