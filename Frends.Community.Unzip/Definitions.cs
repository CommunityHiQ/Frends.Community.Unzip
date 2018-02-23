using Frends.Tasks.Attributes;
using System.Collections.Generic;
using System.ComponentModel;

namespace Frends.Community.Unzip
{

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public enum FileExistAction { Error, Overwrite, Rename };
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

    /// <summary>
    /// Source properties
    /// </summary>
    [DisplayName("Source")]
    public class SourceProperties
    {
        /// <summary>
        /// Full path to the source file
        /// </summary>
        [DefaultValue(@"C:\example\file.zip")]
        [DefaultDisplayType(DisplayType.Text)]
        public string SourceFile { get; set; }
        /// <summary>
        /// Password for the zip file
        /// </summary>
        [DefaultValue("")]
        [PasswordPropertyText]
        public string Password { get; set; }
    }
    /// <summary>
    /// Destination properties
    /// </summary>
    [DisplayName("Destination")]
    public class DestinationProperties
    {
        /// <summary>
        /// Destination directory
        /// </summary>
        [DefaultValue(@"C:\example")]
        [DefaultDisplayType(DisplayType.Text)]
        public string DirectoryPath { get; set; }
    }
    /// <summary>
    /// Options
    /// </summary>
    [DisplayName("Options")]
    public class Options
    {
        /// <summary>
        /// Action to be taken when destination file/files exist
        /// </summary>
        [DefaultValue(FileExistAction.Error)]
        public FileExistAction DestinationFileExistsAction { get; set; }
        /// <summary>
        /// Create destination directory if it does not exist
        /// </summary>
        [DefaultValue(false)]
        public bool CreateDestinationDirectory { get; set; }
    }
    /// <summary>
    /// Output
    /// </summary>
    public class Output
    {
        /// <summary>
        /// a List-object of extracted files
        /// </summary>
        public List<string> ExtractedFiles { get; set; }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public Output()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            ExtractedFiles = new List<string>();
        }
    }
}
