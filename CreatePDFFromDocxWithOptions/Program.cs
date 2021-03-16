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
using log4net.Repository;
using log4net.Config;
using log4net;
using System.Reflection;
using Adobe.DocumentServices.PDFTools;
using Adobe.DocumentServices.PDFTools.auth;
using Adobe.DocumentServices.PDFTools.pdfops;
using Adobe.DocumentServices.PDFTools.io;
using Adobe.DocumentServices.PDFTools.exception;
using Adobe.DocumentServices.PDFTools.options.createpdf;
using Adobe.DocumentServices.PDFTools.options.createpdf.word;

/// <summary>
/// This sample illustrates how to provide documentLanguage option when creating a pdf file from docx file.
/// <para/>
/// Refer to README.md for instructions on how to run the samples.
/// </summary>
namespace CreatePDFFromDocxWithOptions
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
                                .FromFile(Directory.GetCurrentDirectory() + "/pdftools-api-credentials.json")
                                .Build();

                //Create an ExecutionContext using credentials and create a new operation instance.
                ExecutionContext executionContext = ExecutionContext.Create(credentials);
                CreatePDFOperation createPdfOperation = CreatePDFOperation.CreateNew();

                // Set operation input from a source file.
                FileRef source = FileRef.CreateFromLocalFile(@"createPdfInput.docx");
                createPdfOperation.SetInput(source);

                // Provide any custom configuration options for the operation.
                SetCustomOptions(createPdfOperation);

                // Execute the operation.
                FileRef result = createPdfOperation.Execute(executionContext);

                // Save the result to the specified location.
                result.SaveAs(Directory.GetCurrentDirectory() + "/output/createPDFFromDOCXWithOptionsOutput.pdf");
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
        /// <param name="createPdfOperation">operation instance for which the options are provided.</param>
        private static void SetCustomOptions(CreatePDFOperation createPdfOperation)
        {
            // Select the documentLanguage for input file.
            SupportedDocumentLanguage documentLanguage = SupportedDocumentLanguage.EN_US;

            // Set the desired Word-to-PDF conversion options.
            CreatePDFOptions createPDFOptions = CreatePDFOptions.WordOptionsBuilder()
                .WithDocumentLanguage(documentLanguage)
                .Build();
            createPdfOperation.SetOptions(createPDFOptions);
        }

        static void ConfigureLogging()
        {
            ILoggerRepository logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        }
    }
}
