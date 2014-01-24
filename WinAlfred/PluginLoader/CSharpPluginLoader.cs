﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using WinAlfred.Helper;
using WinAlfred.Plugin;
using WinAlfred.Plugin.System;

namespace WinAlfred.PluginLoader
{
    public class CSharpPluginLoader : BasePluginLoader
    {
        public override List<PluginPair> LoadPlugin()
        {
            List<PluginPair> plugins = new List<PluginPair>();

            List<PluginMetadata> metadatas = pluginMetadatas.Where(o => o.Language.ToUpper() == AllowedLanguage.CSharp.ToUpper()).ToList();
            foreach (PluginMetadata metadata in metadatas)
            {
                try
                {
                    byte[] buffer = System.IO.File.ReadAllBytes(metadata.ExecuteFilePath);
                    Assembly asm = Assembly.Load(buffer);
                    List<Type> types = asm.GetTypes().Where(o => o.IsClass && o.GetInterfaces().Contains(typeof(IPlugin)) || o.GetInterfaces().Contains(typeof(ISystemPlugin))).ToList();
                    if (types.Count == 0)
                    {
                        Log.Error(string.Format("Cound't load plugin {0}: didn't find the class who implement IPlugin",
                            metadata.Name));
                        continue;
                    }

                    foreach (Type type in types)
                    {
                        PluginPair pair = new PluginPair()
                        {
                            Plugin = Activator.CreateInstance(type) as IPlugin,
                            Metadata = metadata
                        };
                        plugins.Add(pair);
                    }
                }
                catch (Exception e)
                {
                    Log.Error(string.Format("Cound't load plugin {0}: {1}", metadata.Name, e.Message));
#if (DEBUG)
                    {
                        throw;
                    }
#endif
                }

            }

            InitPlugin(plugins);
            return plugins;
        }

        private void InitPlugin(List<PluginPair> plugins)
        {
      
        }
    }
}