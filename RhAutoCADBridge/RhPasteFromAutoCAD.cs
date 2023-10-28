using System.Text;
using Rhino;
using Rhino.Commands;
using Eto.Forms;
using System.IO;
using System.Linq;

namespace MNML {

	namespace RhAutoCADBridge
	{
	    [System.Runtime.InteropServices.Guid("4e2f278f-6a63-4016-8db2-6d2d332501b4")]
        [CommandStyle(Style.ScriptRunner)]
	    public class RhPasteFromAutoCADCommand : Rhino.Commands.Command
	    {
			public RhPasteFromAutoCADCommand()
			{
			    // Rhino only creates one instance of each command class defined in a
			    // plug-in, so it is safe to store a refence in a static property.
			    Instance = this;
			}

			///<summary>The only instance of this command.</summary>
			public static RhPasteFromAutoCADCommand Instance { get; private set; }

			public override string EnglishName => "PasteFromAutoCAD";

			protected override Result RunCommand(Rhino.RhinoDoc doc, RunMode mode)
			{
				string result = Path.GetTempPath();
				var directory = new DirectoryInfo(result);

				// or...
				var myFile = directory.GetFiles()
				  .OrderByDescending(f => f.LastWriteTime)
				  .Where(f => f.Name.ToLower().EndsWith("dwg"))
				  .First();

				string command = "-_Import " + Path.Combine(result, myFile.ToString()) + " _Enter";
				RhinoApp.RunScript(command, false);
				return Result.Success;
			}
	    }
	}
}

