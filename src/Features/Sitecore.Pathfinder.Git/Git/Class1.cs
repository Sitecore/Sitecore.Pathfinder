// © 2015-2016 Sitecore Corporation A/S. All rights reserved.

using System;
using System.Linq;
using LibGit2Sharp;

namespace Sitecore.Pathfinder.Git.Git
{
    public class Class1
    {
        public void Test()
        {
            using (var localRepo = new Repository(""))
            {
                localRepo.

                // Add a commit
                Commit first = AddCommitToRepo(localRepo);

                Remote remote = localRepo.Network.Remotes.Add("origin", remoteRepoPath);

                localRepo.Branches.Update(localRepo.Head,
                    b => b.Remote = remote.Name,
                    b => b.UpstreamBranch = localRepo.Head.CanonicalName);

                // Push this commit
                localRepo.Network.Push(localRepo.Head);
                AssertRemoteHeadTipEquals(localRepo, first.Sha);

                UpdateTheRemoteRepositoryWithANewCommit(remoteRepoPath);

                // Add another commit
                var oldId = localRepo.Head.Tip.Id;
                Commit second = AddCommitToRepo(localRepo);

                // Try to fast forward push this new commit
                Assert.Throws<NonFastForwardException>(() => localRepo.Network.Push(localRepo.Head));

                // Force push the new commit
                string pushRefSpec = string.Format("+{0}:{0}", localRepo.Head.CanonicalName);

                var before = DateTimeOffset.Now.TruncateMilliseconds();

                localRepo.Network.Push(localRepo.Network.Remotes.Single(), pushRefSpec);

                AssertRemoteHeadTipEquals(localRepo, second.Sha);

                AssertRefLogEntry(localRepo, "refs/remotes/origin/master",
                    "update by push",
                    oldId, localRepo.Head.Tip.Id,
                    Constants.Identity, before);
            }
        } 
    }
}