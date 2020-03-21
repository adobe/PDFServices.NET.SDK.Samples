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
using Adobe.DocumentCloud.Services;
using Adobe.DocumentCloud.Services.auth;
using Adobe.DocumentCloud.Services.exception;
using Adobe.DocumentCloud.Services.io;
using Adobe.DocumentCloud.Services.options.exportpdf;
using Adobe.DocumentCloud.Services.pdfops;
using log4net;
using log4net.Config;
using log4net.Repository;
using System;
using System.IO;
using System.Reflection;

/// <summary>
/// This sample illustrates how to export a PDF file to a Word (DOCX) file.
/// <para>
/// Refer to README.md for instructions on how to run the samples.
/// </para>
/// </summary>
namespace ExportPDFToDocx
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
                                .FromFile(Directory.GetCurrentDirectory() + "/dc-services-sdk-credentials.json")
                                .Build();

                //Create an ExecutionContext using credentials and create a new operation instance.
                ExecutionContext executionContext = ExecutionContext.Create(credentials);
                ExportPDFOperation exportPdfOperation = ExportPDFOperation.CreateNew(ExportPDFTargetFormat.DOCX);

                // Set operation input from a local PDF file
                FileRef sourceFileRef = FileRef.CreateFromLocalFile(@"exportPdfInput.pdf");
                exportPdfOperation.SetInput(sourceFileRef);

                // Execute the operation.
                FileRef result = exportPdfOperation.Execute(executionContext);

                // Save the result to the specified location.
                result.SaveAs(Directory.GetCurrentDirectory() + "/output/exportPdfOutput.docx");
            }
            catch (ServiceUsageException ex)
            {
                log.Error("Exception encountered while executing operation", ex);
            }
            catch(ServiceApiException ex)
            {
                log.Error("Exception encountered while executing operation", ex);
            }
            catch(SDKException ex)
            {
                log.Error("Exception encountered while executing operation", ex);
            }
            catch(IOException ex)
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
    }
}
