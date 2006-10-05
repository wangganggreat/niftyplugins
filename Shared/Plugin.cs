using System;
using System.Collections.Generic;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;

namespace Aurora
{
	// Wrapper class around registering other classes to handle the actual commands.
	// Interfaces with visual studio and handles the dispatch.
	class Plugin
	{
		public delegate void OnCommandFunction(DTE2 app, OutputWindowPane pane);
		private OutputWindowPane m_outputPane;
		
		private DTE2 m_application;
		private Dictionary<string, OnCommandFunction> m_commands;
		private string m_connectPath;

		public OutputWindowPane OutputPane
		{
			get { return m_outputPane; }
		}
		
		public Plugin( DTE2 application, string panelName, string connectPath )
		{
			// TODO: This can be figured out from traversing the assembly and locating the Connect class...
			m_connectPath = connectPath;
		
			m_commands = new Dictionary<string, OnCommandFunction>();
			m_application = application;
			m_outputPane = AquireOutputPane(application, panelName);
		}

		public void RegisterCommand(AddIn addIn, string commandName, string toolbars, string itemName, string description, OnCommandFunction callback)
		{
			m_commands.Add(commandName, callback);
			RegisterWithVisual(addIn, commandName, toolbars.Split(new char[] {';'} ), itemName, description);
		}

		public bool CanHandleCommand( string name )
		{
			// TODO: Gotta be a better way to do this... std::find anyone?
			foreach( string key in m_commands.Keys )
			{
				if( name.EndsWith("." + key) )
					return true;
			}
			
			return false;
		}
		
		public bool OnCommand( string name )
		{
			// TODO: Gotta be a better way to do this... std::find anyone?
			foreach (string key in m_commands.Keys)
			{
				if (name.EndsWith("." + key))
				{
					m_commands[key](m_application, m_outputPane);
					return true;
				}
			}
			
			return false;
		}

		private bool IsCommandRegistered( string commandName )
		{
			Commands2 commands = (Commands2)m_application.Commands;

			string fullName = m_connectPath + "." + commandName;
			
			try{
				Command command = commands.Item( fullName, 0 );
				return null != command;
			}
			catch( System.ArgumentException e )
			{
				return false;
			}
		}

		/*
			The toolbar name should be one of the following:
					MenuBar
					Standard
					Build
					XML Data
					XML Schema
					Context Menus
					Dialog Editor
					Image Editor
					Text Editor
					Source Control
					Formatting
					HTML Source Editing
					Style Sheet
					Device
					Layout
					Microsoft XML Editor
					Class Designer Toolbar
					Help
					Debug Location
					Debug
					Recorder
					Report Formatting
					Report Borders
					Data Design
					Query Designer
					View Designer
					Database Diagram
					Table Designer
					Project Node
					A&dd
					Cab Project Node
					A&dd
					File nodes
					Dep. file nodes
					Assembly nodes
					Dep. assembly nodes
					MSM nodes
					Dep. MSM nodes
					Output nodes
					Simple file nodes
					Simple output nodes
					Dependency node
					Multiple selections
					Dep. Multiple selections
					View
					Editor
					Error List
					Docked Window
					Menu Designer
					Properties Window
					Toolbox
					Task List
					Results List
					Stub Project
					No Commands Available
					Command Window
					AutoHidden Windows
					Expansion Manager
					Find Regular Expression Builder
					Replace Regular Expression Builder
					Wild Card Expression Builder
					Wild Card Expression Builder
					External Tools Arguments
					External Tools Directories
					Easy MDI Tool Window
					Easy MDI Document Window
					Easy MDI Dragging
					Open Drop Down
					Object Browser Objects Pane
					Object Browser Members Pane
					Object Browser Description Pane
					Find Symbol
					Drag and Drop
					Bookmark Window
					Error Correction
					EzMDI Files
					Ca&ll Browser
					Preview Changes
					Smart Tag
					Smart Tag
					Editor Context Menus
					Class View Context Menus
					Debugger Context Menus
					Project and Solution Context Menus
					Other Context Menus
					Sort By
					Show Columns
					Implement Interface
					Resolve
					Resolve
					Refactor
					Organize File
					Class View Project
					Class View Item
					Class View Folder
					Class View Grouping Folder
					Class View Multi-select
					Class View Multi-select members
					Class View Member
					Class View Grouping Members
					Class View Project References Folder
					Class View Project Reference
					Project
					Solution Folder
					Cross Project Solution Project
					Cross Project Solution Item
					Cross Project Project Item
					Cross Project Multi Project
					Cross Project Multi Item
					Cross Project Multi Solution Folder
					Cross Project Multi Project/Folder
					Item
					Folder
					Reference Root
					Reference Item
					Web Reference Folder
					App Designer Folder
					Web Project Folder
					Web Folder
					Web Item
					Web SubWeb
					Misc Files Project
					Solution
					Code Window
					Registry
					File System
					File System
					File Types
					User Interface
					Launch Conditions
					Custom Actions
					New
					Add
					Add Special Folder
					View
					Resource View
					Resource Editors
					Binary Editor
					Propertysheet
					Configuration
					Project
					Multi-Select
					System Propertysheet
					Checkin Dialog Context Menu
					Pending Checkin Window Context Menu
					Standard TreeGrid context menu
					GetVersion Dialog Context Menu
					Check Out Dialog Context Menu
					Context
					Basic Context
					Context
					Context
					Context
					Context
					Context
					Context
					HTML Context
					Script Context
					Context
					ASPX Context
					ASPX Code Context
					ASPX VB Code Context
					ASMX Code Context
					ASMX VB Code Context
					ASMX Context
					CSSDocOutline
					CSSSource
					Project Node
					A&dd
					Cab Project Node
					A&dd
					File nodes
					Dep. file nodes
					Assembly nodes
					Dep. assembly nodes
					MSM nodes
					Dep. MSM nodes
					Output nodes
					Dependency node
					Multiple selections
					Dep. Multiple selections
					View
					Registry
					File System
					File System
					New
					Add
					Add Special Folder
					View
					Selection
					Container
					TraySelection
					Document Outline
					Component Tray
					Exe Project
					Debug
					OTBObjCtxtMenu
					Class Designer Context Menu
					Class Diagram Context Menu
					TocContext
					ResListContext
					Editor
					Script Outline
					DefaultContext
					ImageContext
					SelectionContext
					AnchorContext
					Autos Window
					Breakpoint
					Breakpoints Window
					Call Stack Window
					Data Tip Window
					Disassembly Window
					Locals Window
					Memory Window
					Modules Window
					Output Window
					Processes Window
					Registers Window
					Threads Window
					Watch Window
					Server Explorer
					PropertyBrowser
					Macro
					Module
					Project
					Root
					Control
					Report
					Row/Column
					Cell
					Field Chooser
					Row/Column
					Chart
					Database Project
					DB Project Connection
					DB Project Folder
					Database References Folder
					Folders
					DB Project File
					Query
					Script
					Database Reference Node
					Files
					Multi-select
					Database Connection
					Folder Multi-Selection
					All Diagrams
					All Tables
					All Views
					All Stored Procedures
					All Package Specifications
					All Package Bodies
					All Synonyms
					All Databases
					All Users
					All Roles
					Node Multi-Selection
					Diagram
					Table
					View
					Stored Procedure
					Function
					Synonym
					Package Spec
					Package Body
					Trigger
					Column
					SQL Editor
					All Functions
					Oracle Function
					Oracle Procedure
					Change &View
					Single objet
					Single static
					Homogeneous objects
					Mixed objects
					Multiple static nodes
					Mixed nodes
					Add &New
					Add &New
					Surface
					DataSourceContext
					DbTableContext
					DataTableContext
					RelationContext
					FunctionContext
					ColumnContext
					QueryContext
					DataAccessorContext
					Query Diagram Pane
					Query Diagram Table
					Query Diagram Table Column
					Query Diagram Join Line
					Query Diagram Multi-select
					Query Grid Pane
					Query SQL Pane
					Query Results Pane
					Database Designer
					Database Designer Table
					Database Designer Relationship
					Text Annotation
					Class Details Context Menu
					TopicMenu
					TopicMenu
					Favorites Window Context Menu
					Data Sources
					Managed Resources Editor Context Menu
					Settings Designer
					System
		 */

		private void RegisterWithVisual(AddIn addIn, string commandName, string[] toolbars, string itemName, string description)
		{
			if( IsCommandRegistered(commandName) )
				return;
			
			object[] contextGuids = new object[] { };
			Commands2 commands = (Commands2)m_application.Commands;
			try
			{
				int commandStatus = (int)vsCommandStatus.vsCommandStatusSupported +
									(int)vsCommandStatus.vsCommandStatusEnabled;

				int commandStyle = (int)vsCommandStyle.vsCommandStyleText;
				vsCommandControlType controlType = vsCommandControlType.vsCommandControlTypeButton;

				// TODO: [jt] I think the context guids here are the key to enable commands on just a menu and not through the command line interface.
				Command command = commands.AddNamedCommand2(addIn,
												commandName,
												itemName,
												description,
												true,
												59,
												ref contextGuids,
												commandStatus,
												commandStyle,
												controlType);

				foreach( string toolbarName in toolbars )
				{
					if( "" == toolbarName )
						continue;
					command.AddControl(((CommandBars)m_application.CommandBars)[toolbarName], 1);
				}
			}
			catch (System.ArgumentException)
			{
				if( null != m_outputPane )
					m_outputPane.OutputString("Tried to register the command \"" + commandName + "\" twice!\n");
					
				System.Diagnostics.Debug.WriteLine("Tried to register the command \"" + commandName + "\" twice!\n");
			}
		}
		
		private static OutputWindowPane AquireOutputPane( DTE2 app, string name )
		{
			OutputWindowPane result = FindOutputPane(app, name);
			if( null == result )
				return null;

			OutputWindow outputWindow = (OutputWindow)app.Windows.Item(Constants.vsWindowKindOutput).Object;
			OutputWindowPanes panes = outputWindow.OutputWindowPanes;
			return panes.Add(name);
		}
		
		public static OutputWindowPane FindOutputPane( DTE2 app, string name )
		{
			if ("" == name)
				return null;

			OutputWindow outputWindow = (OutputWindow)app.Windows.Item(Constants.vsWindowKindOutput).Object;
			OutputWindowPanes panes = outputWindow.OutputWindowPanes;

			foreach (OutputWindowPane pane in panes)
			{
				if (name != pane.Name)
					continue;

				return pane;
			}
			
			return null;
		}
		
	}
}