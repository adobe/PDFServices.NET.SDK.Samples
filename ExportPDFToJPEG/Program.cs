﻿/*
 * Copyright 2024 Adobe
 * All Rights Reserved.
 *
 * NOTICE: Adobe permits you to use, modify, and distribute this file in 
 * accordance with the terms of the Adobe license agreement accompanying it.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Adobe.PDFServicesSDK;
using Adobe.PDFServicesSDK.auth;
using Adobe.PDFServicesSDK.exception;
using Adobe.PDFServicesSDK.io;
using Adobe.PDFServicesSDK.pdfjobs.jobs;
using Adobe.PDFServicesSDK.pdfjobs.parameters.exportpdftoimages;
using Adobe.PDFServicesSDK.pdfjobs.results;
using log4net;
using log4net.Config;
using log4net.Repository;

/// <summary>
/// This sample illustrates how to export a PDF file to a list of JPEG files.
/// <para/>
/// Refer to README.md for instructions on how to run the samples.
/// </summary>
namespace ExportPDFToJPEG
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
                using Stream inputStream = File.OpenRead(@"exportPDFToImagesInput.pdf");
                IAsset asset = pdfServices.Upload(inputStream, PDFServicesMediaType.PDF.GetMIMETypeValue());

                // Create parameters for the job
                ExportPDFToImagesParams exportPDFToImagesParams = ExportPDFToImagesParams
                    .ExportPDFToImagesParamsBuilder(ExportPDFToImagesTargetFormat.JPEG,
                        ExportPDFToImagesOutputType.LIST_OF_PAGE_IMAGES)
                    .Build();

                // Creates a new job instance
                ExportPDFToImagesJob exportPDFToImagesJob = new ExportPDFToImagesJob(asset, exportPDFToImagesParams);

                // Submits the job and gets the job result
                String location = pdfServices.Submit(exportPDFToImagesJob);
                PDFServicesResponse<ExportPDFToImagesResult> pdfServicesResponse =
                    pdfServices.GetJobResult<ExportPDFToImagesResult>(location, typeof(ExportPDFToImagesResult));
                List<IAsset> resultAssets = pdfServicesResponse.Result.Assets;

                //Generating a file name
                String outputFilePath = CreateOutputFilePath();

                // Save the result to the specified location.
                int index = 0;
                foreach (IAsset resultAsset in resultAssets)
                {
                    // Get content from the resulting asset(s)
                    StreamAsset streamAsset = pdfServices.GetContent(resultAsset);

                    // Creating output streams and copying stream asset's content to it
                    new FileInfo(Directory.GetCurrentDirectory() + String.Format(outputFilePath, index)).Directory
                        .Create();
                    Stream outputStream =
                        File.OpenWrite(Directory.GetCurrentDirectory() + String.Format(outputFilePath, index));
                    streamAsset.Stream.CopyTo(outputStream);
                    outputStream.Close();
                    index++;
                }
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

        // Generates a string containing a directory structure and indexed file name for the output file.
        private static String CreateOutputFilePath()
        {
            String timeStamp = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH'-'mm'-'ss");
            return ("/output/export" + timeStamp + "_{0}.jpeg");
        }
    }
}