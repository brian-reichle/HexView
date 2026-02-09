// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HexView;

sealed class HexCharSet
{
	public HexCharSet(DependencyObject dobj)
	{
		_typeface = new Typeface(
			TextBlock.GetFontFamily(dobj),
			TextBlock.GetFontStyle(dobj),
			TextBlock.GetFontWeight(dobj),
			TextBlock.GetFontStretch(dobj));

		_emSize = TextBlock.GetFontSize(dobj);

		if (!_typeface.TryGetGlyphTypeface(out _glyphTypeface))
		{
			throw new InvalidOperationException();
		}

		var width = 0d;

		for (var i = 0; i < HexHelper.HexChars.Length; i++)
		{
			var c = HexHelper.HexChars[i];
			var index = _glyphTypeface.CharacterToGlyphMap[c];

			var tmp = _glyphTypeface.AdvanceWidths[index];

			if (tmp > width)
			{
				width = tmp;
			}
		}

		CellWidth = width * _emSize;
		CellHeight = _glyphTypeface.Height * _emSize;
	}

	public double CellWidth { get; }
	public double CellHeight { get; }

	public GlyphRun NewGlyphRun(Point origin, char[] chars, IList<double> advanceWidths, float pixelsPerDip)
	{
		var indexes = new ushort[chars.Length];

		for (var i = 0; i < indexes.Length; i++)
		{
			indexes[i] = _glyphTypeface.CharacterToGlyphMap[chars[i]];
		}

		return new GlyphRun(
			_glyphTypeface,
			0,
			false,
			_emSize,
			pixelsPerDip,
			indexes,
			new Point(origin.X, origin.Y + _glyphTypeface.Baseline * _emSize),
			advanceWidths,
			null,
			null,
			null,
			null,
			null,
			null);
	}

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	readonly Typeface _typeface;
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	readonly GlyphTypeface _glyphTypeface;
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	readonly double _emSize;
}
