using System.Threading;
using Frends.Tasks.Attributes;
using System.IO;
using Ionic.Zip;

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
            //check that source file exists
            if (!File.Exists(source.SourceFile))
                throw new FileNotFoundException($"Source file {source.SourceFile} does not exist.");

            //check that destination directory exist
            if (!Directory.Exists(destination.DirectoryPath) && !options.CreateDestinationDirectory)
                throw new DirectoryNotFoundException($"Destination directory {destination.DirectoryPath} does not exist.");

            //create destination directory 
            if (options.CreateDestinationDirectory)
            {
                Directory.CreateDirectory(destination.DirectoryPath);
            }

            Output output = new Output();

            //passed to event handler 
            FileNameHelper fh = new FileNameHelper
            {
                DestinationPath = destination.DirectoryPath
            };

            using (ZipFile zip = ZipFile.Read(source.SourceFile))
            {
                zip.ExtractProgress += (sender, e) => Zip_ExtractProgress(sender, e, output, fh);

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
                            //if a file exists
                            if (File.Exists(Path.Combine(destination.DirectoryPath, z.FileName)))
                            {
                                //find a filename that does not exist 
                                string FullPath = Extensions.GetNewFilename(Path.Combine(Path.GetDirectoryName(destination.DirectoryPath), z.FileName), z.FileName, cancellationToken);
                                fh.Filename = FullPath;

                                using (FileStream fs = new FileStream(FullPath, FileMode.Create, FileAccess.Write))
                                {
                                    z.Extract(fs);
                                }
                                fh.Filename = null;
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

        private static void Zip_ExtractProgress(object sender, ExtractProgressEventArgs e, Output output, FileNameHelper fnh)
        {
            if (e.EventType == ZipProgressEventType.Extracting_AfterExtractEntry && !e.CurrentEntry.IsDirectory)
            {
                if (fnh.Filename == null)
                {
                    //adds a filename to extracted files-list
                    output.ExtractedFiles.Add(Path.GetFullPath(Path.Combine(fnh.DestinationPath, e.CurrentEntry.FileName)));
                }
                else
                {
                    output.ExtractedFiles.Add(fnh.Filename);
                }
            }
        }

        private class FileNameHelper
        {
            public string DestinationPath { get; set; }
            public string Filename { get; set; }
        }
    }
}

