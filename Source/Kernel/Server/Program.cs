// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Globalization;
using Aksio.Applications.Autofac;
using Cratis.Kernel.Grains.Observation.Placement;
using Cratis.Kernel.Server.Serialization;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Serilog;

#pragma warning disable SA1600
namespace Cratis.Kernel.Server;

public static class Program
{
    public static Task Main(string[] args)
    {
        AppDomain.CurrentDomain.UnhandledException += UnhandledExceptions;
        SelfBindingRegistrationSource.AddNamespaceStartsWithToExclude(
            "Microsoft",
            "Orleans");

        // Force invariant culture for the Kernel
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
        CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;

        return CreateHostBuilder(args).RunConsoleAsync();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
         Host.CreateDefaultBuilder(args)
            .ConfigureCpuBoundWorkers()
            .UseMongoDB()
            .UseAksio(mvcOptions => mvcOptions.Filters.Add<KernelReadyResourceFilter>(0))
            .UseCratis()
            .UseOrleans(_ => _
                .UseCluster()
                .AddPlacementDirector<ConnectedObserverPlacementStrategy, ConnectedObserverPlacementDirector>()
                .UseStreamCaching()
                .AddBroadcastChannel(WellKnownBroadcastChannelNames.ProjectionChanged, _ => _.FireAndForgetDelivery = true)
                .ConfigureSerialization()
                .AddReplayStateManagement()
                .UseTelemetry()
                .UseDashboard(options =>
                {
                    options.Host = "*";
                    options.Port = 8081;
                    options.HostSelf = true;
                })
                .ConfigureStorage()
                .UseMongoDB()
                .AddEventSequenceStreaming()
                .AddExecutionContext())
            .ConfigureWebHostDefaults(_ => _
                .ConfigureKestrel(options =>
                {
                    options.ListenAnyIP(8080, listenOptions => listenOptions.Protocols = HttpProtocols.Http1);
                    options.ListenAnyIP(35000, listenOptions => listenOptions.Protocols = HttpProtocols.Http2);
                    options.Limits.Http2.MaxStreamsPerConnection = 100;
                })
                .UseStartup<Startup>());

    static void UnhandledExceptions(object sender, UnhandledExceptionEventArgs args)
    {
        if (args.ExceptionObject is Exception exception)
        {
            Log.Logger?.Error(exception, "Unhandled exception");
            Log.CloseAndFlush();
            Console.WriteLine("************ BEGIN UNHANDLED EXCEPTION ************");
            PrintExceptionInfo(exception);

            while (exception.InnerException != null)
            {
                Console.WriteLine("\n------------ BEGIN INNER EXCEPTION ------------");
                PrintExceptionInfo(exception.InnerException);
                exception = exception.InnerException;
                Console.WriteLine("------------ END INNER EXCEPTION ------------\n");
            }

            Console.WriteLine("************ END UNHANDLED EXCEPTION ************ ");
        }
    }

    static void PrintExceptionInfo(Exception exception)
    {
        Console.WriteLine($"Exception type: {exception.GetType().FullName}");
        Console.WriteLine($"Exception message: {exception.Message}");
        Console.WriteLine($"Stack Trace: {exception.StackTrace}");
    }
}
