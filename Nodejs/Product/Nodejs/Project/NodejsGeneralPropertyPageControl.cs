﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.NodejsTools.TestFrameworks;
using Microsoft.VisualStudioTools.Project;

namespace Microsoft.NodejsTools.Project
{
    internal partial class NodejsGeneralPropertyPageControl : UserControl
    {
        private readonly NodejsGeneralPropertyPage _propPage;
        private const string _exeFilter = "Executable Files (*.exe)|*.exe|All Files (*.*)|*.*";

        public NodejsGeneralPropertyPageControl()
        {
            InitializeComponent();

            var testFrameworks = TestFrameworkDirectories.GetFrameworkNames();
            this._frameworkSelector.Items.AddRange(testFrameworks);

            LocalizeLabels();
            AddToolTips();
        }

        public NodejsGeneralPropertyPageControl(NodejsGeneralPropertyPage page) : this()
        {
            this._propPage = page;
        }

        private void LocalizeLabels()
        {
            // There's a bug in winforms, where if you use the default localization infra structure
            // the control is not correctly sized in the property page.

            this._nodeExePathLabel.Text = Resources.PropertiesNodeExePath;
            this._nodeArgumentsLabel.Text = Resources.PropertiesNodeExeOptions;
            this._scriptLabel.Text = Resources.PropertiesScript;
            this._scriptArgsLabel.Text = Resources.PropertiesScriptArgs;
            this._workingDirLabel.Text = Resources.PropertiesWorkingDir;
            this._launchUrlLabel.Text = Resources.PropertiesLaunchUrl;
            this._nodePortLabel.Text = Resources.PropertiesNodePort;
            this._debuggerPortLabel.Text = Resources.PropertiesDebuggerPort;
            this._envVarsLabel.Text = Resources.PropertiesEnvVars;
            this._startBrowserCheckBox.Text = Resources.PropertiesStartBrowser;
            this._startActionHeaderLabel.Text = Resources.StartActionHeader;
            this._nodeHeaderLabel.Text = Resources.NodeHeader;
            this._saveInProjectFileCheckBox.Text = Resources.SaveNodeSettingsInProjectFile;
            this._TestHeaderLabel.Text = Resources.UnitTestHeader;
            this._testFrameworkLabel.Text = Resources.TestFramework;
            this._testRootLabel.Text = Resources.TestRoot;

            this._browsePath.AccessibleName = Resources.PropertiesBrowsePathAccessibleName;
            this._browsePath.AccessibleName = Resources.PropertiesBrowseDirectoryAccessibleName;
            this._browseTestroot.AccessibleName = Resources.PropertiesBrowseTestRootAccessibleName;
        }

        private void AddToolTips()
        {
            this._tooltip.SetToolTip(this._nodeExePath, Resources.NodeExePathToolTip);
            this._tooltip.SetToolTip(this._nodeExeArguments, Resources.NodeExeArgumentsToolTip);
            this._tooltip.SetToolTip(this._scriptFile, Resources.ScriptFileTooltip);
            this._tooltip.SetToolTip(this._scriptArguments, Resources.ScriptArgumentsToolTip);
            this._tooltip.SetToolTip(this._nodejsPort, Resources.NodejsPortToolTip);
            this._tooltip.SetToolTip(this._startBrowserCheckBox, Resources.StartBrowserToolTip);
            this._tooltip.SetToolTip(this._workingDir, Resources.WorkingDirToolTip);
            this._tooltip.SetToolTip(this._launchUrl, Resources.LaunchUrlToolTip);
            this._tooltip.SetToolTip(this._debuggerPort, Resources.DebuggerPort);
            this._tooltip.SetToolTip(this._envVars, Resources.EnvironmentVariables);
            this._tooltip.SetToolTip(this._saveInProjectFileCheckBox, Resources.SaveInProjectFileToolTip);
            this._tooltip.SetToolTip(this._testRoot, Resources.TestRootToolTip);
            this._tooltip.SetToolTip(this._frameworkSelector, Resources.TestFrameworkToolTip);
        }

        public string NodeExePath
        {
            get
            {
                return this._nodeExePath.Text;
            }
            set
            {
                this._nodeExePath.Text = value;
            }
        }

        public string NodeExeArguments
        {
            get
            {
                return this._nodeExeArguments.Text;
            }
            set
            {
                this._nodeExeArguments.Text = value;
            }
        }

        public string ScriptFile
        {
            get
            {
                return this._scriptFile.Text;
            }
            set
            {
                this._scriptFile.Text = value;
            }
        }

        public string ScriptArguments
        {
            get
            {
                return this._scriptArguments.Text;
            }
            set
            {
                this._scriptArguments.Text = value;
            }
        }

        public string NodejsPort
        {
            get
            {
                return this._nodejsPort.Text;
            }
            set
            {
                this._nodejsPort.Text = value;
            }
        }

        public bool StartWebBrowser
        {
            get
            {
                return this._startBrowserCheckBox.Checked;
            }
            set
            {
                this._startBrowserCheckBox.Checked = value;
            }
        }

        public string WorkingDirectory
        {
            get
            {
                return this._workingDir.Text;
            }
            set
            {
                this._workingDir.Text = value;
            }
        }

        public string LaunchUrl
        {
            get
            {
                return this._launchUrl.Text;
            }
            set
            {
                this._launchUrl.Text = value;
            }
        }

        private static readonly Regex lfToCrLfRegex = new Regex(@"(?<!\r)\n");

        public string Environment
        {
            get
            {
                return this._envVars.Text;
            }
            set
            {
                // TextBox requires \r\n for line separators, but XML can have either \n or \r\n, and we should treat those equally.
                // (It will always have \r\n when we write it out, but users can edit it by other means.)
                this._envVars.Text = lfToCrLfRegex.Replace(value ?? string.Empty, "\r\n");
            }
        }

        public string DebuggerPort
        {
            get
            {
                return this._debuggerPort.Text;
            }
            set
            {
                this._debuggerPort.Text = value;
            }
        }

        public bool SaveNodeSettingsInProject
        {
            get
            {
                return this._saveInProjectFileCheckBox.Checked;
            }
            set
            {
                this._saveInProjectFileCheckBox.Checked = value;
            }
        }

        public string TestFramework
        {
            get
            {
                return (string)this._frameworkSelector.SelectedItem;
            }
            set
            {
                this._frameworkSelector.SelectedItem = value;
            }
        }

        public string TestRoot
        {
            get
            {
                return this._testRoot.Text;
            }
            set
            {
                this._testRoot.Text = value;
            }
        }

        private void Changed(object sender, EventArgs e)
        {
            this._propPage.IsDirty = true;
        }

        private void NodeExePathChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this._nodeExePath.Text)
                && !this._nodeExePath.Text.Contains("$(")
                && !File.Exists(Nodejs.GetAbsoluteNodeExePath(this._propPage.Project.ProjectHome, this._nodeExePath.Text)))
            {
                DisplayWarning(Resources.NodeExePathNotFound);
            }
            Changed(sender, e);
        }

        private void BrowsePathClick(object sender, EventArgs e)
        {
            var nodeExePath = this._nodeExePath.Text;
            if (this.GetFileViaBrowse(nodeExePath, ref nodeExePath, _exeFilter) && !string.IsNullOrEmpty(nodeExePath))
            {
                this._nodeExePath.Text = nodeExePath;
                this._nodeExePath.ForeColor = SystemColors.ControlText;
            }
        }

        private void BrowseDirectoryClick(object sender, EventArgs e)
        {
            var projectHome = this._propPage.Project.ProjectHome;
            var dir = this._workingDir.Text;
            if (string.IsNullOrEmpty(dir))
            {
                dir = projectHome;
            }

            if (this.GetDirectoryViaBrowseRelative(dir, projectHome, Resources.BrowseWorkingDirDialogTitle, ref dir))
            {
                this._workingDir.Text = string.IsNullOrEmpty(dir) ? "." : dir;
            }
        }

        private void BrowseTestRootClick(object sender, EventArgs e)
        {
            var projectHome = this._propPage.Project.ProjectHome;
            var dir = this._testRoot.Text;
            if (string.IsNullOrEmpty(dir))
            {
                dir = projectHome;
            }

            if (this.GetDirectoryViaBrowseRelative(dir, projectHome, Resources.BrowseWorkingDirDialogTitle, ref dir))
            {
                this._testRoot.Text = string.IsNullOrEmpty(dir) ? "." : dir;
            }
        }

        private void PortChanged(object sender, EventArgs e)
        {
            var textSender = (TextBox)sender;
            if (!textSender.Text.Contains("$(") &&
                textSender.Text.Any(ch => !char.IsDigit(ch)))
            {
                DisplayWarning(Resources.InvalidPortNumber);
            }
            Changed(sender, e);
        }

        private static void DisplayWarning(string text)
        {
            MessageBox.Show(text, Resources.WarningDialogCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void WorkingDirChanged(object sender, EventArgs e)
        {
            if (!IsValidWorkingDir(this._workingDir.Text))
            {
                DisplayWarning(Resources.WorkingDirInvalidOrMissing);
            }

            Changed(sender, e);

            bool IsValidWorkingDir(string workingDir)
            {
                if (workingDir.Contains("$("))
                {
                    return true;
                }

                var fullPath = Path.IsPathRooted(workingDir) ? workingDir : Path.Combine(this._propPage.Project.ProjectHome, workingDir);
                return Directory.Exists(fullPath);
            }
        }
    }
}
