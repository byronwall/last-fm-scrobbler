﻿#pragma checksum "..\..\Window1.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "7C1155390363171DD0C7543A9AC7F263"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using LastFM;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace LastFM {
    
    
    /// <summary>
    /// Window1
    /// </summary>
    public partial class Window1 : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 7 "..\..\Window1.xaml"
        internal LastFM.Window1 Window;
        
        #line default
        #line hidden
        
        
        #line 72 "..\..\Window1.xaml"
        internal System.Windows.Controls.Grid LayoutRoot;
        
        #line default
        #line hidden
        
        
        #line 86 "..\..\Window1.xaml"
        internal System.Windows.Controls.TabControl tabProgramExtras;
        
        #line default
        #line hidden
        
        
        #line 90 "..\..\Window1.xaml"
        internal System.Windows.Controls.TextBlock textPlayingArtist;
        
        #line default
        #line hidden
        
        
        #line 91 "..\..\Window1.xaml"
        internal System.Windows.Controls.TextBlock textPlayingAlbum;
        
        #line default
        #line hidden
        
        
        #line 93 "..\..\Window1.xaml"
        internal System.Windows.Controls.TextBlock textPlayingSong;
        
        #line default
        #line hidden
        
        
        #line 97 "..\..\Window1.xaml"
        internal LastFM.SimilarArtists similarMain;
        
        #line default
        #line hidden
        
        
        #line 100 "..\..\Window1.xaml"
        internal LastFM.UserInfoControl userInfoInstance;
        
        #line default
        #line hidden
        
        
        #line 111 "..\..\Window1.xaml"
        internal System.Windows.Controls.TextBox textBox1;
        
        #line default
        #line hidden
        
        
        #line 112 "..\..\Window1.xaml"
        internal System.Windows.Controls.PasswordBox passwordBox1;
        
        #line default
        #line hidden
        
        
        #line 113 "..\..\Window1.xaml"
        internal System.Windows.Controls.Button button1;
        
        #line default
        #line hidden
        
        
        #line 123 "..\..\Window1.xaml"
        internal System.Windows.Controls.Grid gridStatus;
        
        #line default
        #line hidden
        
        
        #line 139 "..\..\Window1.xaml"
        internal System.Windows.Controls.TextBlock txtDBVal;
        
        #line default
        #line hidden
        
        
        #line 140 "..\..\Window1.xaml"
        internal System.Windows.Controls.TextBlock txtITunesVal;
        
        #line default
        #line hidden
        
        
        #line 141 "..\..\Window1.xaml"
        internal System.Windows.Controls.TextBlock txtCacheVal;
        
        #line default
        #line hidden
        
        
        #line 142 "..\..\Window1.xaml"
        internal System.Windows.Controls.TextBlock txtLastFMVal;
        
        #line default
        #line hidden
        
        
        #line 144 "..\..\Window1.xaml"
        internal System.Windows.Controls.TextBlock txtCacheFireVal;
        
        #line default
        #line hidden
        
        
        #line 152 "..\..\Window1.xaml"
        internal System.Windows.Controls.ListBox lstLogOutput;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/LastFM;component/window1.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\Window1.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal System.Delegate _CreateDelegate(System.Type delegateType, string handler) {
            return System.Delegate.CreateDelegate(delegateType, this, handler);
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.Window = ((LastFM.Window1)(target));
            
            #line 7 "..\..\Window1.xaml"
            this.Window.Loaded += new System.Windows.RoutedEventHandler(this.Window_Loaded);
            
            #line default
            #line hidden
            
            #line 7 "..\..\Window1.xaml"
            this.Window.Closing += new System.ComponentModel.CancelEventHandler(this.Window_Closing);
            
            #line default
            #line hidden
            
            #line 7 "..\..\Window1.xaml"
            this.Window.StateChanged += new System.EventHandler(this.Window_StateChanged);
            
            #line default
            #line hidden
            
            #line 7 "..\..\Window1.xaml"
            this.Window.Activated += new System.EventHandler(this.Window_Activated);
            
            #line default
            #line hidden
            
            #line 7 "..\..\Window1.xaml"
            this.Window.Deactivated += new System.EventHandler(this.Window_Deactivated);
            
            #line default
            #line hidden
            return;
            case 2:
            
            #line 69 "..\..\Window1.xaml"
            ((System.Windows.Controls.TextBlock)(target)).MouseUp += new System.Windows.Input.MouseButtonEventHandler(this.TextBlock_MouseUp);
            
            #line default
            #line hidden
            
            #line 69 "..\..\Window1.xaml"
            ((System.Windows.Controls.TextBlock)(target)).Drop += new System.Windows.DragEventHandler(this.TextBlock_Drop);
            
            #line default
            #line hidden
            return;
            case 3:
            this.LayoutRoot = ((System.Windows.Controls.Grid)(target));
            return;
            case 4:
            this.tabProgramExtras = ((System.Windows.Controls.TabControl)(target));
            return;
            case 5:
            this.textPlayingArtist = ((System.Windows.Controls.TextBlock)(target));
            
            #line 90 "..\..\Window1.xaml"
            this.textPlayingArtist.MouseUp += new System.Windows.Input.MouseButtonEventHandler(this.textPlayingArtist_MouseUp);
            
            #line default
            #line hidden
            return;
            case 6:
            this.textPlayingAlbum = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 7:
            this.textPlayingSong = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 8:
            this.similarMain = ((LastFM.SimilarArtists)(target));
            return;
            case 9:
            this.userInfoInstance = ((LastFM.UserInfoControl)(target));
            return;
            case 10:
            this.textBox1 = ((System.Windows.Controls.TextBox)(target));
            return;
            case 11:
            this.passwordBox1 = ((System.Windows.Controls.PasswordBox)(target));
            return;
            case 12:
            this.button1 = ((System.Windows.Controls.Button)(target));
            
            #line 113 "..\..\Window1.xaml"
            this.button1.Click += new System.Windows.RoutedEventHandler(this.button1_Click);
            
            #line default
            #line hidden
            return;
            case 13:
            this.gridStatus = ((System.Windows.Controls.Grid)(target));
            return;
            case 14:
            this.txtDBVal = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 15:
            this.txtITunesVal = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 16:
            this.txtCacheVal = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 17:
            this.txtLastFMVal = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 18:
            this.txtCacheFireVal = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 19:
            this.lstLogOutput = ((System.Windows.Controls.ListBox)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}