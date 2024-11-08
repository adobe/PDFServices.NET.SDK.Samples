﻿/*
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
using Adobe.PDFServicesSDK.pdfjobs.parameters.htmltopdf;
using Adobe.PDFServicesSDK.pdfjobs.results;
using log4net;
using log4net.Config;
using log4net.Repository;
using Newtonsoft.Json.Linq;

/// <summary>
/// This sample illustrates how to provide data inputs to an HTML file before converting it to PDF. The data input is used
/// by the javascript in the HTML file to manipulate the HTML DOM, thus effectively updating the source HTML file.
/// This mechanismn can be used to provide data to the template HTML dynamically and convert it into a PDF file.
/// <para>
/// Refer to README.md for instructions on how to run the samples.
/// </para>
/// </summary>

namespace DynamicHTMLToPDF
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
                using Stream inputStream = File.OpenRead(@"createPDFFromDynamicHtmlInput.zip");
                IAsset asset = pdfServices.Upload(inputStream, PDFServicesMediaType.ZIP.GetMIMETypeValue());

                // Create parameters for the job
                HTMLToPDFParams htmlToPDFParams = GetHTMLToPDFParams();

                // Creates a new job instance
                HTMLToPDFJob htmlToPDFJob = new HTMLToPDFJob(asset).SetParams(htmlToPDFParams);

                // Submits the job and gets the job result
                String location = pdfServices.Submit(htmlToPDFJob);
                PDFServicesResponse<HTMLToPDFResult> pdfServicesResponse =
                    pdfServices.GetJobResult<HTMLToPDFResult>(location, typeof(HTMLToPDFResult));

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

        private static HTMLToPDFParams GetHTMLToPDFParams()
        {
            // Define the page layout, in this case an 8 x 11.5 inch page (effectively portrait orientation).
            PageLayout pageLayout = new PageLayout();
            pageLayout.SetPageSize(8, 11.5);

            //Set the dataToMerge field that needs to be populated in the HTML before its conversion
            JObject dataToMerge = new JObject
            {
                { "title", "Create, Convert PDFs and More!" },
                { "sub_title", "Easily integrate PDF actions within your document workflows." }
            };

            // Set the desired HTML-to-PDF conversion options.
            HTMLToPDFParams htmlToPDFParams = HTMLToPDFParams.HTMLToPDFParamsBuilder()
                .IncludeHeaderFooter(true)
                .WithPageLayout(pageLayout)
                .WithDataToMerge(dataToMerge)
                .Build();
            return htmlToPDFParams;
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
            return ("/output/create" + timeStamp + ".pdf");
        }
    }
}