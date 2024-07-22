/*
 * Copyright 2024 Adobe
 * All Rights Reserved.
 *
 * NOTICE: Adobe permits you to use, modify, and distribute this file in 
 * accordance with the terms of the Adobe license agreement accompanying it.
 */

using System;
using System.IO;
using System.Reflection;
using Adobe.PDFServicesSDK;
using Adobe.PDFServicesSDK.auth;
using Adobe.PDFServicesSDK.exception;
using Adobe.PDFServicesSDK.io;
using Adobe.PDFServicesSDK.pdfjobs.jobs;
using Adobe.PDFServicesSDK.pdfjobs.parameters.documentmerge;
using Adobe.PDFServicesSDK.pdfjobs.results;
using log4net;
using log4net.Config;
using log4net.Repository;
using Newtonsoft.Json.Linq;

/// <summary>
/// This sample illustrates how to merge the Word based document template with the input JSON data to generate the output document in the DOCX format.
/// <para>
/// To know more about document generation and document templates, please see the <a href="http://www.adobe.com/go/dcdocgen_overview_doc">documentation</a>
/// And to know more about fragments use-case in document generation and document templates, please see the <a href="http://www.adobe.com/go/dcdocgen_fragments_support_doc">documentation</a>
/// </para>
/// Refer to README.md for instructions on how to run the samples.
/// </summary>
///

namespace MergeDocumentToDocxFragments
{
    class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));

        static void Main()
        {
            //Configure the logging
            ConfigureLogging();
            try
            {
                // Initial setup, create credentials instance
                ICredentials credentials = new ServicePrincipalCredentials(
                    Environment.GetEnvironmentVariable("PDF_SERVICES_CLIENT_ID"),
                    Environment.GetEnvironmentVariable("PDF_SERVICES_CLIENT_SECRET"));

                // Creates a PDF Services instance
                PDFServices pdfServices = new PDFServices(credentials);

                // Creates an asset from source file and upload
                using Stream inputStream = File.OpenRead(@"orderDetailTemplate.docx");
                IAsset asset = pdfServices.Upload(inputStream, PDFServicesMediaType.DOCX.GetMIMETypeValue());

                // Setup input data for the document merge process
                String content = File.ReadAllText(@"orderDetail.json");
                JObject jsonDataForMerge = JObject.Parse(content);

                // Fragment one
                JObject obj1 =
                    JObject.Parse(
                        "{\"orderDetails\": \"<b>Quantity</b>:{{quantity}}, <b>Description</b>:{{description}}, <b>Amount</b>:{{amount}}\"}");

                // Fragment two
                JObject obj2 = JObject.Parse("{\"customerDetails\": \"{{customerName}}, Visits: {{customerVisits}}\"}");

                // Fragment Object
                Fragments fragments = new Fragments();

                // Adding Fragments to the Fragment object
                fragments.AddFragment(obj1);
                fragments.AddFragment(obj2);

                // Create parameters for the job
                DocumentMergeParams documentMergeParams = DocumentMergeParams.DocumentMergeParamsBuilder()
                    .WithJsonDataForMerge(jsonDataForMerge)
                    .WithOutputFormat(OutputFormat.DOCX)
                    .WithFragments(fragments)
                    .Build();

                // Creates a new job instance
                DocumentMergeJob documentMergeJob = new DocumentMergeJob(asset, documentMergeParams);

                // Submits the job and gets the job result
                String location = pdfServices.Submit(documentMergeJob);
                PDFServicesResponse<DocumentMergeResult> pdfServicesResponse =
                    pdfServices.GetJobResult<DocumentMergeResult>(location, typeof(DocumentMergeResult));

                // Get content from the resulting asset(s)
                IAsset resultAsset = pdfServicesResponse.Result.Asset;
                StreamAsset streamAsset = pdfServices.GetContent(resultAsset);

                // Creating output streams and copying stream asset's content to it
                String outputFilePath = CreateOutputFilePath();
                new FileInfo(Directory.GetCurrentDirectory() + outputFilePath).Directory.Create();
                Stream outputStream = File.OpenWrite(Directory.GetCurrentDirectory() + outputFilePath);
                streamAsset.Stream.CopyTo(outputStream);
                outputStream.Close();
            }
            catch (ServiceUsageException ex)
            {
                log.Error("Exception encountered while executing operation", ex);
            }
            catch (ServiceApiException ex)
            {
                log.Error("Exception encountered while executing operation", ex);
            }
            catch (SDKException ex)
            {
                log.Error("Exception encountered while executing operation", ex);
            }
            catch (IOException ex)
            {
                log.Error("Exception encountered while executing operation", ex);
            }
            catch (Exception ex)
            {
                log.Error("Exception encountered while executing operation", ex);
            }
        }

        static void ConfigureLogging()
        {
            ILoggerRepository logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        }

        // Generates a string containing a directory structure and file name for the output file.
        private static String CreateOutputFilePath()
        {
            String timeStamp = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH'-'mm'-'ss");
            return ("/output/merge" + timeStamp + ".docx");
        }
    }
}