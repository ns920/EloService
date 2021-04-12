using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EloService.Controllers
{
    //[Produces("application/json")]

    public class EloController : Controller
    {
        [Route("api/Elo")]
        public IActionResult Index()
        {
            return View();
        }


        [Route("api/Elo/Calculate")]
        [HttpPost]
        public IActionResult Calculate(string elo,string matchentity)
        {
            try
            {
                var match = JsonConvert.DeserializeObject<MatchEntity>(matchentity);
                dynamic elosObject = JObject.Parse(elo);
                Elo[] elos = elosObject.elos.ToObject<Elo[]>();
                //elo计算逻辑

                Elo[] newElos = elos;
                var context = new
                {
                    elos=newElos,
                    message = "Success"
                };
                return Json(context);
            }
            catch (System.Exception ex)
            {
                var context = new
                {
                    message = "Error:"+ex.Message,
                };
                return Json(context);
            }
        }
        /// <summary>
        /// 单场分数计算
        /// </summary>
        /// <param name="scores"></param>
        /// <param name="RScores"></param>
        void SingleMatch(int[] scores,ref int[] RScores)
        {
            double k = 40;
            //计算平均分
            double rateAvg = RScores.Average();
            double scoreAvg = scores.Average();
            double[] EScores = new double[scores.Length];

            for(int i=0;i< scores.Length;i++)
            {
                EScores[i]= 1 / (1 + Math.Pow(10, (rateAvg - RScores[i]) / 2000));
            }

            double[] SScores = new double[scores.Length];
            double per = 1 / (scores.Length - 1);//每超过一个人的分数
            for (int i = 0; i < scores.Length; i++)//每个人
            {
                for (int j = 0; j < scores.Length; j++)//循环数组判断超过的人数
                {
                    if (scores[i] > scores[j]) SScores[i] += per;
                }
            }

            for (int i = 0; i < scores.Length; i++)
            {
                SScores[i] = SScores[i] * 0.6 + ScoreRatio(scores[i], scoreAvg) * 0.4;
            }

            int[] delta = new int[scores.Length];
            for (int i = 0; i < scores.Length; i++)
            {
                delta[i]= (int)Math.Round(k * (SScores[i] - EScores[i]));
            }

            //最终得分修改
            for (int i = 0; i < scores.Length; i++)
            {
                RScores[i] = RScores[i] + delta[i];
            }
        }
        /// <summary>
        /// 跟平均分对比得出分数
        /// </summary>
        /// <param name="score"></param>
        /// <param name="scoreAvg">平均分</param>
        /// <returns></returns>
        double ScoreRatio(int score,double scoreAvg)
        {
            double ratio = score / scoreAvg;
            if (ratio > 5) return 1;
            if (ratio <= 5 && ratio >= 1) return (Math.Log(ratio, 5) + 1) / 2;
            if (ratio < 1) return ((1 / (2 - ratio)) - 0.5);
            return 0;
        }
    }
}
