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
using Adobe.DocumentCloud.Services.exception;
using Adobe.DocumentCloud.Services.io;
using Adobe.DocumentCloud.Services.pdfops;
using log4net;
using log4net.Config;
using log4net.Repository;
using System;
using System.IO;
using System.Reflection;

namespace CreatePDFWithInMemoryAuthCredentials
{
    /// <summary>
    /// This sample illustrates how to provide in-memory auth credentials for performing an operation. This enables the
    /// clients to fetch the credentials from a secret server during runtime, instead of storing them in a file.
    /// <para/>
    /// Refer to README.md for instructions on how to run the samples.
    /// </summary>
    class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));
        static void Main()
        {
            //Configure the logging
            ConfigureLogging();
            try
            {
                // Initial setup, create a ClientContext and a new operation instance.
                ClientContext clientContext = ClientContext.CreateFromFile(Directory.GetCurrentDirectory() + "/dc-services-sdk-config.json");
                CreatePDFOperation createPdfOperation = CreatePDFOperation.CreateNew();

                // Set operation input from a source file.
                FileRef source = FileRef.CreateFromLocalFile(@"createPdfInput.docx");
                createPdfOperation.SetInput(source);

                /*
                Set this variable to the value of "identity" key in dc-services-sdk-config.json that you received in Adobe
                Document Cloud Services SDK welcome email
                */
                String authenticationJsonString = "";

                // Create a new ClientContext instance with the provided authentication credentials
                Authentication authentication = Authentication.Create(authenticationJsonString);
                ClientContext clientContextWithAuth = clientContext.WithAuthentication(authentication);

                // Execute the operation.
                FileRef result = createPdfOperation.Execute(clientContextWithAuth);

                // Save the result to the specified location.
                result.SaveAs(Directory.GetCurrentDirectory() + "/output/createPdfOutput.pdf");
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
