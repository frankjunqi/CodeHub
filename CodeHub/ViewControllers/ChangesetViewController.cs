using CodeHub.Controllers;
using MonoTouch.Dialog;
using GitHubSharp.Models;
using System;
using CodeHub.ViewModels;
using CodeFramework.ViewControllers;

namespace CodeHub.ViewControllers
{
    public class ChangesetViewController : ViewModelCollectionDrivenViewController
    {
        public new ChangesetViewModel ViewModel
        {
            get { return (ChangesetViewModel)base.ViewModel; }
            set { base.ViewModel = value; }
        }

        private ChangesetViewController()
        {
            Title = "Changes".t();
            Root.UnevenRows = true;
            EnableSearch = false;
        }

        public ChangesetViewController(string user, string slug) : this()
        {
            ViewModel = new ChangesetViewModel(user, slug);
            DoBinding();
        }

        public ChangesetViewController(string user, string slug, ulong pullRequestId) : this()
        {
            //Controller = new PullRequestCommitsController(this, user, slug, pullRequestId);
            throw new InvalidOperationException("SHIT");
            DoBinding();
        }

        private void DoBinding()
        {
            BindCollection(ViewModel, x => x.Commits, x => {
                var desc = (x.Commit.Message ?? "").Replace("\n", " ").Trim();
                string login;
                var date = DateTime.MinValue;

                if (x.Committer != null)
                    login = x.Committer.Login;
                else if (x.Author != null)
                    login = x.Author.Login;
                else if (x.Commit.Committer != null)
                    login = x.Commit.Committer.Name;
                else
                    login = "Unknown";

                if (x.Commit.Committer != null)
                    date = x.Commit.Committer.Date;

                var el = new NameTimeStringElement { Name = login, Time = date.ToDaysAgo(), String = desc, Lines = 4 };
                el.Tapped += () => NavigationController.PushViewController(new ChangesetInfoViewController(ViewModel.Username, ViewModel.Repository, x.Sha), true);
                return el;
            });
        }
    }
}

