﻿#pragma checksum "..\..\SimilarArtists.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "7C6FC68BF816D181341AE48387F02A46"
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
using Microsoft.Windows.Themes;
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
    /// SimilarArtists
    /// </summary>
    public partial class SimilarArtists : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector, System.Windows.Markup.IStyleConnector {
        
        
        #line 6 "..\..\SimilarArtists.xaml"
        internal LastFM.SimilarArtists UserControl;
        
        #line default
        #line hidden
        
        
        #line 161 "..\..\SimilarArtists.xaml"
        internal LastFM.CustomHyperlink hyperlinkArtistName;
        
        #line default
        #line hidden
        
        
        #line 163 "..\..\SimilarArtists.xaml"
        internal System.Windows.Controls.TextBox txtArtistName;
        
        #line default
        #line hidden
        
        
        #line 166 "..\..\SimilarArtists.xaml"
        internal System.Windows.Controls.Image imageArtist;
        
        #line default
        #line hidden
        
        
        #line 167 "..\..\SimilarArtists.xaml"
        internal System.Windows.Controls.StackPanel stackArtistSongs;
        
        #line default
        #line hidden
        
        
        #line 169 "..\..\SimilarArtists.xaml"
        internal System.Windows.Controls.ListBox listArtistSongs;
        
        #line default
        #line hidden
        
        
        #line 171 "..\..\SimilarArtists.xaml"
        internal System.Windows.Controls.StackPanel stackSimilar;
        
        #line default
        #line hidden
        
        
        #line 173 "..\..\SimilarArtists.xaml"
        internal System.Windows.Controls.ListBox listSimilar;
        
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
            System.Uri resourceLocater = new System.Uri("/LastFM;component/similarartists.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\SimilarArtists.xaml"
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
            this.UserControl = ((LastFM.SimilarArtists)(target));
            return;
            case 4:
            this.hyperlinkArtistName = ((LastFM.CustomHyperlink)(target));
            return;
            case 5:
            this.txtArtistName = ((System.Windows.Controls.TextBox)(target));
            return;
            case 6:
            
            #line 164 "..\..\SimilarArtists.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Button_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            this.imageArtist = ((System.Windows.Controls.Image)(target));
            return;
            case 8:
            this.stackArtistSongs = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 9:
            this.listArtistSongs = ((System.Windows.Controls.ListBox)(target));
            return;
            case 10:
            this.stackSimilar = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 11:
            this.listSimilar = ((System.Windows.Controls.ListBox)(target));
            return;
            }
            this._contentLoaded = true;
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        void System.Windows.Markup.IStyleConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 2:
            
            #line 28 "..\..\SimilarArtists.xaml"
            ((System.Windows.Controls.Border)(target)).PreviewMouseUp += new System.Windows.Input.MouseButtonEventHandler(this.Border_PreviewMouseUp);
            
            #line default
            #line hidden
            break;
            case 3:
            
            #line 69 "..\..\SimilarArtists.xaml"
            ((System.Windows.Controls.ScrollViewer)(target)).PreviewMouseWheel += new System.Windows.Input.MouseWheelEventHandler(this.ScrollViewer_PreviewMouseWheel);
            
            #line default
            #line hidden
            break;
            }
        }
    }
}
