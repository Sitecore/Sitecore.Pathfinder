using Sitecore.Pathfinder.Diagnostics;

namespace Sitecore.Pathfinder.Xml.XPath
{
    public abstract class BinaryOperator : Opcode
    {
        protected BinaryOperator([NotNull] Opcode left, [NotNull] Opcode right)
        {
            Left = left;
            Right = right;
        }

        [NotNull]
        public Opcode Left { get; }

        [NotNull]
        public Opcode Right { get; }

        public override object Evaluate(Query query, object context)
        {
            var result1 = Left.Evaluate(query, context);
            var result2 = Right.Evaluate(query, context);

            return EvaluateOperands(result1, result2);
        }

        [CanBeNull]
        public abstract object EvaluateOperands([CanBeNull] object left, [CanBeNull] object right);
    }
}