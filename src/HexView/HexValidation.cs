// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System.Globalization;
using System.Windows.Controls;

namespace HexView
{
	sealed class HexValidation : ValidationRule
	{
		public HexValidation()
			: base(ValidationStep.RawProposedValue, false)
		{
		}

		public override ValidationResult Validate(object value, CultureInfo cultureInfo)
		{
			if (!HexHelper.TryParse((string)value, out _))
			{
				return new ValidationResult(false, "Invalid Value");
			}

			return ValidationResult.ValidResult;
		}
	}
}
