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
            int teamRobotNearbyCount = CheckHelper.CountTeamRobotsInArea(radiusToCollectEnergy, nearestStationPos, movingRobot, robots);
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
                    if (CheckHelper.CountEnemyRobotsInArea(10, movingRobot.Position, movingRobot, robots) == 0
                        && DistanceHelper.FindNearestFreeRadiusStation(10, movingRobot, map, robots) == null)
                    {
                        return CommandHelper.GoToProfitablePlace(movingRobot, map, robots);
                    }
                    else
                    {
                        double coef = 1000 * (((1 + _roundNumber) * 0.03));
                        if (movingRobot.Energy > 200 &&
                            teamRobotNearbyCount < 1
                            //&& map.GetNearbyResources(currentRobot.Position,7).Count != 0 
                            //&& CheckHelper.CountEnemyRobotsInArea(10,currentRobot.Position,currentRobot,robots) > 0
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
                return new MoveCommand() { NewPosition = newPos };
            }

        }

     
    }
}
