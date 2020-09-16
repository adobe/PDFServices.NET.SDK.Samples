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
using Adobe.DocumentServices.PDFTools;
using Adobe.DocumentServices.PDFTools.auth;
using Adobe.DocumentServices.PDFTools.options.protectpdf;
using Adobe.DocumentServices.PDFTools.pdfops;
using Adobe.DocumentServices.PDFTools.io;
using Adobe.DocumentServices.PDFTools.exception;

namespace ProtectPDFWithOwnerPassword
{
    /// <summary>
    /// This sample illustrates how to secure a PDF file with owner password and allow certain access permissions 
    /// such as copying and editing the contents, and printing of the document at low resolution.
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
                // Initial setup, create credentials instance.
                Credentials credentials = Credentials.ServiceAccountCredentialsBuilder()
                                .FromFile(Directory.GetCurrentDirectory() + "/pdftools-api-credentials.json")
                                .Build();

                // Create an ExecutionContext using credentials.
                ExecutionContext executionContext = ExecutionContext.Create(credentials);

                // Create new permissions instance and add the required permissions
                Permissions permissions = Permissions.CreateNew();
                permissions.AddPermission(Permission.PRINT_LOW_QUALITY);
                permissions.AddPermission(Permission.EDIT_DOCUMENT_ASSEMBLY);
                permissions.AddPermission(Permission.COPY_CONTENT);

                // Build ProtectPDF options by setting an Owner/Permissions Password, Permissions,
                // Encryption Algorithm (used for encrypting the PDF file) and specifying the type of content to encrypt.
                ProtectPDFOptions protectPDFOptions = ProtectPDFOptions.PasswordProtectOptionsBuilder()
                    .SetOwnerPassword("password")
                    .SetPermissions(permissions)
                    .SetEncryptionAlgorithm(EncryptionAlgorithm.AES_256)
                    .SetContentEncryption(ContentEncryption.ALL_CONTENT_EXCEPT_METADATA)
                    .Build();

                // Create a new operation instance
                ProtectPDFOperation protectPDFOperation = ProtectPDFOperation.CreateNew(protectPDFOptions);

                // Set operation input from a source file.
                FileRef sourceFileRef = FileRef.CreateFromLocalFile(@"protectPDFInput.pdf");
                protectPDFOperation.SetInput(sourceFileRef);

                // Execute the operation.
                FileRef result = protectPDFOperation.Execute(executionContext);

                // Save the result to the specified location.
                result.SaveAs(Directory.GetCurrentDirectory() + "/output/protectPDFWithOwnerPasswordOutput.pdf");
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
