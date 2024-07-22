/*
 * Copyright 2024 Adobe
 * All Rights Reserved.
 *
 * NOTICE: Adobe permits you to use, modify, and distribute this file in 
 * accordance with the terms of the Adobe license agreement accompanying it.
 */

using System;
using System.IO;
using System.Reflection;
using Adobe.PDFServicesSDK;
using Adobe.PDFServicesSDK.auth;
using Adobe.PDFServicesSDK.exception;
using Adobe.PDFServicesSDK.io;
using Adobe.PDFServicesSDK.pdfjobs.jobs;
using Adobe.PDFServicesSDK.pdfjobs.parameters.protectpdf;
using Adobe.PDFServicesSDK.pdfjobs.results;
using log4net;
using log4net.Config;
using log4net.Repository;

/// <summary>
///     This sample illustrates how to secure a PDF file with owner password and allow certain access permissions
///     such as copying and editing the contents, and printing of the document at low resolution.
///     <para />
///     Refer to README.md for instructions on how to run the samples.
/// </summary>
namespace ProtectPDFWithOwnerPassword
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
                // Initial setup, create credentials instance
                ICredentials credentials = new ServicePrincipalCredentials(
                    Environment.GetEnvironmentVariable("PDF_SERVICES_CLIENT_ID"),
                    Environment.GetEnvironmentVariable("PDF_SERVICES_CLIENT_SECRET"));

                // Creates a PDF Services instance
                PDFServices pdfServices = new PDFServices(credentials);

                // Creates an asset(s) from source file(s) and upload
                using Stream inputStream = File.OpenRead(@"protectPDFInput.pdf");
                IAsset asset = pdfServices.Upload(inputStream, PDFServicesMediaType.PDF.GetMIMETypeValue());

                // Create new permissions instance and add the required permissions
                Permissions permissions = new Permissions();
                permissions.AddPermission(Permission.PRINT_LOW_QUALITY);
                permissions.AddPermission(Permission.EDIT_DOCUMENT_ASSEMBLY);
                permissions.AddPermission(Permission.COPY_CONTENT);

                // Create parameters for the job
                ProtectPDFParams protectPDFParams = ProtectPDFParams.PasswordProtectParamsBuilder()
                    .SetOwnerPassword("password")
                    .SetPermissions(permissions)
                    .SetEncryptionAlgorithm(EncryptionAlgorithm.AES_256)
                    .SetContentEncryption(ContentEncryption.ALL_CONTENT_EXCEPT_METADATA)
                    .Build();

                // Creates a new job instance
                ProtectPDFJob protectPDFJob = new ProtectPDFJob(asset, protectPDFParams);

                // Submits the job and gets the job result
                String location = pdfServices.Submit(protectPDFJob);
                PDFServicesResponse<ProtectPDFResult> pdfServicesResponse =
                    pdfServices.GetJobResult<ProtectPDFResult>(location, typeof(ProtectPDFResult));

                // Get content from the resulting asset(s)
                IAsset resultAsset = pdfServicesResponse.Result.Asset;
                StreamAsset streamAsset = pdfServices.GetContent(resultAsset);

                // Creating output streams and copying stream asset's content to it
                String outputFilePath = CreateOutputFilePath();
                new FileInfo(Directory.GetCurrentDirectory() + outputFilePath).Directory.Create();
                Stream outputStream = File.OpenWrite(Directory.GetCurrentDirectory() + outputFilePath);
                streamAsset.Stream.CopyTo(outputStream);
                outputStream.Close();
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
        private static String CreateOutputFilePath()
        {
            String timeStamp = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH'-'mm'-'ss");
            return ("/output/protect" + timeStamp + ".pdf");
        }
    }
}