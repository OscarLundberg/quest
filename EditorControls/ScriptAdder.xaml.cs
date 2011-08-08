﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AxeSoftware.Quest.EditorControls
{
    public partial class ScriptAdder : UserControl
    {
        private bool m_initialised = false;
        private EditorController m_controller;
        private string m_selection;

        public event Action<string> AddScript;
        public event Action CloseClicked;

        public ScriptAdder()
        {
            InitializeComponent();
        }

        void ctlEditorTree_SelectionChanged(string key)
        {
            m_selection = key;
        }

        void ctlEditorTree_CommitSelection()
        {
            AddCurrent();
        }

        public void Initialise(EditorController controller)
        {
            if (!m_initialised)
            {
                ctlEditorTree.CommitSelection += ctlEditorTree_CommitSelection;
                ctlEditorTree.SelectionChanged += ctlEditorTree_SelectionChanged;

                m_controller = controller;

                ctlEditorTree.RemoveContextMenu();
                ctlEditorTree.IncludeRootLevelInSearchResults = false;
                ctlEditorTree.ShowFilterBar = false;

                foreach (string cat in m_controller.GetAllScriptEditorCategories())
                {
                    ctlEditorTree.AddNode(cat, cat, null, null, null);
                }

                foreach (var data in m_controller.GetScriptEditorData())
                {
                    ctlEditorTree.AddNode(data.Key, data.Value.AdderDisplayString, data.Value.Category, null, null);
                }

                m_initialised = true;
            }

            ctlEditorTree.ExpandAll();
        }

        public void Uninitialise()
        {
            ctlEditorTree.CommitSelection -= ctlEditorTree_CommitSelection;
            ctlEditorTree.SelectionChanged -= ctlEditorTree_SelectionChanged;
            m_controller = null;
        }

        private WFEditorTree ctlEditorTree
        {
            get { return (WFEditorTree)treeHost.Child; }
        }

        public bool CloseButtonVisible
        {
            get { return closeButton.Visibility == Visibility.Visible; }
            set { closeButton.Visibility = value ? Visibility.Visible : Visibility.Collapsed; }
        }

        private void AddCurrent()
        {
            EditableScriptData data = null;
            if ((m_selection != null) && m_controller.GetScriptEditorData().TryGetValue(m_selection, out data))
            {
                AddScript(data.CreateString);
            }
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            AddCurrent();
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            CloseClicked();
        }
    }
}
