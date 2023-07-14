// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aksio.Cratis;
using Aksio.Cratis.Client;
using Aksio.Cratis.Clients;
using Aksio.Execution;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Microsoft.AspNetCore.Builder;

/// <summary>
/// Extensions for using Aksio.Cratis with a <see cref="WebApplicationBuilder"/>.
/// </summary>
public static class WebApplicationBuilderExtensions
{
    /// <summary>
    /// Configures the <see cref="IClientBuilder{TClientBuilder, TClient}"/> for a non-microservice oriented scenario.
    /// </summary>
    /// <param name="webApplicationBuilder"><see cref="WebApplicationBuilder"/> to build on.</param>
    /// <param name="clientArtifacts">Optional <see cref="IClientArtifactsProvider"/> for the client artifacts. Will default to <see cref="DefaultClientArtifactsProvider"/>.</param>
    /// <param name="configureDelegate">Optional delegate used to configure the Cratis client.</param>
    /// <returns><see cref="WebApplicationBuilder"/> for configuration continuation.</returns>
    public static WebApplicationBuilder UseCratis(
        this WebApplicationBuilder webApplicationBuilder,
        IClientArtifactsProvider? clientArtifacts = default,
        Action<IClientBuilder<IMultiTenantClientBuilder, IMultiTenantClient>>? configureDelegate = default)
    {
        return webApplicationBuilder.UseCratis(MicroserviceId.Unspecified, MicroserviceName.Unspecified, clientArtifacts, configureDelegate);
    }

    /// <summary>
    /// Configures the <see cref="IClientBuilder{TClientBuilder, TClient}"/> for a non-microservice oriented scenario.
    /// </summary>
    /// <param name="webApplicationBuilder"><see cref="WebApplicationBuilder"/> to build on.</param>
    /// <param name="microserviceId">The unique <see cref="MicroserviceId"/> for the microservice.</param>
    /// <param name="microserviceName">The <see cref="MicroserviceName"/> for the microservice.</param>
    /// <param name="clientArtifacts">Optional <see cref="IClientArtifactsProvider"/> for the client artifacts. Will default to <see cref="DefaultClientArtifactsProvider"/>.</param>
    /// <param name="configureDelegate">Optional delegate used to configure the Cratis client.</param>
    /// <returns><see cref="WebApplicationBuilder"/> for configuration continuation.</returns>
    public static WebApplicationBuilder UseCratis(
        this WebApplicationBuilder webApplicationBuilder,
        MicroserviceId microserviceId,
        MicroserviceName microserviceName,
        IClientArtifactsProvider? clientArtifacts = default,
        Action<IClientBuilder<IMultiTenantClientBuilder, IMultiTenantClient>>? configureDelegate = default)
    {
        webApplicationBuilder.Services.AddRules();
        webApplicationBuilder.Host.UseCratis(microserviceId, microserviceName, configureDelegate, clientArtifacts);

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

        var appLifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();
        appLifetime.ApplicationStarted.Register(() => app.ApplicationServices.GetRequiredService<IClient>());

        return app;
    }
}
