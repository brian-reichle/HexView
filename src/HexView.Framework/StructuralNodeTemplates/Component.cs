// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;

namespace HexView.Framework
{
	public struct Component
	{
		public Component(string name, IStructuralNodeTemplate template, long offset)
		{
			if (template == null) throw new ArgumentNullException(nameof(template));

			Name = name;
			Template = template;
			Offset = offset;
		}

		public string Name { get; }
		public IStructuralNodeTemplate Template { get; }
		public long Offset { get; }
	}
}
