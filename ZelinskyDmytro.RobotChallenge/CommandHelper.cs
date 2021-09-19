using Robot.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZelinskyDmytro.RobotChallenge
{
    public static class CommandHelper
    {
        static readonly int radiusToCollectEnergy = 3;
        public static RobotCommand DefendPositionOrCollectEnergy(Position posToDefend, Robot.Common.Robot movingRobot, IList<Robot.Common.Robot> robots)
        {
            int teamRobotCount;
            foreach (var enemyRobot in robots)
            {
                if(movingRobot.OwnerName != enemyRobot.OwnerName &&
                    CheckHelper.IsRobotInRadius(radiusToCollectEnergy, posToDefend, enemyRobot))
                {
                    //add robot with enemy  robor growth
                    teamRobotCount = CheckHelper.CountTeamRobotsInArea(radiusToCollectEnergy, enemyRobot.Position, movingRobot, robots);
                    if (teamRobotCount >= 1)
                    {
                        return KnockEnemy(movingRobot, enemyRobot);
                    }
                    else
                    {
                        if (movingRobot.Energy > 100)
                        {
                            return new CreateNewRobotCommand() { NewRobotEnergy = 50};
                        }
                    }
                }
            }
            return new CollectEnergyCommand();
        }

        public static RobotCommand SpanishExpansion(Robot.Common.Robot movingRobot, Map map, IList<Robot.Common.Robot> robots)
        {
            int distance = radiusToCollectEnergy + 1;
            int teamRobotsCount = 0;
            List<EnergyStation> stations;
            bool isTeamOccupied = false;
            var nearestStationPos = DistanceHelper.FindNearestStation(movingRobot, map);
            for(int i = 0; i < 10; i++)
            {
                stations = map.GetNearbyResources(movingRobot.Position, distance);
                if (stations.Count == 0)
                {
                    distance++;
                    continue;
                }
                foreach (var station in stations)
                {
                    if (nearestStationPos == station.Position) continue;
                    teamRobotsCount = CheckHelper.CountTeamRobotsInArea(radiusToCollectEnergy - 1, station.Position, movingRobot, robots);
                    if (teamRobotsCount >= 1)
                    {
                        isTeamOccupied = true;
                    }
                    else
                    {
                        isTeamOccupied = false;
                    }
                    
                    if (!isTeamOccupied)
                    {
                        var newPos = DistanceHelper.FindNearestFreePointInRadius(radiusToCollectEnergy - 1, station.Position, movingRobot, robots);
                        if (DistanceHelper.FindDistance(newPos, movingRobot.Position) < movingRobot.Energy && !CheckHelper.IsRobotInRadius(radiusToCollectEnergy, station.Position,movingRobot))
                        {
                            return new MoveCommand() { NewPosition = newPos };
                        }
                        return new CollectEnergyCommand();
                    }
                }
                distance++;

            }
            return new CollectEnergyCommand();


        }
        public static RobotCommand KnockEnemy(Robot.Common.Robot moving, Robot.Common.Robot enemyRobot)
        {
            if (DistanceHelper.FindDistance(moving.Position, enemyRobot.Position) < moving.Energy)
            {
                return new MoveCommand() { NewPosition = enemyRobot.Position };
            }
            else
                return new CollectEnergyCommand();
        }
    }
}
