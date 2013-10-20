using System;
using CodeHub.Controllers;
using MonoTouch.UIKit;
using GitHubSharp.Models;
using CodeFramework.Elements;
using CodeHub.Filters.Models;
using CodeHub.Filters.ViewControllers;
using CodeHub.ViewModels;

namespace CodeHub.ViewControllers
{
    public class MyIssuesViewController : BaseIssuesViewController
    {
        private readonly UISegmentedControl _viewSegment;
        private readonly UIBarButtonItem _segmentBarButton;

        public new MyIssuesViewModel ViewModel
        {
            get { return (MyIssuesViewModel)base.ViewModel; }
            protected set { base.ViewModel = value; }
        }

        public MyIssuesViewController()
            : base(new MyIssuesViewModel())
        {
            _viewSegment = new UISegmentedControl(new string[] { "Open".t(), "Closed".t(), "Custom".t() });
            _viewSegment.ControlStyle = UISegmentedControlStyle.Bar;
            _segmentBarButton = new UIBarButtonItem(_viewSegment);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

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
            if (ViewModel.Filter.Equals(MyIssuesFilterModel.CreateOpenFilter()))
                _viewSegment.SelectedSegment = 0;
            else if (ViewModel.Filter.Equals(MyIssuesFilterModel.CreateClosedFilter()))
                _viewSegment.SelectedSegment = 1;
            else
                _viewSegment.SelectedSegment = 2;

            _viewSegment.ValueChanged += SegmentValueChanged;
        }

        void SegmentValueChanged (object sender, EventArgs e)
        {
            if (_viewSegment.SelectedSegment == 0)
            {
                ViewModel.ApplyFilter(MyIssuesFilterModel.CreateOpenFilter(), true);
            }
            else if (_viewSegment.SelectedSegment == 1)
            {
                ViewModel.ApplyFilter(MyIssuesFilterModel.CreateClosedFilter(), true);
            }
            else if (_viewSegment.SelectedSegment == 2)
            {
                var filter = new  MyIssuesFilterViewController(ViewModel);
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
            throw new NotImplementedException("Removed!");
//            //If null then it's been deleted!
//            if (changedModel == null)
//                ViewModel.Items.Remove(oldModel);
//            else
//                ViewModel.UpdateIssue(changedModel);

        }
    }
}

