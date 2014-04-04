using smb3rr;

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace smb3rrGUI
{

    class TreeViewModel : INotifyPropertyChanged
    {

        TreeViewModel(string name)
        {
            Name = name;
            Children = new List<TreeViewModel>();
        }

        private bool? _isChecked = false;
        private TreeViewModel _parent;

        public string Name { get; private set; }
        public List<TreeViewModel> Children { get; private set; }
        public bool IsInitiallySelected { get; private set; }

        public bool? IsChecked
        {
            
            get
            {
                return _isChecked;
            }

            set
            {
                SetIsChecked(value, true, true);
            }

        }

        private void SetIsChecked(bool? value, bool updateChildren, bool updateParent)
        {

            if (value == _isChecked)
            {
                return;
            }

            _isChecked = value;

            if (updateChildren && _isChecked.HasValue)
            {
                Children.ForEach(child => child.SetIsChecked(_isChecked, true, false));
            }

            if (updateParent && (_parent != null))
            {
                _parent.VerifyCheckedState();
            }

            NotifyPropertyChanged("IsChecked");

        }

        private void VerifyCheckedState()
        {

            bool? state = null;

            for (int i = 0; i < Children.Count; ++i)
            {

                bool? current = Children[i].IsChecked;
                if (i == 0)
                {
                    state = current;
                }
                else if (state != current)
                {
                    state = null;
                    break;
                }

            }

            SetIsChecked(state, false, true);

        }

        private void Initialize()
        {

            foreach (TreeViewModel child in Children)
            {
                child._parent = this;
                child.Initialize();
            }

        }

        public static List<TreeViewModel> SetTree()
        {

            List<TreeViewModel> treeView = new List<TreeViewModel>();

            TreeViewModel regular = new TreeViewModel("Regular Levels");
            treeView.Add(regular);
            for (int i = 1; i <= 8; ++i)
                regular.Children.Add(new TreeViewModel("World " + i.ToString()));
            for (int i = 1; i <= 6; ++i)
                regular.Children[0].Children.Add(new TreeViewModel("1-" + i.ToString()));
            for (int i = 1; i <= 5; ++i)
                regular.Children[1].Children.Add(new TreeViewModel("2-" + i.ToString()));
            regular.Children[1].Children.Add(new TreeViewModel("2-sandpit"));
            regular.Children[1].Children.Add(new TreeViewModel("2-pyramid"));
            for (int i = 1; i <= 9; ++i)
                regular.Children[2].Children.Add(new TreeViewModel("3-" + i.ToString()));
            for (int i = 1; i <= 6; ++i)
                regular.Children[3].Children.Add(new TreeViewModel("4-" + i.ToString()));
            for (int i = 1; i <= 9; ++i)
                regular.Children[4].Children.Add(new TreeViewModel("5-" + i.ToString()));
            for (int i = 1; i <= 10; ++i)
                regular.Children[5].Children.Add(new TreeViewModel("6-" + i.ToString()));
            for (int i = 1; i <= 9; ++i)
                regular.Children[6].Children.Add(new TreeViewModel("7-" + i.ToString()));
            for (int i = 1; i <= 2; ++i)
                regular.Children[7].Children.Add(new TreeViewModel("8-" + i.ToString()));

            TreeViewModel fort = new TreeViewModel("Fortresses");
            treeView.Add(fort);
            fort.Children.Add(new TreeViewModel("1-F"));
            fort.Children.Add(new TreeViewModel("2-F"));
            fort.Children.Add(new TreeViewModel("World 3"));
            fort.Children[2].Children.Add(new TreeViewModel("3-F1"));
            fort.Children[2].Children.Add(new TreeViewModel("3-F2"));
            fort.Children.Add(new TreeViewModel("4-F1"));
            fort.Children.Add(new TreeViewModel("World 5"));
            fort.Children[4].Children.Add(new TreeViewModel("5-F1"));
            fort.Children[4].Children.Add(new TreeViewModel("5-F2"));
            fort.Children.Add(new TreeViewModel("6-F2"));
            fort.Children.Add(new TreeViewModel("7-F2"));

            TreeViewModel castle = new TreeViewModel("Castles");
            treeView.Add(castle);
            for (int i = 1; i <= 7; ++i)
                castle.Children.Add(new TreeViewModel("Castle " + i.ToString()));

            foreach (TreeViewModel tv in treeView)
                tv.Initialize();

            return treeView;

        }

        private void NotifyPropertyChanged(string info)
        {

            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;

    }

}
