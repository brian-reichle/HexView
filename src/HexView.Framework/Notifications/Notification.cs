// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;

namespace HexView.Framework
{
	public sealed class Notification
	{
		public Notification(long offset, IStructuralNodeTemplate template, string message)
		{
			if (template == null) throw new ArgumentNullException(nameof(template));
			if (string.IsNullOrEmpty(message)) throw new ArgumentNullException(nameof(message));

			Offset = offset;
			Template = template;
			Message = message;
		}

		public Notification(long offset, long length, string message)
			: this(offset, StandardTemplates.Blob(length), message)
		{
		}

		public long Offset { get; }
		public IStructuralNodeTemplate Template { get; }
		public string Message { get; }
	}
}
