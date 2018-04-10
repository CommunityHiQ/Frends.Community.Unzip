using System.Threading;
using Frends.Tasks.Attributes;
using System.IO;
using Ionic.Zip;
using System.Diagnostics;

namespace Frends.Community.Unzip
{
    /// <summary>
    /// 
    /// </summary>
    public class UnzipTask
    {   /// <summary>
        /// A Frends task for extracting zip archives
        /// </summary>
        /// <param name="source">Source properties</param>
        /// <param name="destination">Destination properties</param>
        /// <param name="options">Options</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Output-object with a List of extracted files</returns>
        public static Output ExtractArchive(
            [CustomDisplay(DisplayOption.Tab)]SourceProperties source,
            [CustomDisplay(DisplayOption.Tab)]DestinationProperties destination,
            [CustomDisplay(DisplayOption.Tab)]Options options,
            CancellationToken cancellationToken)
        {

            if (!File.Exists(source.SourceFile))
                throw new FileNotFoundException($"Source file {source.SourceFile} does not exist.");

            if (!Directory.Exists(destination.DirectoryPath) && !options.CreateDestinationDirectory)
                throw new DirectoryNotFoundException($"Destination directory {destination.DirectoryPath} does not exist.");

            if (options.CreateDestinationDirectory)
            {
                Directory.CreateDirectory(destination.DirectoryPath);
            }

            Output output = new Output();

            using (ZipFile zip = ZipFile.Read(source.SourceFile))
            {
                string path = null;
                zip.ExtractProgress += (sender, e) => Zip_ExtractProgress(sender, e, output, path);

                //if password is set
                if (!string.IsNullOrWhiteSpace(source.Password))
                {
                    zip.Password = source.Password;
                }

                switch (options.DestinationFileExistsAction)
                {
                    case FileExistAction.Error:
                    case FileExistAction.Overwrite:
                        zip.ExtractExistingFile = (options.DestinationFileExistsAction == FileExistAction.Overwrite) ? ExtractExistingFileAction.OverwriteSilently : ExtractExistingFileAction.Throw;
                        zip.ExtractAll(destination.DirectoryPath);
                        break;
                    case FileExistAction.Rename:
                        foreach (ZipEntry z in zip)
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            if (File.Exists(Path.Combine(destination.DirectoryPath, z.FileName)))
                            {
                                //find a filename that does not exist 
                                string FullPath = Extensions.GetNewFilename(Path.Combine(Path.GetDirectoryName(destination.DirectoryPath), z.FileName), z.FileName, cancellationToken);
                                path = FullPath;

                                using (FileStream fs = new FileStream(FullPath, FileMode.Create, FileAccess.Write))
                                {
                                    z.Extract(fs);
                                }
                            }
                            else
                            {
                                z.Extract(destination.DirectoryPath);
                            }
                        }
                        break;
                }
            }
            return output;
        }
        
        private static void Zip_ExtractProgress(object sender, ExtractProgressEventArgs e, Output output, string fullPath)
        {
            if (e.EventType == ZipProgressEventType.Extracting_AfterExtractEntry && !e.CurrentEntry.IsDirectory)
            {
                if (e.ExtractLocation == null)
                {
                   //Path.GetFullPath changes directory separator to "\"
                    output.ExtractedFiles.Add(Path.GetFullPath(fullPath));
                }
                else
                {
                    output.ExtractedFiles.Add(Path.GetFullPath(Path.Combine(e.ExtractLocation, e.CurrentEntry.FileName)));
                }
            }
        }
    }
}

