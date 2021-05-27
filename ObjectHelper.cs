using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace SPT.Helpers
{
	public static class ObjectHelper
	{
		public static DependencyObject GetObjectParent(object child)
		{
			try
			{
				DependencyObject parent = VisualTreeHelper.GetParent(child as DependencyObject);
				return parent;
			}
			catch(Exception ex)
			{
				Debug.WriteLine($"Can't find object parent: {ex.Message}");
				return null;
			}
		}

		public static string GetObjectName(DependencyObject @object)
		{
			string name = String.Empty;

			PropertyInfo propertyInfo = @object.GetType().GetProperty("Name");
			name = propertyInfo.GetValue(@object, null).ToString();
			
			return name;
		}
	}
}
