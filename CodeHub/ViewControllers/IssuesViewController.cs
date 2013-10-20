using CodeHub.Controllers;
using MonoTouch.UIKit;
using CodeFramework.Views;
using GitHubSharp.Models;
using CodeFramework.Elements;
using CodeHub.Filters.Models;
using System;
using CodeHub.Filters.ViewControllers;

namespace CodeHub.ViewControllers
{
    public class IssuesViewController : BaseIssuesViewController
    {
        private UISegmentedControl _viewSegment;
        private UIBarButtonItem _segmentBarButton;

        public new IssuesViewModel ViewModel
        {
            get { return (IssuesViewModel)base.ViewModel; }
            protected set { base.ViewModel = value; }
        }

        public IssuesViewController(string user, string slug)
            : base(new IssuesViewModel(user, slug))
        {
            NavigationItem.RightBarButtonItem = new UIBarButtonItem(NavigationButton.Create(Theme.CurrentTheme.AddButton, () => {
                var b = new IssueEditViewController(ViewModel.User, ViewModel.Slug) {
                    Success = (issue) => ViewModel.CreateIssue(issue)
                };
                NavigationController.PushViewController(b, true);
            }));
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            _viewSegment = new UISegmentedControl(new string[] { "Open".t(), "Closed".t(), "Mine".t(), "Custom".t() });
            _viewSegment.ControlStyle = UISegmentedControlStyle.Bar;
            _segmentBarButton = new UIBarButtonItem(_viewSegment);
            _segmentBarButton.Width = View.Frame.Width - 10f;
            ToolbarItems = new [] { new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace), _segmentBarButton, new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace) };
        }

        protected override void SearchEnd()
        {
            base.SearchEnd();
            if (ToolbarItems != null)
                NavigationController.SetToolbarHidden(false, true);
        }

        public override void ViewWillAppear(bool animated)
        {
            if (ToolbarItems != null && !IsSearching)
                NavigationController.SetToolbarHidden(false, animated);
            base.ViewWillAppear(animated);

            //Before we select which one, make sure we detach the event handler or silly things will happen
            _viewSegment.ValueChanged -= SegmentValueChanged;

            //Select which one is currently selected
            if (ViewModel.Filter.Equals(IssuesFilterModel.CreateOpenFilter()))
                _viewSegment.SelectedSegment = 0;
            else if (ViewModel.Filter.Equals(IssuesFilterModel.CreateClosedFilter()))
                _viewSegment.SelectedSegment = 1;
            else if (ViewModel.Filter.Equals(IssuesFilterModel.CreateMineFilter(Application.Account.Username)))
                _viewSegment.SelectedSegment = 2;
            else
                _viewSegment.SelectedSegment = 3;

            _viewSegment.ValueChanged += SegmentValueChanged;
        }

        void SegmentValueChanged (object sender, EventArgs e)
        {
            if (_viewSegment.SelectedSegment == 0)
            {
                ViewModel.ApplyFilter(IssuesFilterModel.CreateOpenFilter(), true);
            }
            else if (_viewSegment.SelectedSegment == 1)
            {
                ViewModel.ApplyFilter(IssuesFilterModel.CreateClosedFilter(), true);
            }
            else if (_viewSegment.SelectedSegment == 2)
            {
                ViewModel.ApplyFilter(IssuesFilterModel.CreateMineFilter(Application.Account.Username), true);
            }
            else if (_viewSegment.SelectedSegment == 3)
            {
                var filter = new IssuesFilterViewController(ViewModel);
                var nav = new UINavigationController(filter);
                PresentViewController(nav, true, null);
            }
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            if (ToolbarItems != null)
                NavigationController.SetToolbarHidden(true, animated);
        }

        protected override void ChildChangedModel(IssueModel changedModel, IssueModel oldModel)
        {
            //If null then it's been deleted!
            if (changedModel == null)
                ViewModel.Items.Remove(oldModel);
            else
                ViewModel.UpdateIssue(changedModel);
        }
    }
}

