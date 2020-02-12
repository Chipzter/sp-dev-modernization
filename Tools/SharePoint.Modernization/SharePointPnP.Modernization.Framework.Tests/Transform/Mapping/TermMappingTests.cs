﻿using System;
using Microsoft.SharePoint.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharePointPnP.Modernization.Framework.Publishing;
using SharePointPnP.Modernization.Framework.Telemetry.Observers;
using SharePointPnP.Modernization.Framework.Transform;
using SharePointPnP.Modernization.Framework.Utilities;

namespace SharePointPnP.Modernization.Framework.Tests.Transform.Mapping
{
    [TestClass]
    public class TermMappingTests
    {
        [TestMethod]
        public void TermMappingFileLoadTest()
        {
            FileManager fm = new FileManager();
            var mapping = fm.LoadTermMappingFile(@"..\..\Transform\Mapping\term_mapping_sample.csv");

            Assert.IsTrue(mapping.Count > 0);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void TermMappingFileNotFoundTest()
        {
            FileManager fm = new FileManager();
            var mapping = fm.LoadTermMappingFile(@"..\..\Transform\Mapping\idontexist_sample.csv");
        }

        [TestMethod]
        public void TermMappingTransformatorTest_PassThrough()
        {
            using (var targetClientContext = TestCommon.CreateClientContext(TestCommon.AppSetting("SPOTargetSiteUrl")))
            {
                using (var sourceClientContext = TestCommon.CreateOnPremisesClientContext())
                {
                    PublishingPageTransformationInformation pti = new PublishingPageTransformationInformation()
                    {
                        // If target page exists, then overwrite it
                        Overwrite = true,

                        // Don't log test runs
                        SkipTelemetry = true,

                        //Permissions are should work given cross domain with mapping
                        KeepPageSpecificPermissions = true,

                        // Term store mapping
                        TermMappingFile = @"..\..\Transform\Mapping\term_mapping_sample.csv",

                        SkipTermStoreMapping = false

                    };

                    TermTransformator termTransformator = new TermTransformator(pti, sourceClientContext, targetClientContext, null);

                    var inputLabel = "pass-through-test";
                    var inputGuid = Guid.NewGuid().ToString();
                    var result = termTransformator.Transform(new Entities.TermData() { TermGuid = inputGuid, TermLabel = inputLabel });
                    Console.WriteLine(inputLabel + " and " + inputGuid);

                    Assert.AreEqual(inputLabel, result.TermLabel);
                    Assert.AreEqual(inputGuid, result.TermGuid);
                }
            }
        }

        [TestMethod]
        public void BasicOnPremPublishingPage_MultiTermTest()
        {
            using (var targetClientContext = TestCommon.CreateClientContext(TestCommon.AppSetting("SPOTargetSiteUrl")))
            {
                using (var sourceClientContext = TestCommon.CreateOnPremisesClientContext())
                {
                    var pageTransformator = new PublishingPageTransformator(sourceClientContext, targetClientContext, @"C:\temp\onprem-mapping-all-test.xml");
                    //pageTransformator.RegisterObserver(new MarkdownObserver(folder: "c:\\temp", includeVerbose: true));
                    pageTransformator.RegisterObserver(new UnitTestLogObserver());

                    
                    var pages = sourceClientContext.Web.GetPagesFromList("Pages", folder:"News", pageNameStartsWith:"Kitchen");

                    pages.FailTestIfZero();

                    foreach (var page in pages)
                    {
                        PublishingPageTransformationInformation pti = new PublishingPageTransformationInformation(page)
                        {
                            // If target page exists, then overwrite it
                            Overwrite = true,

                            // Don't log test runs
                            SkipTelemetry = true,

                            

                            //Permissions are unlikely to work given cross domain
                            KeepPageSpecificPermissions = false,

                            // Term store mapping
                            TermMappingFile = @"..\..\Transform\Mapping\term_mapping_sample.csv",

                            SkipTermStoreMapping = false

                        };

                        Console.WriteLine("SharePoint Version: {0}", pti.SourceVersion);

                        pti.MappingProperties["SummaryLinksToQuickLinks"] = "true";
                        pti.MappingProperties["UseCommunityScriptEditor"] = "true";

                        var result = pageTransformator.Transform(pti);
                    }

                    pageTransformator.FlushObservers();

                }
            }
        }

        [TestMethod]
        public void BasicOnlinePublishingPage_TermTest()
        {
            using (var targetClientContext = TestCommon.CreateClientContext(TestCommon.AppSetting("SPOTargetSiteUrl")))
            {
                using (var sourceClientContext = TestCommon.CreateClientContext(TestCommon.AppSetting("SPODevSiteUrl")))
                {
                    var pageTransformator = new PublishingPageTransformator(sourceClientContext, targetClientContext, @"C:\temp\onprem-mapping-all-test.xml");
                    //pageTransformator.RegisterObserver(new MarkdownObserver(folder: "c:\\temp", includeVerbose: true));
                    pageTransformator.RegisterObserver(new UnitTestLogObserver());


                    var pages = sourceClientContext.Web.GetPagesFromList("Pages", folder: "News", pageNameStartsWith: "Kitchen");

                    pages.FailTestIfZero();

                    foreach (var page in pages)
                    {
                        PublishingPageTransformationInformation pti = new PublishingPageTransformationInformation(page)
                        {
                            // If target page exists, then overwrite it
                            Overwrite = true,

                            // Don't log test runs
                            SkipTelemetry = true,



                            //Permissions are unlikely to work given cross domain
                            KeepPageSpecificPermissions = false,

                            // Term store mapping
                            TermMappingFile = @"..\..\Transform\Mapping\term_mapping_sample.csv",

                            SkipTermStoreMapping = false

                        };

                        Console.WriteLine("SharePoint Version: {0}", pti.SourceVersion);

                        pti.MappingProperties["SummaryLinksToQuickLinks"] = "true";
                        pti.MappingProperties["UseCommunityScriptEditor"] = "true";

                        var result = pageTransformator.Transform(pti);
                    }

                    pageTransformator.FlushObservers();

                }
            }
        }

        [TestMethod]
        public void BasicOnlineSitePage_TermTest()
        {
            using (var targetClientContext = TestCommon.CreateClientContext(TestCommon.AppSetting("SPOTargetSiteUrl")))
            {
                using (var sourceClientContext = TestCommon.CreateClientContext(TestCommon.AppSetting("SPOPublishingSite")))
                {
                    var pageTransformator = new PublishingPageTransformator(sourceClientContext, targetClientContext, @"C:\temp\onprem-mapping-all-test.xml");
                    //pageTransformator.RegisterObserver(new MarkdownObserver(folder: "c:\\temp", includeVerbose: true));
                    pageTransformator.RegisterObserver(new UnitTestLogObserver());


                    var pages = sourceClientContext.Web.GetPagesFromList("Pages", folder: "News", pageNameStartsWith: "Kitchen");

                    pages.FailTestIfZero();

                    foreach (var page in pages)
                    {
                        PublishingPageTransformationInformation pti = new PublishingPageTransformationInformation(page)
                        {
                            // If target page exists, then overwrite it
                            Overwrite = true,

                            // Don't log test runs
                            SkipTelemetry = true,



                            //Permissions are unlikely to work given cross domain
                            KeepPageSpecificPermissions = false,

                            // Term store mapping
                            TermMappingFile = @"..\..\Transform\Mapping\term_mapping_sample.csv",

                            SkipTermStoreMapping = false

                        };

                        Console.WriteLine("SharePoint Version: {0}", pti.SourceVersion);

                        pti.MappingProperties["SummaryLinksToQuickLinks"] = "true";
                        pti.MappingProperties["UseCommunityScriptEditor"] = "true";

                        var result = pageTransformator.Transform(pti);
                    }

                    pageTransformator.FlushObservers();

                }
            }
        }
    }
}
