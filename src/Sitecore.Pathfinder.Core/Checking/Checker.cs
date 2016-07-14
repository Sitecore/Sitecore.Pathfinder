// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.IO;
using System.Runtime.CompilerServices;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Projects;
using Sitecore.Pathfinder.Snapshots;

namespace Sitecore.Pathfinder.Checking
{
    public class Checker
    {
        protected virtual bool DirectoryExists([NotNull] ICheckerContext context, [NotNull] string directory)
        {
            Assert.IsNotNullOrEmpty(directory);

            if (!directory.StartsWith("~/"))
            {
                throw new ArgumentException("Directory must start with '~/'");
            }

            directory = Path.Combine(context.Project.ProjectDirectory, PathHelper.NormalizeFilePath(directory.Mid(2)));

            return context.FileSystem.DirectoryExists(directory);
        }

        [NotNull]
        protected Diagnostic Error(int msg, [NotNull] string text, [NotNull] string fileName, TextSpan textSpan, [NotNull] string details = "", [NotNull, CallerMemberName] string checkerName = "")
        {
            return new Diagnostic(msg, fileName, textSpan, Severity.Error, GetText(text, details, checkerName));
        }

        [NotNull]
        protected Diagnostic Error(int msg, [NotNull] string text, [NotNull] ISourceFile sourceFile, [NotNull] string details = "", [NotNull, CallerMemberName] string checkerName = "")
        {
            return new Diagnostic(msg, sourceFile.AbsoluteFileName, TextSpan.Empty, Severity.Error, GetText(text, details, checkerName));
        }

        [NotNull]
        protected Diagnostic Error(int msg, [NotNull] string text, [NotNull] ITextNode textNode, [NotNull] string details = "", [NotNull, CallerMemberName] string checkerName = "")
        {
            return new Diagnostic(msg, textNode.Snapshot.SourceFile.AbsoluteFileName, textNode.TextSpan, Severity.Error, GetText(text, details, checkerName));
        }

        [NotNull]
        protected Diagnostic Error(int msg, [NotNull] string text, [NotNull] string details = "", [NotNull, CallerMemberName] string checkerName = "")
        {
            return new Diagnostic(msg, string.Empty, TextSpan.Empty, Severity.Error, GetText(text, details, checkerName));
        }

        protected virtual bool FileExists([NotNull] ICheckerContext context, [NotNull] string fileName)
        {
            Assert.IsNotNullOrEmpty(fileName);

            if (!fileName.StartsWith("~/"))
            {
                throw new ArgumentException("File name must start with '~/'");
            }

            fileName = Path.Combine(context.Project.ProjectDirectory, PathHelper.NormalizeFilePath(fileName.Mid(2)));

            return context.FileSystem.FileExists(fileName);
        }

        [NotNull]
        protected Diagnostic Information(int msg, [NotNull] string text, [NotNull] string fileName, TextSpan textSpan, [NotNull] string details = "", [NotNull, CallerMemberName] string checkerName = "")
        {
            return new Diagnostic(msg, fileName, textSpan, Severity.Information, GetText(text, details, checkerName));
        }

        [NotNull]
        protected Diagnostic Information(int msg, [NotNull] string text, [NotNull] ISourceFile sourceFile, [NotNull] string details = "", [NotNull, CallerMemberName] string checkerName = "")
        {
            return new Diagnostic(msg, sourceFile.AbsoluteFileName, TextSpan.Empty, Severity.Information, GetText(text, details, checkerName));
        }

        [NotNull]
        protected Diagnostic Information(int msg, [NotNull] string text, [NotNull] ITextNode textNode, [NotNull] string details = "", [NotNull, CallerMemberName] string checkerName = "")
        {
            return new Diagnostic(msg, textNode.Snapshot.SourceFile.AbsoluteFileName, textNode.TextSpan, Severity.Information, GetText(text, details, checkerName));
        }

        [NotNull]
        protected Diagnostic Warning(int msg, [NotNull] string text, [NotNull] string details = "", [NotNull, CallerMemberName] string checkerName = "")
        {
            return new Diagnostic(msg, string.Empty, TextSpan.Empty, Severity.Warning, GetText(text, details, checkerName));
        }

        [NotNull]
        protected Diagnostic Warning(int msg, [NotNull] string text, [NotNull] string fileName, TextSpan textSpan, [NotNull] string details = "", [NotNull, CallerMemberName] string checkerName = "")
        {
            return new Diagnostic(msg, fileName, textSpan, Severity.Warning, GetText(text, details, checkerName));
        }

        [NotNull]
        protected Diagnostic Warning(int msg, [NotNull] string text, [NotNull] ISourceFile sourceFile, [NotNull] string details = "", [NotNull, CallerMemberName] string checkerName = "")
        {
            return new Diagnostic(msg, sourceFile.AbsoluteFileName, TextSpan.Empty, Severity.Warning, GetText(text, details, checkerName));
        }

        [NotNull]
        protected Diagnostic Warning(int msg, [NotNull] string text, [NotNull] ITextNode textNode, [NotNull] string details = "", [NotNull, CallerMemberName] string checkerName = "")
        {
            return new Diagnostic(msg, textNode.Snapshot.SourceFile.AbsoluteFileName, textNode.TextSpan, Severity.Warning, GetText(text, details, checkerName));
        }

        [NotNull]
        private string GetText([NotNull] string text, [NotNull] string details, [NotNull] string checkerName)
        {
            if (!string.IsNullOrEmpty(details))
            {
                text += ": " + details;
            }

            text += " [" + checkerName + "]";

            return text;
        }
    }
}
