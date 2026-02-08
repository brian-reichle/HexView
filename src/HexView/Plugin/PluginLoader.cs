// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using HexView.Data;
using HexView.Framework;

namespace HexView
{
	static class PluginLoader
	{
		public static ReadOnlyCollection<IFormatReader> Readers => _readers;

		public static void Load()
		{
			var directory = GetPluginDirectory();

			_readers_internal.Add(NullFormatReader.Instance);

			if (!string.IsNullOrEmpty(directory))
			{
				foreach (var filename in Directory.GetFiles(directory))
				{
					if (filename.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
					{
						var plugin = Load(filename);

						if (plugin != null)
						{
							foreach (var reader in plugin.Readers)
							{
								_readers_internal.Add(reader);
							}
						}
					}
				}
			}
		}

		static string? GetPluginDirectory()
		{
			var domain = AppDomain.CurrentDomain;
			var searchPath = domain.RelativeSearchPath ?? domain.BaseDirectory;
			var pluginDirectory = Path.Combine(searchPath, "Plugins");

			if (!Directory.Exists(pluginDirectory))
			{
				return null;
			}

			return pluginDirectory;
		}

		static IPlugin? Load(string filename)
		{
			Assembly ass;

			try
			{
				ass = Assembly.LoadFrom(filename);
			}
			catch (BadImageFormatException)
			{
				return null;
			}

			var constructor = GetPluginType(ass);

			if (constructor == null)
			{
				return null;
			}

			return (IPlugin)constructor.Invoke(null);
		}

		static ConstructorInfo? GetPluginType(Assembly assembly)
		{
			var att = (PluginAttribute?)Attribute.GetCustomAttribute(assembly, typeof(PluginAttribute));

			if (att == null) return null;

			var entryPoint = att.EntryPoint;

			if (entryPoint == null || !entryPoint.IsClass || entryPoint.IsAbstract || !typeof(IPlugin).IsAssignableFrom(entryPoint))
			{
				return null;
			}

			var constructor = entryPoint.GetConstructor(Type.EmptyTypes);

			if (constructor == null || !constructor.IsPublic || constructor.IsStatic)
			{
				return null;
			}

			return constructor;
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		static readonly List<IFormatReader> _readers_internal = [];
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		static readonly ReadOnlyCollection<IFormatReader> _readers = new(_readers_internal);
	}
}
