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
using Adobe.PDFServicesSDK.io;
using Adobe.PDFServicesSDK.pdfjobs;
using Adobe.PDFServicesSDK.pdfjobs.parameters.pdfaccessibilitychecker;
using Adobe.PDFServicesSDK.exception;
using Adobe.PDFServicesSDK.pdfjobs.jobs;
using Adobe.PDFServicesSDK.pdfjobs.results;
using log4net;
using log4net.Config;
using log4net.Repository;

/// <summary>
/// This sample illustrates how to run accessibility Checker on input PDF file for given page start and page end.
/// <para/>
/// Refer to README.md for instructions on how to run the samples.
/// </summary>
namespace PDFAccessibilityCheckerWithOptions
{
    public class Program
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

                // Creates an asset(s) from source file(s) and upload
                using Stream inputStream = File.OpenRead(@"checkerPDFInput.pdf");
                IAsset inputDocumentAsset =
                    pdfServices.Upload(inputStream, PDFServicesMediaType.PDF.GetMIMETypeValue());

                // Set up PDF Accessibility Checker parameters
                PDFAccessibilityCheckerParams pdfAccessibilityCheckerParams = PDFAccessibilityCheckerParams
                    .PDFAccessibilityCheckerParamsBuilder()
                    .WithPageStart(1)
                    .WithPageEnd(3)
                    .Build();

                // Create the PDF Accessibility Checker job instance
                PDFAccessibilityCheckerJob pdfAccessibilityCheckerJob =
                    new PDFAccessibilityCheckerJob(inputDocumentAsset).SetParams(pdfAccessibilityCheckerParams);

                // Submits the job and gets the job result
                string location = pdfServices.Submit(pdfAccessibilityCheckerJob);
                PDFServicesResponse<PDFAccessibilityCheckerResult> pdfServicesResponse =
                    pdfServices.GetJobResult<PDFAccessibilityCheckerResult>(location,
                        typeof(PDFAccessibilityCheckerResult));

                // Get content from the resulting asset(s)
                IAsset outputAsset = pdfServicesResponse.Result.Asset;
                StreamAsset streamAsset = pdfServices.GetContent(outputAsset);

                IAsset outputReportAsset = pdfServicesResponse.Result.Report;
                StreamAsset streamReportAsset = pdfServices.GetContent(outputReportAsset);

                // Creating output streams and copying stream asset's content to it
                String outputPdfPath = CreateOutputPDFPath();
                new FileInfo(Directory.GetCurrentDirectory() + outputPdfPath).Directory.Create();
                Stream outputStream = File.OpenWrite(Directory.GetCurrentDirectory() + outputPdfPath);
                streamAsset.Stream.CopyTo(outputStream);
                outputStream.Close();

                String outputJSONPath = CreateOutputJSONPath();
                new FileInfo(Directory.GetCurrentDirectory() + outputJSONPath).Directory.Create();
                Stream outputJSONStream = File.OpenWrite(Directory.GetCurrentDirectory() + outputJSONPath);
                streamReportAsset.Stream.CopyTo(outputJSONStream);
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

        private static String CreateOutputPDFPath()
        {
            String timeStamp = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH'-'mm'-'ss");
            return ("/output/accessibilityChecker" + timeStamp + ".pdf");
        }

        private static String CreateOutputJSONPath()
        {
            String timeStamp = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH'-'mm'-'ss");
            return ("/output/accessibilityChecker" + timeStamp + ".json");
        }
    }
}