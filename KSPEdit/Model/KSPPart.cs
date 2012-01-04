using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Collada141;
using KSPEdit.Model.Geometry;
using System.IO;
using System.Text.RegularExpressions;
using SlimDX.Direct3D9;

namespace KSPEdit.Model
{
    public enum KSPPartModule
    {
        AdvSASModule,
        FuelLine,
        FuelTank,
        LiquidEngine,
        CommandPod,
        Parachutes,
        RadialDecoupler,
        RCSModule,
        RCSFuelTank,
        SASModule,
        SolidRocket,
        Decoupler,
        Strut,
        StrutConnector,
        Winglet,
        ControlSurface
    }

    public class KSPPart
    {
        
    }

    public class Mesh
    {
    }

    public struct KSPAttribNodeDefinition
    {
        public String Name;
        public Point3 Position;
        public Point3 UpVector;
    }

    public class KSPPartAttrib
    {
        // --- general parameters ---
        public String name;
        public KSPPartModule module;
        public String author;

        // --- asset parameters ---
        public COLLADA mesh;
        public double scale;
        public String texture;
        
        public double specPower;
        public double rimFalloff;
        public double alphaCutoff;
        public Point3 iconCenter;

        // --- node definitions ---
        List<KSPAttribNodeDefinition> nodeDefinitions;

        // --- editor parameters ---
        public int cost;
        public int category;
        public int subcategory;
        public String title;
        public String manufacturer;
        public String description;

        // attachment rules: stack, srfAttach, allowStack, allowSrfAttach, allowCollision
        public bool attachmentStack;
        public bool attachmentSurf;
        public bool attachmentAllowStack;
        public bool attachmentAllowSurf;
        public bool attachmentAllowCollision;
        
        // --- standard part parameters ---
        public double mass;
        public String dragModelType;
        public double maximum_drag;
        public double minimum_drag;
        public double angularDrag;
        public double crashTolerance;
        public double maxTemp;

        public KSPSpecialAttrib partSpecificAttribs;

        public KSPPartAttrib(TextReader stream)
        {
            int lineIdx = 0;
            String line;

            Regex r = new Regex(@"((\s)*(?<Key>([^\=^\s^\n]+))[\s^\n] \= (\s)*(?<Value>([^\n^\s]+(\n){0,1})))",RegexOptions.Compiled | RegexOptions.ExplicitCapture);

            List<String[]> extraLines = new List<string[]>();

            while ((line = stream.ReadLine()) != "")
            {
                lineIdx++;

                if (line.Substring(0, 1) == "//")
                    continue;

                var match = r.Match(line);

                var key = match.Groups["Key"].Value;
                var val = match.Groups["Value"].Value;

                switch (key.Trim())
                {
                    case "name":
                        name = val;
                        break;
                    case "module":
                        if (!Enum.TryParse<KSPPartModule>(val, out module))
                        {
                            throw new Exception(String.Format("Module string '{0}' could not be parsed into a module known to this program. (Line {1})", val, lineIdx));
                        }
                        break;
                    case "author":
                        author = val;
                        break;
                    case "mesh":
                        mesh = COLLADA.Load(val);
                        break;
                    case "scale":
                        try
                        {
                            scale = Double.Parse(val.Trim());
                        }
                        catch (Exception e)
                        {
                            throw new Exception(String.Format("scale string '{0}' could not be parsed into a double value. (Line {1})", val, lineIdx),e);
                        }
                        break;
                    case "texture":
                        texture = val;
                        break;
                    case "specPower":
                        try
                        {
                            specPower = Double.Parse(val.Trim());
                        }
                        catch (Exception e)
                        {
                            throw new Exception(String.Format("specPower string '{0}' could not be parsed into a double value. (Line {1})", val, lineIdx), e);
                        }
                        break;
                    case "rimFalloff":
                        try
                        {
                            rimFalloff = Double.Parse(val.Trim());


                        }
                        catch (Exception e)
                        {
                            throw new Exception(String.Format("rimFalloff string '{0}' could not be parsed into a double value. (Line {1})", val, lineIdx), e);
                        }
                        break;
                    case "alphaCutoff":
                        try
                        {
                            alphaCutoff = Double.Parse(val.Trim());
                        }
                        catch (Exception e)
                        {
                            throw new Exception(String.Format("alphaCutoff string '{0}' could not be parsed into a double value. (Line {1})", val, lineIdx), e);
                        }
                        break;
                    case "iconCenter":
                        var comps = val.Split(',');
                        try
                        {
                            double x = Double.Parse(comps[0].Trim());
                            double y = Double.Parse(comps[1].Trim());
                            double z = Double.Parse(comps[2].Trim());

                            iconCenter = new Point3() { x = x, y = y, z = z };
                        }
                        catch (Exception e)
                        {
                            throw new Exception(String.Format("iconCenter string '{0}' could not be parsed into doubles. (Line {1})", val, lineIdx), e);
                        }
                        break;
                    case "cost":
                        try
                        {
                            cost = Int32.Parse(val.Trim());
                        }
                        catch (Exception e)
                        {
                            throw new Exception(String.Format("cost string '{0}' could not be parsed into Int32. (Line {1})", val, lineIdx), e);
                        }
                        break;
                    case "category":
                        try
                        {
                            category = Int32.Parse(val.Trim());
                        }
                        catch (Exception e)
                        {
                            throw new Exception(String.Format("category string '{0}' could not be parsed into Int32. (Line {1})", val, lineIdx), e);
                        }
                        break;
                    case "subcategory":
                        try
                        {
                            subcategory = Int32.Parse(val.Trim());
                        }
                        catch (Exception e)
                        {
                            throw new Exception(String.Format("subcategory string '{0}' could not be parsed into Int32. (Line {1})", val, lineIdx), e);
                        }
                        break;
                    case "title":
                        title = val;
                        break;
                    case "manufacturer":
                        manufacturer = val;
                        break;
                    case "description":
                        description = val;
                        break;
                    case "attachRules":
                        try
                        {
                            comps = val.Split(',');

                            attachmentStack = (comps[0].Trim() == "1");
                            attachmentSurf = (comps[1].Trim() == "1");
                            attachmentAllowStack = (comps[2].Trim() == "1");
                            attachmentAllowSurf = (comps[3].Trim() == "1");
                            attachmentAllowCollision = (comps[4].Trim() == "1");
                        }
                        catch (Exception e)
                        {
                            throw new Exception(String.Format("attachRules string '{0}' could not be parsed into valid attachment rules. (Line {1})", val, lineIdx), e);
                        }
                        break;
                    case "mass":
                        try
                        {
                            mass = Double.Parse(val.Trim());
                        }
                        catch (Exception e)
                        {
                            throw new Exception(String.Format("mass string '{0}' could not be parsed into a double value. (Line {1})", val, lineIdx), e);
                        }
                        break;
                    case "dragModelType":
                        dragModelType = val;
                        break;
                    case "maximum_drag":
                        try
                        {
                            maximum_drag = Double.Parse(val.Trim());
                        }
                        catch (Exception e)
                        {
                            throw new Exception(String.Format("maximum_drag string '{0}' could not be parsed into a double value. (Line {1})", val, lineIdx), e);
                        }
                        break;
                    case "minimum_drag":
                        try
                        {
                            minimum_drag = Double.Parse(val.Trim());
                        }
                        catch (Exception e)
                        {
                            throw new Exception(String.Format("minimum_drag string '{0}' could not be parsed into a double value. (Line {1})", val, lineIdx), e);
                        }
                        break;
                    case "angularDrag ":
                        try
                        {
                            angularDrag = Double.Parse(val.Trim());
                        }
                        catch (Exception e)
                        {
                            throw new Exception(String.Format("angularDrag string '{0}' could not be parsed into a double value. (Line {1})", val, lineIdx), e);
                        }
                        break;
                    case "crashTolerance":
                        try
                        {
                            crashTolerance = Double.Parse(val.Trim());
                        }
                        catch (Exception e)
                        {
                            throw new Exception(String.Format("crashTolerance string '{0}' could not be parsed into a double value. (Line {1})", val, lineIdx), e);
                        }
                        break;
                    case "maxTemp":
                        try
                        {
                            maxTemp = Double.Parse(val.Trim());
                        }
                        catch (Exception e)
                        {
                            throw new Exception(String.Format("maxTemp string '{0}' could not be parsed into a double value. (Line {1})", val, lineIdx), e);
                        }
                        break;
                    default:
                        if (val.Substring(0,4) == "node")
                        {
                            try{
                            comps = val.Split(',');

                            double x = Double.Parse(comps[0].Trim());
                            double y = Double.Parse(comps[1].Trim());
                            double z = Double.Parse(comps[2].Trim());

                            double vx = Double.Parse(comps[3].Trim());
                            double vy = Double.Parse(comps[4].Trim());
                            double vz = Double.Parse(comps[5].Trim());

                            nodeDefinitions.Add(new KSPAttribNodeDefinition() { Name = key, Position = new Point3() { x = x, y = y, z = z }, UpVector = new Point3() { x = vx, y = vy, z = vz } });
                            }
                            catch (Exception e)
                        {
                            throw new Exception(String.Format("node string '{0}' ({2}) could not be parsed into doubles. (Line {1})", val, lineIdx, key), e);
                        }
                        }
                        else if (val.Trim() != "")
                        {
                            extraLines.Add(new string[] { key, val });
                        }
                        break;
                }

            }
            switch (module)
            {
                case KSPPartModule.AdvSASModule:
                    break;
                case KSPPartModule.FuelLine:
                    break;
                case KSPPartModule.FuelTank:
                    break;
                case KSPPartModule.LiquidEngine:
                    break;
                case KSPPartModule.CommandPod:
                    break;
                case KSPPartModule.Parachutes:
                    break;
                case KSPPartModule.RadialDecoupler:
                    break;
                case KSPPartModule.RCSModule:
                    break;
                case KSPPartModule.RCSFuelTank:
                    break;
                case KSPPartModule.SASModule:
                    break;
                case KSPPartModule.SolidRocket:
                    break;
                case KSPPartModule.Decoupler:
                    break;
                case KSPPartModule.Strut:
                    break;
                case KSPPartModule.StrutConnector:
                    break;
                case KSPPartModule.Winglet:
                    break;
                case KSPPartModule.ControlSurface:
                    break;
                default:
                    throw new Exception(String.Format("No specific part parser for module '{0}'. Well, that's awkward. (We're in KSPEdit.Model.KSPPartAttrib btw, if you feel like reporting this)", module.ToString()));
                    break;
            }
        }
    }

    public abstract class KSPSpecialAttrib {

    }

    public class KSPAdvancedSAS : KSPSpecialAttrib
    {
        public double Ki;
        public double Kp;
        public double Kd;

        public static KSPSpecialAttrib Load(List<string[]> config)
        {
            KSPAdvancedSAS part = new KSPAdvancedSAS();

            foreach (var line in config)
            {
                switch (line[0].Trim())
                {
                    case "Ki":
                        try
                        {
                            part.Ki = Double.Parse(line[1].Trim());
                        }
                        catch (Exception e)
                        {
                            throw new Exception(String.Format("Ki string '{0}' could not be parsed into a double value.", line[1].Trim()), e);
                        }
                        break;
                    case "Kp":
                        try
                        {
                            part.Kp = Double.Parse(line[1].Trim());
                        }
                        catch (Exception e)
                        {
                            throw new Exception(String.Format("Kp string '{0}' could not be parsed into a double value.", line[1].Trim()), e);
                        }
                        break;
                    case "Kd":
                        try
                        {
                            part.Kd = Double.Parse(line[1].Trim());
                        }
                        catch (Exception e)
                        {
                            throw new Exception(String.Format("Kd string '{0}' could not be parsed into a double value.", line[1].Trim()), e);
                        }
                        break;
                }
            }

            return part;
        }
    }

    public struct KSPNodeAttachment
    {
        String nodeName;
        KSPShipPart Target;
    }

    public struct KSPCData
    {

    }

    public class KSPShipPart
    {
        public KSPPart Part;
        public int Id;

        public Point3 pos;
        public Point3 rot;

        //stage index
        public int stage;

        //???
        public int istg;
        public int dstg;

        //index in stage, is ignored
        public int sidx;

        public int sqor;

        //fuel link
        public KSPShipPart link;

        //symmetries
        public List<KSPShipPart> sym;

        //is attachment
        public int attm;
        public List<KSPNodeAttachment> attN;

        public void Render(Device device)
        {

        }

    }

    public class KSPShip
    {
        public List<KSPShipPart> parts;

        public void Render(Device device)
        {
            parts.ForEach(p => p.Render(device));
        }
    }
}
