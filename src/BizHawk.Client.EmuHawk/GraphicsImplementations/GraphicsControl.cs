using System.Windows.Forms;
using BizHawk.Bizware.BizwareGL;

namespace BizHawk.Client.EmuHawk
{
	/// <summary>
	/// a base class for deriving/wrapping from a IGraphicsControl.
	/// This is to work around the annoyance that we cant inherit from a control whose type is unknown (it would be delivered by the selected BizwareGL driver)
	/// and so we have to resort to composition and c# sucks and events suck.
	/// </summary>
	public class GraphicsControl : UserControl
	{
		public GraphicsControl(IGL owner)
		{
			IGL = owner;

			SetStyle(ControlStyles.Opaque, true);
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.UserMouse, true);

			_igc = owner.Internal_CreateGraphicsControl();
			_managed = (Control)_igc;
			_managed.Dock = DockStyle.Fill;
			Controls.Add(_managed);

			// pass through these events to the form. I tried really hard to find a better way, but there is none.
			// (don't use HTTRANSPARENT, it isn't portable, I would assume)
			_managed.MouseDoubleClick += (_, e) => OnMouseDoubleClick(e);
			_managed.MouseClick += (_, e) => OnMouseClick(e);
			_managed.MouseEnter += (_, e) => OnMouseEnter(e);
			_managed.MouseLeave += (_, e) => OnMouseLeave(e);
			_managed.MouseMove += (_, e) => OnMouseMove(e);

			//the GraphicsControl is occupying all of our area. So we pretty much never get paint events ourselves.
			//So lets capture its paint event and use it for ourselves (it doesn't know how to do anything, anyway)
			_managed.Paint += GraphicsControl_Paint;
		}

		/// <summary>
		/// If this is the main window, things may be special
		/// </summary>
		public bool MainWindow;

		private void GraphicsControl_Paint(object sender, PaintEventArgs e)
		{
			OnPaint(e);
		}

		public readonly IGL IGL;

		private readonly IGraphicsControl _igc;
		private readonly Control _managed;

		public virtual void SetVsync(bool state) { _igc.SetVsync(state); }
		public virtual void SwapBuffers() { _igc.SwapBuffers(); }
		public virtual void Begin() { _igc.Begin(); }
		public virtual void End() { _igc.End(); }
	}
}