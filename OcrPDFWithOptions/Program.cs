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
using Adobe.PDFServicesSDK.options.ocr;
using Adobe.PDFServicesSDK.pdfops;
using Adobe.PDFServicesSDK.io;
using Adobe.PDFServicesSDK.exception;

/// <summary>
/// This sample illustrates how to perform an OCR operation on a PDF file and convert it into an searchable PDF file on
/// the basis of provided locale and SEARCHABLE_IMAGE_EXACT ocr type to keep the original image
/// (Recommended for cases requiring maximum fidelity to the original image.).
/// <para/>
/// Note that OCR operation on a PDF file results in a PDF file.
/// <para/>
/// Refer to README.md for instructions on how to run the samples.
/// </summary>
namespace OcrPDFWithOptions
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
                OCROperation ocrOperation = OCROperation.CreateNew();

                // Set operation input from a source file.
                FileRef sourceFileRef = FileRef.CreateFromLocalFile(@"ocrWithOptionsInput.pdf");
                ocrOperation.SetInput(sourceFileRef);
                // Build OCR options from supported locales and OCR-types and set them into the operation
                OCROptions ocrOptions = OCROptions.OCROptionsBuilder() 
                        .WithOcrLocale(OCRSupportedLocale.EN_US)
                        .WithOcrType(OCRSupportedType.SEARCHABLE_IMAGE_EXACT)
                        .Build();
                ocrOperation.SetOptions(ocrOptions);

                // Execute the operation.
                FileRef result = ocrOperation.Execute(executionContext);

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
            return ("/output/ocr" + timeStamp + ".pdf");
        }
    }
}
