// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using Sitecore.Data.Items;
using Sitecore.Data.Validators;
using Sitecore.Data.Validators.FieldValidators;
using Sitecore.Data.Validators.ItemValidators;
using Sitecore.Pathfinder.Emitting.Writers;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Projects.Templates;

namespace Sitecore.Pathfinder.Emitting.Emitters
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

            if (dataItem != null && context.Configuration.GetBool(Constants.Configuration.BuildProject.RunValidators))
            {
                Validate(context, template, dataItem);
            }
        }

        protected virtual void Validate([Diagnostics.NotNull] IEmitContext context, [Diagnostics.NotNull] Template template, [Diagnostics.NotNull] Item item)
        {
            var validatorCollection = new ValidatorCollection();
            foreach (BaseValidator validator in ValidatorManager.BuildValidators(ValidatorsMode.ValidatorBar, item))
            {
                // remove slow and obsolete validators
                if (validator is FullPageXHtmlValidator)
                {
                    continue;
                }

                if (validator is XhtmlValidator)
                {
                    continue;
                }

                if (validator is W3CXhtmlValidator)
                {
                    continue;
                }

                validatorCollection.Add(validator);
            }


            ValidatorManager.Validate(validatorCollection, new ValidatorOptions(true));

            foreach (BaseValidator validator in validatorCollection)
            {
                var text = validator.Text.TrimEnd('.');
                var details = validator.GetFieldDisplayName();
                if (details == "[unknown]")
                {
                    details = string.Empty;
                }

                switch (validator.Result)
                {
                    case ValidatorResult.Suggestion:
                    case ValidatorResult.Warning:
                        context.Trace.TraceWarning(Msg.E1008, text, template.SourceTextNode, details);
                        break;

                    case ValidatorResult.Error:
                    case ValidatorResult.CriticalError:
                    case ValidatorResult.FatalError:
                        context.Trace.TraceError(Msg.E1008, text, template.SourceTextNode, details);
                        break;
                }
            }
        }
    }
}
