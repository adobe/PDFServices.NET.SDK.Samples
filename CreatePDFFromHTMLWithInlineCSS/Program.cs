/*
 * Copyright 2021 Adobe
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
using System.Reflection;
using Adobe.PDFServicesSDK;
using Adobe.PDFServicesSDK.auth;
using Adobe.PDFServicesSDK.exception;
using Adobe.PDFServicesSDK.io;
using Adobe.PDFServicesSDK.options.createpdf;
using Adobe.PDFServicesSDK.pdfops;
using log4net;
using log4net.Config;
using log4net.Repository;

/// <summary>
/// This sample illustrates how to create a PDF file from an HTML file with inline CSS.
/// <para>
/// Refer to README.md for instructions on how to run the samples.
/// </para>
/// </summary>
namespace CreatePDFFromHTMLWithInlineCSS
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
                // Initial setup, create credentials instance.
                Credentials credentials = Credentials.ServiceAccountCredentialsBuilder()
                                .FromFile(Directory.GetCurrentDirectory() + "/pdfservices-api-credentials.json")
                                .Build();

                //Create an ExecutionContext using credentials and create a new operation instance.
                ExecutionContext executionContext = ExecutionContext.Create(credentials);
                CreatePDFOperation htmlToPDFOperation = CreatePDFOperation.CreateNew();

                // Set operation input from a source file.
                FileRef source = FileRef.CreateFromLocalFile(@"createPDFFromHTMLWithInlineCSSInput.html");
                htmlToPDFOperation.SetInput(source);

                // Provide any custom configuration options for the operation.
                SetCustomOptions(htmlToPDFOperation);
                
                // Execute the operation.
                FileRef result = htmlToPDFOperation.Execute(executionContext);

                //Generating a file name
                String outputFilePath = CreateOutputFilePath();
                
                // Save the result to the specified location.
                result.SaveAs(Directory.GetCurrentDirectory() + outputFilePath);
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

        /// <summary>
        /// Sets any custom options for the operation.
        /// </summary>
        /// <param name="htmlToPDFOperation">operation instance for which the options are provided.</param>
        private static void SetCustomOptions(CreatePDFOperation htmlToPDFOperation)
        {
            // Define the page layout, in this case an 20 x 25 inch page (effectively portrait orientation).
            PageLayout pageLayout = new PageLayout();
            pageLayout.SetPageSize(20, 25);

            // Set the desired HTML-to-PDF conversion options.
            CreatePDFOptions htmlToPdfOptions = CreatePDFOptions.HtmlOptionsBuilder()
                .IncludeHeaderFooter(true)
                .WithPageLayout(pageLayout)
                .Build();
            htmlToPDFOperation.SetOptions(htmlToPdfOptions);
        }
        
        static void ConfigureLogging()
        {
            ILoggerRepository logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        }
        
        //Generates a string containing a directory structure and file name for the output file.
        public static string CreateOutputFilePath()
        {
            String timeStamp = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH'-'mm'-'ss");
            return ("/output/create" + timeStamp + ".pdf");
        }
    }
}
