using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

using MindFusion.Diagramming.Wpf;
using MindFusion.Diagramming.Wpf.Behaviors;
using TalesGenerator.UI;

namespace TalesGenerator.UI.Classes
{
	class CustomBehavior : LinkShapesBehavior
	{
		//Point _mousePosition;
		//bool _rightButtonPressed;

		public CustomBehavior(Diagram view) : base(view)
		{
		}

		protected override void OnMouseDown(Point mousePosition, MouseButton mouseButton)
		{
			//if (mouseButton == MouseButton.Right)
			//{
			//	_rightButtonPressed = true;
			//	_mousePosition = mousePosition;
			//}
			base.OnMouseDown(mousePosition, mouseButton);
		}

		protected override void OnMouseUp(Point mousePosition, MouseButton mouseButton)
		{
			//if (mouseButton == MouseButton.Right)
			//	_rightButtonPressed = false;
			base.OnMouseUp(mousePosition, mouseButton);
		}

		protected override void OnMouseMove(Point mousePosition)
		{
			//if (_rightButtonPressed)
			//{
			//    Diagram.ScrollX += (mousePosition.X - _mousePosition.X);
			//    Diagram.ScrollY += (mousePosition.Y - _mousePosition.Y);
			//    _mousePosition = mousePosition;
			//}
			base.OnMouseMove(mousePosition);
		}
	}
}
