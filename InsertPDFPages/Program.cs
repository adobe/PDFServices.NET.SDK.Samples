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
using Adobe.PDFServicesSDK.pdfjobs.parameters;
using Adobe.PDFServicesSDK.pdfjobs.parameters.insertpages;
using Adobe.PDFServicesSDK.pdfjobs.results;
using log4net;
using log4net.Config;
using log4net.Repository;

/// <summary>
/// This sample illustrates how to insert specific pages of multiple PDF files into a single PDF file.
/// <para/>
/// Refer to README.md for instructions on how to run the samples.
/// </summary>
namespace InsertPDFPages
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

                // Creates an asset from source file and upload
                using Stream baseInputStream = File.OpenRead(@"baseInput.pdf");
                using Stream firstInputStream = File.OpenRead(@"firstFileToInsertInput.pdf");
                using Stream secondInputStream = File.OpenRead(@"secondFileToInsertInput.pdf");
                IAsset baseAsset = pdfServices.Upload(baseInputStream, PDFServicesMediaType.PDF.GetMIMETypeValue());
                IAsset firstAssetToInsert =
                    pdfServices.Upload(firstInputStream, PDFServicesMediaType.PDF.GetMIMETypeValue());
                IAsset secondAssetToInsert =
                    pdfServices.Upload(secondInputStream, PDFServicesMediaType.PDF.GetMIMETypeValue());

                PageRanges pageRanges = GetPageRangeForFirstFile();

                // Create parameters for the job
                InsertPagesParams insertPagesParams = InsertPagesParams.InsertPagesParamsBuilder(baseAsset)
                    .AddPagesToInsertAt(firstAssetToInsert, pageRanges, 2)
                    .AddPagesToInsertAt(secondAssetToInsert, 3)
                    .Build();

                // Creates a new job instance
                InsertPagesPDFJob insertPagesPDFJob = new InsertPagesPDFJob(insertPagesParams);

                // Submits the job and gets the job result
                String location = pdfServices.Submit(insertPagesPDFJob);
                PDFServicesResponse<InsertPagesResult> pdfServicesResponse =
                    pdfServices.GetJobResult<InsertPagesResult>(location, typeof(InsertPagesResult));

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

        private static PageRanges GetPageRangeForFirstFile()
        {
            // Specify which pages of the first file are to be inserted in the base file.
            PageRanges pageRanges = new PageRanges();
            // Add pages 1 to 3.
            pageRanges.AddRange(1, 3);

            // Add page 4.
            pageRanges.AddSinglePage(4);

            return pageRanges;
        }

        static void ConfigureLogging()
        {
            ILoggerRepository logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        }

        // Generates a string containing a directory structure and file name for the output file.
        private static string CreateOutputFilePath()
        {
            String timeStamp = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH'-'mm'-'ss");
            return ("/output/insert" + timeStamp + ".pdf");
        }
    }
}