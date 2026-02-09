// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;

namespace HexView.Framework;

[AttributeUsage(AttributeTargets.Assembly)]
public sealed class PluginAttribute : Attribute
{
	public PluginAttribute(Type entryPoint)
	{
		EntryPoint = entryPoint;
	}

	public Type EntryPoint { get; }
}
