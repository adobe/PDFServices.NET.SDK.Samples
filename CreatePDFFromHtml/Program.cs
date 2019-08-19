/*
 * Copyright 2019 Adobe
 * All Rights Reserved.
 *
 * NOTICE: Adobe permits you to use, modify, and distribute this file in
 * accordance with the terms of the Adobe license agreement accompanying
 * it. If you have received this file from a source other than Adobe,
 * then your use, modification, or distribution of it requires the prior
 * written permission of Adobe.
 */
using System;
using System.IO;
using log4net.Repository;
using log4net;
using log4net.Config;
using System.Reflection;
using Adobe.DocumentCloud.Services;
using Adobe.DocumentCloud.Services.pdfops;
using Adobe.DocumentCloud.Services.io;
using Adobe.DocumentCloud.Services.exception;
using Adobe.DocumentCloud.Services.options.createpdf;

/// <summary>
/// This sample illustrates how to convert an HTML file to PDF. The HTML file and its associated dependencies must be
/// in a single ZIP file.
/// <para>
/// Refer to README.md for instructions on how to run the samples.
/// </para>
/// </summary>

namespace CreatePDFFromHtml
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
                // Initial setup, create a ClientContext and a new operation instance.
                ClientContext clientContext = ClientContext.CreateFromFile(Directory.GetCurrentDirectory() + "/dc-services-sdk-config.json");
                CreatePDFOperation htmlToPDFOperation = CreatePDFOperation.CreateNew();

                // Set operation input from a source file.
                FileRef source = FileRef.CreateFromLocalFile(@"createPdfFromHtmlInput.zip");
                htmlToPDFOperation.SetInput(source);

                // Provide any custom configuration options for the operation.
                SetCustomOptions(htmlToPDFOperation);

                // Execute the operation.
                FileRef result = htmlToPDFOperation.Execute(clientContext);

                // Save the result to the specified location.
                result.SaveAs(Directory.GetCurrentDirectory() + "/output/createPdfFromHtmlOutput.pdf");
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


        /// <summary>
        /// Sets any custom options for the operation.
        /// </summary>
        /// <param name="htmlToPDFOperation">operation instance for which the options are provided.</param>
        private static void SetCustomOptions(CreatePDFOperation htmlToPDFOperation)
        {
            // Define the page layout, in this case an 8 x 11.5 inch page (effectively portrait orientation).
            PageLayout pageLayout = new PageLayout();
            pageLayout.SetPageSize(8, 11.5);
            // Set the desired HTML-to-PDF conversion options.
            CreatePDFOptions htmlToPdfOptions = CreatePDFOptions.HtmlOptionsBuilder()
                    .IncludeHeaderFooter(true)
                    .WithPageLayout(pageLayout)
                    . Build();
            htmlToPDFOperation.SetOptions(htmlToPdfOptions);
        }


        static void ConfigureLogging()
        {
            ILoggerRepository logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        }
    }
}
