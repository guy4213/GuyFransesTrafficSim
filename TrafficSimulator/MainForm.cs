using System;
using System.Windows.Forms;

namespace TrafficSimulator
{
    public partial class MainForm : Form
    {
        private TrafficObjectCollection _collection;
        private Timer _globalTimer;
        private bool _isNightMode;

        public MainForm()
        {
            InitializeComponent();
        }

        private void OnTick(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnAddEntityClick(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnDeleteEntityClick(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnSaveClick(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnLoadClick(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void RefreshAnalyticsPanel()
        {
            throw new NotImplementedException();
        }
    }
}
