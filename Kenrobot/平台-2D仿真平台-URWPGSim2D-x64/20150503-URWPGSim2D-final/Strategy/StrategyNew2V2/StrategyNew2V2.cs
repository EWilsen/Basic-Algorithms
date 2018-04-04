﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using xna = Microsoft.Xna.Framework;
using Microsoft.Xna.Framework;
using URWPGSim2D.Common;
using URWPGSim2D.StrategyLoader;

namespace URWPGSim2D.Strategy
{
    public class Strategy : MarshalByRefObject, IStrategy
    {
        #region reserved code never be changed or removed
        /// <summary>
        /// override the InitializeLifetimeService to return null instead of a valid ILease implementation
        /// to ensure this type of remote object never dies
        /// </summary>
        /// <returns>null</returns>
        public override object InitializeLifetimeService()
        {
            //return base.InitializeLifetimeService();
            return null; // makes the object live indefinitely
        }
        #endregion

        /// <summary>
        /// 决策类当前对象对应的仿真使命参与队伍的决策数组引用 第一次调用GetDecision时分配空间
        /// </summary>
        /// 

        private Decision[] decisions = null;

        /// <summary>
        /// 获取队伍名称 在此处设置参赛队伍的名称
        /// </summary>
        /// <returns>队伍名称字符串</returns>
        public string GetTeamName()
        {
            return "JURASSIC2";
        }

        /// <summary>
        /// 获取当前仿真使命（比赛项目）当前队伍所有仿真机器鱼的决策数据构成的数组
        /// </summary>
        /// <param name="mission">服务端当前运行着的仿真使命Mission对象</param>
        /// <param name="teamId">当前队伍在服务端运行着的仿真使命中所处的编号 
        /// 用于作为索引访问Mission对象的TeamsRef队伍列表中代表当前队伍的元素</param>
        /// <returns>当前队伍所有仿真机器鱼的决策数据构成的Decision数组对象</returns>
        public Decision[] GetDecision(Mission mission, int teamId)
        {
            // 决策类当前对象第一次调用GetDecision时Decision数组引用为null
            if (decisions == null)
            {// 根据决策类当前对象对应的仿真使命参与队伍仿真机器鱼的数量分配决策数组空间
                decisions = new Decision[mission.CommonPara.FishCntPerTeam];
            }

            #region 决策计算过程 需要各参赛队伍实现的部分
            #region 策略编写帮助信息
            //====================我是华丽的分割线====================//
            //======================策略编写指南======================//
            //1.策略编写工作直接目标是给当前队伍决策数组decisions各元素填充决策值
            //2.决策数据类型包括两个int成员，VCode为速度档位值，TCode为转弯档位值
            //3.VCode取值范围0-14共15个整数值，每个整数对应一个速度值，速度值整体但非严格递增
            //有个别档位值对应的速度值低于比它小的档位值对应的速度值，速度值数据来源于实验
            //4.TCode取值范围0-14共15个整数值，每个整数对应一个角速度值
            //整数7对应直游，角速度值为0，整数6-0，8-14分别对应左转和右转，偏离7越远，角度速度值越大
            //5.任意两个速度/转弯档位之间切换，都需要若干个仿真周期，才能达到稳态速度/角速度值
            //目前运动学计算过程决定稳态速度/角速度值接近但小于目标档位对应的速度/角速度值
            //6.决策类Strategy的实例在加载完毕后一直存在于内存中，可以自定义私有成员变量保存必要信息
            //但需要注意的是，保存的信息在中途更换策略时将会丢失
            //====================我是华丽的分割线====================//
            //=======策略中可以使用的比赛环境信息和过程信息说明=======//
            //场地坐标系: 以毫米为单位，矩形场地中心为原点，向右为正X，向下为正Z
            //            负X轴顺时针转回负X轴角度范围为(-PI,PI)的坐标系，也称为世界坐标系
            //mission.CommonPara: 当前仿真使命公共参数
            //mission.CommonPara.FishCntPerTeam: 每支队伍仿真机器鱼数量
            //mission.CommonPara.MsPerCycle: 仿真周期毫秒数
            //mission.CommonPara.RemainingCycles: 当前剩余仿真周期数
            //mission.CommonPara.TeamCount: 当前仿真使命参与队伍数量
            //mission.CommonPara.TotalSeconds: 当前仿真使命运行时间秒数
            //mission.EnvRef.Balls: 
            //当前仿真使命涉及到的仿真水球列表，列表元素的成员意义参见URWPGSim2D.Common.Ball类定义中的注释
            //mission.EnvRef.FieldInfo: 
            //当前仿真使命涉及到的仿真场地，各成员意义参见URWPGSim2D.Common.Field类定义中的注释
            //mission.EnvRef.ObstaclesRect: 
            //当前仿真使命涉及到的方形障碍物列表，列表元素的成员意义参见URWPGSim2D.Common.RectangularObstacle类定义中的注释
            //mission.EnvRef.ObstaclesRound:
            //当前仿真使命涉及到的圆形障碍物列表，列表元素的成员意义参见URWPGSim2D.Common.RoundedObstacle类定义中的注释
            //mission.TeamsRef[teamId]:
            //决策类当前对象对应的仿真使命参与队伍（当前队伍）
            //mission.TeamsRef[teamId].Para:
            //当前队伍公共参数，各成员意义参见URWPGSim2D.Common.TeamCommonPara类定义中的注释
            //mission.TeamsRef[teamId].Fishes:
            //当前队伍仿真机器鱼列表，列表元素的成员意义参见URWPGSim2D.Common.RoboFish类定义中的注释
            //mission.TeamsRef[teamId].Fishes[i].PositionMm和PolygonVertices[0],BodyDirectionRad,VelocityMmPs,
            //                                   AngularVelocityRadPs,Tactic:
            //当前队伍第i条仿真机器鱼鱼体矩形中心和鱼头顶点在场地坐标系中的位置（用到X坐标和Z坐标），鱼体方向，速度值，
            //                                   角速度值，决策值
            //====================我是华丽的分割线====================//
            //========================典型循环========================//
            //for (int i = 0; i < mission.CommonPara.FishCntPerTeam; i++)
            //{
            //  decisions[i].VCode = 0; // 静止
            //  decisions[i].TCode = 7; // 直游
            //}
            //====================我是华丽的分割线====================//
            #endregion
            //请从这里开始编写代码


            int t = mission.CommonPara.RemainingCycles;//定义剩余时间
            int score = mission.TeamsRef[teamId].Para.Score;//定义本队的得分
            xna.Vector3[] fishPt = new xna.Vector3[2];//定义我方鱼的数组
            xna.Vector3[] ballPt = new xna.Vector3[9];//定义所有球的数组
            for (int i = 0; i < 2; i++)//定义我方的两条鱼一号鱼——0，二号鱼——1
                fishPt[i] = mission.TeamsRef[teamId].Fishes[i].PositionMm;
            for (int i = 0; i < 9; i++)//定义比赛场地的所有球
                ballPt[i] = mission.EnvRef.Balls[i].PositionMm;
            int r = 58;//球的半径
            //定义两条鱼的身体方向
            float D0 = mission.TeamsRef[teamId].Fishes[0].AngularVelocityRadPs;
            float D1 = mission.TeamsRef[teamId].Fishes[1].AngularVelocityRadPs;
            #region 场上所需点的坐标
            //A1区的点
            xna.Vector3 UPA1 = new Vector3(-988f, 0f, -688f);
            xna.Vector3 UPA2 = new Vector3(-392f, 0f, -688f);
            xna.Vector3 UPA3 = new Vector3(392, 0f, -688f);
            xna.Vector3 UPA4 = new Vector3(988, 0f, -688f);
            //A2区的点
            xna.Vector3 DOWNA1 = new Vector3(-800f, 0f, 650f);
            xna.Vector3 DOWNA2 = new Vector3(-392f, 0f, 688f);
            xna.Vector3 DOWNA3 = new Vector3(392, 0f, 688f);
            xna.Vector3 DOWNA4 = new Vector3(800, 0f, 688f);
            //B1区的点
            xna.Vector3 UPB1 = new Vector3(-1248f, 0f, -560f);
            xna.Vector3 UPB2 = new Vector3(1248f, 0f, -560f);
            //B2区的点
            xna.Vector3 DOWNB1 = new Vector3(-1248f, 0f, 600f);
            xna.Vector3 DOWNB2 = new Vector3(1248f, 0f, 600f);
            //左场缓冲点
            xna.Vector3 LUP = new Vector3(68f, 0f, -68f);
            xna.Vector3 LDOWN = new Vector3(68f, 0f, 68f);
            //右场缓冲点
            xna.Vector3 RUP = new Vector3(-68f, 0f, -68f);
            xna.Vector3 RDOWN = new Vector3(-68f, 0f, 68f);
            //G1区的点
            xna.Vector3 G1UP = new Vector3(-1288f, 0f, -364f);
            xna.Vector3 G1DOWN = new Vector3(-1288f, 0f, 364f);
            //G2区的点
            xna.Vector3 G2UP = new Vector3(1288f, 0f, -364f);
            xna.Vector3 G2DOWN = new Vector3(1288f, 0f, 364f);
            //左半场的目标点
            xna.Vector3 LgoalUP = new Vector3(-1008f, 0f, -100f);
            xna.Vector3 LgoalDOWN = new Vector3(-1008f, 0f, 100f);
            //右半场的目标点
            xna.Vector3 RgoalUP = new Vector3(1008f, 0f, -100f);
            xna.Vector3 RgoalDOWN = new Vector3(1008f, 0f, 100f);
            #endregion
            #region 进球的判断
            //左半场是否进球的判定
            int b0_l = Convert.ToInt32(mission.HtMissionVariables["Ball_0_Left_Status"]);
            int b1_l = Convert.ToInt32(mission.HtMissionVariables["Ball_1_Left_Status"]);
            int b2_l = Convert.ToInt32(mission.HtMissionVariables["Ball_2_Left_Status"]);
            int b3_l = Convert.ToInt32(mission.HtMissionVariables["Ball_3_Left_Status"]);
            int b4_l = Convert.ToInt32(mission.HtMissionVariables["Ball_4_Left_Status"]);
            int b5_l = Convert.ToInt32(mission.HtMissionVariables["Ball_5_Left_Status"]);
            int b6_l = Convert.ToInt32(mission.HtMissionVariables["Ball_6_Left_Status"]);
            int b7_l = Convert.ToInt32(mission.HtMissionVariables["Ball_7_Left_Status"]);
            int b8_l = Convert.ToInt32(mission.HtMissionVariables["Ball_8_Left_Status"]);
            //右半场是否进球的判定
            int b0_r = Convert.ToInt32(mission.HtMissionVariables["Ball_0_Right_Status"]);
            int b1_r = Convert.ToInt32(mission.HtMissionVariables["Ball_1_Right_Status"]);
            int b2_r = Convert.ToInt32(mission.HtMissionVariables["Ball_2_Right_Status"]);
            int b3_r = Convert.ToInt32(mission.HtMissionVariables["Ball_3_Right_Status"]);
            int b4_r = Convert.ToInt32(mission.HtMissionVariables["Ball_4_Right_Status"]);
            int b5_r = Convert.ToInt32(mission.HtMissionVariables["Ball_5_Right_Status"]);
            int b6_r = Convert.ToInt32(mission.HtMissionVariables["Ball_6_Right_Status"]);
            int b7_r = Convert.ToInt32(mission.HtMissionVariables["Ball_7_Right_Status"]);
            int b8_r = Convert.ToInt32(mission.HtMissionVariables["Ball_8_Right_Status"]);
            #endregion
            #region 每个球分别到我方0号鱼和1号鱼的距离
            //定义0号鱼到每一条鱼的距离
            float dis0_fish0 = DisFishToBall(ballPt[0], fishPt[0]);
            float dis1_fish0 = DisFishToBall(ballPt[1], fishPt[0]);
            float dis2_fish0 = DisFishToBall(ballPt[2], fishPt[0]);
            float dis3_fish0 = DisFishToBall(ballPt[3], fishPt[0]);
            float dis4_fish0 = DisFishToBall(ballPt[4], fishPt[0]);
            float dis5_fish0 = DisFishToBall(ballPt[5], fishPt[0]);
            float dis6_fish0 = DisFishToBall(ballPt[6], fishPt[0]);
            float dis7_fish0 = DisFishToBall(ballPt[7], fishPt[0]);
            float dis8_fish0 = DisFishToBall(ballPt[8], fishPt[0]);
            //定义1号鱼到每一条鱼的距离
            float dis0_fish1 = DisFishToBall(ballPt[0], fishPt[1]);
            float dis1_fish1 = DisFishToBall(ballPt[1], fishPt[1]);
            float dis2_fish1 = DisFishToBall(ballPt[2], fishPt[1]);
            float dis3_fish1 = DisFishToBall(ballPt[3], fishPt[1]);
            float dis4_fish1 = DisFishToBall(ballPt[4], fishPt[1]);
            float dis5_fish1 = DisFishToBall(ballPt[5], fishPt[1]);
            float dis6_fish1 = DisFishToBall(ballPt[6], fishPt[1]);
            float dis7_fish1 = DisFishToBall(ballPt[7], fishPt[1]);
            float dis8_fish1 = DisFishToBall(ballPt[8], fishPt[1]);
            #endregion
            #region 两条鱼的初始化
            //if (t > 5970)
            //{
            //    float dir1 = DirfishToG(fishPt[0], LUP);
            //    float dir2 = DirfishToG(fishPt[1], LDOWN);
            //    StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], LUP,
            //        dir1, 5, 10, 100, 14, 14, 8, 100, true);
            //    StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], LDOWN,
            //        dir2, 5, 10, 100, 14, 14, 8, 100, true);
            //}
            #endregion
            #region 进攻
            if (t < 6000)
            {
                #region 在左半场
                if (mission.TeamsRef[teamId].Para.MyHalfCourt == HalfCourt.RIGHT)//在右半场
                {
                    xna.Vector3[] fishPT2 = new Vector3[2];//定义对方鱼的数组
                    for (int i = 0; i < 2; i++)
                    {
                        fishPt[i] = mission.TeamsRef[teamId].Fishes[i].PositionMm;
                        fishPT2[i] = mission.TeamsRef[(teamId + 1) % 2].Fishes[i].PositionMm;
                    }
                    xna.Vector3 G1 = fishPT2[0];
                    float DIRG1 = DirfishToG(fishPt[0], G1);
                    xna.Vector3 G2 = fishPT2[1];
                    float DIRG2 = DirfishToG(fishPt[1], G2);
                    StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], G1,
                        DIRG1, 5, 10, 100, 14, 14, 8, 50, true);
                    StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], G2,
                        DIRG2, 5, 10, 100, 14, 14, 8, 50, true);
                }

                if (mission.TeamsRef[teamId].Para.MyHalfCourt == HalfCourt.LEFT)//在左半场
                {
                    {
                        //decisions[1].VCode = 0; // 静止
                        //decisions[1].TCode = 7; // 直游
                        #region  1号鱼顶球策略
                        if (b0_l == 0)
                        {
                            #region //1号鱼顶0号球的方案
                            //0号球在D区上部分
                            if (ballPt[0].Z > -560 && ballPt[0].Z < 220)
                            {
                                if (ballPt[0].X < 948 && ballPt[0].X > 0)
                                {
                                    xna.Vector3 UPA22 = new xna.Vector3();
                                    float dirUPA22 = DirBToG(ballPt[0], UPA2);
                                    UPA22.X = (float)(ballPt[0].X + 0.8 * r * Math.Cos(dirUPA22));
                                    UPA22.Z = (float)(ballPt[0].Z + 0.8 * r * Math.Sin(dirUPA22));
                                    StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], UPA22,
                                        dirUPA22, 5, 10, 80, 14, 10, 10, 100, true);
                                }

                                if (ballPt[0].X < 0 && ballPt[0].X > -948)
                                {

                                    xna.Vector3 UPA11 = new xna.Vector3();
                                    float dirUPA11 = DirBToG(ballPt[0], UPA1);
                                    UPA11.X = (float)(ballPt[0].X + 0.8 * r * Math.Cos(dirUPA11));
                                    UPA11.Z = (float)(ballPt[0].Z + 0.8 * r * Math.Sin(dirUPA11));
                                    StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], UPA11,
                                         dirUPA11, 5, 10, 80, 14, 10, 10, 100, true);
                                }
                            }
                            //0号球在D区下部分
                            if (ballPt[0].Z > 220 && ballPt[0].Z < 560)
                            {
                                if (ballPt[0].X < 948 && ballPt[0].X > 0)
                                {
                                    xna.Vector3 DOWNA22 = new xna.Vector3();
                                    float dirDOWNA22 = DirBToG(ballPt[0], DOWNA2);
                                    DOWNA22.X = (float)(ballPt[0].X + 0.8 * r * Math.Cos(dirDOWNA22));
                                    DOWNA22.Z = (float)(ballPt[0].Z + 0.8 * r * Math.Sin(dirDOWNA22));
                                    StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], DOWNA22,
                                        dirDOWNA22, 5, 10, 80, 14, 10, 10, 100, true);
                                }

                                if (ballPt[0].X < 0 && ballPt[0].X > -948)
                                {

                                    xna.Vector3 DOWNA11 = new xna.Vector3();
                                    float dirDOWNA11 = DirBToG(ballPt[0], DOWNA1);
                                    DOWNA11.X = (float)(ballPt[0].X + 0.8 * r * Math.Cos(dirDOWNA11));
                                    DOWNA11.Z = (float)(ballPt[0].Z + 0.8 * r * Math.Sin(dirDOWNA11));
                                    StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], DOWNA11,
                                         dirDOWNA11, 5, 10, 80, 14, 10, 10, 100, true);
                                }
                            }
                            //0号球在G2区
                            if (ballPt[0].X > 1092 && ballPt[0].X < 1500)
                            {
                                if (ballPt[0].Z < 0 && ballPt[0].Z > -560)
                                {
                                    xna.Vector3 GUP = new xna.Vector3();
                                    float dirGUP = DirfishToG(ballPt[0], UPB2);
                                    GUP.X = (float)(ballPt[0].X - 0.8 * r * Math.Cos(dirGUP));
                                    GUP.Z = (float)(ballPt[0].Z - 0.8 * r * Math.Sin(dirGUP));
                                    StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], GUP,
                                        dirGUP, 5, 10, 80, 11, 8, 10, 100, true);
                                }
                                if (ballPt[0].Z < 560 && ballPt[0].Z > 0)
                                {
                                    xna.Vector3 GDOWN = new xna.Vector3();
                                    float dirGDOWN = DirfishToG(ballPt[0], DOWNB2);
                                    GDOWN.X = (float)(ballPt[0].X - 0.8 * r * Math.Cos(dirGDOWN));
                                    GDOWN.Z = (float)(ballPt[0].Z - 0.8 * r * Math.Sin(dirGDOWN));
                                    StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], GDOWN,
                                        dirGDOWN, 5, 10, 80, 11, 8, 10, 100, true);
                                }
                            }
                            if (ballPt[0].Z > -1000 && ballPt[0].Z < -560)
                            {
                                //0号球在A1区
                                if (ballPt[0].X < 1500 && ballPt[0].X > -1200)
                                {
                                    xna.Vector3 UPB11 = new xna.Vector3();
                                    float dirUPB11 = DirBToG(ballPt[0], UPB1);
                                    UPB11.X = (float)(ballPt[0].X + 0.8 * r * Math.Cos(dirUPB11));
                                    UPB11.Z = (float)(ballPt[0].Z + 0.8 * r * Math.Sin(dirUPB11));
                                    StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], UPB11,
                                        dirUPB11, 5, 10, 100, 10, 7, 10, 100, true);
                                }
                                //0号球在B1区
                                if (ballPt[0].X < -1200 && ballPt[0].X > -1500)
                                {
                                    xna.Vector3 G1UP1 = new xna.Vector3();
                                    float dirG1UP1 = DirBToG(ballPt[0], G1UP);
                                    G1UP1.X = (float)(ballPt[0].X + 0.8 * r * Math.Cos(dirG1UP1));
                                    G1UP1.Z = (float)(ballPt[0].Z + 0.8 * r * Math.Sin(dirG1UP1));
                                    StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], G1UP1,
                                        dirG1UP1, 5, 10, 100, 10, 7, 10, 100, true);
                                }
                            }
                            if (ballPt[0].Z > 560 && ballPt[0].Z < 1000)
                            {
                                //0号球在A2区
                                if (ballPt[0].X < 1500 && ballPt[0].X > -1200)
                                {
                                    xna.Vector3 DOWNB11 = new xna.Vector3();
                                    float dirDOWNB11 = DirBToG(ballPt[0], DOWNB1);
                                    DOWNB11.X = (float)(ballPt[0].X + 0.8 * r * Math.Cos(dirDOWNB11));
                                    DOWNB11.Z = (float)(ballPt[0].Z + 0.8 * r * Math.Sin(dirDOWNB11));
                                    StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], DOWNB11,
                                        dirDOWNB11, 5, 10, 100, 10, 7, 10, 100, true);
                                }
                                //0号球在B2区
                                if (ballPt[0].X < -1200 && ballPt[0].X > -1500)
                                {
                                    xna.Vector3 G1DOWN1 = new xna.Vector3();
                                    float dirG1DOWN1 = DirBToG(ballPt[0], G1DOWN);
                                    G1DOWN1.X = (float)(ballPt[0].X + 0.8 * r * Math.Cos(dirG1DOWN1));
                                    G1DOWN1.Z = (float)(ballPt[0].Z + 0.8 * r * Math.Sin(dirG1DOWN1));
                                    StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], G1DOWN1,
                                        dirG1DOWN1, 5, 10, 100, 10, 7, 10, 100, true);
                                }
                            }
                            //0号球在G1区
                            if (ballPt[0].X > -1500 && ballPt[0].X < -1092)
                            {
                                if (ballPt[0].Z < 0 && ballPt[0].Z > -560)
                                {
                                    xna.Vector3 GUP = new xna.Vector3();
                                    float dirGUP = DirfishToG(ballPt[0], LgoalUP);
                                    GUP.X = (float)(ballPt[0].X - 0.8 * r * Math.Cos(dirGUP));
                                    GUP.Z = (float)(ballPt[0].Z - 0.8 * r * Math.Sin(dirGUP));
                                    StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], GUP,
                                        dirGUP, 5, 10, 80, 11, 8, 15, 100, true);
                                }
                                if (ballPt[0].Z < 560 && ballPt[0].Z > 0)
                                {
                                    xna.Vector3 GDOWN = new xna.Vector3();
                                    float dirGDOWN = DirfishToG(ballPt[0], LgoalDOWN);
                                    GDOWN.X = (float)(ballPt[0].X + 0.8 * r * Math.Sin(dirGDOWN));
                                    GDOWN.Z = (float)(ballPt[0].Z + 0.8 * r * Math.Cos(dirGDOWN));
                                    StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], GDOWN,
                                        dirGDOWN, 5, 10, 80, 11, 8, 15, 100, true);
                                }
                            }

                            #endregion
                        }
                        if (b0_l == 1)
                        {

                            if (fishPt[1].X < -1000 && fishPt[1].X > -1500 && fishPt[1].Z < 560 && fishPt[1].Z > -560
                                && ballPt[7].X > -1000 && ballPt[8].X > -1000)
                            {
                                if (D1 > -Math.PI && D1 < 0)
                                {
                                    float dir1 = DirfishToG(fishPt[1], UPB1);
                                    StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], UPB1,
                                          dir1, 5, 10, 100, 14, 14, 8, 100, true);
                                }
                                else
                                {
                                    float dir2 = DirfishToG(fishPt[1], DOWNB1);
                                    StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], UPB1,
                                          dir2, 5, 10, 100, 14, 14, 8, 100, true);
                                }
                            }
                            else
                            {
                                if (b7_l == 0 && b7_r == 0 && b8_r == 0 && b8_l == 0)
                                {
                                    if (dis7_fish1 < dis8_fish1)
                                    {
                                        #region //1号鱼顶7号球的方案
                                        //7号球在D区上部分
                                        if (ballPt[7].Z > -560 && ballPt[7].Z < 220)
                                        {
                                            if (ballPt[7].X < 948 && ballPt[7].X > 0)
                                            {
                                                xna.Vector3 UPA22 = new xna.Vector3();
                                                float dirUPA22 = DirBToG(ballPt[7], UPA2);
                                                UPA22.X = (float)(ballPt[7].X + 0.8 * r * Math.Cos(dirUPA22));
                                                UPA22.Z = (float)(ballPt[7].Z + 0.8 * r * Math.Sin(dirUPA22));
                                                StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], UPA22,
                                                    dirUPA22, 5, 10, 80, 14, 10, 10, 100, true);
                                            }

                                            if (ballPt[7].X < 0 && ballPt[7].X > -948)
                                            {

                                                xna.Vector3 UPA11 = new xna.Vector3();
                                                float dirUPA11 = DirBToG(ballPt[7], UPA1);
                                                UPA11.X = (float)(ballPt[7].X + 0.8 * r * Math.Cos(dirUPA11));
                                                UPA11.Z = (float)(ballPt[7].Z + 0.8 * r * Math.Sin(dirUPA11));
                                                StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], UPA11,
                                                     dirUPA11, 5, 10, 80, 14, 10, 10, 100, true);
                                            }
                                        }
                                        //7号球在D区下部分
                                        if (ballPt[7].Z > 220 && ballPt[7].Z < 560)
                                        {
                                            if (ballPt[7].X < 948 && ballPt[7].X > 0)
                                            {
                                                xna.Vector3 DOWNA22 = new xna.Vector3();
                                                float dirDOWNA22 = DirBToG(ballPt[7], DOWNA2);
                                                DOWNA22.X = (float)(ballPt[7].X + 0.8 * r * Math.Cos(dirDOWNA22));
                                                DOWNA22.Z = (float)(ballPt[7].Z + 0.8 * r * Math.Sin(dirDOWNA22));
                                                StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], DOWNA22,
                                                    dirDOWNA22, 5, 10, 80, 14, 10, 10, 100, true);
                                            }

                                            if (ballPt[7].X < 0 && ballPt[7].X > -948)
                                            {

                                                xna.Vector3 DOWNA11 = new xna.Vector3();
                                                float dirDOWNA11 = DirBToG(ballPt[7], DOWNA1);
                                                DOWNA11.X = (float)(ballPt[7].X + 0.8 * r * Math.Cos(dirDOWNA11));
                                                DOWNA11.Z = (float)(ballPt[7].Z + 0.8 * r * Math.Sin(dirDOWNA11));
                                                StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], DOWNA11,
                                                     dirDOWNA11, 5, 10, 80, 14, 10, 10, 100, true);
                                            }
                                        }
                                        //7号球在G2区
                                        if (ballPt[7].X > 1092 && ballPt[7].X < 1500)
                                        {
                                            if (ballPt[7].Z < 0 && ballPt[7].Z > -560)
                                            {
                                                xna.Vector3 GUP = new xna.Vector3();
                                                float dirGUP = DirfishToG(ballPt[7], UPB2);
                                                GUP.X = (float)(ballPt[7].X - 0.8 * r * Math.Cos(dirGUP));
                                                GUP.Z = (float)(ballPt[7].Z - 0.8 * r * Math.Sin(dirGUP));
                                                StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], GUP,
                                                    dirGUP, 5, 10, 80, 11, 8, 10, 100, true);
                                            }
                                            if (ballPt[7].Z < 560 && ballPt[7].Z > 0)
                                            {
                                                xna.Vector3 GDOWN = new xna.Vector3();
                                                float dirGDOWN = DirfishToG(ballPt[7], DOWNB2);
                                                GDOWN.X = (float)(ballPt[7].X - 0.8 * r * Math.Cos(dirGDOWN));
                                                GDOWN.Z = (float)(ballPt[7].Z - 0.8 * r * Math.Sin(dirGDOWN));
                                                StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], GDOWN,
                                                    dirGDOWN, 5, 10, 80, 11, 8, 10, 100, true);
                                            }
                                        }
                                        if (ballPt[7].Z > -1000 && ballPt[7].Z < -560)
                                        {
                                            //7号球在A1区
                                            if (ballPt[7].X < 1500 && ballPt[7].X > -1200)
                                            {
                                                xna.Vector3 UPB11 = new xna.Vector3();
                                                float dirUPB11 = DirBToG(ballPt[7], UPB1);
                                                UPB11.X = (float)(ballPt[7].X + 0.8 * r * Math.Cos(dirUPB11));
                                                UPB11.Z = (float)(ballPt[7].Z + 0.8 * r * Math.Sin(dirUPB11));
                                                StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], UPB11,
                                                    dirUPB11, 5, 10, 100, 10, 7, 10, 100, true);
                                            }
                                            //7号球在B1区
                                            if (ballPt[7].X < -1200 && ballPt[7].X > -1500)
                                            {
                                                xna.Vector3 G1UP1 = new xna.Vector3();
                                                float dirG1UP1 = DirBToG(ballPt[7], G1UP);
                                                G1UP1.X = (float)(ballPt[7].X + 0.8 * r * Math.Cos(dirG1UP1));
                                                G1UP1.Z = (float)(ballPt[7].Z + 0.8 * r * Math.Sin(dirG1UP1));
                                                StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], G1UP1,
                                                    dirG1UP1, 5, 10, 100, 10, 7, 10, 100, true);
                                            }
                                        }
                                        if (ballPt[7].Z > 560 && ballPt[7].Z < 1000)
                                        {
                                            //7号球在A2区
                                            if (ballPt[7].X < 1500 && ballPt[7].X > -1200)
                                            {
                                                xna.Vector3 DOWNB11 = new xna.Vector3();
                                                float dirDOWNB11 = DirBToG(ballPt[7], DOWNB1);
                                                DOWNB11.X = (float)(ballPt[7].X + 0.8 * r * Math.Cos(dirDOWNB11));
                                                DOWNB11.Z = (float)(ballPt[7].Z + 0.8 * r * Math.Sin(dirDOWNB11));
                                                StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], DOWNB11,
                                                    dirDOWNB11, 5, 10, 100, 10, 7, 10, 100, true);
                                            }
                                            //7号球在B2区
                                            if (ballPt[7].X < -1200 && ballPt[7].X > -1500)
                                            {
                                                xna.Vector3 G1DOWN1 = new xna.Vector3();
                                                float dirG1DOWN1 = DirBToG(ballPt[7], G1DOWN);
                                                G1DOWN1.X = (float)(ballPt[7].X + 0.8 * r * Math.Cos(dirG1DOWN1));
                                                G1DOWN1.Z = (float)(ballPt[7].Z + 0.8 * r * Math.Sin(dirG1DOWN1));
                                                StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], G1DOWN1,
                                                    dirG1DOWN1, 5, 10, 100, 10, 7, 10, 100, true);
                                            }
                                        }
                                        //7号球在G1区
                                        if (ballPt[7].X > -1500 && ballPt[7].X < -1092)
                                        {
                                            if (ballPt[7].Z < 0 && ballPt[7].Z > -560)
                                            {
                                                xna.Vector3 GUP = new xna.Vector3();
                                                float dirGUP = DirfishToG(ballPt[7], LgoalUP);
                                                GUP.X = (float)(ballPt[7].X - 0.8 * r * Math.Cos(dirGUP));
                                                GUP.Z = (float)(ballPt[7].Z - 0.8 * r * Math.Sin(dirGUP));
                                                StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], GUP,
                                                    dirGUP, 5, 10, 80, 11, 8, 15, 100, true);
                                            }
                                            if (ballPt[7].Z < 560 && ballPt[7].Z > 0)
                                            {
                                                xna.Vector3 GDOWN = new xna.Vector3();
                                                float dirGDOWN = DirfishToG(ballPt[7], LgoalDOWN);
                                                GDOWN.X = (float)(ballPt[7].X - 0.8 * r * Math.Cos(dirGDOWN));
                                                GDOWN.Z = (float)(ballPt[7].Z - 0.8 * r * Math.Sin(dirGDOWN));
                                                StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], GDOWN,
                                                    dirGDOWN, 5, 10, 80, 11, 8, 15, 100, true);
                                            }
                                        }

                                        #endregion
                                    }
                                    else
                                    {
                                        #region //1号鱼顶8号球的方案
                                        //8号球在D区上部分
                                        if (ballPt[8].Z > -560 && ballPt[8].Z < 220)
                                        {
                                            if (ballPt[8].X < 948 && ballPt[8].X > 0)
                                            {
                                                xna.Vector3 UPA22 = new xna.Vector3();
                                                float dirUPA22 = DirBToG(ballPt[8], UPA2);
                                                UPA22.X = (float)(ballPt[8].X + 0.8 * r * Math.Cos(dirUPA22));
                                                UPA22.Z = (float)(ballPt[8].Z + 0.8 * r * Math.Sin(dirUPA22));
                                                StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], UPA22,
                                                    dirUPA22, 5, 10, 80, 14, 10, 10, 100, true);
                                            }

                                            if (ballPt[8].X < 0 && ballPt[8].X > -948)
                                            {

                                                xna.Vector3 UPA11 = new xna.Vector3();
                                                float dirUPA11 = DirBToG(ballPt[8], UPA1);
                                                UPA11.X = (float)(ballPt[8].X + 0.8 * r * Math.Cos(dirUPA11));
                                                UPA11.Z = (float)(ballPt[8].Z + 0.8 * r * Math.Sin(dirUPA11));
                                                StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], UPA11,
                                                     dirUPA11, 5, 10, 80, 14, 10, 10, 100, true);
                                            }
                                        }
                                        //8号球在D区下部分
                                        if (ballPt[8].Z > 220 && ballPt[8].Z < 560)
                                        {
                                            if (ballPt[8].X < 948 && ballPt[8].X > 0)
                                            {
                                                xna.Vector3 DOWNA22 = new xna.Vector3();
                                                float dirDOWNA22 = DirBToG(ballPt[8], DOWNA2);
                                                DOWNA22.X = (float)(ballPt[8].X + 0.8 * r * Math.Cos(dirDOWNA22));
                                                DOWNA22.Z = (float)(ballPt[8].Z + 0.8 * r * Math.Sin(dirDOWNA22));
                                                StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], DOWNA22,
                                                    dirDOWNA22, 5, 10, 80, 14, 10, 10, 100, true);
                                            }

                                            if (ballPt[8].X < 0 && ballPt[8].X > -948)
                                            {

                                                xna.Vector3 DOWNA11 = new xna.Vector3();
                                                float dirDOWNA11 = DirBToG(ballPt[8], DOWNA1);
                                                DOWNA11.X = (float)(ballPt[8].X + 0.8 * r * Math.Cos(dirDOWNA11));
                                                DOWNA11.Z = (float)(ballPt[8].Z + 0.8 * r * Math.Sin(dirDOWNA11));
                                                StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], DOWNA11,
                                                     dirDOWNA11, 5, 10, 80, 14, 10, 10, 100, true);
                                            }
                                        }
                                        //8号球在G2区
                                        if (ballPt[8].X > 1092 && ballPt[8].X < 1500)
                                        {
                                            if (ballPt[8].Z < 0 && ballPt[8].Z > -560)
                                            {
                                                xna.Vector3 GUP = new xna.Vector3();
                                                float dirGUP = DirfishToG(ballPt[8], UPB2);
                                                GUP.X = (float)(ballPt[8].X - 0.8 * r * Math.Cos(dirGUP));
                                                GUP.Z = (float)(ballPt[8].Z - 0.8 * r * Math.Sin(dirGUP));
                                                StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], GUP,
                                                    dirGUP, 5, 10, 80, 11, 8, 10, 100, true);
                                            }
                                            if (ballPt[8].Z < 560 && ballPt[8].Z > 0)
                                            {
                                                xna.Vector3 GDOWN = new xna.Vector3();
                                                float dirGDOWN = DirfishToG(ballPt[8], DOWNB2);
                                                GDOWN.X = (float)(ballPt[8].X - 0.8 * r * Math.Cos(dirGDOWN));
                                                GDOWN.Z = (float)(ballPt[8].Z - 0.8 * r * Math.Sin(dirGDOWN));
                                                StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], GDOWN,
                                                    dirGDOWN, 5, 10, 80, 11, 8, 10, 100, true);
                                            }
                                        }
                                        if (ballPt[8].Z > -1000 && ballPt[8].Z < -560)
                                        {
                                            //8号球在A1区
                                            if (ballPt[8].X < 1500 && ballPt[8].X > -1200)
                                            {
                                                xna.Vector3 UPB11 = new xna.Vector3();
                                                float dirUPB11 = DirBToG(ballPt[8], UPB1);
                                                UPB11.X = (float)(ballPt[8].X + 0.8 * r * Math.Cos(dirUPB11));
                                                UPB11.Z = (float)(ballPt[8].Z + 0.8 * r * Math.Sin(dirUPB11));
                                                StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], UPB11,
                                                    dirUPB11, 5, 10, 100, 10, 7, 10, 100, true);
                                            }
                                            //8号球在B1区
                                            if (ballPt[8].X < -1200 && ballPt[8].X > -1500)
                                            {
                                                xna.Vector3 G1UP1 = new xna.Vector3();
                                                float dirG1UP1 = DirBToG(ballPt[8], G1UP);
                                                G1UP1.X = (float)(ballPt[8].X + 0.8 * r * Math.Cos(dirG1UP1));
                                                G1UP1.Z = (float)(ballPt[8].Z + 0.8 * r * Math.Sin(dirG1UP1));
                                                StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], G1UP1,
                                                    dirG1UP1, 5, 10, 100, 10, 7, 10, 100, true);
                                            }
                                        }
                                        if (ballPt[8].Z > 560 && ballPt[8].Z < 1000)
                                        {
                                            //8号球在A2区
                                            if (ballPt[8].X < 1500 && ballPt[8].X > -1200)
                                            {
                                                xna.Vector3 DOWNB11 = new xna.Vector3();
                                                float dirDOWNB11 = DirBToG(ballPt[8], DOWNB1);
                                                DOWNB11.X = (float)(ballPt[8].X + 0.8 * r * Math.Cos(dirDOWNB11));
                                                DOWNB11.Z = (float)(ballPt[8].Z + 0.8 * r * Math.Sin(dirDOWNB11));
                                                StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], DOWNB11,
                                                    dirDOWNB11, 5, 10, 100, 10, 7, 10, 100, true);
                                            }
                                            //8号球在B2区
                                            if (ballPt[8].X < -1200 && ballPt[8].X > -1500)
                                            {
                                                xna.Vector3 G1DOWN1 = new xna.Vector3();
                                                float dirG1DOWN1 = DirBToG(ballPt[8], G1DOWN);
                                                G1DOWN1.X = (float)(ballPt[8].X + 0.8 * r * Math.Cos(dirG1DOWN1));
                                                G1DOWN1.Z = (float)(ballPt[8].Z + 0.8 * r * Math.Sin(dirG1DOWN1));
                                                StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], G1DOWN1,
                                                    dirG1DOWN1, 5, 10, 100, 10, 7, 10, 100, true);
                                            }
                                        }
                                        //8号球在G1区
                                        if (ballPt[8].X > -1500 && ballPt[8].X < -1092)
                                        {
                                            if (ballPt[8].Z < 0 && ballPt[8].Z > -560)
                                            {
                                                xna.Vector3 GUP = new xna.Vector3();
                                                float dirGUP = DirfishToG(ballPt[8], LgoalUP);
                                                GUP.X = (float)(ballPt[8].X - 0.8 * r * Math.Cos(dirGUP));
                                                GUP.Z = (float)(ballPt[8].Z - 0.8 * r * Math.Sin(dirGUP));
                                                StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], GUP,
                                                    dirGUP, 5, 10, 80, 11, 8, 15, 100, true);
                                            }
                                            if (ballPt[8].Z < 560 && ballPt[8].Z > 0)
                                            {
                                                xna.Vector3 GDOWN = new xna.Vector3();
                                                float dirGDOWN = DirfishToG(ballPt[8], LgoalDOWN);
                                                GDOWN.X = (float)(ballPt[8].X - 0.8 * r * Math.Cos(dirGDOWN));
                                                GDOWN.Z = (float)(ballPt[8].Z - 0.8 * r * Math.Sin(dirGDOWN));
                                                StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], GDOWN,
                                                    dirGDOWN, 5, 10, 80, 11, 8, 15, 100, true);
                                            }
                                        }

                                        #endregion
                                    }
                                }
                                if ((b7_l == 1 || b7_r == 1) && b8_r == 0 && b8_l == 0)
                                {
                                    #region //1号鱼顶8号球的方案
                                    //8号球在D区上部分
                                    if (ballPt[8].Z > -560 && ballPt[8].Z < 220)
                                    {
                                        if (ballPt[8].X < 948 && ballPt[8].X > 0)
                                        {
                                            xna.Vector3 UPA22 = new xna.Vector3();
                                            float dirUPA22 = DirBToG(ballPt[8], UPA2);
                                            UPA22.X = (float)(ballPt[8].X + 0.8 * r * Math.Cos(dirUPA22));
                                            UPA22.Z = (float)(ballPt[8].Z + 0.8 * r * Math.Sin(dirUPA22));
                                            StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], UPA22,
                                                dirUPA22, 5, 10, 80, 14, 10, 10, 100, true);
                                        }

                                        if (ballPt[8].X < 0 && ballPt[8].X > -948)
                                        {

                                            xna.Vector3 UPA11 = new xna.Vector3();
                                            float dirUPA11 = DirBToG(ballPt[8], UPA1);
                                            UPA11.X = (float)(ballPt[8].X + 0.8 * r * Math.Cos(dirUPA11));
                                            UPA11.Z = (float)(ballPt[8].Z + 0.8 * r * Math.Sin(dirUPA11));
                                            StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], UPA11,
                                                 dirUPA11, 5, 10, 80, 14, 10, 10, 100, true);
                                        }
                                    }
                                    //8号球在D区下部分
                                    if (ballPt[8].Z > 220 && ballPt[8].Z < 560)
                                    {
                                        if (ballPt[8].X < 948 && ballPt[8].X > 0)
                                        {
                                            xna.Vector3 DOWNA22 = new xna.Vector3();
                                            float dirDOWNA22 = DirBToG(ballPt[8], DOWNA2);
                                            DOWNA22.X = (float)(ballPt[8].X + 0.8 * r * Math.Cos(dirDOWNA22));
                                            DOWNA22.Z = (float)(ballPt[8].Z + 0.8 * r * Math.Sin(dirDOWNA22));
                                            StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], DOWNA22,
                                                dirDOWNA22, 5, 10, 80, 14, 10, 10, 100, true);
                                        }

                                        if (ballPt[8].X < 0 && ballPt[8].X > -948)
                                        {

                                            xna.Vector3 DOWNA11 = new xna.Vector3();
                                            float dirDOWNA11 = DirBToG(ballPt[8], DOWNA1);
                                            DOWNA11.X = (float)(ballPt[8].X + 0.8 * r * Math.Cos(dirDOWNA11));
                                            DOWNA11.Z = (float)(ballPt[8].Z + 0.8 * r * Math.Sin(dirDOWNA11));
                                            StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], DOWNA11,
                                                 dirDOWNA11, 5, 10, 80, 14, 10, 10, 100, true);
                                        }
                                    }
                                    //8号球在G2区
                                    if (ballPt[8].X > 1092 && ballPt[8].X < 1500)
                                    {
                                        if (ballPt[8].Z < 0 && ballPt[8].Z > -560)
                                        {
                                            xna.Vector3 GUP = new xna.Vector3();
                                            float dirGUP = DirfishToG(ballPt[8], UPB2);
                                            GUP.X = (float)(ballPt[8].X - 0.8 * r * Math.Cos(dirGUP));
                                            GUP.Z = (float)(ballPt[8].Z - 0.8 * r * Math.Sin(dirGUP));
                                            StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], GUP,
                                                dirGUP, 5, 10, 80, 11, 8, 10, 100, true);
                                        }
                                        if (ballPt[8].Z < 560 && ballPt[8].Z > 0)
                                        {
                                            xna.Vector3 GDOWN = new xna.Vector3();
                                            float dirGDOWN = DirfishToG(ballPt[8], DOWNB2);
                                            GDOWN.X = (float)(ballPt[8].X - 0.8 * r * Math.Cos(dirGDOWN));
                                            GDOWN.Z = (float)(ballPt[8].Z - 0.8 * r * Math.Sin(dirGDOWN));
                                            StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], GDOWN,
                                                dirGDOWN, 5, 10, 80, 11, 8, 10, 100, true);
                                        }
                                    }
                                    if (ballPt[8].Z > -1000 && ballPt[8].Z < -560)
                                    {
                                        //8号球在A1区
                                        if (ballPt[8].X < 1500 && ballPt[8].X > -1200)
                                        {
                                            xna.Vector3 UPB11 = new xna.Vector3();
                                            float dirUPB11 = DirBToG(ballPt[8], UPB1);
                                            UPB11.X = (float)(ballPt[8].X + 0.8 * r * Math.Cos(dirUPB11));
                                            UPB11.Z = (float)(ballPt[8].Z + 0.8 * r * Math.Sin(dirUPB11));
                                            StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], UPB11,
                                                dirUPB11, 5, 10, 100, 10, 7, 10, 100, true);
                                        }
                                        //8号球在B1区
                                        if (ballPt[8].X < -1200 && ballPt[8].X > -1500)
                                        {
                                            xna.Vector3 G1UP1 = new xna.Vector3();
                                            float dirG1UP1 = DirBToG(ballPt[8], G1UP);
                                            G1UP1.X = (float)(ballPt[8].X + 0.8 * r * Math.Cos(dirG1UP1));
                                            G1UP1.Z = (float)(ballPt[8].Z + 0.8 * r * Math.Sin(dirG1UP1));
                                            StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], G1UP1,
                                                dirG1UP1, 5, 10, 100, 10, 7, 10, 100, true);
                                        }
                                    }
                                    if (ballPt[8].Z > 560 && ballPt[8].Z < 1000)
                                    {
                                        //8号球在A2区
                                        if (ballPt[8].X < 1500 && ballPt[8].X > -1200)
                                        {
                                            xna.Vector3 DOWNB11 = new xna.Vector3();
                                            float dirDOWNB11 = DirBToG(ballPt[8], DOWNB1);
                                            DOWNB11.X = (float)(ballPt[8].X + 0.8 * r * Math.Cos(dirDOWNB11));
                                            DOWNB11.Z = (float)(ballPt[8].Z + 0.8 * r * Math.Sin(dirDOWNB11));
                                            StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], DOWNB11,
                                                dirDOWNB11, 5, 10, 100, 10, 7, 10, 100, true);
                                        }
                                        //8号球在B2区
                                        if (ballPt[8].X < -1200 && ballPt[8].X > -1500)
                                        {
                                            xna.Vector3 G1DOWN1 = new xna.Vector3();
                                            float dirG1DOWN1 = DirBToG(ballPt[8], G1DOWN);
                                            G1DOWN1.X = (float)(ballPt[8].X + 0.8 * r * Math.Cos(dirG1DOWN1));
                                            G1DOWN1.Z = (float)(ballPt[8].Z + 0.8 * r * Math.Sin(dirG1DOWN1));
                                            StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], G1DOWN1,
                                                dirG1DOWN1, 5, 10, 100, 10, 7, 10, 100, true);
                                        }
                                    }
                                    //8号球在G1区
                                    if (ballPt[8].X > -1500 && ballPt[8].X < -1092)
                                    {
                                        if (ballPt[8].Z < 0 && ballPt[8].Z > -560)
                                        {
                                            xna.Vector3 GUP = new xna.Vector3();
                                            float dirGUP = DirfishToG(ballPt[8], LgoalUP);
                                            GUP.X = (float)(ballPt[8].X - 0.8 * r * Math.Cos(dirGUP));
                                            GUP.Z = (float)(ballPt[8].Z - 0.8 * r * Math.Sin(dirGUP));
                                            StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], GUP,
                                                dirGUP, 5, 10, 80, 11, 8, 15, 100, true);
                                        }
                                        if (ballPt[8].Z < 560 && ballPt[8].Z > 0)
                                        {
                                            xna.Vector3 GDOWN = new xna.Vector3();
                                            float dirGDOWN = DirfishToG(ballPt[8], LgoalDOWN);
                                            GDOWN.X = (float)(ballPt[8].X - 0.8 * r * Math.Cos(dirGDOWN));
                                            GDOWN.Z = (float)(ballPt[8].Z - 0.8 * r * Math.Sin(dirGDOWN));
                                            StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], GDOWN,
                                                dirGDOWN, 5, 10, 80, 11, 8, 15, 100, true);
                                        }
                                    }

                                    #endregion
                                }
                                if (b7_l == 0 && b7_r == 0 && (b8_r == 1 || b8_l == 1))
                                {
                                    #region //1号鱼顶7号球的方案
                                    //7号球在D区上部分
                                    if (ballPt[7].Z > -560 && ballPt[7].Z < 220)
                                    {
                                        if (ballPt[7].X < 948 && ballPt[7].X > 0)
                                        {
                                            xna.Vector3 UPA22 = new xna.Vector3();
                                            float dirUPA22 = DirBToG(ballPt[7], UPA2);
                                            UPA22.X = (float)(ballPt[7].X + 0.8 * r * Math.Cos(dirUPA22));
                                            UPA22.Z = (float)(ballPt[7].Z + 0.8 * r * Math.Sin(dirUPA22));
                                            StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], UPA22,
                                                dirUPA22, 5, 10, 80, 14, 10, 10, 100, true);
                                        }

                                        if (ballPt[7].X < 0 && ballPt[7].X > -948)
                                        {

                                            xna.Vector3 UPA11 = new xna.Vector3();
                                            float dirUPA11 = DirBToG(ballPt[7], UPA1);
                                            UPA11.X = (float)(ballPt[7].X + 0.8 * r * Math.Cos(dirUPA11));
                                            UPA11.Z = (float)(ballPt[7].Z + 0.8 * r * Math.Sin(dirUPA11));
                                            StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], UPA11,
                                                 dirUPA11, 5, 10, 80, 14, 10, 10, 100, true);
                                        }
                                    }
                                    //7号球在D区下部分
                                    if (ballPt[7].Z > 220 && ballPt[7].Z < 560)
                                    {
                                        if (ballPt[7].X < 948 && ballPt[7].X > 0)
                                        {
                                            xna.Vector3 DOWNA22 = new xna.Vector3();
                                            float dirDOWNA22 = DirBToG(ballPt[7], DOWNA2);
                                            DOWNA22.X = (float)(ballPt[7].X + 0.8 * r * Math.Cos(dirDOWNA22));
                                            DOWNA22.Z = (float)(ballPt[7].Z + 0.8 * r * Math.Sin(dirDOWNA22));
                                            StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], DOWNA22,
                                                dirDOWNA22, 5, 10, 80, 14, 10, 10, 100, true);
                                        }

                                        if (ballPt[7].X < 0 && ballPt[7].X > -948)
                                        {

                                            xna.Vector3 DOWNA11 = new xna.Vector3();
                                            float dirDOWNA11 = DirBToG(ballPt[7], DOWNA1);
                                            DOWNA11.X = (float)(ballPt[7].X + 0.8 * r * Math.Cos(dirDOWNA11));
                                            DOWNA11.Z = (float)(ballPt[7].Z + 0.8 * r * Math.Sin(dirDOWNA11));
                                            StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], DOWNA11,
                                                 dirDOWNA11, 5, 10, 80, 14, 10, 10, 100, true);
                                        }
                                    }
                                    //7号球在G2区
                                    if (ballPt[7].X > 1092 && ballPt[7].X < 1500)
                                    {
                                        if (ballPt[7].Z < 0 && ballPt[7].Z > -560)
                                        {
                                            xna.Vector3 GUP = new xna.Vector3();
                                            float dirGUP = DirfishToG(ballPt[7], UPB2);
                                            GUP.X = (float)(ballPt[7].X - 0.8 * r * Math.Cos(dirGUP));
                                            GUP.Z = (float)(ballPt[7].Z - 0.8 * r * Math.Sin(dirGUP));
                                            StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], GUP,
                                                dirGUP, 5, 10, 80, 11, 8, 10, 100, true);
                                        }
                                        if (ballPt[7].Z < 560 && ballPt[7].Z > 0)
                                        {
                                            xna.Vector3 GDOWN = new xna.Vector3();
                                            float dirGDOWN = DirfishToG(ballPt[7], DOWNB2);
                                            GDOWN.X = (float)(ballPt[7].X - 0.8 * r * Math.Cos(dirGDOWN));
                                            GDOWN.Z = (float)(ballPt[7].Z - 0.8 * r * Math.Sin(dirGDOWN));
                                            StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], GDOWN,
                                                dirGDOWN, 5, 10, 80, 11, 8, 10, 100, true);
                                        }
                                    }
                                    if (ballPt[7].Z > -1000 && ballPt[7].Z < -560)
                                    {
                                        //7号球在A1区
                                        if (ballPt[7].X < 1500 && ballPt[7].X > -1200)
                                        {
                                            xna.Vector3 UPB11 = new xna.Vector3();
                                            float dirUPB11 = DirBToG(ballPt[7], UPB1);
                                            UPB11.X = (float)(ballPt[7].X + 0.8 * r * Math.Cos(dirUPB11));
                                            UPB11.Z = (float)(ballPt[7].Z + 0.8 * r * Math.Sin(dirUPB11));
                                            StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], UPB11,
                                                dirUPB11, 5, 10, 100, 10, 7, 10, 100, true);
                                        }
                                        //7号球在B1区
                                        if (ballPt[7].X < -1200 && ballPt[7].X > -1500)
                                        {
                                            xna.Vector3 G1UP1 = new xna.Vector3();
                                            float dirG1UP1 = DirBToG(ballPt[7], G1UP);
                                            G1UP1.X = (float)(ballPt[7].X + 0.8 * r * Math.Cos(dirG1UP1));
                                            G1UP1.Z = (float)(ballPt[7].Z + 0.8 * r * Math.Sin(dirG1UP1));
                                            StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], G1UP1,
                                                dirG1UP1, 5, 10, 100, 10, 7, 10, 100, true);
                                        }
                                    }
                                    if (ballPt[7].Z > 560 && ballPt[7].Z < 1000)
                                    {
                                        //7号球在A2区
                                        if (ballPt[7].X < 1500 && ballPt[7].X > -1200)
                                        {
                                            xna.Vector3 DOWNB11 = new xna.Vector3();
                                            float dirDOWNB11 = DirBToG(ballPt[7], DOWNB1);
                                            DOWNB11.X = (float)(ballPt[7].X + 0.8 * r * Math.Cos(dirDOWNB11));
                                            DOWNB11.Z = (float)(ballPt[7].Z + 0.8 * r * Math.Sin(dirDOWNB11));
                                            StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], DOWNB11,
                                                dirDOWNB11, 5, 10, 100, 10, 7, 10, 100, true);
                                        }
                                        //7号球在B2区
                                        if (ballPt[7].X < -1200 && ballPt[7].X > -1500)
                                        {
                                            xna.Vector3 G1DOWN1 = new xna.Vector3();
                                            float dirG1DOWN1 = DirBToG(ballPt[7], G1DOWN);
                                            G1DOWN1.X = (float)(ballPt[7].X + 0.8 * r * Math.Cos(dirG1DOWN1));
                                            G1DOWN1.Z = (float)(ballPt[7].Z + 0.8 * r * Math.Sin(dirG1DOWN1));
                                            StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], G1DOWN1,
                                                dirG1DOWN1, 5, 10, 100, 10, 7, 10, 100, true);
                                        }
                                    }
                                    //7号球在G1区
                                    if (ballPt[7].X > -1500 && ballPt[7].X < -1092)
                                    {
                                        if (ballPt[7].Z < 0 && ballPt[7].Z > -560)
                                        {
                                            xna.Vector3 GUP = new xna.Vector3();
                                            float dirGUP = DirfishToG(ballPt[7], LgoalUP);
                                            GUP.X = (float)(ballPt[7].X - 0.8 * r * Math.Cos(dirGUP));
                                            GUP.Z = (float)(ballPt[7].Z - 0.8 * r * Math.Sin(dirGUP));
                                            StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], GUP,
                                                dirGUP, 5, 10, 80, 11, 8, 15, 100, true);
                                        }
                                        if (ballPt[7].Z < 560 && ballPt[7].Z > 0)
                                        {
                                            xna.Vector3 GDOWN = new xna.Vector3();
                                            float dirGDOWN = DirfishToG(ballPt[7], LgoalDOWN);
                                            GDOWN.X = (float)(ballPt[7].X - 0.8 * r * Math.Cos(dirGDOWN));
                                            GDOWN.Z = (float)(ballPt[7].Z - 0.8 * r * Math.Sin(dirGDOWN));
                                            StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], GDOWN,
                                                dirGDOWN, 5, 10, 80, 11, 8, 15, 100, true);
                                        }
                                    }

                                    #endregion
                                }
                                if ((b7_l == 1 || b7_r == 1) && (b8_l == 1 || b8_r == 1) && b1_r == 0 && b1_l == 0)
                                {
                                    if (fishPt[1].X < -1000 && fishPt[1].X > -1500 && fishPt[1].Z < 560 && fishPt[1].Z > -560
                                && ballPt[7].X > -1000 && ballPt[8].X > -1000 && ballPt[1].X > -1000)
                                    {
                                        if (D1 > -Math.PI && D1 < 0)
                                        {
                                            float dir1 = DirfishToG(fishPt[1], UPB1);
                                            StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], UPB1,
                                                  dir1, 5, 10, 100, 14, 14, 8, 100, true);
                                        }
                                        else
                                        {
                                            float dir2 = DirfishToG(fishPt[1], DOWNB1);
                                            StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], UPB1,
                                                  dir2, 5, 10, 100, 14, 14, 8, 100, true);
                                        }
                                    }
                                    else
                                    {
                                        #region //1号鱼顶1号球的方案
                                        //1号球在D区上部分
                                        if (ballPt[1].Z > -560 && ballPt[1].Z < 220)
                                        {
                                            if (ballPt[1].X < 948 && ballPt[1].X > 0)
                                            {
                                                xna.Vector3 UPA22 = new xna.Vector3();
                                                float dirUPA22 = DirBToG(ballPt[1], UPA2);
                                                UPA22.X = (float)(ballPt[1].X + 0.8 * r * Math.Cos(dirUPA22));
                                                UPA22.Z = (float)(ballPt[1].Z + 0.8 * r * Math.Sin(dirUPA22));
                                                StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], UPA22,
                                                    dirUPA22, 5, 10, 80, 14, 10, 10, 100, true);
                                            }

                                            if (ballPt[1].X < 0 && ballPt[1].X > -948)
                                            {

                                                xna.Vector3 UPA11 = new xna.Vector3();
                                                float dirUPA11 = DirBToG(ballPt[1], UPA1);
                                                UPA11.X = (float)(ballPt[1].X + 0.8 * r * Math.Cos(dirUPA11));
                                                UPA11.Z = (float)(ballPt[1].Z + 0.8 * r * Math.Sin(dirUPA11));
                                                StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], UPA11,
                                                     dirUPA11, 5, 10, 80, 14, 10, 10, 100, true);
                                            }
                                        }
                                        //1号球在D区下部分
                                        if (ballPt[1].Z > 220 && ballPt[1].Z < 560)
                                        {
                                            if (ballPt[1].X < 948 && ballPt[1].X > 0)
                                            {
                                                xna.Vector3 DOWNA22 = new xna.Vector3();
                                                float dirDOWNA22 = DirBToG(ballPt[1], DOWNA2);
                                                DOWNA22.X = (float)(ballPt[1].X + 0.8 * r * Math.Cos(dirDOWNA22));
                                                DOWNA22.Z = (float)(ballPt[1].Z + 0.8 * r * Math.Sin(dirDOWNA22));
                                                StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], DOWNA22,
                                                    dirDOWNA22, 5, 10, 80, 14, 10, 10, 100, true);
                                            }

                                            if (ballPt[1].X < 0 && ballPt[1].X > -948)
                                            {

                                                xna.Vector3 DOWNA11 = new xna.Vector3();
                                                float dirDOWNA11 = DirBToG(ballPt[1], DOWNA1);
                                                DOWNA11.X = (float)(ballPt[1].X + 0.8 * r * Math.Cos(dirDOWNA11));
                                                DOWNA11.Z = (float)(ballPt[1].Z + 0.8 * r * Math.Sin(dirDOWNA11));
                                                StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], DOWNA11,
                                                     dirDOWNA11, 5, 10, 80, 14, 10, 10, 100, true);
                                            }
                                        }
                                        //7号球在G2区
                                        if (ballPt[1].X > 1092 && ballPt[1].X < 1500)
                                        {
                                            if (ballPt[1].Z < 0 && ballPt[1].Z > -560)
                                            {
                                                xna.Vector3 GUP = new xna.Vector3();
                                                float dirGUP = DirfishToG(ballPt[1], UPB2);
                                                GUP.X = (float)(ballPt[1].X - 0.8 * r * Math.Cos(dirGUP));
                                                GUP.Z = (float)(ballPt[1].Z - 0.8 * r * Math.Sin(dirGUP));
                                                StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], GUP,
                                                    dirGUP, 5, 10, 80, 11, 8, 10, 100, true);
                                            }
                                            if (ballPt[1].Z < 560 && ballPt[1].Z > 0)
                                            {
                                                xna.Vector3 GDOWN = new xna.Vector3();
                                                float dirGDOWN = DirfishToG(ballPt[1], DOWNB2);
                                                GDOWN.X = (float)(ballPt[1].X - 0.8 * r * Math.Cos(dirGDOWN));
                                                GDOWN.Z = (float)(ballPt[1].Z - 0.8 * r * Math.Sin(dirGDOWN));
                                                StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], GDOWN,
                                                    dirGDOWN, 5, 10, 80, 11, 8, 10, 100, true);
                                            }
                                        }
                                        if (ballPt[1].Z > -1000 && ballPt[1].Z < -560)
                                        {
                                            //7号球在A1区
                                            if (ballPt[1].X < 1500 && ballPt[1].X > -1200)
                                            {
                                                xna.Vector3 UPB11 = new xna.Vector3();
                                                float dirUPB11 = DirBToG(ballPt[1], UPB1);
                                                UPB11.X = (float)(ballPt[1].X + 0.8 * r * Math.Cos(dirUPB11));
                                                UPB11.Z = (float)(ballPt[1].Z + 0.8 * r * Math.Sin(dirUPB11));
                                                StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], UPB11,
                                                    dirUPB11, 5, 10, 100, 10, 7, 10, 100, true);
                                            }
                                            //7号球在B1区
                                            if (ballPt[1].X < -1200 && ballPt[1].X > -1500)
                                            {
                                                xna.Vector3 G1UP1 = new xna.Vector3();
                                                float dirG1UP1 = DirBToG(ballPt[1], G1UP);
                                                G1UP1.X = (float)(ballPt[1].X + 0.8 * r * Math.Cos(dirG1UP1));
                                                G1UP1.Z = (float)(ballPt[1].Z + 0.8 * r * Math.Sin(dirG1UP1));
                                                StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], G1UP1,
                                                    dirG1UP1, 5, 10, 100, 10, 7, 10, 100, true);
                                            }
                                        }
                                        if (ballPt[1].Z > 560 && ballPt[1].Z < 1000)
                                        {
                                            //7号球在A2区
                                            if (ballPt[1].X < 1500 && ballPt[1].X > -1200)
                                            {
                                                xna.Vector3 DOWNB11 = new xna.Vector3();
                                                float dirDOWNB11 = DirBToG(ballPt[1], DOWNB1);
                                                DOWNB11.X = (float)(ballPt[1].X + 0.8 * r * Math.Cos(dirDOWNB11));
                                                DOWNB11.Z = (float)(ballPt[1].Z + 0.8 * r * Math.Sin(dirDOWNB11));
                                                StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], DOWNB11,
                                                    dirDOWNB11, 5, 10, 100, 10, 7, 10, 100, true);
                                            }
                                            //7号球在B2区
                                            if (ballPt[1].X < -1200 && ballPt[1].X > -1500)
                                            {
                                                xna.Vector3 G1DOWN1 = new xna.Vector3();
                                                float dirG1DOWN1 = DirBToG(ballPt[1], G1DOWN);
                                                G1DOWN1.X = (float)(ballPt[1].X + 0.8 * r * Math.Cos(dirG1DOWN1));
                                                G1DOWN1.Z = (float)(ballPt[1].Z + 0.8 * r * Math.Sin(dirG1DOWN1));
                                                StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], G1DOWN1,
                                                    dirG1DOWN1, 5, 10, 100, 10, 7, 10, 100, true);
                                            }
                                        }
                                        //7号球在G1区
                                        if (ballPt[1].X > -1500 && ballPt[1].X < -1092)
                                        {
                                            if (ballPt[1].Z < 0 && ballPt[1].Z > -560)
                                            {
                                                xna.Vector3 GUP = new xna.Vector3();
                                                float dirGUP = DirfishToG(ballPt[1], LgoalUP);
                                                GUP.X = (float)(ballPt[1].X - 0.8 * r * Math.Cos(dirGUP));
                                                GUP.Z = (float)(ballPt[1].Z - 0.8 * r * Math.Sin(dirGUP));
                                                StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], GUP,
                                                    dirGUP, 5, 10, 80, 11, 8, 15, 100, true);
                                            }
                                            if (ballPt[1].Z < 560 && ballPt[1].Z > 0)
                                            {
                                                xna.Vector3 GDOWN = new xna.Vector3();
                                                float dirGDOWN = DirfishToG(ballPt[1], LgoalDOWN);
                                                GDOWN.X = (float)(ballPt[1].X - 0.8 * r * Math.Cos(dirGDOWN));
                                                GDOWN.Z = (float)(ballPt[1].Z - 0.8 * r * Math.Sin(dirGDOWN));
                                                StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], GDOWN,
                                                    dirGDOWN, 5, 10, 80, 11, 8, 15, 100, true);
                                            }
                                        }

                                        #endregion

                                    }
                                }
                            }
                        }
                        #endregion
                        #region 0号鱼顶球策略
                        if (b2_l == 0)
                        {
                            #region //0号鱼顶2号球的方案
                            //0号球在D区上部分
                            if (ballPt[2].Z > -560 && ballPt[2].Z < 220)
                            {
                                if (ballPt[2].X < 948 && ballPt[2].X > 0)
                                {
                                    xna.Vector3 UPA22 = new xna.Vector3();
                                    float dirUPA22 = DirBToG(ballPt[2], UPA2);
                                    UPA22.X = (float)(ballPt[2].X + 0.8 * r * Math.Cos(dirUPA22));
                                    UPA22.Z = (float)(ballPt[2].Z + 0.8 * r * Math.Sin(dirUPA22));
                                    StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], UPA22,
                                        dirUPA22, 5, 10, 80, 14, 10, 15, 100, true);
                                }

                                if (ballPt[2].X < 0 && ballPt[2].X > -948)
                                {

                                    xna.Vector3 UPA11 = new xna.Vector3();
                                    float dirUPA11 = DirBToG(ballPt[2], UPA1);
                                    UPA11.X = (float)(ballPt[2].X + 0.8 * r * Math.Cos(dirUPA11));
                                    UPA11.Z = (float)(ballPt[2].Z + 0.8 * r * Math.Sin(dirUPA11));
                                    StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], UPA11,
                                         dirUPA11, 5, 10, 80, 14, 10, 15, 100, true);
                                }
                            }
                            //0号球在D区下部分
                            if (ballPt[2].Z > 220 && ballPt[2].Z < 560)
                            {
                                if (ballPt[2].X < 948 && ballPt[2].X > 0)
                                {
                                    xna.Vector3 DOWNA22 = new xna.Vector3();
                                    float dirDOWNA22 = DirBToG(ballPt[2], DOWNA2);
                                    DOWNA22.X = (float)(ballPt[2].X + 0.8 * r * Math.Cos(dirDOWNA22));
                                    DOWNA22.Z = (float)(ballPt[2].Z + 0.8 * r * Math.Sin(dirDOWNA22));
                                    StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], DOWNA22,
                                        dirDOWNA22, 5, 10, 80, 14, 10, 15, 100, true);
                                }

                                if (ballPt[2].X < 0 && ballPt[2].X > -948)
                                {

                                    xna.Vector3 DOWNA11 = new xna.Vector3();
                                    float dirDOWNA11 = DirBToG(ballPt[2], DOWNA1);
                                    DOWNA11.X = (float)(ballPt[2].X + 0.8 * r * Math.Cos(dirDOWNA11));
                                    DOWNA11.Z = (float)(ballPt[2].Z + 0.8 * r * Math.Sin(dirDOWNA11));
                                    StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], DOWNA11,
                                         dirDOWNA11, 5, 10, 80, 14, 10, 15, 100, true);
                                }
                            }
                            //0号球在G2区
                            if (ballPt[2].X > 1092 && ballPt[2].X < 1500)
                            {
                                if (ballPt[2].Z < 0 && ballPt[2].Z > -560)
                                {
                                    xna.Vector3 GUP = new xna.Vector3();
                                    float dirGUP = DirfishToG(ballPt[2], UPB2);
                                    GUP.X = (float)(ballPt[2].X - 0.8 * r * Math.Cos(dirGUP));
                                    GUP.Z = (float)(ballPt[2].Z - 0.8 * r * Math.Sin(dirGUP));
                                    StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], GUP,
                                        dirGUP, 5, 10, 80, 11, 8, 15, 100, true);
                                }
                                if (ballPt[2].Z < 560 && ballPt[2].Z > 0)
                                {
                                    xna.Vector3 GDOWN = new xna.Vector3();
                                    float dirGDOWN = DirfishToG(ballPt[2], DOWNB2);
                                    GDOWN.X = (float)(ballPt[2].X + 0.8 * r * Math.Cos(dirGDOWN));
                                    GDOWN.Z = (float)(ballPt[2].Z + 0.8 * r * Math.Sin(dirGDOWN));
                                    StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], GDOWN,
                                        dirGDOWN, 5, 10, 80, 11, 8, 15, 100, true);
                                }
                            }
                            if (ballPt[2].Z > -1000 && ballPt[2].Z < -560)
                            {
                                //0号球在A1区
                                if (ballPt[2].X < 1500 && ballPt[2].X > -1200)
                                {
                                    xna.Vector3 UPB11 = new xna.Vector3();
                                    float dirUPB11 = DirBToG(ballPt[2], UPB1);
                                    UPB11.X = (float)(ballPt[2].X + 0.8 * r * Math.Cos(dirUPB11));
                                    UPB11.Z = (float)(ballPt[2].Z + 0.8 * r * Math.Sin(dirUPB11));
                                    StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], UPB11,
                                        dirUPB11, 5, 10, 100, 10, 7, 15, 100, true);
                                }
                                //0号球在B1区
                                if (ballPt[2].X < -1200 && ballPt[2].X > -1500)
                                {
                                    xna.Vector3 G1UP1 = new xna.Vector3();
                                    float dirG1UP1 = DirBToG(ballPt[2], G1UP);
                                    G1UP1.X = (float)(ballPt[2].X - 0.8 * r * Math.Cos(dirG1UP1));
                                    G1UP1.Z = (float)(ballPt[2].Z - 0.8 * r * Math.Sin(dirG1UP1));
                                    StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], G1UP1,
                                        dirG1UP1, 5, 10, 100, 10, 7, 15, 100, true);
                                }
                            }
                            if (ballPt[2].Z > 560 && ballPt[2].Z < 1000)
                            {
                                //0号球在A2区
                                if (ballPt[2].X < 1500 && ballPt[2].X > -1200)
                                {
                                    xna.Vector3 DOWNB11 = new xna.Vector3();
                                    float dirDOWNB11 = DirBToG(ballPt[2], DOWNB1);
                                    DOWNB11.X = (float)(ballPt[2].X + 0.8 * r * Math.Cos(dirDOWNB11));
                                    DOWNB11.Z = (float)(ballPt[2].Z + 0.8 * r * Math.Sin(dirDOWNB11));
                                    StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], DOWNB11,
                                        dirDOWNB11, 5, 10, 100, 10, 7, 15, 100, true);
                                }
                                //0号球在B2区
                                if (ballPt[2].X < -1200 && ballPt[2].X > -1500)
                                {
                                    xna.Vector3 G1DOWN1 = new xna.Vector3();
                                    float dirG1DOWN1 = DirBToG(ballPt[2], G1DOWN);
                                    G1DOWN1.X = (float)(ballPt[2].X + 0.8 * r * Math.Cos(dirG1DOWN1));
                                    G1DOWN1.Z = (float)(ballPt[2].Z + 0.8 * r * Math.Sin(dirG1DOWN1));
                                    StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], G1DOWN1,
                                        dirG1DOWN1, 5, 10, 100, 10, 7, 15, 100, true);
                                }
                            }
                            //0号球在G1区
                            if (ballPt[2].X > -1500 && ballPt[2].X < -1092)
                            {
                                if (ballPt[2].Z < 0 && ballPt[2].Z > -560)
                                {
                                    xna.Vector3 GUP = new xna.Vector3();
                                    float dirGUP = DirfishToG(ballPt[2], LgoalUP);
                                    GUP.X = (float)(ballPt[2].X - 0.8 * r * Math.Cos(dirGUP));
                                    GUP.Z = (float)(ballPt[2].Z - 0.8 * r * Math.Sin(dirGUP));
                                    StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], GUP,
                                        dirGUP, 5, 10, 80, 11, 8, 15, 100, true);
                                }
                                if (ballPt[2].Z < 560 && ballPt[2].Z > 0)
                                {
                                    xna.Vector3 GDOWN = new xna.Vector3();
                                    float dirGDOWN = DirfishToG(ballPt[2], LgoalDOWN);
                                    GDOWN.X = (float)(ballPt[2].X - 0.8 * r * Math.Cos(dirGDOWN));
                                    GDOWN.Z = (float)(ballPt[2].Z - 0.8 * r * Math.Sin(dirGDOWN));
                                    StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], GDOWN,
                                        dirGDOWN, 5, 10, 80, 11, 8, 15, 100, true);
                                }
                            }

                            #endregion
                        }
                        #region
                        if (b2_l == 1)
                        {

                            if (fishPt[0].X < -1000 && fishPt[0].X > -1500 && fishPt[0].Z < 560 && fishPt[0].Z > -560
                                && ballPt[7].X > -1000 && ballPt[8].X > -1000)
                            {
                                if (D0 > -Math.PI && D0 < 0)
                                {
                                    float dir1 = DirfishToG(fishPt[0], UPB1);
                                    StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], UPB1,
                                          dir1, 5, 10, 100, 14, 14, 8, 100, true);
                                }
                                else
                                {
                                    float dir2 = DirfishToG(fishPt[0], DOWNB1);
                                    StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], UPB1,
                                          dir2, 5, 10, 100, 14, 14, 8, 100, true);
                                }
                            }
                            else
                            {
                                if (b7_l == 0 && b7_r == 0 && b8_r == 0 && b8_l == 0)
                                {
                                    if (dis7_fish0 < dis8_fish0)
                                    {
                                        #region //0号鱼顶7号球的方案
                                        //7号球在D区上部分
                                        if (ballPt[7].Z > -560 && ballPt[7].Z < 220)
                                        {
                                            if (ballPt[7].X < 948 && ballPt[7].X > 0)
                                            {
                                                xna.Vector3 UPA22 = new xna.Vector3();
                                                float dirUPA22 = DirBToG(ballPt[7], UPA2);
                                                UPA22.X = (float)(ballPt[7].X + 0.8 * r * Math.Cos(dirUPA22));
                                                UPA22.Z = (float)(ballPt[7].Z + 0.8 * r * Math.Sin(dirUPA22));
                                                StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], UPA22,
                                                    dirUPA22, 5, 10, 80, 14, 10, 10, 100, true);
                                            }

                                            if (ballPt[7].X < 0 && ballPt[7].X > -948)
                                            {

                                                xna.Vector3 UPA11 = new xna.Vector3();
                                                float dirUPA11 = DirBToG(ballPt[7], UPA1);
                                                UPA11.X = (float)(ballPt[7].X + 0.8 * r * Math.Cos(dirUPA11));
                                                UPA11.Z = (float)(ballPt[7].Z + 0.8 * r * Math.Sin(dirUPA11));
                                                StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], UPA11,
                                                     dirUPA11, 5, 10, 80, 14, 10, 10, 100, true);
                                            }
                                        }
                                        //7号球在D区下部分
                                        if (ballPt[7].Z > 220 && ballPt[7].Z < 560)
                                        {
                                            if (ballPt[7].X < 948 && ballPt[7].X > 0)
                                            {
                                                xna.Vector3 DOWNA22 = new xna.Vector3();
                                                float dirDOWNA22 = DirBToG(ballPt[7], DOWNA2);
                                                DOWNA22.X = (float)(ballPt[7].X + 0.8 * r * Math.Cos(dirDOWNA22));
                                                DOWNA22.Z = (float)(ballPt[7].Z + 0.8 * r * Math.Sin(dirDOWNA22));
                                                StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], DOWNA22,
                                                    dirDOWNA22, 5, 10, 80, 14, 10, 10, 100, true);
                                            }

                                            if (ballPt[7].X < 0 && ballPt[7].X > -948)
                                            {

                                                xna.Vector3 DOWNA11 = new xna.Vector3();
                                                float dirDOWNA11 = DirBToG(ballPt[7], DOWNA1);
                                                DOWNA11.X = (float)(ballPt[7].X + 0.8 * r * Math.Cos(dirDOWNA11));
                                                DOWNA11.Z = (float)(ballPt[7].Z + 0.8 * r * Math.Sin(dirDOWNA11));
                                                StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], DOWNA11,
                                                     dirDOWNA11, 5, 10, 80, 14, 10, 10, 100, true);
                                            }
                                        }
                                        //7号球在G2区
                                        if (ballPt[7].X > 1092 && ballPt[7].X < 1500)
                                        {
                                            if (ballPt[7].Z < 0 && ballPt[7].Z > -560)
                                            {
                                                xna.Vector3 GUP = new xna.Vector3();
                                                float dirGUP = DirfishToG(ballPt[7], UPB2);
                                                GUP.X = (float)(ballPt[7].X - 0.8 * r * Math.Cos(dirGUP));
                                                GUP.Z = (float)(ballPt[7].Z - 0.8 * r * Math.Sin(dirGUP));
                                                StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], GUP,
                                                    dirGUP, 5, 10, 80, 11, 8, 10, 100, true);
                                            }
                                            if (ballPt[7].Z < 560 && ballPt[7].Z > 0)
                                            {
                                                xna.Vector3 GDOWN = new xna.Vector3();
                                                float dirGDOWN = DirfishToG(ballPt[7], DOWNB2);
                                                GDOWN.X = (float)(ballPt[7].X - 0.8 * r * Math.Cos(dirGDOWN));
                                                GDOWN.Z = (float)(ballPt[7].Z - 0.8 * r * Math.Sin(dirGDOWN));
                                                StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], GDOWN,
                                                    dirGDOWN, 5, 10, 80, 11, 8, 10, 100, true);
                                            }
                                        }
                                        if (ballPt[7].Z > -1000 && ballPt[7].Z < -560)
                                        {
                                            //7号球在A1区
                                            if (ballPt[7].X < 1500 && ballPt[7].X > -1200)
                                            {
                                                xna.Vector3 UPB11 = new xna.Vector3();
                                                float dirUPB11 = DirBToG(ballPt[7], UPB1);
                                                UPB11.X = (float)(ballPt[7].X + 0.8 * r * Math.Cos(dirUPB11));
                                                UPB11.Z = (float)(ballPt[7].Z + 0.8 * r * Math.Sin(dirUPB11));
                                                StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], UPB11,
                                                    dirUPB11, 5, 10, 100, 10, 7, 10, 100, true);
                                            }
                                            //7号球在B1区
                                            if (ballPt[7].X < -1200 && ballPt[7].X > -1500)
                                            {
                                                xna.Vector3 G1UP1 = new xna.Vector3();
                                                float dirG1UP1 = DirBToG(ballPt[7], G1UP);
                                                G1UP1.X = (float)(ballPt[7].X + 0.8 * r * Math.Cos(dirG1UP1));
                                                G1UP1.Z = (float)(ballPt[7].Z + 0.8 * r * Math.Sin(dirG1UP1));
                                                StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], G1UP1,
                                                    dirG1UP1, 5, 10, 100, 10, 7, 10, 100, true);
                                            }
                                        }
                                        if (ballPt[7].Z > 560 && ballPt[7].Z < 1000)
                                        {
                                            //7号球在A2区
                                            if (ballPt[0].X < 1500 && ballPt[0].X > -1200)
                                            {
                                                xna.Vector3 DOWNB11 = new xna.Vector3();
                                                float dirDOWNB11 = DirBToG(ballPt[7], DOWNB1);
                                                DOWNB11.X = (float)(ballPt[7].X + 0.8 * r * Math.Cos(dirDOWNB11));
                                                DOWNB11.Z = (float)(ballPt[7].Z + 0.8 * r * Math.Sin(dirDOWNB11));
                                                StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], DOWNB11,
                                                    dirDOWNB11, 5, 10, 100, 10, 7, 10, 100, true);
                                            }
                                            //7号球在B2区
                                            if (ballPt[7].X < -1200 && ballPt[7].X > -1500)
                                            {
                                                xna.Vector3 G1DOWN1 = new xna.Vector3();
                                                float dirG1DOWN1 = DirBToG(ballPt[7], G1DOWN);
                                                G1DOWN1.X = (float)(ballPt[7].X + 0.8 * r * Math.Cos(dirG1DOWN1));
                                                G1DOWN1.Z = (float)(ballPt[7].Z + 0.8 * r * Math.Sin(dirG1DOWN1));
                                                StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], G1DOWN1,
                                                    dirG1DOWN1, 5, 10, 100, 10, 7, 10, 100, true);
                                            }
                                        }
                                        //7号球在G1区
                                        if (ballPt[7].X > -1500 && ballPt[7].X < -1092)
                                        {
                                            if (ballPt[7].Z < 0 && ballPt[7].Z > -560)
                                            {
                                                xna.Vector3 GUP = new xna.Vector3();
                                                float dirGUP = DirfishToG(ballPt[7], LgoalUP);
                                                GUP.X = (float)(ballPt[7].X - 0.8 * r * Math.Cos(dirGUP));
                                                GUP.Z = (float)(ballPt[7].Z - 0.8 * r * Math.Sin(dirGUP));
                                                StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], GUP,
                                                    dirGUP, 5, 10, 80, 11, 8, 15, 100, true);
                                            }
                                            if (ballPt[7].Z < 560 && ballPt[7].Z > 0)
                                            {
                                                xna.Vector3 GDOWN = new xna.Vector3();
                                                float dirGDOWN = DirfishToG(ballPt[7], LgoalDOWN);
                                                GDOWN.X = (float)(ballPt[7].X - 0.8 * r * Math.Cos(dirGDOWN));
                                                GDOWN.Z = (float)(ballPt[7].Z - 0.8 * r * Math.Sin(dirGDOWN));
                                                StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], GDOWN,
                                                    dirGDOWN, 5, 10, 80, 11, 8, 15, 100, true);
                                            }
                                        }

                                        #endregion
                                    }
                                    else
                                    {
                                        #region //0号鱼顶8号球的方案
                                        //8号球在D区上部分
                                        if (ballPt[8].Z > -560 && ballPt[8].Z < 220)
                                        {
                                            if (ballPt[8].X < 948 && ballPt[8].X > 0)
                                            {
                                                xna.Vector3 UPA22 = new xna.Vector3();
                                                float dirUPA22 = DirBToG(ballPt[8], UPA2);
                                                UPA22.X = (float)(ballPt[8].X + 0.8 * r * Math.Cos(dirUPA22));
                                                UPA22.Z = (float)(ballPt[8].Z + 0.8 * r * Math.Sin(dirUPA22));
                                                StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], UPA22,
                                                    dirUPA22, 5, 10, 80, 14, 10, 10, 100, true);
                                            }

                                            if (ballPt[8].X < 0 && ballPt[8].X > -948)
                                            {

                                                xna.Vector3 UPA11 = new xna.Vector3();
                                                float dirUPA11 = DirBToG(ballPt[8], UPA1);
                                                UPA11.X = (float)(ballPt[8].X + 0.8 * r * Math.Cos(dirUPA11));
                                                UPA11.Z = (float)(ballPt[8].Z + 0.8 * r * Math.Sin(dirUPA11));
                                                StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], UPA11,
                                                     dirUPA11, 5, 10, 80, 14, 10, 10, 100, true);
                                            }
                                        }
                                        //8号球在D区下部分
                                        if (ballPt[8].Z > 220 && ballPt[8].Z < 560)
                                        {
                                            if (ballPt[8].X < 948 && ballPt[8].X > 0)
                                            {
                                                xna.Vector3 DOWNA22 = new xna.Vector3();
                                                float dirDOWNA22 = DirBToG(ballPt[8], DOWNA2);
                                                DOWNA22.X = (float)(ballPt[8].X + 0.8 * r * Math.Cos(dirDOWNA22));
                                                DOWNA22.Z = (float)(ballPt[8].Z + 0.8 * r * Math.Sin(dirDOWNA22));
                                                StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], DOWNA22,
                                                    dirDOWNA22, 5, 10, 80, 14, 10, 10, 100, true);
                                            }

                                            if (ballPt[8].X < 0 && ballPt[8].X > -948)
                                            {

                                                xna.Vector3 DOWNA11 = new xna.Vector3();
                                                float dirDOWNA11 = DirBToG(ballPt[8], DOWNA1);
                                                DOWNA11.X = (float)(ballPt[8].X + 0.8 * r * Math.Cos(dirDOWNA11));
                                                DOWNA11.Z = (float)(ballPt[8].Z + 0.8 * r * Math.Sin(dirDOWNA11));
                                                StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], DOWNA11,
                                                     dirDOWNA11, 5, 10, 80, 14, 10, 10, 100, true);
                                            }
                                        }
                                        //8号球在G2区
                                        if (ballPt[8].X > 1092 && ballPt[8].X < 1500)
                                        {
                                            if (ballPt[8].Z < 0 && ballPt[8].Z > -560)
                                            {
                                                xna.Vector3 GUP = new xna.Vector3();
                                                float dirGUP = DirfishToG(ballPt[8], UPB2);
                                                GUP.X = (float)(ballPt[8].X - 0.8 * r * Math.Cos(dirGUP));
                                                GUP.Z = (float)(ballPt[8].Z - 0.8 * r * Math.Sin(dirGUP));
                                                StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], GUP,
                                                    dirGUP, 5, 10, 80, 11, 8, 10, 100, true);
                                            }
                                            if (ballPt[8].Z < 560 && ballPt[8].Z > 0)
                                            {
                                                xna.Vector3 GDOWN = new xna.Vector3();
                                                float dirGDOWN = DirfishToG(ballPt[8], DOWNB2);
                                                GDOWN.X = (float)(ballPt[8].X - 0.8 * r * Math.Cos(dirGDOWN));
                                                GDOWN.Z = (float)(ballPt[8].Z - 0.8 * r * Math.Sin(dirGDOWN));
                                                StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], GDOWN,
                                                    dirGDOWN, 5, 10, 80, 11, 8, 10, 100, true);
                                            }
                                        }
                                        if (ballPt[8].Z > -1000 && ballPt[8].Z < -560)
                                        {
                                            //8号球在A1区
                                            if (ballPt[8].X < 1500 && ballPt[8].X > -1200)
                                            {
                                                xna.Vector3 UPB11 = new xna.Vector3();
                                                float dirUPB11 = DirBToG(ballPt[8], UPB1);
                                                UPB11.X = (float)(ballPt[8].X + 0.8 * r * Math.Cos(dirUPB11));
                                                UPB11.Z = (float)(ballPt[8].Z + 0.8 * r * Math.Sin(dirUPB11));
                                                StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], UPB11,
                                                    dirUPB11, 5, 10, 100, 10, 7, 10, 100, true);
                                            }
                                            //8号球在B1区
                                            if (ballPt[8].X < -1200 && ballPt[8].X > -1500)
                                            {
                                                xna.Vector3 G1UP1 = new xna.Vector3();
                                                float dirG1UP1 = DirBToG(ballPt[8], G1UP);
                                                G1UP1.X = (float)(ballPt[8].X + 0.8 * r * Math.Cos(dirG1UP1));
                                                G1UP1.Z = (float)(ballPt[8].Z + 0.8 * r * Math.Sin(dirG1UP1));
                                                StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], G1UP1,
                                                    dirG1UP1, 5, 10, 100, 10, 7, 10, 100, true);
                                            }
                                        }
                                        if (ballPt[8].Z > 560 && ballPt[8].Z < 1000)
                                        {
                                            //8号球在A2区
                                            if (ballPt[8].X < 1500 && ballPt[8].X > -1200)
                                            {
                                                xna.Vector3 DOWNB11 = new xna.Vector3();
                                                float dirDOWNB11 = DirBToG(ballPt[8], DOWNB1);
                                                DOWNB11.X = (float)(ballPt[8].X + 0.8 * r * Math.Cos(dirDOWNB11));
                                                DOWNB11.Z = (float)(ballPt[8].Z + 0.8 * r * Math.Sin(dirDOWNB11));
                                                StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], DOWNB11,
                                                    dirDOWNB11, 5, 10, 100, 10, 7, 10, 100, true);
                                            }
                                            //8号球在B2区
                                            if (ballPt[8].X < -1200 && ballPt[8].X > -1500)
                                            {
                                                xna.Vector3 G1DOWN1 = new xna.Vector3();
                                                float dirG1DOWN1 = DirBToG(ballPt[8], G1DOWN);
                                                G1DOWN1.X = (float)(ballPt[8].X + 0.8 * r * Math.Cos(dirG1DOWN1));
                                                G1DOWN1.Z = (float)(ballPt[8].Z + 0.8 * r * Math.Sin(dirG1DOWN1));
                                                StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], G1DOWN1,
                                                    dirG1DOWN1, 5, 10, 100, 10, 7, 10, 100, true);
                                            }
                                        }
                                        //8号球在G1区
                                        if (ballPt[8].X > -1500 && ballPt[8].X < -1092)
                                        {
                                            if (ballPt[8].Z < 0 && ballPt[8].Z > -560)
                                            {
                                                xna.Vector3 GUP = new xna.Vector3();
                                                float dirGUP = DirfishToG(ballPt[8], LgoalUP);
                                                GUP.X = (float)(ballPt[8].X - 0.8 * r * Math.Cos(dirGUP));
                                                GUP.Z = (float)(ballPt[8].Z - 0.8 * r * Math.Sin(dirGUP));
                                                StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], GUP,
                                                    dirGUP, 5, 10, 80, 11, 8, 15, 100, true);
                                            }
                                            if (ballPt[8].Z < 560 && ballPt[8].Z > 0)
                                            {
                                                xna.Vector3 GDOWN = new xna.Vector3();
                                                float dirGDOWN = DirfishToG(ballPt[8], LgoalDOWN);
                                                GDOWN.X = (float)(ballPt[8].X - 0.8 * r * Math.Cos(dirGDOWN));
                                                GDOWN.Z = (float)(ballPt[8].Z - 0.8 * r * Math.Sin(dirGDOWN));
                                                StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], GDOWN,
                                                    dirGDOWN, 5, 10, 80, 11, 8, 15, 100, true);
                                            }
                                        }

                                        #endregion
                                    }
                                }
                                if ((b7_l == 1 || b7_r == 1) && b8_r == 0 && b8_l == 0)
                                {
                                    #region //0号鱼顶8号球的方案
                                    //8号球在D区上部分
                                    if (ballPt[8].Z > -560 && ballPt[8].Z < 220)
                                    {
                                        if (ballPt[8].X < 948 && ballPt[8].X > 0)
                                        {
                                            xna.Vector3 UPA22 = new xna.Vector3();
                                            float dirUPA22 = DirBToG(ballPt[8], UPA2);
                                            UPA22.X = (float)(ballPt[8].X + 0.8 * r * Math.Cos(dirUPA22));
                                            UPA22.Z = (float)(ballPt[8].Z + 0.8 * r * Math.Sin(dirUPA22));
                                            StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], UPA22,
                                                dirUPA22, 5, 10, 80, 14, 10, 10, 100, true);
                                        }

                                        if (ballPt[8].X < 0 && ballPt[8].X > -948)
                                        {

                                            xna.Vector3 UPA11 = new xna.Vector3();
                                            float dirUPA11 = DirBToG(ballPt[8], UPA1);
                                            UPA11.X = (float)(ballPt[8].X + 0.8 * r * Math.Cos(dirUPA11));
                                            UPA11.Z = (float)(ballPt[8].Z + 0.8 * r * Math.Sin(dirUPA11));
                                            StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], UPA11,
                                                 dirUPA11, 5, 10, 80, 14, 10, 10, 100, true);
                                        }
                                    }
                                    //8号球在D区下部分
                                    if (ballPt[8].Z > 220 && ballPt[8].Z < 560)
                                    {
                                        if (ballPt[8].X < 948 && ballPt[8].X > 0)
                                        {
                                            xna.Vector3 DOWNA22 = new xna.Vector3();
                                            float dirDOWNA22 = DirBToG(ballPt[8], DOWNA2);
                                            DOWNA22.X = (float)(ballPt[8].X + 0.8 * r * Math.Cos(dirDOWNA22));
                                            DOWNA22.Z = (float)(ballPt[8].Z + 0.8 * r * Math.Sin(dirDOWNA22));
                                            StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], DOWNA22,
                                                dirDOWNA22, 5, 10, 80, 14, 10, 10, 100, true);
                                        }

                                        if (ballPt[8].X < 0 && ballPt[8].X > -948)
                                        {

                                            xna.Vector3 DOWNA11 = new xna.Vector3();
                                            float dirDOWNA11 = DirBToG(ballPt[8], DOWNA1);
                                            DOWNA11.X = (float)(ballPt[8].X + 0.8 * r * Math.Cos(dirDOWNA11));
                                            DOWNA11.Z = (float)(ballPt[8].Z + 0.8 * r * Math.Sin(dirDOWNA11));
                                            StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], DOWNA11,
                                                 dirDOWNA11, 5, 10, 80, 14, 10, 10, 100, true);
                                        }
                                    }
                                    //8号球在G2区
                                    if (ballPt[8].X > 1092 && ballPt[8].X < 1500)
                                    {
                                        if (ballPt[8].Z < 0 && ballPt[8].Z > -560)
                                        {
                                            xna.Vector3 GUP = new xna.Vector3();
                                            float dirGUP = DirfishToG(ballPt[8], UPB2);
                                            GUP.X = (float)(ballPt[8].X - 0.8 * r * Math.Cos(dirGUP));
                                            GUP.Z = (float)(ballPt[8].Z - 0.8 * r * Math.Sin(dirGUP));
                                            StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], GUP,
                                                dirGUP, 5, 10, 80, 11, 8, 10, 100, true);
                                        }
                                        if (ballPt[8].Z < 560 && ballPt[8].Z > 0)
                                        {
                                            xna.Vector3 GDOWN = new xna.Vector3();
                                            float dirGDOWN = DirfishToG(ballPt[8], DOWNB2);
                                            GDOWN.X = (float)(ballPt[8].X - 0.8 * r * Math.Cos(dirGDOWN));
                                            GDOWN.Z = (float)(ballPt[8].Z - 0.8 * r * Math.Sin(dirGDOWN));
                                            StrategyHelper.Helpers.Dribble(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], GDOWN,
                                                dirGDOWN, 5, 10, 80, 11, 8, 10, 100, true);
                                        }
                                    }
                                    if (ballPt[8].Z > -1000 && ballPt[8].Z < -560)
                                    {
                                        //8号球在A1区
                                        if (ballPt[8].X < 1500 && ballPt[8].X > -1200)
                                        {
                                            xna.Vector3 UPB11 = new xna.Vector3();
                                            float dirUPB11 = DirBToG(ballPt[8], UPB1);
                                            UPB11.X = (float)(ballPt[8].X + 0.8 * r * Math.Cos(dirUPB11));
                                            UPB11.Z = (float)(ballPt[8].Z + 0.8 * r * Math.Sin(dirUPB11));
                                            StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], UPB11,
                                                dirUPB11, 5, 10, 100, 10, 7, 10, 100, true);
                                        }
                                        //8号球在B1区
                                        if (ballPt[8].X < -1200 && ballPt[8].X > -1500)
                                        {
                                            xna.Vector3 G1UP1 = new xna.Vector3();
                                            float dirG1UP1 = DirBToG(ballPt[8], G1UP);
                                            G1UP1.X = (float)(ballPt[8].X + 0.8 * r * Math.Cos(dirG1UP1));
                                            G1UP1.Z = (float)(ballPt[8].Z + 0.8 * r * Math.Sin(dirG1UP1));
                                            StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], G1UP1,
                                                dirG1UP1, 5, 10, 100, 10, 7, 10, 100, true);
                                        }
                                    }
                                    if (ballPt[8].Z > 560 && ballPt[8].Z < 1000)
                                    {
                                        //8号球在A2区
                                        if (ballPt[8].X < 1500 && ballPt[8].X > -1200)
                                        {
                                            xna.Vector3 DOWNB11 = new xna.Vector3();
                                            float dirDOWNB11 = DirBToG(ballPt[8], DOWNB1);
                                            DOWNB11.X = (float)(ballPt[8].X + 0.8 * r * Math.Cos(dirDOWNB11));
                                            DOWNB11.Z = (float)(ballPt[8].Z + 0.8 * r * Math.Sin(dirDOWNB11));
                                            StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], DOWNB11,
                                                dirDOWNB11, 5, 10, 100, 10, 7, 10, 100, true);
                                        }
                                        //8号球在B2区
                                        if (ballPt[8].X < -1200 && ballPt[8].X > -1500)
                                        {
                                            xna.Vector3 G1DOWN1 = new xna.Vector3();
                                            float dirG1DOWN1 = DirBToG(ballPt[8], G1DOWN);
                                            G1DOWN1.X = (float)(ballPt[8].X + 0.8 * r * Math.Cos(dirG1DOWN1));
                                            G1DOWN1.Z = (float)(ballPt[8].Z + 0.8 * r * Math.Sin(dirG1DOWN1));
                                            StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], G1DOWN1,
                                                dirG1DOWN1, 5, 10, 100, 10, 7, 10, 100, true);
                                        }
                                    }
                                    //8号球在G1区
                                    if (ballPt[8].X > -1500 && ballPt[8].X < -1092)
                                    {
                                        if (ballPt[8].Z < 0 && ballPt[8].Z > -560)
                                        {
                                            xna.Vector3 GUP = new xna.Vector3();
                                            float dirGUP = DirfishToG(ballPt[8], LgoalUP);
                                            GUP.X = (float)(ballPt[8].X - 0.8 * r * Math.Cos(dirGUP));
                                            GUP.Z = (float)(ballPt[8].Z - 0.8 * r * Math.Sin(dirGUP));
                                            StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], GUP,
                                                dirGUP, 5, 10, 80, 11, 8, 15, 100, true);
                                        }
                                        if (ballPt[8].Z < 560 && ballPt[8].Z > 0)
                                        {
                                            xna.Vector3 GDOWN = new xna.Vector3();
                                            float dirGDOWN = DirfishToG(ballPt[8], LgoalDOWN);
                                            GDOWN.X = (float)(ballPt[8].X - 0.8 * r * Math.Cos(dirGDOWN));
                                            GDOWN.Z = (float)(ballPt[8].Z - 0.8 * r * Math.Sin(dirGDOWN));
                                            StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], GDOWN,
                                                dirGDOWN, 5, 10, 80, 11, 8, 15, 100, true);
                                        }
                                    }

                                    #endregion
                                }
                                if (b7_l == 0 && b7_r == 0 && (b8_r == 1 || b8_l == 1))
                                {
                                    #region //0号鱼顶7号球的方案
                                    //7号球在D区上部分
                                    if (ballPt[7].Z > -560 && ballPt[7].Z < 220)
                                    {
                                        if (ballPt[7].X < 948 && ballPt[7].X > 0)
                                        {
                                            xna.Vector3 UPA22 = new xna.Vector3();
                                            float dirUPA22 = DirBToG(ballPt[7], UPA2);
                                            UPA22.X = (float)(ballPt[7].X + 0.8 * r * Math.Cos(dirUPA22));
                                            UPA22.Z = (float)(ballPt[7].Z + 0.8 * r * Math.Sin(dirUPA22));
                                            StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], UPA22,
                                                dirUPA22, 5, 10, 80, 14, 10, 10, 100, true);
                                        }

                                        if (ballPt[7].X < 0 && ballPt[7].X > -948)
                                        {

                                            xna.Vector3 UPA11 = new xna.Vector3();
                                            float dirUPA11 = DirBToG(ballPt[7], UPA1);
                                            UPA11.X = (float)(ballPt[7].X + 0.8 * r * Math.Cos(dirUPA11));
                                            UPA11.Z = (float)(ballPt[7].Z + 0.8 * r * Math.Sin(dirUPA11));
                                            StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], UPA11,
                                                 dirUPA11, 5, 10, 80, 14, 10, 10, 100, true);
                                        }
                                    }
                                    //7号球在D区下部分
                                    if (ballPt[7].Z > 220 && ballPt[7].Z < 560)
                                    {
                                        if (ballPt[7].X < 948 && ballPt[7].X > 0)
                                        {
                                            xna.Vector3 DOWNA22 = new xna.Vector3();
                                            float dirDOWNA22 = DirBToG(ballPt[7], DOWNA2);
                                            DOWNA22.X = (float)(ballPt[7].X + 0.8 * r * Math.Cos(dirDOWNA22));
                                            DOWNA22.Z = (float)(ballPt[7].Z + 0.8 * r * Math.Sin(dirDOWNA22));
                                            StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], DOWNA22,
                                                dirDOWNA22, 5, 10, 80, 14, 10, 10, 100, true);
                                        }

                                        if (ballPt[7].X < 0 && ballPt[7].X > -948)
                                        {

                                            xna.Vector3 DOWNA11 = new xna.Vector3();
                                            float dirDOWNA11 = DirBToG(ballPt[7], DOWNA1);
                                            DOWNA11.X = (float)(ballPt[7].X + 0.8 * r * Math.Cos(dirDOWNA11));
                                            DOWNA11.Z = (float)(ballPt[7].Z + 0.8 * r * Math.Sin(dirDOWNA11));
                                            StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], DOWNA11,
                                                 dirDOWNA11, 5, 10, 80, 14, 10, 10, 100, true);
                                        }
                                    }
                                    //7号球在G2区
                                    if (ballPt[7].X > 1092 && ballPt[7].X < 1500)
                                    {
                                        if (ballPt[7].Z < 0 && ballPt[7].Z > -560)
                                        {
                                            xna.Vector3 GUP = new xna.Vector3();
                                            float dirGUP = DirfishToG(ballPt[7], UPB2);
                                            GUP.X = (float)(ballPt[7].X - 0.8 * r * Math.Cos(dirGUP));
                                            GUP.Z = (float)(ballPt[7].Z - 0.8 * r * Math.Sin(dirGUP));
                                            StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], GUP,
                                                dirGUP, 5, 10, 80, 11, 8, 10, 100, true);
                                        }
                                        if (ballPt[7].Z < 560 && ballPt[7].Z > 0)
                                        {
                                            xna.Vector3 GDOWN = new xna.Vector3();
                                            float dirGDOWN = DirfishToG(ballPt[7], DOWNB2);
                                            GDOWN.X = (float)(ballPt[7].X - 0.8 * r * Math.Cos(dirGDOWN));
                                            GDOWN.Z = (float)(ballPt[7].Z - 0.8 * r * Math.Sin(dirGDOWN));
                                            StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], GDOWN,
                                                dirGDOWN, 5, 10, 80, 11, 8, 10, 100, true);
                                        }
                                    }
                                    if (ballPt[7].Z > -1000 && ballPt[7].Z < -560)
                                    {
                                        //7号球在A1区
                                        if (ballPt[7].X < 1500 && ballPt[7].X > -1200)
                                        {
                                            xna.Vector3 UPB11 = new xna.Vector3();
                                            float dirUPB11 = DirBToG(ballPt[7], UPB1);
                                            UPB11.X = (float)(ballPt[7].X + 0.8 * r * Math.Cos(dirUPB11));
                                            UPB11.Z = (float)(ballPt[7].Z + 0.8 * r * Math.Sin(dirUPB11));
                                            StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], UPB11,
                                                dirUPB11, 5, 10, 100, 10, 7, 10, 100, true);
                                        }
                                        //7号球在B1区
                                        if (ballPt[7].X < -1200 && ballPt[7].X > -1500)
                                        {
                                            xna.Vector3 G1UP1 = new xna.Vector3();
                                            float dirG1UP1 = DirBToG(ballPt[7], G1UP);
                                            G1UP1.X = (float)(ballPt[7].X + 0.8 * r * Math.Cos(dirG1UP1));
                                            G1UP1.Z = (float)(ballPt[7].Z + 0.8 * r * Math.Sin(dirG1UP1));
                                            StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], G1UP1,
                                                dirG1UP1, 5, 10, 100, 10, 7, 10, 100, true);
                                        }
                                    }
                                    if (ballPt[7].Z > 560 && ballPt[7].Z < 1000)
                                    {
                                        //7号球在A2区
                                        if (ballPt[0].X < 1500 && ballPt[0].X > -1200)
                                        {
                                            xna.Vector3 DOWNB11 = new xna.Vector3();
                                            float dirDOWNB11 = DirBToG(ballPt[7], DOWNB1);
                                            DOWNB11.X = (float)(ballPt[7].X + 0.8 * r * Math.Cos(dirDOWNB11));
                                            DOWNB11.Z = (float)(ballPt[7].Z + 0.8 * r * Math.Sin(dirDOWNB11));
                                            StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], DOWNB11,
                                                dirDOWNB11, 5, 10, 100, 10, 7, 10, 100, true);
                                        }
                                        //7号球在B2区
                                        if (ballPt[7].X < -1200 && ballPt[7].X > -1500)
                                        {
                                            xna.Vector3 G1DOWN1 = new xna.Vector3();
                                            float dirG1DOWN1 = DirBToG(ballPt[7], G1DOWN);
                                            G1DOWN1.X = (float)(ballPt[7].X + 0.8 * r * Math.Cos(dirG1DOWN1));
                                            G1DOWN1.Z = (float)(ballPt[7].Z + 0.8 * r * Math.Sin(dirG1DOWN1));
                                            StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], G1DOWN1,
                                                dirG1DOWN1, 5, 10, 100, 10, 7, 10, 100, true);
                                        }
                                    }
                                    //7号球在G1区
                                    if (ballPt[7].X > -1500 && ballPt[7].X < -1092)
                                    {
                                        if (ballPt[7].Z < 0 && ballPt[7].Z > -560)
                                        {
                                            xna.Vector3 GUP = new xna.Vector3();
                                            float dirGUP = DirfishToG(ballPt[7], LgoalUP);
                                            GUP.X = (float)(ballPt[7].X - 0.8 * r * Math.Cos(dirGUP));
                                            GUP.Z = (float)(ballPt[7].Z - 0.8 * r * Math.Sin(dirGUP));
                                            StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], GUP,
                                                dirGUP, 5, 10, 80, 11, 8, 15, 100, true);
                                        }
                                        if (ballPt[7].Z < 560 && ballPt[7].Z > 0)
                                        {
                                            xna.Vector3 GDOWN = new xna.Vector3();
                                            float dirGDOWN = DirfishToG(ballPt[7], LgoalDOWN);
                                            GDOWN.X = (float)(ballPt[7].X - 0.8 * r * Math.Cos(dirGDOWN));
                                            GDOWN.Z = (float)(ballPt[7].Z - 0.8 * r * Math.Sin(dirGDOWN));
                                            StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], GDOWN,
                                                dirGDOWN, 5, 10, 80, 11, 8, 15, 100, true);
                                        }
                                    }

                                    #endregion
                                }
                                if ((b7_l == 1 || b7_r == 1) && (b8_l == 1 || b8_r == 1) && b1_r == 0 && b1_l == 0)
                                {
                                    if (fishPt[0].X < -1000 && fishPt[0].X > -1500 && fishPt[0].Z < 560 && fishPt[0].Z > -560
                                && ballPt[7].X > -1000 && ballPt[8].X > -1000 && ballPt[1].X > -1000)
                                    {
                                        if (D1 > -Math.PI && D1 < 0)
                                        {
                                            float dir1 = DirfishToG(fishPt[0], UPB1);
                                            StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], UPB1,
                                                  dir1, 5, 10, 100, 14, 14, 8, 100, true);
                                        }
                                        else
                                        {
                                            float dir2 = DirfishToG(fishPt[0], DOWNB1);
                                            StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], UPB1,
                                                  dir2, 5, 10, 100, 14, 14, 8, 100, true);
                                        }
                                    }
                                    else
                                    {
                                        #region //0号鱼顶1号球的方案
                                        //1号球在D区上部分
                                        if (ballPt[1].Z > -560 && ballPt[1].Z < 220)
                                        {
                                            if (ballPt[1].X < 948 && ballPt[1].X > 0)
                                            {
                                                xna.Vector3 UPA22 = new xna.Vector3();
                                                float dirUPA22 = DirBToG(ballPt[1], UPA2);
                                                UPA22.X = (float)(ballPt[1].X + 0.8 * r * Math.Cos(dirUPA22));
                                                UPA22.Z = (float)(ballPt[1].Z + 0.8 * r * Math.Sin(dirUPA22));
                                                StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], UPA22,
                                                    dirUPA22, 5, 10, 80, 14, 10, 10, 100, true);
                                            }

                                            if (ballPt[1].X < 0 && ballPt[1].X > -948)
                                            {

                                                xna.Vector3 UPA11 = new xna.Vector3();
                                                float dirUPA11 = DirBToG(ballPt[1], UPA1);
                                                UPA11.X = (float)(ballPt[1].X + 0.8 * r * Math.Cos(dirUPA11));
                                                UPA11.Z = (float)(ballPt[1].Z + 0.8 * r * Math.Sin(dirUPA11));
                                                StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], UPA11,
                                                     dirUPA11, 5, 10, 80, 14, 10, 10, 100, true);
                                            }
                                        }
                                        //1号球在D区下部分
                                        if (ballPt[1].Z > 220 && ballPt[1].Z < 560)
                                        {
                                            if (ballPt[1].X < 948 && ballPt[1].X > 0)
                                            {
                                                xna.Vector3 DOWNA22 = new xna.Vector3();
                                                float dirDOWNA22 = DirBToG(ballPt[1], DOWNA2);
                                                DOWNA22.X = (float)(ballPt[1].X + 0.8 * r * Math.Cos(dirDOWNA22));
                                                DOWNA22.Z = (float)(ballPt[1].Z + 0.8 * r * Math.Sin(dirDOWNA22));
                                                StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], DOWNA22,
                                                    dirDOWNA22, 5, 10, 80, 14, 10, 10, 100, true);
                                            }

                                            if (ballPt[1].X < 0 && ballPt[1].X > -948)
                                            {

                                                xna.Vector3 DOWNA11 = new xna.Vector3();
                                                float dirDOWNA11 = DirBToG(ballPt[1], DOWNA1);
                                                DOWNA11.X = (float)(ballPt[1].X + 0.8 * r * Math.Cos(dirDOWNA11));
                                                DOWNA11.Z = (float)(ballPt[1].Z + 0.8 * r * Math.Sin(dirDOWNA11));
                                                StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], DOWNA11,
                                                     dirDOWNA11, 5, 10, 80, 14, 10, 10, 100, true);
                                            }
                                        }
                                        //1号球在G2区
                                        if (ballPt[1].X > 1092 && ballPt[1].X < 1500)
                                        {
                                            if (ballPt[1].Z < 0 && ballPt[1].Z > -560)
                                            {
                                                xna.Vector3 GUP = new xna.Vector3();
                                                float dirGUP = DirfishToG(ballPt[1], UPB2);
                                                GUP.X = (float)(ballPt[1].X - 0.8 * r * Math.Cos(dirGUP));
                                                GUP.Z = (float)(ballPt[1].Z - 0.8 * r * Math.Sin(dirGUP));
                                                StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], GUP,
                                                    dirGUP, 5, 10, 80, 11, 8, 10, 100, true);
                                            }
                                            if (ballPt[1].Z < 560 && ballPt[1].Z > 0)
                                            {
                                                xna.Vector3 GDOWN = new xna.Vector3();
                                                float dirGDOWN = DirfishToG(ballPt[1], DOWNB2);
                                                GDOWN.X = (float)(ballPt[1].X - 0.8 * r * Math.Cos(dirGDOWN));
                                                GDOWN.Z = (float)(ballPt[1].Z - 0.8 * r * Math.Sin(dirGDOWN));
                                                StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], GDOWN,
                                                    dirGDOWN, 5, 10, 80, 11, 8, 10, 100, true);
                                            }
                                        }
                                        if (ballPt[1].Z > -1000 && ballPt[1].Z < -560)
                                        {
                                            //1号球在A1区
                                            if (ballPt[1].X < 1500 && ballPt[1].X > -1200)
                                            {
                                                xna.Vector3 UPB11 = new xna.Vector3();
                                                float dirUPB11 = DirBToG(ballPt[1], UPB1);
                                                UPB11.X = (float)(ballPt[1].X + 0.8 * r * Math.Cos(dirUPB11));
                                                UPB11.Z = (float)(ballPt[1].Z + 0.8 * r * Math.Sin(dirUPB11));
                                                StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], UPB11,
                                                    dirUPB11, 5, 10, 100, 10, 7, 10, 100, true);
                                            }
                                            //1号球在B1区
                                            if (ballPt[1].X < -1200 && ballPt[1].X > -1500)
                                            {
                                                xna.Vector3 G1UP1 = new xna.Vector3();
                                                float dirG1UP1 = DirBToG(ballPt[1], G1UP);
                                                G1UP1.X = (float)(ballPt[1].X + 0.8 * r * Math.Cos(dirG1UP1));
                                                G1UP1.Z = (float)(ballPt[1].Z + 0.8 * r * Math.Sin(dirG1UP1));
                                                StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], G1UP1,
                                                    dirG1UP1, 5, 10, 100, 10, 7, 10, 100, true);
                                            }
                                        }
                                        if (ballPt[1].Z > 560 && ballPt[1].Z < 1000)
                                        {
                                            //1号球在A2区
                                            if (ballPt[1].X < 1500 && ballPt[1].X > -1200)
                                            {
                                                xna.Vector3 DOWNB11 = new xna.Vector3();
                                                float dirDOWNB11 = DirBToG(ballPt[1], DOWNB1);
                                                DOWNB11.X = (float)(ballPt[1].X + 0.8 * r * Math.Cos(dirDOWNB11));
                                                DOWNB11.Z = (float)(ballPt[1].Z + 0.8 * r * Math.Sin(dirDOWNB11));
                                                StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], DOWNB11,
                                                    dirDOWNB11, 5, 10, 100, 10, 7, 10, 100, true);
                                            }
                                            //1号球在B2区
                                            if (ballPt[1].X < -1200 && ballPt[1].X > -1500)
                                            {
                                                xna.Vector3 G1DOWN1 = new xna.Vector3();
                                                float dirG1DOWN1 = DirBToG(ballPt[1], G1DOWN);
                                                G1DOWN1.X = (float)(ballPt[1].X + 0.8 * r * Math.Cos(dirG1DOWN1));
                                                G1DOWN1.Z = (float)(ballPt[1].Z + 0.8 * r * Math.Sin(dirG1DOWN1));
                                                StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], G1DOWN1,
                                                    dirG1DOWN1, 5, 10, 100, 10, 7, 10, 100, true);
                                            }
                                        }
                                        //1号球在G1区
                                        if (ballPt[1].X > -1500 && ballPt[1].X < -1092)
                                        {
                                            if (ballPt[1].Z < 0 && ballPt[1].Z > -560)
                                            {
                                                xna.Vector3 GUP = new xna.Vector3();
                                                float dirGUP = DirfishToG(ballPt[1], LgoalUP);
                                                GUP.X = (float)(ballPt[1].X - 0.8 * r * Math.Cos(dirGUP));
                                                GUP.Z = (float)(ballPt[1].Z - 0.8 * r * Math.Sin(dirGUP));
                                                StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], GUP,
                                                    dirGUP, 5, 10, 80, 11, 8, 15, 100, true);
                                            }
                                            if (ballPt[1].Z < 560 && ballPt[1].Z > 0)
                                            {
                                                xna.Vector3 GDOWN = new xna.Vector3();
                                                float dirGDOWN = DirfishToG(ballPt[1], LgoalDOWN);
                                                GDOWN.X = (float)(ballPt[1].X - 0.8 * r * Math.Cos(dirGDOWN));
                                                GDOWN.Z = (float)(ballPt[1].Z - 0.8 * r * Math.Sin(dirGDOWN));
                                                StrategyHelper.Helpers.Dribble(ref decisions[0], mission.TeamsRef[teamId].Fishes[0], GDOWN,
                                                    dirGDOWN, 5, 10, 80, 11, 8, 15, 100, true);
                                            }
                                        }

                                        #endregion

                                    }
                                }
                            }
                        }
                        #endregion
                        #endregion
                    }
                }

                #endregion

            }
            #endregion
            #endregion
            return decisions;
        }
        #region 策略中所用到的函数
        //求球到目标角度的值
        public float DirBToG(xna.Vector3 ballPt, xna.Vector3 goalPt)
        {
            float dirBToG = (float)Math.Atan((ballPt.Z - goalPt.Z) / (ballPt.X - goalPt.X));
            return dirBToG;
        }

        //求鱼到目标角度的值
        public float DirfishToG(xna.Vector3 fishPt, xna.Vector3 goalPt)
        {
            float dirFishToG = (float)Math.Atan((fishPt.Z - goalPt.Z) / (fishPt.X - goalPt.X));
            return dirFishToG;
        }
        //鱼到球之间的距离
        public float DisFishToBall(xna.Vector3 ballPt, xna.Vector3 fishPt)
        {
            float disFishToBall = (float)Math.Sqrt(Math.Pow((ballPt.Z - fishPt.Z), 2) + Math.Pow((ballPt.X - fishPt.X), 2));
            return disFishToBall;
        }
        //球到球门之间的距离
        public float DisBallToGoal(xna.Vector3 ballPt, xna.Vector3 goalPt)
        {
            float disBallToGoal = (float)Math.Sqrt(Math.Pow((ballPt.Z - goalPt.Z), 2) + Math.Pow((ballPt.X - goalPt.X), 2));
            return disBallToGoal;
        }
        #endregion

    }
}