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
/// This sample illustrates how to generate a tagged PDF by setting options with command line arguments.
/// <para/>
/// Refer to README.md for instructions on how to run the samples.
/// </summary>
namespace AutotagPDFParameterised
{
    class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));

        private static AutotagPDFOptions GetOptionsFromCmdArgs(String[] args)
        {
            Boolean generateReport = GetGenerateReportFromCmdArgs(args);
            Boolean shiftHeadings = GetShiftHeadingsFromCmdArgs(args);

            AutotagPDFOptions.Builder builder = AutotagPDFOptions.AutotagPDFOptionsBuilder();

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

        static void Main(string[] args)
        {
            // Configure the logging
            ConfigureLogging();

            log.Info("--input " + GetInputFilePathFromCmdArgs(args));
            log.Info("--output " + GetOutputFilePathFromCmdArgs(args));
            log.Info("--report " + GetGenerateReportFromCmdArgs(args));
            log.Info("--shift_headings " + GetShiftHeadingsFromCmdArgs(args));

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
                autotagPDFOperation.SetInput(FileRef.CreateFromLocalFile(GetInputFilePathFromCmdArgs(args)));

                // Get and Build AutotagPDF options from command line args and set them into the operation
                AutotagPDFOptions autotagPDFOptions = GetOptionsFromCmdArgs(args);
                autotagPDFOperation.SetOptions(autotagPDFOptions);

                // Execute the operation
                AutotagPDFOutput autotagPDFOutput = autotagPDFOperation.Execute(executionContext);

                // Save the output files at the specified location
                string outputPath = GetOutputFilePathFromCmdArgs(args);
                FileRef taggedPDF = autotagPDFOutput.GetTaggedPDF();
                taggedPDF.SaveAs(CreateOutputFilePathForTaggedPDF(outputPath));
                if (autotagPDFOptions != null && autotagPDFOptions.IsGenerateReport)
                    autotagPDFOutput.GetReport()
                        .SaveAs(CreateOutputFilePathForTaggingReport(outputPath));
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
        public static string CreateOutputFilePathForTaggedPDF(string outputPath)
        {
            String timeStamp = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH'-'mm'-'ss");
            return outputPath + "/AutotagPDF-tagged" + timeStamp + ".pdf";
        }

        // Generates a string containing a directory structure and file name for the output file.
        public static string CreateOutputFilePathForTaggingReport(string outputPath)
        {
            String timeStamp = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH'-'mm'-'ss");
            return outputPath + "/AutotagPDF-report" + timeStamp + ".xlsx";
        }
    }
}