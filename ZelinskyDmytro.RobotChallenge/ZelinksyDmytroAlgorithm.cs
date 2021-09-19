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
            
            var movingRobot = robots[robotToMoveIndex];
            Position nearestStationPos;

            nearestStationPos = DistanceHelper.FindNearestStation(movingRobot, map);
            var newPos = DistanceHelper.FindNearestFreePointInRadius(radiusToCollectEnergy - 1, nearestStationPos, movingRobot, robots);
            int teamRobotNearbyCount = CheckHelper.CountTeamRobotsInArea(radiusToCollectEnergy, nearestStationPos, movingRobot, robots);
            if (CheckHelper.IsRobotInRadius(radiusToCollectEnergy, nearestStationPos, movingRobot))
            {
                if (CheckHelper.CountEnemyRobotsInArea(radiusToCollectEnergy - 1, nearestStationPos, movingRobot, robots) > 0)
                {
                    return CommandHelper.DefendPositionOrCollectEnergy(newPos, movingRobot, robots);
                }
                else
                if (teamRobotNearbyCount >= robotCountToExpand)
                {
                    return CommandHelper.SpanishExpansion(movingRobot, map, robots);
                }
                else
                {
                    double coef = 1000 * (((1 + _roundNumber) * 0.03));
                    if (movingRobot.Energy > coef && 
                        teamRobotNearbyCount < 1 
                        //&& map.GetNearbyResources(currentRobot.Position,7).Count != 0 
                        //&& CheckHelper.CountEnemyRobotsInArea(10,currentRobot.Position,currentRobot,robots) > 0
                        )
                    {
                        return new CreateNewRobotCommand() { NewRobotEnergy = (int)(coef/2)};
                    }
                    else
                    {
                        return CommandHelper.DefendPositionOrCollectEnergy(newPos, movingRobot, robots);
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
