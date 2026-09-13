using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ValidationComponents
{
	public partial class RequiredFieldValidator : BaseValidator
	{
		public RequiredFieldValidator()
		{
			InitializeComponent();
		}

		public RequiredFieldValidator(IContainer container)
		{
			container.Add(this);

			InitializeComponent();
		}

		protected override bool EvaluateIsValid()
		{
			return ControlToValidate != null
				&& !string.IsNullOrWhiteSpace(ControlToValidate.Text);
		}

	}
}
