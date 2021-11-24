using Controller;
using UnityEngine;

namespace PathPlanning
{
    public class OsmPathPlanner : MonoBehaviour, IPlanner
    {
        private void Update()
        {
            var localizationOutput = SensorStore.LocalizationOutput;
            // var firstPoint = new Vector2(localizationOutput.carPosX, localizationOutput.carPosY);
            // var thirdPoint = new Vector2(localizationOutput.nextNodeX, localizationOutput.nextNodeY);
            var carDirection = new Vector3(localizationOutput.CarDirX, 0.0f, localizationOutput.CarDirY);
            // var secondPoint = CalculateSecondPoint(firstPoint, carDirection,thirdPoint);
            // var controlPoints = new List<Vector2>()
            // {
            //     firstPoint,
            //     secondPoint,
            //     thirdPoint
            // };
            //
            // var nextCoord =  BezierCurve.Point2(0.01f, controlPoints);

            var nextCoord = new Vector3((localizationOutput.NextNodeLx + localizationOutput.NextNodeRx) / 2 , 0,
                (localizationOutput.NextNodeLy + localizationOutput.NextNodeRy) / 2);
            var carPos = new Vector3(localizationOutput.CarPosX, 0, localizationOutput.CarPosY);
            
            var directionToMove = nextCoord - carPos;
            Debug.DrawLine(nextCoord, carPos);
            Debug.DrawRay(carPos, directionToMove, Color.blue);

            SensorStore.PathPlanningTrajectory = -Vector3.SignedAngle(directionToMove, carDirection, Vector3.up);
        }


        private static Vector2 CalculateSecondPoint(Vector2 firstPoint, Vector2 firstPointDirection, Vector2 thirdPoint)
        {
            var a = firstPointDirection.x;
            var b = firstPointDirection.y;
            var c = firstPoint.x;
            var d = firstPoint.y;
            var e = thirdPoint.x;
            var f = thirdPoint.y;
            var secondPointX = (a * a * e + b * b * c + a * b * f - a * b * d) / (a * a + b * b);
            var secondPointY = (a * a * d + b * b * f + a * b * e - a * b * c) / (a * a + b * b);

            return new Vector2(secondPointX, secondPointY);
        }
    }
}

