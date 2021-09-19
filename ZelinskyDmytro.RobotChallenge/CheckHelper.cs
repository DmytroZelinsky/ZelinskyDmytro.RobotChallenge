using Robot.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZelinskyDmytro.RobotChallenge
{
    static class CheckHelper
    {
        static readonly int radiusToCollectEnergy = 3;
        public static bool IsStationFree(EnergyStation station, Robot.Common.Robot movingRobot,
        IList<Robot.Common.Robot> robots)
        {
            return IsCellFree(station.Position, movingRobot, robots);
        }
        public static bool IsStationFreeInRadius(int radius, EnergyStation station, Robot.Common.Robot movingRobot, IList<Robot.Common.Robot> robots)
        {
            int allRobots;
            allRobots = CountTeamRobotsInArea(radiusToCollectEnergy, station.Position, movingRobot, robots);
            allRobots += CountEnemyRobotsInArea(radiusToCollectEnergy, station.Position, movingRobot, robots);
            return allRobots == 0 ? true : false;
        }

        public static bool IsCellFree(Position cell, Robot.Common.Robot movingRobot, IList<Robot.Common.Robot> robots)
        {
            foreach (var robot in robots)
            {
                if (robot != movingRobot)
                {
                    if (robot.Position == cell)
                        return false;
                }
            }
            return true;
        }
        public static bool IsRobotInRadius(int radius, Position pos, Robot.Common.Robot movingRobot)
        {
            var robotPos = movingRobot.Position;
            int I = pos.Y + radius;
            int J = pos.X + radius;
            for (int i = pos.Y - radius; i <= I; i++)
            {
                if (i < 0) continue;
                for (int j = pos.X - radius; j <= J; j++)
                {
                    if (j < 0) continue;
                    if (robotPos.Y == i && robotPos.X == j)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public static int CountTeamRobotsInArea(int radius, Position centerPos, Robot.Common.Robot movingRobot, IList<Robot.Common.Robot> robots)
        {
            int counter = 0;
            foreach (var teamRobot in robots)
            {
                if (movingRobot.OwnerName == teamRobot.OwnerName &&
                    movingRobot != teamRobot &&
                    IsRobotInRadius(radius, centerPos, teamRobot))
                {
                    counter++;
                }
            }
            return counter;
        }

        public static int CountEnemyRobotsInArea(int radius, Position centerPos, Robot.Common.Robot movingRobot, IList<Robot.Common.Robot> robots)
        {
            int counter = 0;
            foreach (var enemyRobot in robots)
            {
                if (movingRobot.OwnerName != enemyRobot.OwnerName &&
                    IsRobotInRadius(radius, centerPos, enemyRobot))
                {
                    counter++;
                }
            }
            return counter;
        }
    }
}
