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
using Adobe.PDFServicesSDK.pdfjobs.results;
using log4net;
using log4net.Config;
using log4net.Repository;

/// <summary>
/// This sample illustrates how to perform PDF Watermark operation on a PDF file.
/// Note that PDF Watermark operation on a PDF file results in a PDF file.
/// <para/>
/// Refer to README.md for instructions on how to run the samples.
/// </summary>
namespace PDFWatermark
{
    internal class Program
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
                
                PDFServices pdfServices = new PDFServices(credentials);

                // Creates an asset(s) from source file(s) and upload
                Stream sourceFileInputStream = File.OpenRead(@"pdfWatermarkInput.pdf");
                IAsset inputDocumentAsset = pdfServices.Upload(sourceFileInputStream, PDFServicesMediaType.PDF.GetMIMETypeValue());
                
                // Creates a watermark asset from source file(s) and upload
                Stream watermarkFileInputStream = File.OpenRead(@"watermark.pdf");
                IAsset watermarkDocumentAsset = pdfServices.Upload(watermarkFileInputStream, PDFServicesMediaType.PDF.GetMIMETypeValue());

                // Submits the job and gets the job result
                PDFWatermarkJob pdfWatermarkJob = new PDFWatermarkJob(inputDocumentAsset, watermarkDocumentAsset);
                String location = pdfServices.Submit(pdfWatermarkJob);

                // Get content from the resulting asset(s)
                PDFServicesResponse<PDFWatermarkResult> pdfServicesResponse =
                    pdfServices.GetJobResult<PDFWatermarkResult>(location, typeof(PDFWatermarkResult));
                
                // Creating output streams and copying stream asset's content to it
                IAsset resultAsset = pdfServicesResponse.Result.Asset;
                StreamAsset streamAsset = pdfServices.GetContent(resultAsset);

                String outputFilePath = CreateOutputFilePath();
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

        private static String CreateOutputFilePath()
        {
            String timeStamp = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH'-'mm'-'ss");
            return ("/output/watermark" + timeStamp + ".pdf");
        }
    }
}