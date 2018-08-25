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
			var strValue = (string)value;

			if (!HexHelper.TryParse(strValue, out var longValue))
			{
				return new ValidationResult(false, "Invalid Value");
			}

			return ValidationResult.ValidResult;
		}
	}
}
