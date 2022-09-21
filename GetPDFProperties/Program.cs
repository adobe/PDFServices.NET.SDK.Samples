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
using System.Reflection;
using Adobe.PDFServicesSDK;
using Adobe.PDFServicesSDK.auth;
using Adobe.PDFServicesSDK.exception;
using Adobe.PDFServicesSDK.io;
using Adobe.PDFServicesSDK.io.pdfproperties;
using Adobe.PDFServicesSDK.options.pdfproperties;
using Adobe.PDFServicesSDK.pdfops;
using log4net;
using log4net.Config;
using log4net.Repository;

/// <summary>
/// This sample illustrates how to retrieve properties of an input PDF file.
/// <para/>
/// Refer to README.md for instructions on how to run the samples.
/// </summary>
namespace GetPDFProperties
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

                //Create an ExecutionContext using credentials and create a new operation instance.
                ExecutionContext executionContext = ExecutionContext.Create(credentials);
                PDFPropertiesOperation pdfPropertiesOperation = PDFPropertiesOperation.CreateNew();
                
                // Provide an input FileRef for the operation.
                FileRef source = FileRef.CreateFromLocalFile(@"pdfPropertiesInput.pdf");
                pdfPropertiesOperation.SetInput(source);
                
                // Build PDF Properties options to include page level properties and set them into the operation.
                PDFPropertiesOptions pdfPropertiesOptions = PDFPropertiesOptions.PDFPropertiesOptionsBuilder()
                        .IncludePageLevelProperties(true)               
                        .Build();
                pdfPropertiesOperation.SetOptions(pdfPropertiesOptions);
            
                // Execute the operation.
                PDFProperties pdfProperties = pdfPropertiesOperation.Execute(executionContext);
                
                // Fetch the requisite properties of the specified PDF.
                log.Info("The size of input PDF is : " + pdfProperties.Document?.FileSize);
                log.Info("Input PDF Version is : " + pdfProperties.Document?.PDFVersion);
                log.Info("Number of pages in input PDF : " + pdfProperties.Document?.PageCount);
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
