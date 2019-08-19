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
using Adobe.DocumentCloud.Services;
using Adobe.DocumentCloud.Services.pdfops;
using Adobe.DocumentCloud.Services.options.exportpdf;
using Adobe.DocumentCloud.Services.io;
using Adobe.DocumentCloud.Services.exception;

/// <summary>
/// This sample illustrates how to export a PDF file to JPEG.
/// <para/>
/// Note that exporting a PDF file to an image format results in a ZIP archive containing one image per page.
/// <para/>
/// Refer to README.md for instructions on how to run the samples.
/// </summary>
namespace ExportPDFToImage
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
                // Initial setup, create a ClientContext and a new operation instance by specifying the intended export format.
                ClientContext clientContext = ClientContext.CreateFromFile(Directory.GetCurrentDirectory() + "/dc-services-sdk-config.json");
                ExportPDFOperation exportPdfOperation = ExportPDFOperation.CreateNew(ExportPDFTargetFormat.JPEG);

                // Set operation input from a source file.
                FileRef sourceFileRef = FileRef.CreateFromLocalFile(@"exportPdfToImageInput.pdf");
                exportPdfOperation.SetInput(sourceFileRef);

                // Execute the operation.
                FileRef result = exportPdfOperation.Execute(clientContext);

                // Save the result to the specified location.
                result.SaveAs(Directory.GetCurrentDirectory() + "/output/exportPdfToImageOutput.zip");
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
    }
}
