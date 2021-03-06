﻿using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.Community.Unzip
{

#pragma warning disable CS1591
    public enum FileExistAction { Error, Overwrite, Rename };
#pragma warning restore CS1591 
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
        [DisplayName(@"Source file")]
        [DisplayFormat(DataFormatString="Text")]
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
        [DisplayName(@"Destination directory")]
        [DisplayFormat(DataFormatString ="Text")]
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
        [DisplayName(@"File exist action")]
        public FileExistAction DestinationFileExistsAction { get; set; }
        /// <summary>
        /// Create destination directory if it does not exist
        /// </summary>
        [DefaultValue(false)]
        [DisplayName(@"Create destination directory")]
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

#pragma warning disable CS1591
        public Output()
#pragma warning restore CS1591
        {
            ExtractedFiles = new List<string>();
        }
    }
}
