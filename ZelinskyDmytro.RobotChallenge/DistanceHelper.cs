using Robot.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ZelinskyDmytro.RobotChallenge
{
    public class DistanceHelper
    {
        static readonly int radiusToCollectEnergy = 3;
        public static int FindDistance(Position a, Position b)
        {
            return (int)(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
        }
        public static Position FindNearestFreeStation(Robot.Common.Robot movingRobot, Map map, IList<Robot.Common.Robot> robots)
        {
            EnergyStation nearest = null;
            int minDistance = int.MaxValue;
            foreach (var station in map.Stations)
            {
                if (CheckHelper.IsStationFree(station, movingRobot, robots))
                {
                    int d = FindDistance(station.Position, movingRobot.Position);
                    if (d < minDistance)
                    {
                        minDistance = d;
                        nearest = station;
                    }
                }
            }
            return nearest == null ? null : nearest.Position;
        }

        public static Position FindNearestStation(Robot.Common.Robot movingRobot, Map map)
        {
            EnergyStation nearest = null;
            int minDistance = int.MaxValue;
            foreach (var station in map.Stations)
            {
                int d = FindDistance(station.Position, movingRobot.Position);
                if (d < minDistance)
                {
                    minDistance = d;
                    nearest = station;
                }
                
            }
            
            return nearest == null ? null : nearest.Position;
        }



        public static Position FindNearestFreePointInRadius(int radius, Position centerPos, Robot.Common.Robot movingRobot, IList<Robot.Common.Robot> robots)
        {
            int distance = int.MaxValue;
            Position robotPos = movingRobot.Position;
            Position resPos = null;
            int I = centerPos.Y + radius;
            int J = centerPos.X + radius;
            for (int i = centerPos.Y - radius; i <= I; i++)
            {
                if (i < 0) continue;
                for (int j = centerPos.X - radius; j <= J; j++)
                {
                    if (j < 0) continue;

                    var newPos = new Position(j, i);
                    var newDistance = FindDistance(newPos, robotPos);
                    if (newDistance < distance)
                    {
                        if (CheckHelper.IsCellFree(newPos, movingRobot, robots))
                        {
                            resPos = newPos;
                            distance = newDistance;
                        }
                    }
                }
            }
            return resPos;

        }


        public static EnergyStation FindNearestFreeRadiusStation(int radius, Robot.Common.Robot movingRobot, Map map, IList<Robot.Common.Robot> robots)
        {
            var stations = map.Stations;
            EnergyStation nearestFreeStation = null;
            int distance = int.MaxValue;
            foreach (var station in stations)
            {
                if (CheckHelper.IsStationFreeInRadius(radius, station, movingRobot, robots))
                {
                    var newDistance = DistanceHelper.FindDistance(movingRobot.Position, station.Position);
                    if (newDistance < distance)
                    {
                        nearestFreeStation = station;
                        distance = newDistance;
                    }
                }
            }
            return nearestFreeStation;
        }



        public static EnergyStation FindFreeStationInRadius(int radiusToFind, int radiusToCollect, Robot.Common.Robot movingRobot,
            Map map, IList<Robot.Common.Robot> robots)
        {
            var stations = map.GetNearbyResources(movingRobot.Position, radiusToFind);
            int distance = int.MaxValue;
            int newDistance;
            EnergyStation nearestStation = null;
            foreach (var station in stations)
            {
                newDistance = DistanceHelper.FindDistance(movingRobot.Position, station.Position);
                if (CheckHelper.IsStationFreeInRadius(radiusToCollect, station, movingRobot, robots) && newDistance < distance)
                {
                    nearestStation = station;
                    distance = newDistance;
                }
            }

            return nearestStation;
        }
    }
   

}
