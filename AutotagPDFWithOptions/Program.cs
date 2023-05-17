/*
 * Copyright 2023 Adobe
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
using log4net;
using log4net.Config;
using System.Reflection;
using log4net.Repository;
using Adobe.PDFServicesSDK;
using Adobe.PDFServicesSDK.auth;
using Adobe.PDFServicesSDK.pdfops;
using Adobe.PDFServicesSDK.io;
using Adobe.PDFServicesSDK.exception;
using Adobe.PDFServicesSDK.io.autotag;
using Adobe.PDFServicesSDK.options.autotag;

/// <summary>
/// This sample illustrates how to generate a tagged PDF along with a report and shift the headings in
/// the output PDF file.
/// <para/>
/// Refer to README.md for instructions on how to run the samples.
/// </summary>
namespace AutotagPDFWithOptions
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

                // Create an ExecutionContext using credentials and create a new operation instance.
                ExecutionContext executionContext = ExecutionContext.Create(credentials);
                AutotagPDFOperation autotagPDFOperation = AutotagPDFOperation.CreateNew();

                // Provide an input FileRef for the operation
                autotagPDFOperation.SetInput(FileRef.CreateFromLocalFile(@"autotagPdfInput.pdf"));

                // Build AutotagPDF options and set them into the operation
                AutotagPDFOptions autotagPDFOptions = AutotagPDFOptions.AutotagPDFOptionsBuilder()
                    .ShiftHeadings()
                    .GenerateReport()
                    .Build();
                autotagPDFOperation.SetOptions(autotagPDFOptions);

                // Execute the operation
                AutotagPDFOutput autotagPDFOutput = autotagPDFOperation.Execute(executionContext);

                // Save the output files at the specified location
                autotagPDFOutput.GetTaggedPDF()
                    .SaveAs(Directory.GetCurrentDirectory() + CreateOutputFilePathForTaggedPDF());
                autotagPDFOutput.GetReport()
                    .SaveAs(Directory.GetCurrentDirectory() + CreateOutputFilePathForTaggingReport());
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

        // Generates a string containing a directory structure and file name for the output file.
        public static string CreateOutputFilePathForTaggedPDF()
        {
            String timeStamp = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH'-'mm'-'ss");
            return "/output/AutotagPDF-tagged" + timeStamp + ".pdf";
        }

        // Generates a string containing a directory structure and file name for the output file.
        public static string CreateOutputFilePathForTaggingReport()
        {
            String timeStamp = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH'-'mm'-'ss");
            return "/output/AutotagPDF-report" + timeStamp + ".xlsx";
        }
    }
}