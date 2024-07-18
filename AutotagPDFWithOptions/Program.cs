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
using Adobe.PDFServicesSDK.pdfjobs.parameters.autotag;
using Adobe.PDFServicesSDK.pdfjobs.results;
using log4net;
using log4net.Config;
using log4net.Repository;

/// <summary>
/// This sample illustrates how to generate a tagged PDF along with a report and shift the headings in
/// the output PDF file.
/// <para/>
/// Refer to README.md for instructions on how to run the samples.
/// </summary>
namespace AutotagPDFWithOptions
{
    class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));

        static void Main()
        {
            // Configure the logging
            ConfigureLogging();
            try
            {
                // Initial setup, create credentials instance
                ICredentials credentials = new ServicePrincipalCredentials(
                    Environment.GetEnvironmentVariable("PDF_SERVICES_CLIENT_ID"),
                    Environment.GetEnvironmentVariable("PDF_SERVICES_CLIENT_SECRET"));

                // Creates a PDF Services instance
                PDFServices pdfServices = new PDFServices(credentials);

                // Creates an asset(s) from source file(s) and upload
                using Stream inputStream = File.OpenRead(@"autotagPdfInput.pdf");
                IAsset asset = pdfServices.Upload(inputStream, PDFServicesMediaType.PDF.GetMIMETypeValue());

                // Create parameters for the job
                AutotagPDFParams autotagPDFParams = AutotagPDFParams.AutotagPDFParamsBuilder().GenerateReport().Build();

                // Creates a new job instance
                AutotagPDFJob autotagPDFJob = new AutotagPDFJob(asset).SetParams(autotagPDFParams);

                // Submits the job and gets the job result
                String location = pdfServices.Submit(autotagPDFJob);
                PDFServicesResponse<AutotagPDFResult> pdfServicesResponse =
                    pdfServices.GetJobResult<AutotagPDFResult>(location, typeof(AutotagPDFResult));

                // Get content from the resulting asset(s)
                IAsset resultAsset = pdfServicesResponse.Result.TaggedPDF;
                IAsset resultAssetReport = pdfServicesResponse.Result.Report;
                StreamAsset streamAsset = pdfServices.GetContent(resultAsset);
                StreamAsset streamAssetReport = pdfServices.GetContent(resultAssetReport);

                // Creating output streams and copying stream asset's content to it
                String outputFilePathForTaggedPDF = CreateOutputFilePathForTaggedPDF();
                String outputFilePathForReport = CreateOutputFilePathForTaggingReport();
                new FileInfo(Directory.GetCurrentDirectory() + outputFilePathForTaggedPDF).Directory.Create();
                new FileInfo(Directory.GetCurrentDirectory() + outputFilePathForReport).Directory.Create();
                Stream outputStream = File.OpenWrite(Directory.GetCurrentDirectory() + outputFilePathForTaggedPDF);
                Stream outputStreamReport =
                    File.OpenWrite(Directory.GetCurrentDirectory() + CreateOutputFilePathForTaggingReport());
                streamAsset.Stream.CopyTo(outputStream);
                streamAssetReport.Stream.CopyTo(outputStreamReport);
                outputStream.Close();
                outputStreamReport.Close();
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
        private static String CreateOutputFilePathForTaggedPDF()
        {
            String timeStamp = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH'-'mm'-'ss");
            return "/output/AutotagPDF-tagged" + timeStamp + ".pdf";
        }

        // Generates a string containing a directory structure and file name for the output file.
        private static String CreateOutputFilePathForTaggingReport()
        {
            String timeStamp = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH'-'mm'-'ss");
            return "/output/AutotagPDF-report" + timeStamp + ".xlsx";
        }
    }
}