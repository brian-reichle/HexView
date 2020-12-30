// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System.Collections.Generic;

namespace HexView.Framework
{
	public sealed class NotificationNode : VirtualizingStructuralNode
	{
		public NotificationNode(IDataSource data, IStructuralNode? parent, IReadOnlyList<Notification> notifications)
			: base(parent)
		{
			_data = data;
			_notifications = notifications;
		}

		public override string Name => "Notifications";
		public override ByteRange? ByteRange => null;
		protected override int Count => _notifications.Count;

		protected override IStructuralNode CreateChildNode(int index)
		{
			var notification = _notifications[index];

			return new TemplatedStructuralNode(
				_data,
				this,
				notification.Message,
				notification.Template,
				notification.Offset);
		}

		readonly IDataSource _data;
		readonly IReadOnlyList<Notification> _notifications;
	}
}
