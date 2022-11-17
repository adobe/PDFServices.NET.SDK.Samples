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
using System.IO;
using System;
using log4net.Repository;
using log4net.Config;
using log4net;
using System.Reflection;
using Adobe.PDFServicesSDK;
using Adobe.PDFServicesSDK.auth;
using Adobe.PDFServicesSDK.options.documentmerge;
using Adobe.PDFServicesSDK.pdfops;
using Adobe.PDFServicesSDK.io;
using Adobe.PDFServicesSDK.exception;
using Newtonsoft.Json.Linq;

/// <summary>
/// This sample illustrates how to merge the Word based document template with the input JSON data to generate the output document in the PDF format.
/// <para>
/// To know more about document generation and document templates, please see the <a href="http://www.adobe.com/go/dcdocgen_overview_doc">documentation</a>
/// </para>
/// Refer to README.md for instructions on how to run the samples.
/// </summary>
///

namespace MergeDocumentToPDF
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

                // Create an ExecutionContext using credentials.
                ExecutionContext executionContext = ExecutionContext.Create(credentials);

                // Setup input data for the document merge process
                var content = File.ReadAllText(@"salesOrder.json");
                JObject jsonDataForMerge = JObject.Parse(content);

                // Create a new DocumentMerge Options instance
                DocumentMergeOptions documentMergeOptions = new DocumentMergeOptions(jsonDataForMerge, OutputFormat.PDF);

                // Create a new DocumentMerge Operation instance with the DocumentMerge Options instance
                DocumentMergeOperation documentMergeOperation = DocumentMergeOperation.CreateNew(documentMergeOptions);

                // Set the operation input document template from a source file.
                documentMergeOperation.SetInput(FileRef.CreateFromLocalFile(@"salesOrderTemplate.docx"));

                // Execute the operation.
                FileRef result = documentMergeOperation.Execute(executionContext);

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
            return ("/output/merge" + timeStamp + ".pdf");
        }
    }
}
