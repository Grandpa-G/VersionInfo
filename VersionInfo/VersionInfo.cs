using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Data;
using System.ComponentModel;
using System.Reflection;
using System.Collections.ObjectModel;

using Terraria;
using TShockAPI;
using System.Threading;
using TerrariaApi.Server;
using Rests;
using HttpServer;

namespace VersionInfo
{
    [ApiVersion(1, 22)]
    public class versionInfo : TerrariaPlugin
    {
        public static string SavePath = "tshock";

        public override Version Version
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version; }
        }
        public override string Author
        {
            get { return "Grandpa-G"; }
        }
        public override string Name
        {
            get { return "VersionInfo"; }
        }

        public override string Description
        {
            get { return "Version Info Rest API commands"; }
        }

        public static ConfigFile Config;


         protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
            base.Dispose(disposing);
        }

         public versionInfo(Main game)
            : base(game)
        {
            Order = 1;
        }

         public override void Initialize()
         {
             TShock.RestApi.Register(new SecureRestCommand("/VersionInfoREST/plugins", getPlugins, "versioninfo.allow"));
             TShock.RestApi.Register(new SecureRestCommand("/VersionInfoREST/getServerVersion", getServerVersion, "versioninfo.allow"));
         }

         private object getServerVersion(RestRequestArgs args)
         {
             var ret = new RestObject()
			{
				{"name", TShock.Config.ServerName},
				{"serverversion", Main.versionNumber.ToString()},
				{"tshockversion", TShock.VersionNum.ToString()},
				{"port", TShock.Config.ServerPort},
				{"APIversion", TerrariaApi.Server.ServerApi.ApiVersion.Major + "." + TerrariaApi.Server.ServerApi.ApiVersion.Minor}
			};
             return ret;
         }

        private object getPlugins(RestRequestArgs args)
        {
            List<PluginList> pluginList = new List<PluginList>();
            PluginList rec = null;

            for (int i = 0; i < ServerApi.Plugins.Count; i++)
            {
                PluginContainer pc = ServerApi.Plugins.ElementAt(i);
                Console.WriteLine(String.Format("{0} {1} {2} {3}", pc.Plugin.Name, pc.Plugin.Description, pc.Plugin.Author, pc.Plugin.Version), Color.LightSalmon);
                rec = new PluginList(pc.Plugin.Name, pc.Plugin.Description, pc.Plugin.Author, pc.Plugin.Version.ToString());
                pluginList.Add(rec);
            }


            /*
            string ServerPluginsDirectoryPath = Path.Combine(Environment.CurrentDirectory, TerrariaApi.Server.ServerApi.PluginsPath);

            string[] plugins = Directory.GetFiles(ServerPluginsDirectoryPath, "*.DLL");
            Console.WriteLine(ServerPluginsDirectoryPath);
            string excludeDLL = "bcrypt.net.dllhttpserver.dllmono.data.sqlite.dllmysql.data.dllmysql.web.dllnewtonsoft.json.dlltshockapi.dll";
            string version;
            string fileName;
            Assembly asembly = null;
            string pluginName = "";
            Version APIversion = null;
            CustomAttributeData VersionAttribute;
            foreach (string path in plugins)
            {
                fileName = Path.GetFileName(path);
                Console.WriteLine(path);
                if (!excludeDLL.Contains(fileName.ToLower()))
                {
                    asembly = Assembly.LoadFrom(path);
                    Console.Write(fileName);
                    try
                    {
                        version = asembly.FullName;
                        Console.WriteLine(version);
                        IEnumerable<TypeInfo> df = asembly.DefinedTypes;// CustomAttributes.First(Attr => Attr.AttributeType.Name == "ApiVersionAttribute");
                        VersionAttribute = asembly.GetType(df.First().ToString()).CustomAttributes.First(Attr => Attr.AttributeType.Name == "ApiVersionAttribute");

                        APIversion = new Version((int)VersionAttribute.ConstructorArguments[0].Value, (int)VersionAttribute.ConstructorArguments[1].Value);
                        Console.WriteLine(df.First().ToString() + ":" + APIversion.ToString());
                    }
                    catch (Exception ex)
                    {
                        TShock.Log.Error(ex.ToString());
                        Console.WriteLine(ex.StackTrace);
                        version = "??";
                    }
                    Console.WriteLine(fileName + ":" + version); 
                 }
            }
            */
            return new RestObject() { { "Plugins", pluginList } };
        }

        #region Utility Methods

        private static RestObject RestError(string message, string status = "400")
        {
            return new RestObject(status) { Error = message };
        }

        private RestObject RestResponse(string message, string status = "200")
        {
            return new RestObject(status) { Response = message };
        }

        private static RestObject RestMissingParam(string var)
        {
            return RestError("Missing or empty " + var + " parameter");
        }

        private static RestObject RestMissingParam(params string[] vars)
        {
            return RestMissingParam(string.Join(", ", vars));
        }

        private RestObject RestInvalidParam(string var)
        {
            return RestError("Missing or invalid " + var + " parameter");
        }

        #endregion
    }

    public class PluginList
    {
         public string Name { get; set; }

        public string Description { get; set; }
        public string Author { get; set; }

        public string Version { get; set; }

        public PluginList(string name, string description, string author, string version)
        {
            Name = name;
            Description = description;
            Author = author;
            Version = version;
         }

        public PluginList()
        {
            Name = string.Empty;
            Description = string.Empty;
            Author = string.Empty;
            Version = string.Empty;
        }
    }

}
