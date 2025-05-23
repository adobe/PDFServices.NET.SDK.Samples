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
using Adobe.PDFServicesSDK.pdfjobs.parameters.importpdfformdata;
using Adobe.PDFServicesSDK.pdfjobs.results;
using log4net;
using log4net.Config;
using log4net.Repository;
using Newtonsoft.Json.Linq;

/// <summary>
/// This sample demonstrates how to use Adobe PDF Services SDK to import form data into a PDF form.
/// The process involves uploading a source PDF, providing form data in JSON format, and submitting
/// an import form data job.
/// <para/>
/// Refer to README.md for instructions on how to run the samples.
/// </summary>
namespace ImportPDFFormData
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
                using Stream inputStream = File.OpenRead(@"importPdfFormDataInput.pdf");
                IAsset asset = pdfServices.Upload(inputStream, PDFServicesMediaType.PDF.GetMIMETypeValue());

                // Create parameters for the job
                var formData = new JObject(
                    new JProperty("option_two", "Yes"),
                    new JProperty("option_one", "Yes"),
                    new JProperty("name", "sufia"),
                    new JProperty("option_three", "Off"),
                    new JProperty("age", "25"),
                    new JProperty("favorite_movie", "Star Wars Again")
                );

                ImportPDFFormDataParams importParams = ImportPDFFormDataParams.ImportPDFFormDataParamsBuilder()
                    .WithJsonFormFieldsData(formData)
                    .Build();

                // Creates a new job instance
                ImportPDFFormDataJob importPDFFormDataJob = new ImportPDFFormDataJob(asset);
                importPDFFormDataJob.SetParams(importParams);

                // Submits the job and gets the job result
                String location = pdfServices.Submit(importPDFFormDataJob);
                PDFServicesResponse<ImportPDFFormDataResult> pdfServicesResponse =
                    pdfServices.GetJobResult<ImportPDFFormDataResult>(location, typeof(ImportPDFFormDataResult));

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

        private static String CreateOutputFilePath()
        {
            String timeStamp = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH'-'mm'-'ss");
            return ("/output/ImportPDFFormData/setFormData" + timeStamp + ".pdf");
        }
    }
}