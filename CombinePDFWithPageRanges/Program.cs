/*
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
using Adobe.PDFServicesSDK.pdfjobs.parameters;
using Adobe.PDFServicesSDK.pdfjobs.parameters.combinepdf;
using Adobe.PDFServicesSDK.pdfjobs.results;
using log4net;
using log4net.Config;
using log4net.Repository;


/// <summary>
/// This sample illustrates how to combine specific pages of multiple PDF files into a single PDF file.
/// <para/>
/// Note that the SDK supports combining upto 20 files in one operation.
/// <para/>
/// Refer to README.md for instructions on how to run the samples.
/// </summary>
namespace CombinePDFWithPageRanges
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

                // Creates an asset(s) from source file(s) and upload
                using Stream inputStream1 = File.OpenRead(@"combineFileWithPageRangeInput1.pdf");
                using Stream inputStream2 = File.OpenRead(@"combineFileWithPageRangeInput2.pdf");
                List<IAsset> assets = pdfServices.UploadAssets(new List<StreamAsset>()
                {
                    new StreamAsset(inputStream1, PDFServicesMediaType.PDF.GetMIMETypeValue()),
                    new StreamAsset(inputStream2, PDFServicesMediaType.PDF.GetMIMETypeValue())
                });

                PageRanges pageRangesForFirstFile = GetPageRangeForFirstFile();
                PageRanges pageRangesForSecondFile = GetPageRangeForSecondFile();

                // Create parameters for the job
                CombinePDFParams combinePDFParams = CombinePDFParams.CombinePDFParamsBuilder()
                    .AddAsset(assets[0], pageRangesForFirstFile)
                    .AddAsset(assets[1], pageRangesForSecondFile)
                    .Build();

                // Creates a new job instance
                CombinePDFJob combinePDFJob = new CombinePDFJob(combinePDFParams);

                // Submits the job and gets the job result
                String location = pdfServices.Submit(combinePDFJob);
                PDFServicesResponse<CombinePDFResult> pdfServicesResponse =
                    pdfServices.GetJobResult<CombinePDFResult>(location, typeof(CombinePDFResult));

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

        private static PageRanges GetPageRangeForSecondFile()
        {
            // Specify which pages of the second file are to be included in the combined file.
            PageRanges pageRangesForSecondFile = new PageRanges();
            // Add all pages including and after page 5.
            pageRangesForSecondFile.AddAllFrom(5);
            return pageRangesForSecondFile;
        }

        private static PageRanges GetPageRangeForFirstFile()
        {
            // Specify which pages of the first file are to be included in the combined file.
            PageRanges pageRangesForFirstFile = new PageRanges();
            // Add page 2.
            pageRangesForFirstFile.AddSinglePage(2);
            // Add page 3.
            pageRangesForFirstFile.AddSinglePage(3);
            // Add pages 5 to 7.
            pageRangesForFirstFile.AddRange(5, 7);
            return pageRangesForFirstFile;
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
            return ("/output/combine" + timeStamp + ".pdf");
        }
    }
}