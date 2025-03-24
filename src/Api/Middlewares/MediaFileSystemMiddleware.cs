using Application.Common.Extensions;
using Application.Common.MediaStorage;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

namespace Api.Middlewares
{
    public static class MediaFileSystemMiddleware
    {

        public static void UseDocumentStaticFiles(this IApplicationBuilder app, IHostEnvironment env)
        {
            var mediaConfig = app.ApplicationServices.GetRequiredService<IOptions<MediaConfig>>().Value;
            if (mediaConfig.StorageProviderType != MediaStorageProviderType.FileSystem) return;

            UseMediaStaticFiles(app, env, mediaConfig.FileSystem!.UploadDocumentPath);
        }

        public static void UseMediaDocumentStaticFiles(this IApplicationBuilder app, IHostEnvironment env)
        {
            var mediaConfig = app.ApplicationServices.GetRequiredService<IOptions<MediaConfig>>().Value;
            if (mediaConfig.StorageProviderType != MediaStorageProviderType.FileSystem) return;

            UseMediaStaticFiles(
                app,
                env,
                mediaConfig.FileSystem!.UploadMediaPath,
                cfg =>
                {
                    var cacheStaticFile = mediaConfig.FileSystem.CacheStaticFile;
                    if (cacheStaticFile.HasValue)
                    {
                        cfg.OnPrepareResponse = ctx =>
                        {
                            ctx.Context.Response.Headers.Append(
                                "Cache-Control",
                                $"public, max-age={cacheStaticFile.Value.TotalSeconds}");
                        };
                    }

                    return cfg;
                });
        }

        private static void UseMediaStaticFiles(
            this IApplicationBuilder app,
            IHostEnvironment env,
            string rootPath,
            Func<StaticFileOptions, StaticFileOptions>? onConfigure = null)
        {
            var uploadRootPath = rootPath.TrimStart('/');

            var uploadRootAbsolutePath = Path.Combine(env.ContentRootPath, uploadRootPath);
            uploadRootAbsolutePath.EnsureDirectoryExist();

            var staticFileOptions = new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(uploadRootAbsolutePath),
                RequestPath = $"/{uploadRootPath}",
            };

            onConfigure?.Invoke(staticFileOptions);

            app.UseStaticFiles(staticFileOptions);
        }
    }
}
