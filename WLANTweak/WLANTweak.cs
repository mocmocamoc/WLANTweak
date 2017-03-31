using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace WLANTweak
{
    public class WLANTweakForm : Form
    {

        public enum WLAN_INTF_OPCODE
        {
            wlan_intf_opcode_autoconf_enabled = 1,
            wlan_intf_opcode_background_scan_enabled = 2,
            wlan_intf_opcode_media_streaming_mode = 3
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct WLAN_INTERFACE_INFO
        {
            public Guid InterfaceGuid;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string strInterfaceDescription;
            public int isState;
        }

        [DllImport("wlanapi.dll", SetLastError = true)]
        private static extern UInt32 WlanOpenHandle(
            UInt32 dwClientVersion,
            IntPtr pReserved,
            out UInt32 pdwNegotiatedVersion,
            out IntPtr phClientHandle);

        [DllImport("wlanapi.dll", SetLastError = true)]
        private static extern UInt32 WlanEnumInterfaces(
            IntPtr hClientHandle,
            IntPtr pReserved,
            out IntPtr ppInterfaceList);

        [DllImport("wlanapi.dll")]
        public static extern int WlanSetInterface(IntPtr clientHandle,
            [MarshalAs(UnmanagedType.LPStruct)] Guid interfaceGuid,
            WLAN_INTF_OPCODE opCode,
            uint dataSize,
            IntPtr pData,
            IntPtr pReserved);

        [DllImport("wlanapi.dll")]
        public static extern int WlanQueryInterface(IntPtr clientHandle,
            [MarshalAs(UnmanagedType.LPStruct)] Guid interfaceGuid,
            WLAN_INTF_OPCODE opCode,
            IntPtr pReserved,
            out int dataSize,
            out IntPtr ppData,
            out IntPtr wlanOpcodeValueType);

        protected override void OnLoad(EventArgs e)
        {
            ShowInTaskbar = false;
            base.OnLoad(e);
        }

        private System.Windows.Forms.Label labelInfo;
        private System.Windows.Forms.Label labelAutoconf;
        private System.Windows.Forms.Label labelStreaming;
        private System.Windows.Forms.Label labelBackground;

        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem MenuItemExit;
        private System.Windows.Forms.ComboBox comboBoxDevice;

        private System.Windows.Forms.CheckBox checkAutoconf;
        private System.Windows.Forms.CheckBox checkBackground;
        private System.Windows.Forms.CheckBox checkStreaming;
        private IntPtr handle;
        private List<Guid> allGuids;

        private System.ComponentModel.IContainer components = null;
        static System.Windows.Forms.Timer myTimer = new System.Windows.Forms.Timer();

        private void setupForm()
        {
            this.components = new System.ComponentModel.Container();
            this.labelInfo = new System.Windows.Forms.Label();
            this.labelAutoconf = new System.Windows.Forms.Label();
            this.labelStreaming = new System.Windows.Forms.Label();
            this.labelBackground = new System.Windows.Forms.Label();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.MenuItemExit = new System.Windows.Forms.ToolStripMenuItem();
            this.comboBoxDevice = new System.Windows.Forms.ComboBox();
            this.checkAutoconf = new System.Windows.Forms.CheckBox();
            this.checkBackground = new System.Windows.Forms.CheckBox();
            this.checkStreaming = new System.Windows.Forms.CheckBox();

            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();

            this.checkAutoconf.Location = new System.Drawing.Point(100, 90);
            this.checkAutoconf.Name = "autoconf";
            this.checkAutoconf.MouseClick += new System.Windows.Forms.MouseEventHandler(this.changeSetting);

            this.checkBackground.Location = new System.Drawing.Point(100, 120);
            this.checkBackground.Name = "background";
            this.checkBackground.MouseClick += new System.Windows.Forms.MouseEventHandler(this.changeSetting);

            this.checkStreaming.Location = new System.Drawing.Point(100, 150);
            this.checkStreaming.Name = "streaming";
            this.checkStreaming.MouseClick += new System.Windows.Forms.MouseEventHandler(this.changeSetting);

            this.comboBoxDevice.Location = new System.Drawing.Point(12, 12);
            this.comboBoxDevice.Name = "comboBoxDevice";
            this.comboBoxDevice.Size = new System.Drawing.Size(220, 20);
            this.comboBoxDevice.SelectedIndexChanged += new System.EventHandler(this.updateDevice);

            this.labelInfo.Location = new System.Drawing.Point(12, 51);
            this.labelInfo.Name = "labelInfo";
            this.labelInfo.Size = new System.Drawing.Size(260, 20);

            this.labelAutoconf.Location = new System.Drawing.Point(12, 95);
            this.labelAutoconf.Text = "Autoconf";

            this.labelBackground.Location = new System.Drawing.Point(12, 125);
            this.labelBackground.Text = "Background";

            this.labelStreaming.Location = new System.Drawing.Point(12, 155);
            this.labelStreaming.Text = "Streaming";

            this.notifyIcon.ContextMenuStrip = this.contextMenuStrip;
            this.notifyIcon.Icon = WLANTweak.Properties.Resources.notifyIcon;
            this.notifyIcon.Text = "WLANTweak";
            this.notifyIcon.Visible = true;
            this.notifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyDoubleClick);

            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuItemExit});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(93, 26);

            this.MenuItemExit.Name = "MenuItemExit";
            this.MenuItemExit.Size = new System.Drawing.Size(92, 22);
            this.MenuItemExit.Text = "Exit";
            this.MenuItemExit.Click += new System.EventHandler(this.exitClick);

            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(250, 200);
            this.Controls.Add(this.labelInfo);
            this.Controls.Add(this.comboBoxDevice);
            this.Controls.Add(this.checkAutoconf);
            this.Controls.Add(this.checkBackground);
            this.Controls.Add(this.checkStreaming);
            this.Controls.Add(this.labelAutoconf);
            this.Controls.Add(this.labelBackground);
            this.Controls.Add(this.labelStreaming);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form";
            this.Text = "WLAN Tweak";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.closeForm);
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        public WLANTweakForm()
        {
            setupForm();

            notifyIcon.Visible = true;

            uint serviceVersion = 0;
            WlanOpenHandle(2, IntPtr.Zero, out serviceVersion, out handle);

            IntPtr pList = IntPtr.Zero;
            WlanEnumInterfaces(handle, IntPtr.Zero, out pList);

            this.allGuids = new List<Guid>();
            Int32 dwNumberofItems = Marshal.ReadInt32(pList, 0);
            for (int i = 0; i < dwNumberofItems; i++)
            {
                IntPtr pItemList = new IntPtr(pList.ToInt32() + (i * 532) + 8);
                WLAN_INTERFACE_INFO wii = new WLAN_INTERFACE_INFO();
                wii = (WLAN_INTERFACE_INFO)Marshal.PtrToStructure(pItemList, typeof(WLAN_INTERFACE_INFO));
                allGuids.Add(wii.InterfaceGuid);
                this.comboBoxDevice.Items.Add((i + 1).ToString() + ": " + wii.strInterfaceDescription);
            }
            if (dwNumberofItems > 0)
            {
                this.comboBoxDevice.SelectedIndex = 0;
            }

            myTimer.Tick += new EventHandler(updateDevice);
            myTimer.Interval = 60000;
            myTimer.Start();
        }

        private void changeSetting(object sender, MouseEventArgs e)
        {
            IntPtr flag = Marshal.AllocHGlobal(sizeof(Int32));
            Marshal.WriteInt32(flag, ((CheckBox)sender).Checked ? 1 : 0);
            Guid wguid = allGuids[this.comboBoxDevice.SelectedIndex];
            if (((CheckBox)sender).Name == "autoconf")
            {
                WlanSetInterface(handle, wguid, WLAN_INTF_OPCODE.wlan_intf_opcode_autoconf_enabled, sizeof(Int32), flag, IntPtr.Zero);
            }
            else if (((CheckBox)sender).Name == "streaming")
            {
                WlanSetInterface(handle, wguid, WLAN_INTF_OPCODE.wlan_intf_opcode_media_streaming_mode, sizeof(Int32), flag, IntPtr.Zero);
            }
            else if (((CheckBox)sender).Name == "background")
            {
                WlanSetInterface(handle, wguid, WLAN_INTF_OPCODE.wlan_intf_opcode_background_scan_enabled, sizeof(Int32), flag, IntPtr.Zero);
            }
            updateDevice(sender, e);
        }

        private void updateDevice(object sender, EventArgs e)
        {
            Guid wguid = allGuids[this.comboBoxDevice.SelectedIndex];
            int flagSize;
            IntPtr ppData = IntPtr.Zero;
            IntPtr valType = IntPtr.Zero;
            this.labelInfo.Text = wguid.ToString();
            WlanQueryInterface(handle, wguid, WLAN_INTF_OPCODE.wlan_intf_opcode_autoconf_enabled, IntPtr.Zero, out flagSize, out ppData, out valType);
            this.checkAutoconf.Checked = (bool)Marshal.PtrToStructure(ppData, typeof(bool));
            WlanQueryInterface(handle, wguid, WLAN_INTF_OPCODE.wlan_intf_opcode_media_streaming_mode, IntPtr.Zero, out flagSize, out ppData, out valType);
            this.checkStreaming.Checked = (bool)Marshal.PtrToStructure(ppData, typeof(bool));
            WlanQueryInterface(handle, wguid, WLAN_INTF_OPCODE.wlan_intf_opcode_background_scan_enabled, IntPtr.Zero, out flagSize, out ppData, out valType);
            this.checkBackground.Checked = (bool)Marshal.PtrToStructure(ppData, typeof(bool));
        }

        private void closeForm(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
            }
        }

        private void notifyDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.BringToFront();
        }

        private void exitClick(object sender, EventArgs e)
        {
            Application.Exit();
        }

    }

    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.Run(new WLANTweakForm());
        }
    }
}
