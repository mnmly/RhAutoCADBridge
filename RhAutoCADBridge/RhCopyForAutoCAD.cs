using System;
using System.IO;
using Rhino;
using Rhino.Commands;
using Rhino.Input.Custom;
using System.Linq;
using System.Collections.Generic;

#if ON_RUNTIME_APPLE
 using Eto.Drawing;
 using Eto.Forms;
#endif

#if ON_RUNTIME_WIN
using System.Windows.Forms;
using System.Text;
#endif

namespace MNML { 

	namespace RhAutoCADBridge
	{
	    [CommandStyle(Style.ScriptRunner)]
	    public class RhCopyForAutoCADCommand : Rhino.Commands.Command
	    {

			public override string EnglishName => "CopyForAutoCAD";

			protected override Result RunCommand(Rhino.RhinoDoc doc, RunMode mode)
			{
			    if (doc.Objects.GetSelectedObjects(false, false).Count() == 0) {
					RhinoApp.WriteLine("Please select objects before running this commands");
					return Result.Failure;
				}

			    var gp = new GetPoint();
			    gp.SetCommandPrompt("Pick Insertion Point");
			    gp.Get();
			    if (gp.CommandResult() != Result.Success) { return gp.CommandResult(); }

			    var point = gp.Point();
			    var pointStr = point.X + "," + point.Y + "," + point.Z;
			    var outPath = Path.Combine(Path.GetTempPath(), Path.GetTempFileName()+ ".dwg");

			    // Export with origin
			    var command = "_-ExportWithOrigin " + pointStr + " \"" + outPath + "\" _Enter" ;
			    RhinoApp.RunScript(command, false);

#if ON_RUNTIME_WIN
                // Save the path to clipboard
                UnicodeEncoding Unicode = new UnicodeEncoding();
                var bytes = Unicode.GetBytes(outPath);
                List<byte> arrays = new List<byte>(bytes);
                // Set actual path allocated length = 520
                for (var i = arrays.Count; i < 520; i++) { arrays.Add(0); ; }
                // Set dwg path as dummy allocated length = 520
                for (var i = 0; i < 520; i++)
                {
                    if (i < bytes.Length) { arrays.Add(bytes[i]); }
                    else { arrays.Add(0); }
                }
                // Version Number length = 16
                var bytes3 = Unicode.GetBytes("R15");
                for (var i = 0; i < 16; i++)
                {
                    if (i < bytes3.Length) { arrays.Add(bytes3[i]); }
                    else { arrays.Add(0); }
                }

                // magic numbers different per copy length = 40
                // const byte[] data = { 0xC9, 0x5A, 0xA3, 0x97, 0x41, 0xB6, 0xF1, 0x40, 0xC4, 0xC4, 0x30, 0xBF, 0x1C, 0x9E, 0xDE, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                for (var i = 0; i < 40; i++) { arrays.Add(0); } 

                // another magic numbers same per copy; length = 8
                var bytes4 = new byte[] { 0x10, 0x21, 0xB3, 0x00, 0x00, 0x00, 0x00 };

                for (var i = 0; i < 8; i++)
                {
                    if (i < bytes4.Length)
                    {
                        arrays.Add(bytes4[i]);
                    }
                    else
                    {
                        arrays.Add(0);
                    }
                }

                // yet another magic numbers same per copy; length = 16
                var bytes5 = new byte[] { 0x50, 0x65, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                for (var i = 0; i < 16; i++)
                {
                    if (i < bytes5.Length) { arrays.Add(bytes5[i]); }
                    else { arrays.Add(0); }
                }

                DataObject data = new DataObject();
                byte[] byteArr = arrays.ToArray();
                using (MemoryStream memStream = new MemoryStream())
                {
                    memStream.Write(byteArr, 0, byteArr.Length);
                    data.SetText(outPath);
                    data.SetData("AutoCAD.r22", false, memStream);
                    Clipboard.SetDataObject(data, true);
                }
#endif
#if ON_RUNTIME_APPLE
				var bytes = Encoding.UTF32.GetBytes(outPath);
			    List<byte> arrays = new List<byte>(bytes);
				
			    for (var i = 0; i < 4; i++) { 
					arrays.Add(0); arrays.Add(0); arrays.Add(0); arrays.Add(0);
				}
			    // Set data to clipboard
			    Clipboard.Instance.Clear();
			    Clipboard.Instance.SetData(arrays.ToArray(), "AutoCAD.r22");
#endif
                return Result.Success;
			}
	    }
	}
}

