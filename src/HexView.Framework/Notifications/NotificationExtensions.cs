// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System.Collections.Generic;
using System.Globalization;

namespace HexView.Framework;

public static class NotificationExtensions
{
	public static void Add(this IList<Notification> notifications, long offset, long width, string message)
		=> notifications.Add(new Notification(offset, width, message));

	public static void Add(this IList<Notification> notifications, long offset, IStructuralNodeTemplate template, string message)
		=> notifications.Add(new Notification(offset, template, message));

	public static void Add(this IList<Notification> notifications, long offset, long width, string message, params object[] args)
		=> notifications.Add(new Notification(offset, width, string.Format(CultureInfo.CurrentCulture, message, args)));

	public static void Add(this IList<Notification> notifications, long offset, IStructuralNodeTemplate template, string message, params object[] args)
		=> notifications.Add(new Notification(offset, template, string.Format(CultureInfo.CurrentCulture, message, args)));
}
