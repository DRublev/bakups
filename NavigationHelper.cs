using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;

namespace SPT
{
	public class NavigationHelper
	{
		private static NavigationHelper instance;
		public static NavigationHelper Instance
		{
			get
			{
				if(instance == null)
				{
					instance = new NavigationHelper();
				}

				return instance;
			}
		}
		private NavigationHelper() { }

		public List<Window> Journal = new List<Window>();

		public void NavigateBack(Window current)
		{
			Window toNavigate = GetPreviousWindow();

			NavigateTo(toNavigate, current);

			RemoveWindowFromJornal(current);
		}
		public void NavigateTo(Window toNavigate, Window current)
		{
			toNavigate.Show();
			current.Hide();
		}

		public void AddWindowToJournal(Window window)
		{
			Journal.Add(window);

			Debug.Print($"Window added to the journal {window.Title}");
		}
		public void RemoveWindowFromJornal(Window window)
		{
			Journal.Remove(window);
			Debug.Print($"Window removed from the journal {window.Title}");
		}

		private Window GetPreviousWindow()
		{
			int previousWindowIndex = (Journal.Count >= 1) ? Journal.Count - 2 : 0;
			Window previousWindow = Journal.ElementAt(previousWindowIndex);

			return previousWindow;
		}

		public void OnWindowClosing(Window window)
		{
			if (Journal.Contains(window) && Journal.Count >= 1)
			{
				Application.Current.Shutdown();
			}
		}
	}
}
