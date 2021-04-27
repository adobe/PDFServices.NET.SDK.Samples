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
using Adobe.PDFServicesSDK.options.rotatepages;
using Adobe.PDFServicesSDK.exception;

/// <summary>
/// This sample illustrates how to rotate pages in a PDF file.
/// <para/>
/// Refer to README.md for instructions on how to run the samples.
/// </summary>
namespace RotatePDFPages
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
                RotatePagesOperation rotatePagesOperation = RotatePagesOperation.CreateNew();

                // Set operation input from a source file.
                FileRef sourceFileRef = FileRef.CreateFromLocalFile(@"rotatePagesInput.pdf");
                rotatePagesOperation.SetInput(sourceFileRef);

                // Sets angle by 90 degrees (in clockwise direction) for rotating the specified pages of
                // the input PDF file.
                PageRanges firstPageRange = GetFirstPageRangeForRotation();
                rotatePagesOperation.SetAngleToRotatePagesBy(Angle._90, firstPageRange);

                // Sets angle by 180 degrees (in clockwise direction) for rotating the specified pages of
                // the input PDF file.
                PageRanges secondPageRange = GetSecondPageRangeForRotation();
                rotatePagesOperation.SetAngleToRotatePagesBy(Angle._180, secondPageRange);

                // Execute the operation.
                FileRef result = rotatePagesOperation.Execute(executionContext);

                // Save the result to the specified location.
                result.SaveAs(Directory.GetCurrentDirectory() + "/output/rotatePagesOutput.pdf");
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

        private static PageRanges GetFirstPageRangeForRotation()
        {
            // Specify pages for rotation.
            PageRanges firstPageRange = new PageRanges();
            // Add page 1.
            firstPageRange.AddSinglePage(1);

            // Add pages 3 to 4.
            firstPageRange.AddRange(3, 4);
            return firstPageRange;
        }

        private static PageRanges GetSecondPageRangeForRotation()
        {
            // Specify pages for rotation.
            PageRanges secondPageRange = new PageRanges();
            // Add page 2.
            secondPageRange.AddSinglePage(2);

            return secondPageRange;
        }
    }
}
