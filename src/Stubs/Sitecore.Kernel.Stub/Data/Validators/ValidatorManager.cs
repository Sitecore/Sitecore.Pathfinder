// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Collections.Generic;
using Sitecore.Data.Items;

namespace Sitecore.Data.Validators
{
    public static class ValidatorManager
    {
        [NotNull, ItemNotNull]
        public static IEnumerable<BaseValidator> BuildValidators(ValidatorsMode validateButton, [NotNull] Item dataItem)
        {
            throw new NotImplementedException();
        }

        public static void Validate([NotNull, ItemNotNull] IEnumerable<BaseValidator> validatorCollection, [NotNull] ValidatorOptions validatorOptions)
        {
            throw new NotImplementedException();
        }
    }
}
