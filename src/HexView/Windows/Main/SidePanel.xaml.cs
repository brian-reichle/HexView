// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System.Windows;
using System.Windows.Controls;
using HexView.Framework;

namespace HexView
{
	public partial class SidePanel : UserControl
	{
		public SidePanel()
		{
			InitializeComponent();
		}

		void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
			=> Commands.SelectNode.Execute(e.NewValue as IStructuralNode, this);
	}
}
