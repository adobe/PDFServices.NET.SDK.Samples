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
using Adobe.PDFServicesSDK.pdfjobs.parameters.autotag;
using Adobe.PDFServicesSDK.pdfjobs.results;
using log4net;
using log4net.Config;
using log4net.Repository;

/// <summary>
/// This sample illustrates how to generate a tagged PDF by setting options with command line arguments.
/// <para/>
/// Refer to README.md for instructions on how to run the samples.
/// </summary>
namespace AutotagPDFParameterised
{
    class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));

        private static AutotagPDFParams GetParamsFromCmdArgs(String[] args)
        {
            Boolean generateReport = GetGenerateReportFromCmdArgs(args);
            Boolean shiftHeadings = GetShiftHeadingsFromCmdArgs(args);

            AutotagPDFParams.Builder builder = AutotagPDFParams.AutotagPDFParamsBuilder();

            if (generateReport) builder.GenerateReport();
            if (shiftHeadings) builder.ShiftHeadings();

            return builder.Build();
        }

        private static Boolean GetShiftHeadingsFromCmdArgs(String[] args)
        {
            return Array.Exists(args, element => element == "--shift_headings");
        }

        private static Boolean GetGenerateReportFromCmdArgs(String[] args)
        {
            return Array.Exists(args, element => element == "--report");
        }

        private static String GetInputFilePathFromCmdArgs(String[] args)
        {
            String inputFilePath = @"autotagPdfInput.pdf";
            int inputFilePathIndex = Array.IndexOf(args, "--input");
            if (inputFilePathIndex >= 0 && inputFilePathIndex < args.Length - 1)
            {
                inputFilePath = args[inputFilePathIndex + 1];
            }
            else
                log.Info("input file not specified, using default value : autotagPdfInput.pdf");

            return inputFilePath;
        }

        private static String GetOutputFilePathFromCmdArgs(String[] args)
        {
            String outputFilePath = Directory.GetCurrentDirectory() + "/output/";
            int outputFilePathIndex = Array.IndexOf(args, "--output");
            if (outputFilePathIndex >= 0 && outputFilePathIndex < args.Length - 1)
            {
                outputFilePath = args[outputFilePathIndex + 1];
            }
            else
                log.Info("output path not specified, using default value : /output/");

            return outputFilePath;
        }

        static void Main(String[] args)
        {
            // Configure the logging
            ConfigureLogging();

            log.Info("--input " + GetInputFilePathFromCmdArgs(args));
            log.Info("--output " + GetOutputFilePathFromCmdArgs(args));
            log.Info("--report " + GetGenerateReportFromCmdArgs(args));
            log.Info("--shift_headings " + GetShiftHeadingsFromCmdArgs(args));

            try
            {
                // Initial setup, create credentials instance
                ICredentials credentials = new ServicePrincipalCredentials(
                    Environment.GetEnvironmentVariable("PDF_SERVICES_CLIENT_ID"),
                    Environment.GetEnvironmentVariable("PDF_SERVICES_CLIENT_SECRET"));

                // Creates a PDF Services instance
                PDFServices pdfServices = new PDFServices(credentials);

                // Creates an asset(s) from source file(s) and upload
                using Stream inputStream = File.OpenRead(GetInputFilePathFromCmdArgs(args));
                IAsset asset = pdfServices.Upload(inputStream, PDFServicesMediaType.PDF.GetMIMETypeValue());

                // Create parameters for the job
                AutotagPDFParams autotagPDFParams = GetParamsFromCmdArgs(args);

                // Creates a new job instance
                AutotagPDFJob autotagPDFJob = new AutotagPDFJob(asset).SetParams(autotagPDFParams);

                // Submits the job and gets the job result
                String location = pdfServices.Submit(autotagPDFJob);
                PDFServicesResponse<AutotagPDFResult> pdfServicesResponse =
                    pdfServices.GetJobResult<AutotagPDFResult>(location, typeof(AutotagPDFResult));

                // Get content from the resulting asset(s)
                IAsset resultAsset = pdfServicesResponse.Result.TaggedPDF;
                StreamAsset streamAsset = pdfServices.GetContent(resultAsset);

                // Creating output streams and copying stream asset's content to it
                String outputPath = GetOutputFilePathFromCmdArgs(args);
                String outputFilePathForTaggedPDF = CreateOutputFilePathForTaggedPDF(outputPath);
                new FileInfo(outputFilePathForTaggedPDF).Directory.Create();
                Stream outputStream = File.OpenWrite(outputFilePathForTaggedPDF);
                streamAsset.Stream.CopyTo(outputStream);
                outputStream.Close();

                if (autotagPDFParams != null && autotagPDFParams.IsGenerateReport)
                {
                    // Get content from the resulting asset(s)
                    IAsset resultAssetReport = pdfServicesResponse.Result.Report;
                    StreamAsset streamAssetReport = pdfServices.GetContent(resultAssetReport);

                    // Creating output streams and copying stream asset's content to it
                    String outputFilePathForReport = CreateOutputFilePathForTaggingReport(outputPath);
                    new FileInfo(outputFilePathForReport).Directory.Create();
                    Stream outputStreamReport =
                        File.OpenWrite(outputFilePathForReport);
                    streamAssetReport.Stream.CopyTo(outputStreamReport);
                    outputStreamReport.Close();
                }
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
        private static String CreateOutputFilePathForTaggedPDF(String outputPath)
        {
            String timeStamp = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH'-'mm'-'ss");
            return outputPath + "/AutotagPDF-tagged" + timeStamp + ".pdf";
        }

        // Generates a string containing a directory structure and file name for the output file.
        private static String CreateOutputFilePathForTaggingReport(String outputPath)
        {
            String timeStamp = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH'-'mm'-'ss");
            return outputPath + "/AutotagPDF-report" + timeStamp + ".xlsx";
        }
    }
}