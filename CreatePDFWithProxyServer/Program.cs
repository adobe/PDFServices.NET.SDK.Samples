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
using Adobe.PDFServicesSDK.config;
using Adobe.PDFServicesSDK.config.proxy;
using Adobe.PDFServicesSDK.exception;
using Adobe.PDFServicesSDK.io;
using Adobe.PDFServicesSDK.pdfjobs.jobs;
using Adobe.PDFServicesSDK.pdfjobs.results;
using log4net;
using log4net.Config;
using log4net.Repository;


/// <summary>
/// This sample illustrates how to setup Proxy Server configurations for performing an operation. This enables the
/// clients to set proxy server configurations to enable the API calls in a network where calls are blocked unless they
/// are routed via Proxy server.
/// <para/>
/// Refer to README.md for instructions on how to run the samples.
/// </summary>
namespace CreatePDFWithProxyServer
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

                /*
                Initial setup, creates proxy server config instance for client config
                Replace the values of PROXY_HOSTNAME with the proxy server hostname.
                If the scheme of proxy server is not HTTP then, replace ProxyScheme parameter with HTTPS.
                If the port for proxy server is diff than the default port for HTTP and HTTPS, then please set the
                PROXY_PORT,
                else, remove its setter statement.
                 */
                ProxyServerConfig proxyServerConfig = new ProxyServerConfig.Builder()
                    .WithHost("PROXY_HOSTNAME")
                    .WithProxyScheme(ProxyScheme.HTTP)
                    .WithPort(443)
                    .Build();

                // Creates client config instance
                ClientConfig clientConfig = ClientConfig.ConfigBuilder()
                    .WithProxyServerConfig(proxyServerConfig)
                    .Build();

                // Creates a PDF Services instance
                PDFServices pdfServices = new PDFServices(credentials, clientConfig);

                // Creates an asset(s) from source file(s) and upload
                using Stream inputStream = File.OpenRead(@"createPdfInput.docx");
                IAsset asset = pdfServices.Upload(inputStream, PDFServicesMediaType.DOCX.GetMIMETypeValue());

                // Creates a new job instance
                CreatePDFJob createPDFJob = new CreatePDFJob(asset);

                // Submits the job and gets the job result
                String location = pdfServices.Submit(createPDFJob);
                PDFServicesResponse<CreatePDFResult> pdfServicesResponse =
                    pdfServices.GetJobResult<CreatePDFResult>(location, typeof(CreatePDFResult));

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
            return ("/output/create" + timeStamp + ".pdf");
        }
    }
}