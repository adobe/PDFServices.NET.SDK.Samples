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
using System;
using System.IO;
using log4net.Repository;
using log4net;
using log4net.Config;
using System.Reflection;
using Adobe.DocumentServices.PDFTools;
using Adobe.DocumentServices.PDFTools.auth;
using Adobe.DocumentServices.PDFTools.pdfops;
using Adobe.DocumentServices.PDFTools.io;
using Adobe.DocumentServices.PDFTools.options;
using Adobe.DocumentServices.PDFTools.exception;

/// <summary>
/// This sample illustrates how to combine specific pages of multiple PDF files into a single PDF file.
/// <para/>
/// Note that the SDK supports combining upto 20 files in one operation.
/// <para/>
/// Refer to README.md for instructions on how to run the samples.
/// </summary>
namespace CombinePDFWithPageRanges
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
                CombineFilesOperation combineFilesOperation = CombineFilesOperation.CreateNew();

                // Create a FileRef instance from a local file.
                FileRef firstFileToCombine = FileRef.CreateFromLocalFile(@"combineFileWithPageRangeInput1.pdf");
                PageRanges pageRangesForFirstFile = GetPageRangeForFirstFile();
                // Add the first file as input to the operation, along with its page range.
                combineFilesOperation.AddInput(firstFileToCombine, pageRangesForFirstFile);

                // Create a second FileRef instance using a local file.
                FileRef secondFileToCombine = FileRef.CreateFromLocalFile(@"combineFileWithPageRangeInput2.pdf");
                PageRanges pageRangesForSecondFile = GetPageRangeForSecondFile();
                // Add the second file as input to the operation, along with its page range.
                combineFilesOperation.AddInput(secondFileToCombine, pageRangesForSecondFile);

                // Execute the operation.
                FileRef result = combineFilesOperation.Execute(executionContext);

                // Save the result to the specified location.
                result.SaveAs(Directory.GetCurrentDirectory() + "/output/combineFilesOutput.pdf");

            }
            catch (ServiceUsageException ex)
            {
                log.Error("Exception encountered while executing operation", ex);
            }
            catch (ServiceApiException ex)
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
            catch(Exception ex)
            {
                log.Error("Exception encountered while executing operation", ex);
            } 
        }

        private static PageRanges GetPageRangeForSecondFile()
        {
            // Specify which pages of the second file are to be included in the combined file.
            PageRanges pageRangesForSecondFile = new PageRanges();
            // Add all pages including and after page 5.
            pageRangesForSecondFile.AddAllFrom(5);
            return pageRangesForSecondFile;
        }

        private static PageRanges GetPageRangeForFirstFile()
        {
            // Specify which pages of the first file are to be included in the combined file.
            PageRanges pageRangesForFirstFile = new PageRanges();
            // Add page 2.
            pageRangesForFirstFile.AddSinglePage(2);
            // Add page 3.
            pageRangesForFirstFile.AddSinglePage(3);
            // Add pages 5 to 7.
            pageRangesForFirstFile.AddRange(5, 7);
            return pageRangesForFirstFile;
        }


        static void ConfigureLogging()
        {
            ILoggerRepository logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        }
    }
}
