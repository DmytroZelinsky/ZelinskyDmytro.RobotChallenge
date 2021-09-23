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
            int allTeamRobotsCount = robots.Where(x => x.OwnerName == movingRobot.OwnerName).Count();
            int teamRobotCount;
            foreach (var enemyRobot in robots)
            {
                if(movingRobot.OwnerName != enemyRobot.OwnerName &&
                    CheckHelper.IsRobotInRadius(radiusToCollectEnergy, posToDefend, enemyRobot))
                {
                    //add robot with enemy  robot growth
                    teamRobotCount = CheckHelper.CountTeamRobotsInArea(radiusToCollectEnergy, enemyRobot.Position, movingRobot, robots);
                    if (teamRobotCount >= 1)
                    {
                        return KnockEnemy(movingRobot, enemyRobot);
                    }
                    else
                    {
                        if (movingRobot.Energy > 100 && allTeamRobotsCount < 100)
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
            //int distance = radiusToCollectEnergy + 1;
            int teamRobotsCount;
            int enemyRobotCount;
            List<EnergyStation> stations;
            bool isTeamOccupied = false;
            var nearestStationPos = DistanceHelper.FindNearestStation(movingRobot, map);
            for(int distance = radiusToCollectEnergy + 1; distance < 15; distance++)
            {
                stations = map.GetNearbyResources(movingRobot.Position, distance);
                if (stations.Count == 0)
                {
                    continue;
                }
                foreach (var station in stations)
                {

                    if (nearestStationPos == station.Position) continue;
                    teamRobotsCount = CheckHelper.CountTeamRobotsInArea(radiusToCollectEnergy - 1, station.Position, movingRobot, robots);
                    enemyRobotCount = CheckHelper.CountEnemyRobotsInArea(radiusToCollectEnergy - 1, station.Position, movingRobot, robots);

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
        public static RobotCommand GoToProfitablePlace(Robot.Common.Robot movingRobot, Map map, IList<Robot.Common.Robot> robots)
        {
            var nearestFreeStation =  DistanceHelper.FindNearestFreeRadiusStation(radiusToCollectEnergy + 2, movingRobot, map, robots);
            if (nearestFreeStation == null)
            {
                return DefendPositionOrCollectEnergy(movingRobot.Position,movingRobot,robots);
            }
            if(DistanceHelper.FindDistance(movingRobot.Position, nearestFreeStation.Position) > movingRobot.Energy)
            {
                if (DistanceHelper.FindDistance(movingRobot.Position, nearestFreeStation.Position) / 2 >
                    movingRobot.Energy)
                    return DefendPositionOrCollectEnergy(movingRobot.Position, movingRobot, robots);
                return CommandHelper.GoToIntermediatePosition(movingRobot, nearestFreeStation.Position, robots);
            }
            else
            {
                return new MoveCommand { NewPosition = DistanceHelper.FindNearestFreePointInRadius(radiusToCollectEnergy - 1, nearestFreeStation.Position, movingRobot, robots) };
            }

        }

        public static RobotCommand GoToIntermediatePosition(Robot.Common.Robot movingRobot, Position endPos, IList<Robot.Common.Robot> robots)
        {
            int distance = DistanceHelper.FindDistance(movingRobot.Position, endPos);
            double segments = 1;
            int x1, y1;
            Position intermediatePos = endPos;
            do
            {
                x1 = (int)((movingRobot.Position.X + endPos.X * segments) / (segments + 1));
                y1 = (int)((movingRobot.Position.Y + endPos.Y * segments) / (segments + 1));
                intermediatePos.X = x1;
                intermediatePos.Y = y1;
                ++segments;
            }
            while (movingRobot.Energy <= DistanceHelper.FindDistance(movingRobot.Position, intermediatePos) && segments < 10);

            return new MoveCommand() { NewPosition = intermediatePos };
        }
    }
}
