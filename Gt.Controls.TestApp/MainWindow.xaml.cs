using System;
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
using Gt.Controls.Diagramming;
using System.IO;
using Microsoft.Win32;
using Gt.Controls.Diagramming.NodeDrawers;

namespace Gt.Controls.TestApp
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			Viewer.Diagram = MyDiagram;
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			using (DiagramRenderLock diagramLock = new DiagramRenderLock(MyDiagram))
			{
				DiagramNode node = new DiagramNode(MyDiagram);
				node.Bounds = new Rect(50, 50, 50, 50);
				node.Background = new SolidColorBrush(Colors.Azure);
				node.RadiusX = 15;
				node.RadiusY = 15;
				node.Drawer = new RoundedRectangleItemDrawer();
				node.Label.Text = "AAZzzффффф";
				node.Label.Foreground = new SolidColorBrush(Colors.DarkRed);
				node.Label.FontStyle = FontStyles.Italic;

				DiagramNode node1 = new DiagramNode(MyDiagram);
				node1.Bounds = new Rect(75, 200, 50, 50);
				node1.Background = new SolidColorBrush(Colors.Violet);
				node1.Label.Text = "wow";
				LinearGradientBrush brush = new LinearGradientBrush();
				brush.GradientStops.Add(new GradientStop(Colors.White, 0));
				brush.GradientStops.Add(new GradientStop(Colors.Blue, 1));
				brush.Opacity = 0.5;
				node1.Label.Background = brush;
				node1.Label.BorderPen = new Pen(Brushes.BlanchedAlmond, 2);

				//DiagramEdge edge = new DiagramEdge(MyDiagram);
				//edge.SourcePoint = new Point(100, 100);
				//edge.DestinationPoint = new Point(150, 150);
				//edge.Label.Text = "lololol";
				//edge.Label.Background = brush;

				//DiagramEdge edge1 = new DiagramEdge(MyDiagram);
				//edge1.AnchoringMode = EdgeAnchoringMode.NodeToNode;
				//edge1.SourceNode = node;
				//edge1.DestinationNode = node1;
			}
		}

		private void MyDiagram_MouseDown(object sender, MouseButtonEventArgs e)
		{

		}

		private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
		{
			
		}

		private void Window_MouseDown(object sender, MouseButtonEventArgs e)
		{
			
		}

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
			MyDiagram.DefaultNodeDrawer = new DiamondNodeDrawer();
		}

		private void Button_Click_2(object sender, RoutedEventArgs e)
		{
			MyDiagram.DefaultNodeDrawer = new EllipseNodeDrawer();
		}

		private void Button_Click_3(object sender, RoutedEventArgs e)
		{
			PngBitmapEncoder encoder = new PngBitmapEncoder();
			MemoryStream stream = MyDiagram.RenderToImage(96, 96, new Rect(0, 0, MyDiagram.ActualWidth, MyDiagram.ActualHeight), encoder);

			SaveFileDialog dialog = new SaveFileDialog();
			dialog.Filter = "png files | *.png";
			dialog.AddExtension = true;
			if (dialog.ShowDialog() == true)
			{
				string fileName = dialog.FileName;
				FileStream fileStream = new FileStream(fileName, FileMode.Create);
				stream.WriteTo(fileStream);
				fileStream.Close();
			}

			stream.Close();
		}

		private void Button_Click_4(object sender, RoutedEventArgs e)
		{
			MyDiagram.XViewOffset = MyDiagram.XViewOffset + 10;
		}

		private void Button_Click_5(object sender, RoutedEventArgs e)
		{
			MyDiagram.YViewOffset += 10;
		}

		private void plus_Click(object sender, RoutedEventArgs e)
		{
			MyDiagram.Scale = MyDiagram.Scale * 2;
		}

		private void minus_Click(object sender, RoutedEventArgs e)
		{
			MyDiagram.Scale = MyDiagram.Scale / 2;
		}
	}
}
