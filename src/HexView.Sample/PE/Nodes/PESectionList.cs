// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System.Collections.Generic;
using HexView.Framework;

namespace HexView.Plugins.Sample.PE
{
	sealed class PESectionList : LazyStructuralNode
	{
		public PESectionList(PEStructuralNodeProvider provider, IStructuralNode? parent)
			: base(parent)
		{
			_provider = provider;
		}

		public override string Name => "Sections";
		public override ByteRange? ByteRange => null;

		protected override IList<IStructuralNode> CreateChildNodes()
		{
			var result = new IStructuralNode[_provider.PESectionCount];
			var startingOffset = _provider.PESectionTableOffset;

			for (var i = 0; i < result.Length; i++)
			{
				result[i] = new PESection(this, _provider, startingOffset);
				startingOffset += Constants.Section_Length;
			}

			return result;
		}

		readonly PEStructuralNodeProvider _provider;
	}
}
