using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Zen.Abp.Domain.Shared;

// ReSharper disable once CheckNamespace
namespace Zen.Abp.HttpApi.Hosting
{
    public static class ZenWinServiceExtension
    {
        private static readonly string ZenWinSrvDir = Path.Combine(AppContext.BaseDirectory, "WinService");
        public static IApplicationBuilder UseZenWinService(this IApplicationBuilder app, ZenWinServiceOption option)
        {
            if (option == null||string.IsNullOrWhiteSpace(option.Name)||string.IsNullOrWhiteSpace(option.Executable))
            {
                throw new ArgumentException("ZenWinServiceOption argument is illegal");
            }

            option.Format();
            //确保目录存在
            if (!Directory.Exists(ZenWinSrvDir))
            {
                Directory.CreateDirectory(ZenWinSrvDir);
            }
            // 内嵌文件释放
            var assembly = typeof(ZenWinServiceExtension).GetTypeInfo().Assembly;
            var resourceNames = assembly.GetManifestResourceNames();
            foreach (var resourceName in resourceNames)
            {
                SaveResourceFile(assembly, resourceName);
            }
            //服务名称替换
            var xmlPath = ResourceFileMappers["ZenWinSrv.xml"];
            var xmlContent = File.ReadAllText(xmlPath);
            xmlContent = xmlContent.Replace("@zenSrvName@", option.Name)
                .Replace("@zenSrvDescription@", option.Description)
                .Replace("@zenSrvExecutable@", option.Executable)
                .Replace("@zenSrvArguments@", option.Arguments);
            File.WriteAllText(xmlPath, xmlContent);
            return app;
        }


        private static readonly Dictionary<string, string> ResourceFileMappers =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                {"zen-install.bat", Path.Combine(ZenWinSrvDir, "zen-install.bat")},
                {"zen-uninstall.bat", Path.Combine(ZenWinSrvDir, "zen-uninstall.bat")},
                {"ZenWinSrv.exe", Path.Combine(AppContext.BaseDirectory, "ZenWinSrv.exe")},
                {"ZenWinSrv.exe.config", Path.Combine(AppContext.BaseDirectory, "ZenWinSrv.exe.config")},
                {"ZenWinSrv.xml", Path.Combine(AppContext.BaseDirectory, "ZenWinSrv.xml")},
            };

        private static void SaveResourceFile(Assembly assembly, string resourceFileName)
        {
            foreach (var mapper in ResourceFileMappers)
            {
                if (resourceFileName.EndsWith(mapper.Key, StringComparison.OrdinalIgnoreCase)
                    && !File.Exists(mapper.Value))
                {
                    var stream = assembly.GetManifestResourceStream(resourceFileName);
                    if (stream == null)
                    {
                        continue;
                    }
                    var fileBytes = new byte[stream.Length];
                    stream.Read(fileBytes, 0, fileBytes.Length);
                    stream.Seek(0, SeekOrigin.Begin);
                    File.WriteAllBytes(mapper.Value, fileBytes);
                    stream.Close();
                    stream.Dispose();
                    return;
                }
            }
        }
    }

    public class ZenWinServiceOption
    {
        /// <summary>
        ///     服务名称，英文
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     服务描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     服务可执行Exe文件名称，带后缀，如“HttpApi.Hosting.exe”
        /// </summary>
        public string Executable { get; set; }

        /// <summary>
        ///     启动参数，默认为空
        /// </summary>
        public string Arguments { get; set; }

        /// <summary>
        ///     格式化
        /// </summary>
        public void Format()
        {
            Name = Name.EnsureTrim();
            Description = Description.EnsureTrim();
            Executable = Executable.EnsureTrim();
            Arguments = Arguments.EnsureTrim();
        }
    }
}