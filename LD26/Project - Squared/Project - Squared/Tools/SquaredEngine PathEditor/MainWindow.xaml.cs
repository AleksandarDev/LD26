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

// NOTE: Thanks to http://petosky.net/node/13

namespace SquaredEngine.PathEditor {
	public partial class MainWindow : Window {
		public MainWindow() {
			InitializeComponent();

			DataContext = new GameViewModel();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e) {

		}

		private void comboBoxTool_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			if (DataContext != null)
				(DataContext as GameViewModel).ChangeTool((Library.PathEditorTools)comboBoxTool.SelectedIndex);
		}
	}
}
