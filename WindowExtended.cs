using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace SPT
{
	public class WindowExtended : Window
	{
		public WindowExtended()
		{
			//Add window name to scope so that runtime properties can be referenced from XAML
			//(Name setting must be done here and not in xaml because this is a base class)
			var ns = new NameScope();
			NameScope.SetNameScope(this, ns);
			ns["window"] = this;

			//Call Initialize Component via Reflection, so you do not need 
			//to call InitializeComponent() every time in your base class
			GetType()
				.GetMethod("InitializeComponent",
					System.Reflection.BindingFlags.Public |
					System.Reflection.BindingFlags.NonPublic |
					System.Reflection.BindingFlags.Instance)
				.Invoke(this, null);

			//Set runtime DataContext - Designer mode will not run this code
			DataContext = this;

			Loaded += new RoutedEventHandler(OnOpening);
            Closed += new EventHandler(OnClosing);
			KeyDown += new KeyEventHandler(OnButtonKeyDown);
		}

		public void Back()
		{
			NavigationHelper.Instance.NavigateBack(this);
		}
		public void NextTo(Window toNavigate)
		{
			NavigationHelper.Instance.NavigateTo(toNavigate, this);
		}

		private void OnOpening(object sender, RoutedEventArgs e)
		{
			NavigationHelper.Instance.AddWindowToJournal(this);
		}

        private void OnClosing(object sender, EventArgs e)
        {
            NavigationHelper.Instance.OnWindowClosing(this);
        }

		private void OnButtonKeyDown(object sender, KeyEventArgs e)
		{
			if(e.Key.Equals(Key.F1))
			{
				OpenReference();
			}
		}

		private void OpenReference()
		{
			(new Views.Reference()).ShowDialog();
		}

		private void AddSideMebu()
		{

		}

        #region AcrylicWIndow
        [StructLayout(LayoutKind.Sequential)]
		internal struct WindowCompositionAttributeData
		{
			public WindowCompositionAttribute Attribute;
			public IntPtr Data;
			public int SizeOfData;
		}

		internal enum WindowCompositionAttribute
		{
			// ...
			WCA_ACCENT_POLICY = 19
			// ...
		}

		internal enum AccentState
		{
			ACCENT_DISABLED = 0,
			ACCENT_ENABLE_GRADIENT = 1,
			ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
			ACCENT_ENABLE_BLURBEHIND = 3,
			ACCENT_INVALID_STATE = 4
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct AccentPolicy
		{
			public AccentState AccentState;
			public int AccentFlags;
			public uint GradientColor;
			public int AnimationId;
		}

		/// <summary>
		///     xmlns:MyNamespace="clr-namespace:SourceChord.FluentWPF"
		///     xmlns:MyNamespace="clr-namespace:SourceChord.FluentWPF;assembly=SourceChord.FluentWPF"
		///     <MyNamespace:AcrylicWindow/>
		/// </summary>
		public class AcrylicWindow : Window
		{
			[DllImport("user32.dll")]
			internal static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

			static AcrylicWindow()
			{
				DefaultStyleKeyProperty.OverrideMetadata(typeof(AcrylicWindow), new FrameworkPropertyMetadata(typeof(AcrylicWindow)));

				TintColorProperty = AcrylicElement.TintColorProperty.AddOwner(typeof(AcrylicWindow), new FrameworkPropertyMetadata(Colors.White, FrameworkPropertyMetadataOptions.Inherits));
				TintOpacityProperty = AcrylicElement.TintOpacityProperty.AddOwner(typeof(AcrylicWindow), new FrameworkPropertyMetadata(0.6, FrameworkPropertyMetadataOptions.Inherits));
				NoiseOpacityProperty = AcrylicElement.NoiseOpacityProperty.AddOwner(typeof(AcrylicWindow), new FrameworkPropertyMetadata(0.03, FrameworkPropertyMetadataOptions.Inherits));
				FallbackColorProperty = AcrylicElement.FallbackColorProperty.AddOwner(typeof(AcrylicWindow), new FrameworkPropertyMetadata(Colors.LightGray, FrameworkPropertyMetadataOptions.Inherits));
				ShowTitleBarProperty = AcrylicElement.ShowTitleBarProperty.AddOwner(typeof(AcrylicWindow), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.Inherits));
				ExtendViewIntoTitleBarProperty = AcrylicElement.ExtendViewIntoTitleBarProperty.AddOwner(typeof(AcrylicWindow), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));
			}

			public override void OnApplyTemplate()
			{
				base.OnApplyTemplate();
				EnableBlur(this);
			}

			internal static void EnableBlur(Window win)
			{
				var windowHelper = new WindowInteropHelper(win);

				var accent = new AccentPolicy();
				var accentStructSize = Marshal.SizeOf(accent);
				// ウィンドウ背景のぼかしを行うのはWindows10の場合のみ
				accent.AccentState = IsWin10() ? AccentState.ACCENT_ENABLE_BLURBEHIND : AccentState.ACCENT_ENABLE_TRANSPARENTGRADIENT;
				accent.AccentFlags = 2;
				//accent.GradientColor = 0x99FFFFFF;  // 60%の透明度が基本
				accent.GradientColor = 0x00FFFFFF;  // Tint Colorはここでは設定せず、Bindingで外部から変えられるようにXAML側のレイヤーとして定義

				var accentPtr = Marshal.AllocHGlobal(accentStructSize);
				Marshal.StructureToPtr(accent, accentPtr, false);

				var data = new WindowCompositionAttributeData();
				data.Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY;
				data.SizeOfData = accentStructSize;
				data.Data = accentPtr;

				SetWindowCompositionAttribute(windowHelper.Handle, ref data);

				Marshal.FreeHGlobal(accentPtr);

				win.CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, (_, __) => { SystemCommands.CloseWindow(win); }));
				win.CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand, (_, __) => { SystemCommands.MinimizeWindow(win); }));
				win.CommandBindings.Add(new CommandBinding(SystemCommands.MaximizeWindowCommand, (_, __) => { SystemCommands.MaximizeWindow(win); }));
				win.CommandBindings.Add(new CommandBinding(SystemCommands.RestoreWindowCommand, (_, __) => { SystemCommands.RestoreWindow(win); }));
			}

			/// <summary>
			/// 実行環境のOSがWindows10か否かを判定
			/// </summary>
			/// <returns></returns>
			internal static bool IsWin10()
			{
				var isWin10 = false;
				using (var mc = new System.Management.ManagementClass("Win32_OperatingSystem"))
				using (var moc = mc.GetInstances())
				{
					foreach (System.Management.ManagementObject mo in moc)
					{
						var version = mo["Version"] as string;
						var majar = version.Split('.')
										   .FirstOrDefault();
						isWin10 = majar == "10";
					}
				}

				return isWin10;
			}

			#region Dependency Property

			public System.Drawing.Color TintColor
			{
				get { return (System.Drawing.Color) GetValue(TintColorProperty); }
				set { SetValue(TintColorProperty, value); }
			}

			// Using a DependencyProperty as the backing store for TintColor.  This enables animation, styling, binding, etc...
			public static readonly DependencyProperty TintColorProperty;
			public static System.Drawing.Color GetTintColor(DependencyObject obj)
			{
				return (System.Drawing.Color) obj.GetValue(AcrylicElement.TintColorProperty);
			}

			public static void SetTintColor(DependencyObject obj, System.Drawing.Color value)
			{
				obj.SetValue(AcrylicElement.TintColorProperty, value);
			}


			public double TintOpacity
			{
				get { return (double) GetValue(TintOpacityProperty); }
				set { SetValue(TintOpacityProperty, value); }
			}

			// Using a DependencyProperty as the backing store for TintOpacity.  This enables animation, styling, binding, etc...
			public static readonly DependencyProperty TintOpacityProperty;
			public static double GetTintOpacity(DependencyObject obj)
			{
				return (double) obj.GetValue(AcrylicElement.TintOpacityProperty);
			}

			public static void SetTintOpacity(DependencyObject obj, double value)
			{
				obj.SetValue(AcrylicElement.TintOpacityProperty, value);
			}



			public double NoiseOpacity
			{
				get { return (double) GetValue(NoiseOpacityProperty); }
				set { SetValue(NoiseOpacityProperty, value); }
			}

			// Using a DependencyProperty as the backing store for NoiseOpacity.  This enables animation, styling, binding, etc...
			public static readonly DependencyProperty NoiseOpacityProperty;
			public static double GetNoiseOpacity(DependencyObject obj)
			{
				return (double) obj.GetValue(AcrylicElement.NoiseOpacityProperty);
			}

			public static void SetNoiseOpacity(DependencyObject obj, double value)
			{
				obj.SetValue(AcrylicElement.NoiseOpacityProperty, value);
			}


			public System.Drawing.Color FallbackColor
			{
				get { return (System.Drawing.Color) GetValue(FallbackColorProperty); }
				set { SetValue(FallbackColorProperty, value); }
			}

			// Using a DependencyProperty as the backing store for FallbackColor.  This enables animation, styling, binding, etc...
			public static readonly DependencyProperty FallbackColorProperty;
			public static System.Drawing.Color GetFallbackColor(DependencyObject obj)
			{
				return (System.Drawing.Color) obj.GetValue(AcrylicElement.FallbackColorProperty);
			}

			public static void SetFallbackColor(DependencyObject obj, System.Drawing.Color value)
			{
				obj.SetValue(AcrylicElement.FallbackColorProperty, value);
			}



			public bool ShowTitleBar
			{
				get { return (bool) GetValue(ShowTitleBarProperty); }
				set { SetValue(ShowTitleBarProperty, value); }
			}

			// Using a DependencyProperty as the backing store for ShowTitleBar.  This enables animation, styling, binding, etc...
			public static readonly DependencyProperty ShowTitleBarProperty;
			public static bool GetShowTitleBar(DependencyObject obj)
			{
				return (bool) obj.GetValue(AcrylicElement.ShowTitleBarProperty);
			}

			public static void SetShowTitleBar(DependencyObject obj, bool value)
			{
				obj.SetValue(AcrylicElement.ShowTitleBarProperty, value);
			}


			public bool ExtendViewIntoTitleBar
			{
				get { return (bool) GetValue(ExtendViewIntoTitleBarProperty); }
				set { SetValue(ExtendViewIntoTitleBarProperty, value); }
			}

			// Using a DependencyProperty as the backing store for ExtendViewIntoTitleBar.  This enables animation, styling, binding, etc...
			public static readonly DependencyProperty ExtendViewIntoTitleBarProperty;
			public static bool GetExtendViewIntoTitleBar(DependencyObject obj)
			{
				return (bool) obj.GetValue(AcrylicElement.ExtendViewIntoTitleBarProperty);
			}

			public static void SetExtendViewIntoTitleBar(DependencyObject obj, bool value)
			{
				obj.SetValue(AcrylicElement.ExtendViewIntoTitleBarProperty, value);
			}




			#endregion


			#region Attached Property


			public static bool GetEnabled(DependencyObject obj)
			{
				return (bool) obj.GetValue(EnabledProperty);
			}

			public static void SetEnabled(DependencyObject obj, bool value)
			{
				obj.SetValue(EnabledProperty, value);
			}

			// Using a DependencyProperty as the backing store for Enabled.  This enables animation, styling, binding, etc...
			public static readonly DependencyProperty EnabledProperty =
				DependencyProperty.RegisterAttached("Enabled", typeof(bool), typeof(AcrylicWindow), new PropertyMetadata(false, OnEnableChanged));

			private static void OnEnableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
			{
				var win = d as Window;
				if (win == null)
				{ return; }

				var value = (bool) e.NewValue;
				if (value)
				{
					var dic = new ResourceDictionary() { Source = new Uri("pack://application:,,,/FluentWPF;component/Styles/Window.xaml") };
					var style = dic["AcrylicWindowStyle"] as Style;
					win.Style = style;

					win.Loaded += (_, __) => { EnableBlur(win); };
					if (win.IsLoaded)
						EnableBlur(win);
				}
			}
			#endregion
		}

		internal class AcrylicElement
		{


			public static System.Drawing.Color GetTintColor(DependencyObject obj)
			{
				return (System.Drawing.Color) obj.GetValue(TintColorProperty);
			}

			public static void SetTintColor(DependencyObject obj, System.Drawing.Color value)
			{
				obj.SetValue(TintColorProperty, value);
			}

			// Using a DependencyProperty as the backing store for TintColor.  This enables animation, styling, binding, etc...
			public static readonly DependencyProperty TintColorProperty =
				DependencyProperty.RegisterAttached("TintColor", typeof(System.Drawing.Color), typeof(AcrylicElement), new FrameworkPropertyMetadata(Colors.White, FrameworkPropertyMetadataOptions.Inherits));




			public static double GetTintOpacity(DependencyObject obj)
			{
				return (double) obj.GetValue(TintOpacityProperty);
			}

			public static void SetTintOpacity(DependencyObject obj, double value)
			{
				obj.SetValue(TintOpacityProperty, value);
			}

			// Using a DependencyProperty as the backing store for TintOpacity.  This enables animation, styling, binding, etc...
			public static readonly DependencyProperty TintOpacityProperty =
				DependencyProperty.RegisterAttached("TintOpacity", typeof(double), typeof(AcrylicElement), new PropertyMetadata(0.6));




			public static double GetNoiseOpacity(DependencyObject obj)
			{
				return (double) obj.GetValue(NoiseOpacityProperty);
			}

			public static void SetNoiseOpacity(DependencyObject obj, double value)
			{
				obj.SetValue(NoiseOpacityProperty, value);
			}

			// Using a DependencyProperty as the backing store for NoiseOpacity.  This enables animation, styling, binding, etc...
			public static readonly DependencyProperty NoiseOpacityProperty =
				DependencyProperty.RegisterAttached("NoiseOpacity", typeof(double), typeof(AcrylicElement), new PropertyMetadata(0.03));




			public static System.Drawing.Color GetFallbackColor(DependencyObject obj)
			{
				return (System.Drawing.Color) obj.GetValue(FallbackColorProperty);
			}

			public static void SetFallbackColor(DependencyObject obj, System.Drawing.Color value)
			{
				obj.SetValue(FallbackColorProperty, value);
			}

			// Using a DependencyProperty as the backing store for FallbackColor.  This enables animation, styling, binding, etc...
			public static readonly DependencyProperty FallbackColorProperty =
				DependencyProperty.RegisterAttached("FallbackColor", typeof(System.Drawing.Color), typeof(AcrylicElement), new PropertyMetadata(Colors.LightGray));





			public static bool GetShowTitleBar(DependencyObject obj)
			{
				return (bool) obj.GetValue(ShowTitleBarProperty);
			}

			public static void SetShowTitleBar(DependencyObject obj, bool value)
			{
				obj.SetValue(ShowTitleBarProperty, value);
			}

			// Using a DependencyProperty as the backing store for ShowTitleBar.  This enables animation, styling, binding, etc...
			public static readonly DependencyProperty ShowTitleBarProperty =
				DependencyProperty.RegisterAttached("ShowTitleBar", typeof(bool), typeof(AcrylicElement), new PropertyMetadata(true));



			public static bool GetExtendViewIntoTitleBar(DependencyObject obj)
			{
				return (bool) obj.GetValue(ExtendViewIntoTitleBarProperty);
			}

			public static void SetExtendViewIntoTitleBar(DependencyObject obj, bool value)
			{
				obj.SetValue(ExtendViewIntoTitleBarProperty, value);
			}

			// Using a DependencyProperty as the backing store for ExtendViewIntoTitleBar.  This enables animation, styling, binding, etc...
			public static readonly DependencyProperty ExtendViewIntoTitleBarProperty =
				DependencyProperty.RegisterAttached("ExtendViewIntoTitleBar", typeof(bool), typeof(AcrylicElement), new PropertyMetadata(false));


		}
		#endregion
	}
}
