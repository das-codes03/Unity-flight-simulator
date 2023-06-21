using System;
using Unity.Mathematics;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[Serializable]
public class LiftCurve
{

    public double mp = 1;
    public double mn = -1;
    public double ap = 15;
    public double an = -15;
    public double steepP = 10;
    public double steepN = 10;
    const double PI = Math.PI;
    public double fMax = 1.2;
    public double offsetAOA;
    public float buffetting = 0.5f;
    double r(double deg)
    {
        return PI / 180 * deg;
    }

    public static void Average(LiftCurve c1, LiftCurve c2, LiftCurve output, double ratio)
    {
        if (output == null)
        {
            output = new LiftCurve();
        }
        output.mp = (c1.mp * (1 - ratio) + c2.mp * ratio);
        output.mn = (c1.mn * (1 - ratio) + c2.mn * ratio);
        output.ap = (c1.ap * (1 - ratio) + c2.ap * ratio);
        output.an = (c1.an * (1 - ratio) + c2.an * ratio);
        output.steepP = (c1.steepP * (1 - ratio) + c2.steepP * ratio);
        output.steepN = (c1.steepN * (1 - ratio) + c2.steepN * ratio);
        output.fMax = (c1.fMax * (1 - ratio) + c2.fMax * ratio);
        output.offsetAOA = (c1.offsetAOA * (1 - ratio) + c2.offsetAOA * ratio);

    }
    public void Modify(double aoaShift, double mpShift, double aCritShift, LiftCurve output)
    {
        output.ap += aCritShift;
        output.offsetAOA+= aoaShift;
        output.mp+= mpShift;
    }
    public double Evaluate(double aoa)
    {
        aoa += offsetAOA;
        double M = (mp - mn) / 2;
        double A = PI / r(ap - an);
        double f_k = (an < aoa && aoa < ap) ? ((mn + mp) / 2 + M * Math.Sin(A * r(aoa - (ap + an) / 2))) : 0;
        double s_flat = fMax * Math.Sin(2 * r(aoa)) *(1+ UnityEngine.Random.Range(-buffetting,buffetting));
        double X = Math.Pow(Math.Sin(PI * Math.Clamp(((aoa - ap) / 90 * steepP + 1) / 2, 0, 1)), 2);
        double Y = Math.Pow(Math.Sin(PI * Math.Clamp(((aoa - an) / 90 * steepN + 1) / 2, 0, 1)), 2);

        double stall_p = aoa > ap ? X * mp + s_flat * (1 - X) : 0;
        double stall_n = aoa < an ? Y * mn + s_flat * (1 - Y) : 0;
        double Cl = f_k + stall_p + stall_n;

        return Cl;
    }
}


/*
#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(LiftCurve))]
class LiftCurveDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Rect rect = new Rect(position.x,position.y,position.width,50);
        base.OnGUI(rect, property, label);
        //GUI.BeginScrollView(rect,Vector2.zero,rect);
        EditorGUI.DrawRect(rect, Color.black);
            GUI.BeginClip(rect);
            GL.PushMatrix();

            GL.Clear(true, false, Color.black);
            var shader = Shader.Find("Hidden/Internal-Colored");
            var mat = new Material(shader);

            mat.SetPass(0);

            float paddingLeft = 0;
            float paddingRight = 0;
            float rectWidth = rect.width - paddingLeft - paddingRight;
            float paddingTop = 0;
            float paddingBottom = 0;
            float rectHeight = rect.height - paddingTop - paddingBottom;

            float x_AxisOffset = rectHeight * math.remap(-1, 1, 0, 1, 0);
            float defaultValueOffset = rectHeight * math.remap(-1, 1, 0, 1, 1); ;

            // draw base graph
            GL.Begin(GL.LINES);
            GL.Color(new Color(1, 1, 1, 1));
            // draw Y axis
            GL.Vertex3(paddingLeft, paddingTop, 0);
            GL.Vertex3(paddingLeft, rect.height - paddingBottom, 0);
            // draw X axis
            GL.Vertex3(paddingLeft, rect.height - x_AxisOffset - paddingBottom, 0);
            GL.Vertex3(rect.width - paddingRight, rect.height - x_AxisOffset - paddingBottom, 0);
            // draw default values
            GL.Color(Color.green);
            GL.Vertex3(paddingLeft, rect.height - defaultValueOffset - paddingBottom, 0);
            GL.Vertex3(rect.width - paddingRight, rect.height - defaultValueOffset - paddingBottom, 0);
            GL.End();

            // evaluate func values
            *//* if (evalData.IsEmpty) EvaluateFunction();

             // re-evaluate func values after input values changed
             if (f != f0 || z != z0 || r != r0)
             {
                 InitFunction();
                 EvaluateFunction();
             }

             // draw graph
             GL.Begin(GL.LINE_STRIP);
             GL.Color(Color.cyan);
             for (int i = 0; i < evalData.Length; i++)
             {
                 Vector2 point = evalData.GetItem(i);

                 float x_remap = math.remap(evalData.X_min, evalData.X_max, 0, rectWidth, point.x);
                 float y_remap = math.remap(evalData.Y_min, evalData.Y_max, 0, rectHeight, point.y);

                 GL.Vertex3(paddingLeft + x_remap, rect.height - y_remap - paddingBottom, 0.0f);
             }*/
            /*    GL.End();
*//*
            GL.PopMatrix();
            GUI.EndClip();

            // draw values
            float squareSize = 10;
            EditorGUI.LabelField(new Rect(rect.x + paddingLeft - squareSize, rect.y + rect.height - defaultValueOffset - paddingBottom - squareSize / 2, squareSize, squareSize), "1"); // heigt "1" mark
            EditorGUI.LabelField(new Rect(rect.x + paddingLeft - squareSize, rect.y + rect.height - x_AxisOffset - paddingBottom + (squareSize * 0.2f), squareSize, squareSize), "0"); // height "0" mark
            EditorGUI.LabelField(new Rect(rect.x + rect.width - paddingRight - squareSize, rect.y + rect.height - x_AxisOffset - paddingBottom + (squareSize * 0.2f), squareSize, squareSize), "2"); // max lenght mark
      //  GUI.EndScrollView();

    }
}

#endif*/