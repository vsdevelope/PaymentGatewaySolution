using BoDi;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PaymentGateway.Api.FunctionalTests.Helpers;
using PaymentGateway.Api.FunctionalTests.Model;
using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using TechTalk.SpecFlow;

namespace PaymentGateway.Api.FunctionalTests
{
    [Binding]
    public class Hooks
    {
        private readonly IObjectContainer _objectContainer;
        private static LocalSettings _settings;

        public Hooks(IObjectContainer objectContainer)
        {
            _objectContainer = objectContainer;
        }

        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            var basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var appSettingsFilePath = Path.Combine(basePath, "appSettings.json");

            File.Exists(appSettingsFilePath).Should().BeTrue("appSettings.json file not found");

            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder
                .AddJsonFile(appSettingsFilePath, false)
                .AddEnvironmentVariables(); 
                
            var configurationRoot = configurationBuilder.Build();
            _settings = new LocalSettings();
            configurationRoot.Bind(_settings);
        }

        [BeforeScenario]
        public void BeforeScenario()
        {
            _objectContainer.RegisterInstanceAs(_settings);
       
                _objectContainer.RegisterInstanceAs(new HttpClient
                {
                    BaseAddress =new Uri(_settings.HostUrl)
                });
            }
        }
    }
