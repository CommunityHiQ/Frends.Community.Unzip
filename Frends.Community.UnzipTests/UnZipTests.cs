﻿using NUnit.Framework;
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
        string inputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\TestData\TestIn\");
        string outputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\TestData\TestOut\");
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
            Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\TestData\TestOut\"));
        }
        
        [TearDown]
        public void TearDown()
        {
            Directory.Delete(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\TestData\TestOut"), true);
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
            //create destination directory if it does not exist
            sp.SourceFile = Path.Combine(inputPath, @"HiQLogos.zip");
            opt.DestinationFileExistsAction = FileExistAction.Error;
            opt.CreateDestinationDirectory = true;
            dp.DirectoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\TestData\TestOut\new_directory\");

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
            //extract password protected archive
            sp.SourceFile = Path.Combine(inputPath, @"HiQLogosWithPassword.zip");
            sp.Password = "secret";

            opt.DestinationFileExistsAction = FileExistAction.Overwrite;
            opt.CreateDestinationDirectory = true;

            dp.DirectoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\TestData\TestOut\new_directory");

            Output output = UnzipTask.ExtractArchive(sp, dp, opt, new System.Threading.CancellationToken());
            Assert.True(File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\TestData\TestOut\new_directory\logo1.png")));
            Assert.True(File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\TestData\TestOut\new_directory\logo2.png")));
        }

        [Test]
        public void PasswordError()
        {
            //Should throw Ionic.Zip.BadPasswordException
            sp.SourceFile = Path.Combine(inputPath, @"HiQLogosWithPassword.zip");
            opt.DestinationFileExistsAction = FileExistAction.Overwrite;
            opt.CreateDestinationDirectory = true;
            dp.DirectoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\TestData\TestOut\new_directory");

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

            dp.DirectoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\TestData\TestOut\new_directory");

            //unzip files to TestOut, so that there are existing files
            UnzipTask.ExtractArchive(sp, dp, opt2, new CancellationToken());

            Assert.That(() => UnzipTask.ExtractArchive(sp, dp, opt, new System.Threading.CancellationToken()), Throws.TypeOf<Ionic.Zip.ZipException>());
        }

        [Test]
        public void OverwriteFiles()
        {
           
            sp.SourceFile = Path.Combine(inputPath, @"HiQLogos.zip");

            Options opt = new Options()
            {
                DestinationFileExistsAction = FileExistAction.Overwrite,
                CreateDestinationDirectory = true
            };

            dp.DirectoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\TestData\TestOut\new_directory");

            //unzip files to TestOut, so that there are existing files
            Unzip.Output output = UnzipTask.ExtractArchive(sp, dp, opt, new CancellationToken());

            //create a dictionary (filename, LastAccessTime)
            Dictionary<string, long> files = output.ExtractedFiles.ToDictionary(x => x, x => new FileInfo(x).LastAccessTime.Ticks);

            //overwrite files in TestOut
            Unzip.Output output2 = UnzipTask.ExtractArchive(sp, dp, opt, new CancellationToken());

            Assert.AreEqual(output.ExtractedFiles.Count, output2.ExtractedFiles.Count);

            foreach(string s in output2.ExtractedFiles)
            {
                FileInfo fi = new FileInfo(s);
                Assert.That(files.ContainsKey(fi.FullName) && files[fi.FullName]!=fi.LastAccessTime.Ticks);
            }
        }

        [Test]
        public void RenameFiles()
        {
            sp.SourceFile = Path.Combine(inputPath, @"HiQLogos.zip");

            opt.DestinationFileExistsAction = FileExistAction.Rename;
            opt.CreateDestinationDirectory = true;

            dp.DirectoryPath = outputPath;

            //extract files to TestOut, so that there are existing files
            UnzipTask.ExtractArchive(sp, dp, opt, new CancellationToken());
            Output output = UnzipTask.ExtractArchive(sp, dp, opt, new CancellationToken());
            //create filenames to test against
            outputFiles = new List<string>();
            renamedFilenames.ToList().ForEach(x => outputFiles.Add(Path.Combine(dp.DirectoryPath, x)));

            foreach (string s in outputFiles)
            {
                Assert.True(File.Exists(s));
               
            }

            Assert.AreEqual(output.ExtractedFiles.Count, 7);
        }
        
    }
}