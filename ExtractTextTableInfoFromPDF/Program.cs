/*
 * Copyright 2020 Adobe
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
using System.Collections.Generic;
using log4net.Repository;
using log4net.Config;
using log4net;
using System.Reflection;
using Adobe.PDFServicesSDK;
using Adobe.PDFServicesSDK.auth;
using Adobe.PDFServicesSDK.pdfops;
using Adobe.PDFServicesSDK.io;
using Adobe.PDFServicesSDK.exception;
using Adobe.PDFServicesSDK.options.extractpdf;

/// <summary>
/// This sample illustrates how to extract text and table information from PDF.
/// <para/>
/// Refer to README.md for instructions on how to run the samples and understand output zip file.
/// </summary>
namespace ExtractTextTableInfoFromPDF
{
    class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));
        static void Main()
        {
            // Configure the logging.
            ConfigureLogging();
            try
            {
                // Initial setup, create credentials instance.
                Credentials credentials = Credentials.ServiceAccountCredentialsBuilder()
                    .FromFile(Directory.GetCurrentDirectory() + "/pdfservices-api-credentials.json")
                    .Build();

                // Create an ExecutionContext using credentials and create a new operation instance.
                ExecutionContext executionContext = ExecutionContext.Create(credentials);
                ExtractPDFOperation extractPdfOperation = ExtractPDFOperation.CreateNew();

                // Provide an input FileRef for the operation.
                FileRef sourceFileRef = FileRef.CreateFromLocalFile(@"extractPDFInput.pdf");
                extractPdfOperation.SetInputFile(sourceFileRef);
                
                // Build ExtractPDF options and set them into the operation.
                ExtractPDFOptions extractPdfOptions = ExtractPDFOptions.ExtractPDFOptionsBuilder()
                    .AddElementsToExtract(new List<ExtractElementType>(new []{ ExtractElementType.TEXT, ExtractElementType.TABLES}))
                    .Build();
                extractPdfOperation.SetOptions(extractPdfOptions);
                
                // Execute the operation.
                FileRef result = extractPdfOperation.Execute(executionContext);

                // Save the result to the specified location.
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

        static void ConfigureLogging()
        {
            ILoggerRepository logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        }
        
        //Generates a string containing a directory structure and file name for the output file.
        public static string CreateOutputFilePath()
        {
            String timeStamp = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH'-'mm'-'ss");
            return ("/output/extract" + timeStamp + ".zip");
        }
    }
}
