// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System.Windows;

namespace HexView
{
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			PluginLoader.Load();

			var model = new Model();

			if (TryGetFirstArgument(e, out var filename))
			{
				model.Buffer = DataSource.Load(filename);
			}

			Resources.Add("Model", model);
			base.OnStartup(e);
		}

		static bool TryGetFirstArgument(StartupEventArgs e, out string arg)
		{
			string result;

			if (e != null &&
				e.Args is string[] args &&
				args.Length > 0 &&
				!string.IsNullOrEmpty(result = args[0]))
			{
				arg = result;
				return true;
			}

			arg = null;
			return false;
		}
	}
}
