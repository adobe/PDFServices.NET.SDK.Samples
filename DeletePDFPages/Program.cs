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
using Adobe.PDFServicesSDK.options;
using Adobe.PDFServicesSDK.pdfops;
using Adobe.PDFServicesSDK.io;
using Adobe.PDFServicesSDK.exception;

/// <summary>
/// This sample illustrates how to delete pages in a PDF file.
/// <para/>
/// Refer to README.md for instructions on how to run the samples.
/// </summary>
namespace DeletePDFPages
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
                // Initial setup, create credentials instance.
                Credentials credentials = Credentials.ServiceAccountCredentialsBuilder()
                                .FromFile(Directory.GetCurrentDirectory() + "/pdfservices-api-credentials.json")
                                .Build();

                // Create an ExecutionContext using credentials.
                ExecutionContext executionContext = ExecutionContext.Create(credentials);

                // Create a new operation instance
                DeletePagesOperation deletePagesOperation = DeletePagesOperation.CreateNew();

                // Set operation input from a source file.
                FileRef sourceFileRef = FileRef.CreateFromLocalFile(@"deletePagesInput.pdf");
                deletePagesOperation.SetInput(sourceFileRef);

                // Delete pages of the document (as specified by PageRanges).
                PageRanges pageRangeForDeletion = GetPageRangeForDeletion();
                deletePagesOperation.SetPageRanges(pageRangeForDeletion);

                // Execute the operation.
                FileRef result = deletePagesOperation.Execute(executionContext);

                // Save the result to the specified location.
                result.SaveAs(Directory.GetCurrentDirectory() + "/output/deletePagesOutput.pdf");
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

        private static PageRanges GetPageRangeForDeletion()
        {
            // Specify pages for deletion.
            PageRanges pageRangeForDeletion = new PageRanges();
            // Add page 1.
            pageRangeForDeletion.AddSinglePage(1);

            // Add pages 3 to 4.
            pageRangeForDeletion.AddRange(3, 4);
            return pageRangeForDeletion;
        }

        static void ConfigureLogging()
        {
            ILoggerRepository logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        }
    }
}
