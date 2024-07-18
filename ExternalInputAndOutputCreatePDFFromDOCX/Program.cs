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
using System.Threading;
using Adobe.PDFServicesSDK;
using Adobe.PDFServicesSDK.auth;
using Adobe.PDFServicesSDK.exception;
using Adobe.PDFServicesSDK.io;
using Adobe.PDFServicesSDK.pdfjobs.jobs;
using Adobe.PDFServicesSDK.pdfjobs.results;
using log4net;
using log4net.Config;
using log4net.Repository;

/// <summary>
/// This sample illustrates how to use external storage as input and output in PDF Services.
/// For this illustration a PDF file will be created and stored externally from a DOCX file stored externally.
/// <para/>
/// Refer to README.md for instructions on how to run the samples.
/// </summary>
namespace ExternalInputAndOutputCreatePDFFromDOCX
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

                // Creating external assets from pre signed URLs.
                String inputPreSignedURL = "INPUT_PRESIGNED_URL";
                String outputPreSignedURL = "OUTPUT_PRESIGNED_URL";
                IAsset inputExternalAsset = new ExternalAsset(inputPreSignedURL, ExternalStorageType.S3);
                IAsset outputExternalAsset = new ExternalAsset(outputPreSignedURL, ExternalStorageType.S3);

                // Creates a new job instance
                CreatePDFJob createPDFJob = new CreatePDFJob(inputExternalAsset).SetOutput(outputExternalAsset);

                // Submits the job and gets the job result
                String location = pdfServices.Submit(createPDFJob);
                PDFServicesResponse<CreatePDFResult> pdfServicesResponse =
                    pdfServices.GetJobResult<CreatePDFResult>(location, typeof(CreatePDFResult));

                // Poll to check job status and wait until job is done
                PDFServicesJobStatusResponse pdfServicesJobStatusResponse = null;
                while (pdfServicesJobStatusResponse == null ||
                       PDFServicesJobStatus.IN_PROGRESS.GetPDFServicesJobStatusValue()
                           .Equals(pdfServicesJobStatusResponse.Status))
                {
                    pdfServicesJobStatusResponse = pdfServices.GetJobStatus(location);
                    // get retry interval from response
                    int retryAfter = pdfServicesJobStatusResponse.GetRetryInterval();
                    Thread.Sleep(retryAfter * 1000);
                }

                log.Info("Output is now available on the provided output external storage.");
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
    }
}