# Samples for the DC Services SDK

This sample solution helps you get started with the DC services SDK .

The sample projects illustrate how to perform PDF-related actions (such as converting to and from the PDF format) using the DC Services SDK.

## Prerequisites
The sample solution has the following requirements:
* .NET Core: version 2.1 or above  
* Build Tool: The solution requires Visual studio or .NET Core CLI to be installed to be able to run the sample projects.

## Authentication Setup
The configuration file for the samples is ```dc-services-sdk-config.json```. Before the samples can be run, replace the 
contents of this file with config.json that you received in Adobe Document Cloud Services SDK welcome email.

## Build with .NET Core CLI
Run the following command to build the project:
```$xslt
dotnet build
```

Note that the DC Services SDK is listed as a dependency and will be downloaded automatically.

## A Note on Logging
Following component is being used by SDK:
* [LibLog](https://github.com/damianh/LibLog) (MIT) as bridge between different logging frameworks.  

Log4net is used as a logging provider in the sample projects and the logging configurations are provided in ```log4net.config```.   

**Note**: Add the configuration for your preferred provider and set up the necessary appender as required for logging to work.

## Running the samples
The following sub-sections describe how to run the samples. Prior to running the samples, check that the configuration file is set up as described above and that the project has been built.  

The code is in the different sample projects under the solution folder. Test files used by the samples can be found in the project directory itself. When executed, all samples create an ```output``` 
child folder under the working directory to store their results.

### Create a PDF File

These samples illustrate how to convert files of some formats to PDF. Refer the documentation of CreatePDFOperation.cs to see the list of all supported media types which can be converted to PDF.

#### Create a PDF File From a DOCX File 

The sample project CreatePDFFromDocx creates a PDF file from a DOCX file.

```$xslt
cd CreatePDFFromDocx/
dotnet run CreatePDFFromDocx.csproj
```

#### Create a PDF File From a DOCX File (Write to an OutputStream)

The sample project CreatePDFFromDocx creates a PDF file from a DOCX file. Instead of saving the result to a local file, it writes the 
result to an output stream.

```$xslt
cd CreatePDFFromDocxToOutputStream/
dotnet run CreatePDFFromDocxToOutputStream.csproj
```

#### Create a PDF File From a DOCX File (By providing in-memory Authentication credentials)

The sample project CreatePDFWithInMemoryAuthCredentials highlights how to provide in-memory auth credentials
for performing an operation. This enables the clients to fetch the credentials from a secret server during runtime, 
instead of storing them in a file.

Before running the sample, authentication credentials need to be updated as per the instructions in the code.

```$xslt
cd CreatePDFWithInMemoryAuthCredentials/
dotnet run CreatePDFWithInMemoryAuthCredentials.csproj
```

####  Create a PDF File From a PPTX File 

The sample project CreatePDFFromPPTX creates a PDF file from a PPTX file.

```$xslt
cd CreatePDFFromPPTX/
dotnet run CreatePDFFromPPTX.csproj
```

#### Create a PDF File From HTML (via Zip Archive)

The sample project CreatePDFFromHtml creates a PDF file from a zip file containing the input HTML file and its resources. 
Please refer the documentation of CreatePDFOperation.cs to see instructions on the structure of the zip file.

```$xslt
cd CreatePDFFromHtml/
dotnet run CreatePDFFromHtml.csproj
```

#### Create a PDF File From HTML (via URL)

The sample project CreatePDFFromURL converts an HTML page specified by a URL to a PDF file.

```$xslt
cd CreatePDFFromURL/
dotnet run CreatePDFFromURL.csproj 
```

### Export PDF To Other Formats

These samples illustrate how to export PDF files to other formats. Refer to the documentation of ExportPDFOperation.cs
to see the list of supported export formats.

#### Export a PDF File To a DOCX File 

The sample project ExportPDFToDocx converts a PDF file to a DOCX file.

```$xslt
cd ExportPDFToDocx/
dotnet run ExportPDFToDocx.csproj
```

#### Export a PDF File To an Image Format (JPEG)

The sample project ExportPDFToImage converts a PDF file's pages to JPEG images. Note that the output is a zip archive containing
the individual images.

```$xslt
cd ExportPDFToImage/
dotnet run ExportPDFToImage.csproj
```

### Combine PDF Files

These samples illustrate how to combine multiple PDF files into a single PDF file.

#### Combine Multiple PDF Files

The sample project CombineFiles combines multiple PDF files into a single PDF file. The combined PDF file contains all pages
of the source files.

```$xslt
cd CombinePDF/
dotnet run CombinePDF.csproj
```

#### Combine Specific Pages of Multiple PDF Files

The sample project CombineFilesWithPageOptions combines specific pages of multiple PDF files into into a single PDF file.

```$xslt
cd CombinePDFWithPageRanges/
dotnet run CombinePDFWithPageRanges.csproj
```

## Licensing

This project is licensed under the MIT License. See [LICENSE](LICENSE.md) for more information.
 
