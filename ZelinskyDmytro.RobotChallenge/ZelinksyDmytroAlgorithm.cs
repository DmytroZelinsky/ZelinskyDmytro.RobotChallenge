using Robot.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ZelinskyDmytro.RobotChallenge
{
    class ZelinksyDmytroAlgorithm : IRobotAlgorithm
    {
        readonly int radiusToCollectEnergy = 3;
        public string Author => "Dmytro Zelinsky";

        private int _roundNumber;
        public ZelinksyDmytroAlgorithm()
        {
            Logger.OnLogRound += (sender, args) => ++_roundNumber;
        }

        public RobotCommand DoStep(IList<Robot.Common.Robot> robots, int robotToMoveIndex, Map map)
        {
            int robotCountToExpand = 1;
            int allTeamRobotCount = robots.Where(x => x.OwnerName == Author).Count();
            var movingRobot = robots[robotToMoveIndex];
            Position nearestStationPos;

            nearestStationPos = DistanceHelper.FindNearestStation(movingRobot, map);
            var newPos = DistanceHelper.FindNearestFreePointInRadius(radiusToCollectEnergy - 1, nearestStationPos, movingRobot, robots);
            int teamRobotNearbyCount = CheckHelper.CountTeamRobotsInArea(radiusToCollectEnergy, movingRobot.Position, movingRobot, robots);
            int enemyRobotNearbyCount = CheckHelper.CountEnemyRobotsInArea(radiusToCollectEnergy - 1, nearestStationPos, movingRobot, robots);

            if (CheckHelper.IsRobotInRadius(radiusToCollectEnergy, nearestStationPos, movingRobot))
            { 

                if (CheckHelper.CountEnemyRobotsInArea(radiusToCollectEnergy - 1, nearestStationPos, movingRobot, robots) > 0)
                {
                    if (allTeamRobotCount < 50)
                    {
                        //return CommandHelper.DefendPositionOrCollectEnergy(newPos, movingRobot, robots);
                        return CommandHelper.GoToProfitablePlace(movingRobot, map, robots);
                    }
                    else
                    {
                        return CommandHelper.DefendPositionOrCollectEnergy(newPos, movingRobot, robots);
                    }
                }
                else
                if (teamRobotNearbyCount >= robotCountToExpand)
                {
                    return CommandHelper.SpanishExpansion(movingRobot, map, robots);
                }
                else
                {
                    if (CheckHelper.CountEnemyRobotsInArea(7, movingRobot.Position, movingRobot, robots) == 0
                        && DistanceHelper.FindFreeStationInRadius(7, radiusToCollectEnergy, movingRobot, map, robots) == null
                        && CheckHelper.CountTeamRobotsInArea(3, nearestStationPos, movingRobot, robots) > 0)
                    {
                        return CommandHelper.GoToProfitablePlace(movingRobot, map, robots);
                    }
                    else
                    {
                        if (movingRobot.Energy > 200
                            && teamRobotNearbyCount < 1
                            && allTeamRobotCount < 100
                            && (CheckHelper.CountEnemyRobotsInArea(7, movingRobot.Position, movingRobot, robots) > 0
                            || DistanceHelper.FindFreeStationInRadius(7, radiusToCollectEnergy, movingRobot,map,robots) != null)
                            )
                        {
                            return new CreateNewRobotCommand() { NewRobotEnergy = 150};
                        }
                        else
                        {
                            return CommandHelper.DefendPositionOrCollectEnergy(newPos, movingRobot, robots);
                        }
                    }
                }
            } 
            else
            {
                return CommandHelper.GoToProfitablePlace(movingRobot, map, robots);
            }

        }

     
    }
}
