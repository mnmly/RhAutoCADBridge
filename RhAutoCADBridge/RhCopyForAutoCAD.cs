using System;
using System.IO;
using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using Rhino.UI;
using Eto.Drawing;
using Eto.Forms;
using System.Text;
using System.Linq;
using System.Collections.Generic;

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

			    var bytes = Encoding.UTF32.GetBytes(outPath);
			    List<byte> arrays = new List<byte>(bytes);
				
			    for (var i = 0; i < 4; i++) { 
					arrays.Add(0); arrays.Add(0); arrays.Add(0); arrays.Add(0);
				}
			    // Set data to clipboard
			    Clipboard.Instance.Clear();
			    Clipboard.Instance.SetData(arrays.ToArray(), RhAutoCADBridgePlugin.AutoCADFileTypeIdentifier);
			    return Result.Success;
			}
	    }
	}
}

