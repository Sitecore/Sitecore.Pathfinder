// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using Sitecore.Data.Items;

namespace Sitecore.Data.Validators
{
    public static class ValidatorManager
    {
        [NotNull]
        public static ValidatorCollection BuildValidators(ValidatorsMode validateButton, [NotNull] Item dataItem)
        {
            throw new NotImplementedException();
        }

        public static void Validate([NotNull] ValidatorCollection validatorCollection, [NotNull] ValidatorOptions validatorOptions)
        {
            throw new NotImplementedException();
        }
    }
}
