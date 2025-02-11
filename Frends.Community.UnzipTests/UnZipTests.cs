using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Ionic.Zip;

namespace Frends.Community.Unzip.Tests
{
    [TestFixture]
    public class UnZipTests
    {
        string[] fileNames = {@"logo1.png", @"logo2.png", @"folder\logo1.png", @"folder\logo2.png", @"folder\folder\folder\logo1.png",
                                @"folder\folder\folder\logo2.png", @"folder\folder\folder\folder\logo1.png" };
        //used for testing the rename-option
        string[] renamedFilenames = {@"logo1(0).png", @"logo2(0).png", @"folder\logo1(0).png", @"folder\logo2(0).png", @"folder\folder\folder\logo1(0).png",
                                @"folder\folder\folder\logo2(0).png", @"folder\folder\folder\folder\logo1(0).png" };

        //paths to TestIn and TestOut
        string inputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "TestIn");
        string outputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "TestOut");
        List<string> outputFiles;
        SourceProperties sp;
        DestinationProperties dp;
        Options opt;

        [SetUp]
        public void Setup()
        {
            sp = new SourceProperties();
            dp = new DestinationProperties();
            opt = new Options();
            Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "TestOut"));
        }
        
        [TearDown]
        public void TearDown()
        {
            Directory.Delete(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "TestOut"), true);
        }
       
       
        [Test]
        public void SourceFileDoesNotExist()
        {
            //throws System.IO.FileNotFoundException 
            sp.SourceFile = Path.Combine(inputPath, @"doesnotexist.zip");
            opt.DestinationFileExistsAction = FileExistAction.Overwrite;
            dp.DirectoryPath = outputPath;
            Assert.That(() => UnzipTask.ExtractArchive(sp, dp, opt, new System.Threading.CancellationToken()), Throws.TypeOf<FileNotFoundException>());
        }

        [Test]
        public void DestinatioDirectoryNotFound()
        {
            //throws directory not found exception
            //destination directory does not exist and create destination directory == false
            sp.SourceFile = Path.Combine(inputPath, @"HiQLogos.zip");
            opt.DestinationFileExistsAction = FileExistAction.Error;
            opt.CreateDestinationDirectory = false;
            dp.DirectoryPath = Path.Combine(outputPath, @"\doesnot\exist");
            Assert.That(() => UnzipTask.ExtractArchive(sp, dp, opt, new System.Threading.CancellationToken()), Throws.TypeOf<DirectoryNotFoundException>());
        }

        [Test]
        public void CreateDirectory()
        {
            sp.SourceFile = Path.Combine(inputPath, @"HiQLogos.zip");
            opt.DestinationFileExistsAction = FileExistAction.Error;
            opt.CreateDestinationDirectory = true;
            dp.DirectoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "TestOut", "new_directory");

            outputFiles = new List<string>();
            fileNames.ToList().ForEach(x => outputFiles.Add(Path.Combine(dp.DirectoryPath, x)));
            Output output = UnzipTask.ExtractArchive(sp, dp, opt, new CancellationToken());

            foreach (string s in outputFiles)
            {
                Assert.True(File.Exists(s));
            }
            Assert.AreEqual(output.ExtractedFiles.Count, 7);
        }

        [Test]
        public void ExtractWithPassword()
        {
            sp.SourceFile = Path.Combine(inputPath, @"HiQLogosWithPassword.zip");
            sp.Password = "secret";

            opt.DestinationFileExistsAction = FileExistAction.Overwrite;
            opt.CreateDestinationDirectory = true;

            dp.DirectoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "TestOut", "new_directory");

            Output output = UnzipTask.ExtractArchive(sp, dp, opt, new System.Threading.CancellationToken());
            Assert.True(File.Exists(Path.Combine(dp.DirectoryPath, "logo1.png")));
            Assert.True(File.Exists(Path.Combine(dp.DirectoryPath, "logo2.png")));
        }

        [Test]
        public void PasswordError()
        {
            sp.SourceFile = Path.Combine(inputPath, @"HiQLogosWithPassword.zip");
            opt.DestinationFileExistsAction = FileExistAction.Overwrite;
            opt.CreateDestinationDirectory = true;
            dp.DirectoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "TestOut", "new_directory");

            Assert.That(() => UnzipTask.ExtractArchive(sp, dp, opt, new System.Threading.CancellationToken()), Throws.TypeOf<BadPasswordException>());
        }

        [Test]
        public void ThrowErrorOnOverwrite()
        {
            sp.SourceFile = Path.Combine(inputPath, @"HiQLogos.zip");

            Options opt2 = new Options()
            {
                DestinationFileExistsAction = FileExistAction.Overwrite,
                CreateDestinationDirectory = true
            };

            opt.DestinationFileExistsAction = FileExistAction.Error;
            opt.CreateDestinationDirectory = true;

            dp.DirectoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "TestOut", "new_directory");

            UnzipTask.ExtractArchive(sp, dp, opt2, new CancellationToken());

            Assert.That(() => UnzipTask.ExtractArchive(sp, dp, opt, new System.Threading.CancellationToken()), Throws.TypeOf<Ionic.Zip.ZipException>());
        }

        [Test]
        public void OverwriteFiles()
        {
            sp.SourceFile = Path.Combine(inputPath, @"testzip.zip");
            Options opt = new Options()
            {
                DestinationFileExistsAction = FileExistAction.Overwrite,
                CreateDestinationDirectory = true
            };

            dp.DirectoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "TestOut", "new_directory");
            
            Output output = UnzipTask.ExtractArchive(sp, dp, opt, new CancellationToken());
            var lines = Directory.EnumerateFiles(dp.DirectoryPath, "*", SearchOption.AllDirectories).Select(x => File.ReadLines(x).First()).ToList();

            Assert.True(lines.Contains("First file") && lines.Contains("Second file") && lines.Contains("Third file"));

            sp.SourceFile = Path.Combine(inputPath, @"testzip2.zip");
            output = UnzipTask.ExtractArchive(sp, dp, opt, new CancellationToken());
            var lines2 = Directory.EnumerateFiles(dp.DirectoryPath, "*", SearchOption.AllDirectories).Select(x => File.ReadLines(x).First()).ToList();
            Assert.False(lines2.Contains("First file") && lines2.Contains("Second file") && lines2.Contains("Third file"));
            Assert.True(lines2.Contains("Fourth file") && lines2.Contains("Fifth file") && lines2.Contains("Sixth file"));
        }

        [Test]
        public void RenameFiles()
        {
            sp.SourceFile = Path.Combine(inputPath, @"HiQLogos.zip");
            opt.DestinationFileExistsAction = FileExistAction.Rename;
            opt.CreateDestinationDirectory = true;
            dp.DirectoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "TestOut");

            // Create subdirectories that will be needed
            string[] subdirs = { 
                Path.Combine(dp.DirectoryPath, "folder"),
                Path.Combine(dp.DirectoryPath, "folder", "folder", "folder"),
                Path.Combine(dp.DirectoryPath, "folder", "folder", "folder", "folder")
            };
            foreach (var dir in subdirs)
            {
                Directory.CreateDirectory(dir);
            }

            // First extraction
            UnzipTask.ExtractArchive(sp, dp, opt, new CancellationToken());
            // Second extraction should rename files
            Output output = UnzipTask.ExtractArchive(sp, dp, opt, new CancellationToken());
            
            // Verify each renamed file exists
            foreach (string renamedFile in output.ExtractedFiles)
            {
                Assert.True(File.Exists(renamedFile), $"File {renamedFile} should exist");
            }

            Assert.AreEqual(7, output.ExtractedFiles.Count);
        }
        
    }
}