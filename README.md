# Frends.Community.Unzip
FRENDS community task for extracting Zip-archives

- [Installing](#installing)
- [Tasks](#tasks)
  - [Extract archive](#extractarchive)
- [License](#license)
- [Building](#building)
- [Contributing](#contributing)
- [Change Log](#change-log)

# Installing
You can install the task via FRENDS UI Task View or you can find the nuget package from the following nuget feed
'https://www.myget.org/F/frends-community/api/v2'

Tasks
=====

## ExtractArchive
Extracts files from a zip-archive 

### Task Properties

#### Source

| Property             | Type                 | Description                          | Example |
| ---------------------| ---------------------| ------------------------------------ | ----- |
| SourceFile | string | Full path to the zip-archive | c:\source_folder\file.zip |
| Password | string | (Optional) Archive password | secret |

#### Destination

| Property             | Type                 | Description                          | Example |
| ---------------------| ---------------------| ------------------------------------ | ----- |
| DestinationDirectory | string | Destination directory | c:\destination_directory\ |

#### Options

| Property             | Type                 | Description                          | Example |
| ---------------------| ---------------------| ------------------------------------ | ----- |
| FileExistAction | enum(Error, Overwrite, Rename) | Throw error, Overwrite file or Rename file | Error |
| CreateDestinationDirectory | bool | Create destination directory if it does not exist | true|


### Result
| Property             | Type                 | Description                          | Example |
| ---------------------| ---------------------| ------------------------------------ | ----- |
| ExtractedFiles | List`<string>` | a List of extracted files | {"file1.txt", "file2.txt", ...}|

# License

This project is licensed under the MIT License - see the LICENSE file for details

# Building

Clone a copy of the repo:

`git clone https://github.com/CommunityHiQ/Frends.Community.Unzip.git`

Build the project:

```bash
dotnet restore
dotnet build
```

Run Tests:

```bash
dotnet test
```

Create a NuGet package:

```bash
dotnet pack -C Release
```

The NuGet package will be generated in the `Frends.Community.Unzip/bin/Release` directory.

# Contributing
When contributing to this repository, please first discuss the change you wish to make via issue, email, or any other method with the owners of this repository before making a change.

1. Fork the repo on GitHub
2. Clone the project to your own machine
3. Commit changes to your own branch
4. Push your work back up to your fork
5. Submit a Pull request so that we can review your changes

NOTE: Be sure to merge the latest from "upstream" before making a pull request!

# Change Log

| Version             | Changes                 |
| ---------------------| ---------------------|
| 1.0.0 | Initial Frends.Community version of the Unzip-task |
| 1.1.0 | Updated dotNetZip nuget to 1.20.0, if it would not have 'We found potential security vulnerabilities in your dependencies.' issue |
| 1.2.0 | Modernized the tasks to use .NET Standard 2.0 and .NET 6.0 |

