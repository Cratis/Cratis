// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aksio.Cratis;
using Aksio.Cratis.Client;
using Aksio.Cratis.Observation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Microsoft.AspNetCore.Builder;

/// <summary>
/// Extensions for using Aksio.Cratis with a <see cref="WebApplicationBuilder"/>.
/// </summary>
public static class WebApplicationBuilderExtensions
{
    /// <summary>
    /// Configures the <see cref="IClientBuilder"/> for a non-microservice oriented scenario.
    /// </summary>
    /// <param name="webApplicationBuilder"><see cref="WebApplicationBuilder"/> to build on.</param>
    /// <param name="configureDelegate">Optional delegate used to configure the Cratis client.</param>
    /// <returns><see cref="WebApplicationBuilder"/> for configuration continuation.</returns>
    public static WebApplicationBuilder UseCratis(
        this WebApplicationBuilder webApplicationBuilder,
        Action<IClientBuilder>? configureDelegate = default)
    {
        webApplicationBuilder.Services.AddTransient<ClientObservers>();
        webApplicationBuilder.Services.AddRules();
        webApplicationBuilder.Host.UseCratis(configureDelegate);
        return webApplicationBuilder;
    }

    /// <summary>
    /// Configures the usage of Cratis for the app.
    /// </summary>
    /// <param name="app"><see cref="IApplicationBuilder"/> to extend.</param>
    /// <returns><see cref="IApplicationBuilder"/> for continuation.</returns>
    public static IApplicationBuilder UseCratis(this IApplicationBuilder app)
    {
        app.UseExecutionContext();

        app.UseRouting();
        app.UseEndpoints(endpoints => endpoints.MapClientObservers());

        var appLifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();
        appLifetime.ApplicationStarted.Register(() => app.ApplicationServices.GetRequiredService<IClient>().Connect().Wait());

        return app;
    }
}
