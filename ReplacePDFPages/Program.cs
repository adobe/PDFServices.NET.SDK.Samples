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

using System.IO;
using System;
using log4net.Repository;
using log4net.Config;
using log4net;
using System.Reflection;
using Adobe.PDFServicesSDK;
using Adobe.PDFServicesSDK.auth;
using Adobe.PDFServicesSDK.pdfops;
using Adobe.PDFServicesSDK.io;
using Adobe.PDFServicesSDK.exception;
using Adobe.PDFServicesSDK.options;

/// <summary>
/// This sample illustrates how to replace specific pages in a PDF file.
/// <para/>
/// Refer to README.md for instructions on how to run the samples.
/// </summary>
namespace ReplacePDFPages
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

                // Create an ExecutionContext using credentials.
                ExecutionContext executionContext = ExecutionContext.Create(credentials);

                // Create a new operation instance
                ReplacePagesOperation replacePagesOperation = ReplacePagesOperation.CreateNew();

                // Set operation base input from a source file.
                FileRef baseSourceFile = FileRef.CreateFromLocalFile(@"baseInput.pdf");
                replacePagesOperation.SetBaseInput(baseSourceFile);

                // Create a FileRef instance using a local file.
                FileRef firstInputFile = FileRef.CreateFromLocalFile(@"replacePagesInput1.pdf");
                PageRanges pageRanges = GetPageRangeForFirstFile();

                // Adds the pages (specified by the page ranges) of the input PDF file for replacing the
                // page of the base PDF file.
                replacePagesOperation.AddPagesForReplace(firstInputFile, pageRanges, 1);

                // Create a FileRef instance using a local file.
                FileRef secondInputFile = FileRef.CreateFromLocalFile(@"replacePagesInput2.pdf");

                // Adds all the pages of the input PDF file for replacing the page of the base PDF file.
                replacePagesOperation.AddPagesForReplace(secondInputFile, 3);

                // Execute the operation.
                FileRef result = replacePagesOperation.Execute(executionContext);

                // Save the result to the specified location.
                result.SaveAs(Directory.GetCurrentDirectory() + "/output/replacePagesOutput.pdf");
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
            // Specify pages of the first file for replacing the page of base PDF file.
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
    }
}
