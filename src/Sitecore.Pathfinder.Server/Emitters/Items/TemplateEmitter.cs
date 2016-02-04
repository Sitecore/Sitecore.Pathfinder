// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System.Linq;
using Sitecore.Data.Items;
using Sitecore.Data.Validators;
using Sitecore.Pathfinder.Emitters.Writers;
using Sitecore.Pathfinder.Emitting;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Templates;

namespace Sitecore.Pathfinder.Emitters.Items
{
    public class TemplateEmitter : EmitterBase
    {
        public TemplateEmitter() : base(Constants.Emitters.Templates)
        {
        }

        public override bool CanEmit(IEmitContext context, IProjectItem projectItem)
        {
            return projectItem is Template;
        }

        public override void Emit(IEmitContext context, IProjectItem projectItem)
        {
            var template = (Template)projectItem;
            if (!template.IsEmittable || template.IsImport)
            {
                return;
            }

            var templateWriter = new TemplateWriter(template);

            var dataItem = templateWriter.Write(context);

            if (dataItem != null)
            {
                Check(context, template, dataItem);
            }
        }

        protected virtual void Check([Diagnostics.NotNull] IEmitContext context, [Diagnostics.NotNull] Template item, [Diagnostics.NotNull] Item dataItem)
        {
            var validatorCollection = ValidatorManager.BuildValidators(ValidatorsMode.ValidateButton, dataItem);

            ValidatorManager.Validate(validatorCollection, new ValidatorOptions(true));

            foreach (BaseValidator validator in validatorCollection)
            {
                switch (validator.Result)
                {
                    case ValidatorResult.Suggestion:
                    case ValidatorResult.Warning:
                        context.Trace.TraceWarning(validator.Text, item.SourceTextNodes.First(), validator.GetFieldDisplayName());
                        break;

                    case ValidatorResult.Error:
                    case ValidatorResult.CriticalError:
                    case ValidatorResult.FatalError:
                        context.Trace.TraceError(validator.Text, item.SourceTextNodes.First(), validator.GetFieldDisplayName());
                        break;
                }
            }
        }
    }
}
