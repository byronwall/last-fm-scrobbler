﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LastFM;
using System.Drawing;

namespace LastFM
{
    class Tray
    {
        private static NotifyIcon _trayIcon = null;
        
        public static NotifyIcon TrayIcon{
       
            get
            {
                if (_trayIcon == null)
                {
                    Initialize();
                }
                return Tray._trayIcon;
            }
        }

        private static void Initialize()
        {
            _trayIcon = new NotifyIcon();
            _trayIcon.Icon = new Icon(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("LastFM.trayIcon.ico"));

            _trayIcon.MouseDoubleClick += new MouseEventHandler(_trayIcon_MouseDoubleClick);

            ContextMenu menu = new ContextMenu();
            MenuItem _exitItem = new MenuItem("Exit");
            _exitItem.Click += new EventHandler(_exitItem_Click);
            menu.MenuItems.Add(_exitItem);


            _trayIcon.ContextMenu = menu;
        }
        static void _exitItem_Click(object sender, EventArgs e)
        {
            HideIcon();
            InterfaceHelper.CloseProgram();
        }

        static void _trayIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            InterfaceHelper.RestoreFromTrayIcon();
        }
        public static void ShowIcon()
        {
            TrayIcon.Visible = true;
        }
        public static void HideIcon()
        {
            TrayIcon.Visible = false;
        }
    }
}
