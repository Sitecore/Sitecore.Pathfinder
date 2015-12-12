// © 2015 Sitecore Corporation A/S. All rights reserved.

using System;

namespace Sitecore.Pathfinder.Diagnostics
{
    public static class Assert
    {
        [AssertionMethod]
        public static void Cast([AssertionCondition(AssertionConditionType.IS_NOT_NULL), CanBeNull] object instance, [CanBeNull] string variableName = null, [CanBeNull] string detailMessageFormat = null, [NotNull, ItemCanBeNull] params object[] args)
        {
            if (instance != null)
            {
                return;
            }

            throw new InvalidCastException(Format($"Failed to cast '{variableName}'", detailMessageFormat, args));
        }

        [AssertionMethod]
        public static void IsFalse([AssertionCondition(AssertionConditionType.IS_FALSE)] bool condition, [CanBeNull] string message = null, [CanBeNull] string detailMessageFormat = null, [NotNull, ItemCanBeNull] params object[] args)
        {
            if (!condition)
            {
                return;
            }

            throw new AssertException(Format(message, detailMessageFormat, args));
        }

        [AssertionMethod]
        public static void IsNotNull([AssertionCondition(AssertionConditionType.IS_NOT_NULL), CanBeNull] object instance, [CanBeNull] string message = null, [CanBeNull] string detailMessageFormat = null, [NotNull, ItemCanBeNull] params object[] args)
        {
            if (instance != null)
            {
                return;
            }

            throw new AssertException(Format(message, detailMessageFormat, args));
        }

        [AssertionMethod]
        public static void IsNotNullOrEmpty([AssertionCondition(AssertionConditionType.IS_NOT_NULL), CanBeNull] string text, [CanBeNull] string message = null, [CanBeNull] string detailMessageFormat = null, [NotNull, ItemCanBeNull] params object[] args)
        {
            if (!string.IsNullOrEmpty(text))
            {
                return;
            }

            throw new AssertException(Format(message, detailMessageFormat, args));
        }

        [AssertionMethod]
        public static void IsNull([AssertionCondition(AssertionConditionType.IS_NULL), CanBeNull] object instance, [CanBeNull] string message = null, [CanBeNull] string detailMessageFormat = null, [NotNull, ItemCanBeNull] params object[] args)
        {
            if (instance == null)
            {
                return;
            }

            throw new AssertException(Format(message, detailMessageFormat, args));
        }

        [AssertionMethod]
        public static void IsTrue([AssertionCondition(AssertionConditionType.IS_TRUE)] bool condition, [CanBeNull] string message = null, [CanBeNull] string detailMessageFormat = null, [NotNull, ItemCanBeNull] params object[] args)
        {
            if (condition)
            {
                return;
            }

            throw new AssertException(Format(message, detailMessageFormat, args));
        }

        [NotNull]
        private static string Format([CanBeNull] string message, [CanBeNull] string detailMessageFormat, [NotNull, ItemCanBeNull] object[] args)
        {
            var text = message ?? string.Empty;
            if (detailMessageFormat != null)
            {
                text += ": " + string.Format(detailMessageFormat, args);
            }

            return text;
        }
    }
}
