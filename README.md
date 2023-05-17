# Samples for the PDF Services .NET SDK

This sample solution helps you get started with the PDF Services .NET SDK.

The sample projects illustrate how to perform PDF-related actions (such as converting to and from the PDF format) using the PDF Services .NET SDK. **Please note that the PDF Services .NET SDK supports only server side use cases**.

## Prerequisites
The sample solution has the following requirements:
* .NET: version 6.0 or above
* Build Tool: The solution requires Visual studio or .NET Core CLI to be installed to be able to run the sample projects.

## Authentication Setup
The credentials file and corresponding private key file for the samples is ```pdfservices-api-credentials.json``` and ```private.key``` respectively.
Before the samples can be run, replace both the files with the ones present in the downloaded zip file at the end of creation of credentials via [Get Started](https://www.adobe.io/apis/documentcloud/dcsdk/gettingstarted.html?ref=getStartedWithServicesSdk) workflow.

## Client Configurations

The SDK supports setting up custom socket timeout or connect timeout for the API calls. Please
refer this [section](#create-a-pdf-file-from-a-docx-file-by-providing-custom-value-for-timeouts) to
know more.

Additionally, SDK can be configured to process the documents in the specified region.
Please refer this [section](#export-a-pdf-file-to-a-docx-file-by-providing-the-region) to know more.

## Quota Exhaustion
If you receive ServiceUsageException during the Samples run, it means that trial credentials have exhausted their usage quota. Please [contact us](https://www.adobe.com/go/pdftoolsapi_requestform) to get paid credentials.

## Build with .NET Core CLI
Run the following command to build the project:
```$xslt
dotnet build
```

Note that the PDF Services SDK is listed as a dependency and will be downloaded automatically.

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

#### Create a PDF File From a DOCX File with options

The sample project CreatePDFFromDocxWithOptions creates a PDF file from a DOCX file by setting documentLanguage as
the language of input file.

```$xslt
cd CreatePDFFromDocxWithOptions/
dotnet run CreatePDFFromDocxWithOptions.csproj
```

#### Create a PDF File From a DOCX Input Stream

The sample project CreatePDFFromDOCXInputStream creates a PDF file from a DOCX input stream.

```$xslt
cd CreatePDFFromDOCXInputStream/
dotnet run CreatePDFFromDOCXInputStream.csproj
```

#### Create a PDF File From a DOCX File (Write to an OutputStream)

The sample project CreatePDFFromDocxToOutputStream creates a PDF file from a DOCX file. Instead of saving the result to a local file, it writes the
result to an output stream.

```$xslt
cd CreatePDFFromDocxToOutputStream/
dotnet run CreatePDFFromDocxToOutputStream.csproj
```

####  Create a PDF File From a PPTX File

The sample project CreatePDFFromPPTX creates a PDF file from a PPTX file.

```$xslt
cd CreatePDFFromPPTX/
dotnet run CreatePDFFromPPTX.csproj
```

#### Create a PDF File From Static HTML (via Zip Archive)

The sample project CreatePDFFromStaticHTML creates a PDF file from a zip file containing the input HTML file and its resources.
Please refer the documentation of CreatePDFOperation.java to see instructions on the structure of the zip file.	Please refer the documentation of CreatePDFOperation.java to see instructions on the structure of the zip file.

```$xslt
cd CreatePDFFromStaticHtml/
dotnet run CreatePDFFromStaticHtml.csproj
```

#### Create a PDF File From Dynamic HTML (via Zip Archive)

The sample project CreatePDFFromDynamicHTML converts a zip file, containing the input HTML file and its resources, along
with the input data to a PDF file. The input data is used by the javascript in the HTML file to manipulate the HTML DOM,
thus effectively updating the source HTML file. This mechanism can be used to provide data to the template HTML
dynamically and then, convert it into a PDF file.

```$xslt
cd CreatePDFFromDynamicHtml/
dotnet run CreatePDFFromDynamicHtml.csproj
```

#### Create a PDF File From a Static HTML file with inline CSS

The sample project CreatePDFFromHTMLWithInlineCSS creates a PDF file from an input HTML file with inline CSS.

```$xslt
cd CreatePDFFromHTMLWithInlineCSS/
dotnet run CreatePDFFromHTMLWithInlineCSS.csproj
```

#### Create a PDF File From HTML specified via URL

The sample project CreatePDFFromURL creates a PDF file from an HTML specified via URL.
```$xslt
cd CreatePDFFromURL/
dotnet run CreatePDFFromURL.csproj
```


### Export PDF To Other Formats

These samples illustrate how to export PDF files to other formats. Refer to the documentation of ExportPDFOperation.cs
and ExportPDFToImagesOperation.cs for supported export formats.

#### Export a PDF File To a DOCX File

The sample project ExportPDFToDocx converts a PDF file to a DOCX file.

```$xslt
cd ExportPDFToDocx/
dotnet run ExportPDFToDocx.csproj
```

#### Export a PDF file to a DOCX file (apply OCR on the PDF file)

The sample project ExportPDFToDocxWithOCROption converts a PDF file to a DOCX file.
OCR processing is also performed on the input PDF file to extract text from images in the document.

```$xslt
cd ExportPDFToDocxWithOCROption/
dotnet run ExportPDFToDocxWithOCROption.csproj
```

#### Export a PDF File To an Image Format (JPEG)

The sample project ExportPDFToJPEG converts a PDF file's pages to a list of JPEG images.

```$xslt
cd ExportPDFToJPEG/
dotnet run ExportPDFToJPEG.csproj
```

#### Export a PDF File To a Zip of Images (JPEG)

The sample project ExportPDFToJPEGZip converts a PDF file's pages to JPEG images.
The resulting file is a ZIP archive containing one image per page of the source PDF file

```$xslt
cd ExportPDFToJPEGZip/
dotnet run ExportPDFToJPEGZip.csproj
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
### OCR PDF File

These samples illustrate how to apply OCR(Optical Character Recognition) to a PDF file and convert it to a searchable copy of your PDF. The supported input format is application/pdf.

#### Convert a PDF File into a Searchable PDF File

The sample project OcrPDF converts a PDF file into a searchable PDF file.

```$xslt
cd OcrPDF/
dotnet run OcrPDF.csproj
```

#### Convert a PDF File into a Searchable PDF File while keeping the original image

The sample project OcrPDFWithOptions converts a PDF file to a searchable PDF file with maximum fidelity to the original
image and default en-us locale. Refer to the documentation of OCRSupportedLocale.cs and OCRSupportedType.cs to see
the list of supported OCR locales and OCR types.

```$xslt
cd OcrPDFWithOptions/
dotnet run OcrPDFWithOptions.csproj
```

### Compress PDF File

The sample illustrates how to reduce the size of a PDF file.

#### Reduce PDF File Size

The sample project CompressPDF reduces the size of a PDF file.

```$xslt
cd CompressPDF/
dotnet run CompressPDF.csproj
```

#### Reduce PDF File Size on the basis of Compression Level

The sample project CompressPDFWithOptions reduces the size of a PDF file on the basis of provided compression level.
Refer to the documentation of CompressionLevel.cs to see the list of supported compression levels.

```$xslt
cd CompressPDFWithOptions/
dotnet run CompressPDFWithOptions.csproj
```

### Linearize PDF File

The sample illustrates how to convert a PDF file into a Linearized (also known as "web optimized") PDF file. Such PDF files are
optimized for incremental access in network environments.

#### Convert a PDF File into a Web Optimized File

The sample project LinearizePDF optimizes the PDF file for a faster Web View.

```$xslt
cd LinearizePDF/
dotnet run LinearizePDF.csproj
```

### Protect PDF File

These samples illustrate how to secure a PDF file with a password.

#### Convert a PDF File into a Password Protected PDF File

The sample project ProtectPDF converts a PDF file into a password protected PDF file.

```$xslt
cd ProtectPDF/
dotnet run ProtectPDF.csproj
```

#### Protect a PDF File with an Owner Password and Permissions

The sample project ProtectPDFWithOwnerPassword secures an input PDF file with owner password and allows certain access permissions such as copying and editing the contents, and printing of the document at low resolution.

```$xslt
cd ProtectPDFWithOwnerPassword/
dotnet run ProtectPDFWithOwnerPassword.csproj
```

### Remove Protection

The sample illustrates how to remove a password security from a PDF document.

#### Remove Protection from a PDF File

The sample project RemoveProtection removes a password security from a secured PDF document.

```$xslt
cd RemoveProtection/
dotnet run RemoveProtection.csproj
```

### Rotate Pages

The sample illustrates how to rotate pages in a PDF file.

#### Rotate Pages in PDF File

The sample project RotatePDFPages rotates specific pages in a PDF file.

```$xslt
cd RotatePDFPages/
dotnet run RotatePDFPages.csproj
```

### Delete Pages

The sample illustrates how to delete pages in a PDF file.

#### Delete Pages from PDF File

The sample project DeletePDFPages removes specific pages from a PDF file.

```$xslt
cd DeletePDFPages/
dotnet run DeletePDFPages.csproj
```

### Reorder Pages

The sample illustrates how to reorder the pages in a PDF file.

#### Reorder Pages in PDF File

The sample project ReorderPDFPages rearranges the pages of a PDF file according to the specified order.

```$xslt
cd ReorderPDFPages/
dotnet run ReorderPDFPages.csproj
```

### Insert Pages

The sample illustrates how to insert pages in a PDF file.

#### Insert Pages into a single PDF File

The sample project InsertPDFPages inserts pages of multiple PDF files into a base PDF file.

```$xslt
cd InsertPDFPages/
dotnet run InsertPDFPages.csproj
```

### Replace Pages

The sample illustrates how to replace pages of a PDF file.

#### Replace PDF File Pages with Multiple PDF Files

The sample project ReplacePDFPages replaces specific pages in a PDF file with pages from multiple PDF files.

```$xslt
cd ReplacePDFPages/
dotnet run ReplacePDFPages.csproj
```

### Split PDF File
These samples illustrate how to split PDF file into multiple PDF files.

#### Split PDF By Number of Pages

The sample project SplitPDFByNumberOfPages splits input PDF into multiple PDF files on the basis of the maximum number
of pages each of the output files can have.

```$xslt
cd SplitPDFByNumberOfPages/
dotnet run SplitPDFByNumberOfPages.csproj
```

#### Split PDF Into Number of PDF Files

The sample project SplitPDFIntoNumberOfFiles splits input PDF into multiple PDF files on the basis of the number
of documents.

```$xslt
cd SplitPDFIntoNumberOfFiles/
dotnet run SplitPDFIntoNumberOfFiles.csproj
```

#### Split PDF By Page Ranges

The sample project SplitPDFByPageRanges splits input PDF into multiple PDF files on the basis of page ranges.
Each page range corresponds to a single output file having the pages specified in the page range.

```$xslt
cd SplitPDFByPageRanges/
dotnet run SplitPDFByPageRanges.csproj
```

### Document Merge
Adobe Document Merge Operation allows you to produce high fidelity PDF and Word documents with dynamic data inputs.
Using this operation, you can merge your JSON data with Word templates to create dynamic documents for
contracts and agreements, invoices, proposals, reports, forms, branded marketing documents and more.
To know more about document generation and document templates, please checkout the [documentation](http://www.adobe.com/go/dcdocgen_overview_doc)

#### Merge Document to DOCX

The sample project MergeDocumentToDocx merges the Word based document template with the input JSON data to generate
the output document in the DOCX format.

```$xslt
cd MergeDocumentToDocx/
dotnet run MergeDocumentToDocx.csproj
```

#### Merge Document to PDF

The sample project MergeDocumentToPDF merges the Word based document template with the input JSON data to generate
the output document in the PDF format.

```$xslt
cd MergeDocumentToPDF/
dotnet run MergeDocumentToPDF.csproj
```

#### Merge Document to DOCX Fragments

The sample project MergeDocumentToDocxFragments merges the Word based document template with the input JSON data and Fragments JSON to generate
the output document in the Docx format.

```$xslt
cd MergeDocumentToDocxFragments/
dotnet run MergeDocumentToDocxFragments.csproj
```

#### Extract PDF

These samples illustrate extracting content of PDF in a structured JSON format along with the renditions inside PDF.
* The structuredData.json file with the extracted content & PDF element structure.
* A renditions folder(s) containing renditions for each element type selected as input.
  The folder name is either “tables” or “figures” depending on your specified element type.
  Each folder contains renditions with filenames that correspond to the element information in the JSON file.

#### Extract Text Elements

The sample project ExtractTextInfoFromPDF extracts text elements from PDF document.

```$xslt
cd ExtractTextInfoFromPDF/
dotnet run ExtractTextInfoFromPDF.csproj
```

#### Extract Text Elements and bounding boxes for Characters present in text blocks

The sample project ExtractTextInfoWithCharBoundsFromPDF extracts text elements and bounding boxes for characters present in text blocks.

```$xslt
cd ExtractTextInfoWithCharBoundsFromPDF/
dotnet run ExtractTextInfoWithCharBoundsFromPDF.csproj
```

#### Extract Text, Table Elements

The sample project ExtractTextTableInfoFromPDF extracts text, table elements from PDF document.

```$xslt
cd ExtractTextTableInfoFromPDF/
dotnet run ExtractTextTableInfoFromPDF.csproj
```

#### Extract Text, Table Elements and bounding boxes for Characters present in text blocks with Renditions of Table Elements

The sample project ExtractTextTableInfoWithCharBoundsFromPDF extracts text, table elements, bounding boxes for characters present in text blocks and
table element's renditions from PDF document.

```$xslt
cd ExtractTextTableInfoWithCharBoundsFromPDF/
dotnet run ExtractTextTableInfoWithCharBoundsFromPDF.csproj
```

#### Extract Text, Table Elements with Renditions of Figure, Table Elements

The sample project ExtractTextTableInfoWithFiguresTablesRenditionsFromPDF extracts text, table elements along with figure
and table element's renditions from PDF document.


```$xslt
cd ExtractTextTableInfoWithFiguresTablesRenditionsFromPDF/
dotnet run ExtractTextTableInfoWithFiguresTablesRenditionsFromPDF.csproj
```

#### Extract Text, Table Elements with Renditions of Table Elements

The sample project ExtractTextTableInfoWithRenditionsFromPDF extracts text, table elements along with table renditions
from PDF document.


```$xslt
cd ExtractTextTableInfoWithRenditionsFromPDF/
dotnet run ExtractTextTableInfoWithRenditionsFromPDF.csproj
```

#### Extract Text, Table Elements with Styling information of text

The sample project ExtractTextTableInfoWithStylingFromPDF extracts text and table elements along with the styling information of the text blocks.

```$xslt
cd ExtractTextTableInfoWithStylingFromPDF/
dotnet run ExtractTextTableInfoWithStylingFromPDF.csproj
```

#### Extract Text, Table Elements with Renditions and CSV's of Table Elements

The sample project ExtractTextTableInfoWithTableStructureFromPdf extracts text, table elements, table structures as CSV and
table element's renditions from PDF document.


```$xslt
cd ExtractTextTableInfoWithTableStructureFromPDF/
dotnet run ExtractTextTableInfoWithTableStructureFromPDF.csproj
```


### PDF Properties

This sample illustrates how to fetch properties of a PDF file

#### Fetch PDF Properties

The sample project GetPDFProperties fetches the properties of an input PDF.

```
cd GetPDFProperties/
dotnet run GetPDFProperties.csproj
```

### Custom Client Configuration

These samples illustrate how to provide a custom client configurations(timeouts and region) and in-memory authentication
credentials.

#### Create a PDF File From a DOCX File (By providing custom value for timeouts)

The sample project CreatePDFWithCustomTimeouts highlights how to provide the custom value for timeout and read write timeout.

```$xslt
cd CreatePDFWithCustomTimeouts/
dotnet run CreatePDFWithCustomTimeouts.csproj
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

#### Export a PDF File to a DOCX File (By providing the region)

The sample project ExportPDFWithSpecifiedRegion highlights how to configure the SDK to process the documents in the specified region.

```$xslt
cd ExportPDFWithSpecifiedRegion/
dotnet run ExportPDFWithSpecifiedRegion.csproj
```

### Create Tagged PDF

These samples illustrate how to create a PDF document with enhanced readability from existing PDF document. All tags from the input file will be removed except for existing alt-text images and a
new tagged PDF will be created as output. However, the generated PDF is not guaranteed to comply with accessibility standards such as WCAG and PDF/UA as you may need to perform further downstream remediation to meet those standards.

#### Create Tagged PDF from a PDF

The sample project AutotagPDF highlights how to add tags to PDF document to make the PDF more accessible.

```$xslt
cd AutotagPDF/
dotnet run AutotagPDF.csproj
```

#### Create Tagged PDF from a PDF along with a report and shift the headings in the output PDF file

The sample project AutotagPDFWithOptions highlights how to add tags to PDF documents to make the PDF more accessible and also shift the headings in the output PDF file.
Also, it generates a tagging report which contains the information about the tags that the tagged output PDF document contains.

```$xslt
cd AutotagPDFWithOptions/
dotnet run AutotagPDFWithOptions.csproj
```

#### Create Tagged PDF from a PDF by setting options with command line arguments

The sample project AutotagPDFParameterised highlights how to add tags to PDF documents to make the PDF more accessible by setting options through command line arguments.

Here is a sample list of command line arguments and their description: </br>
--input &lt; input file path &gt; </br>
--output &lt; output file path &gt; </br>
--report { If this argument is present then the output will be generated with the tagging report } </br>
--shift_headings { If this argument is present then the headings will be shifted in the output PDF document } </br>

```$xslt
cd AutotagPDFParameterised/
dotnet run AutotagPDFParameterised.csproj --report --shift_headings --input autotagPdfInput.pdf --output output/
```

## Licensing

This project is licensed under the MIT License. See [LICENSE](LICENSE.md) for more information.
 
