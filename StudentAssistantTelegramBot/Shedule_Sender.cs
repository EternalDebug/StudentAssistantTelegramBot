using System;
using System.Collections.Generic;
using System.Text;

namespace StudentAssistantTelegramBot
{
    class Shedule_Sender
    {
        static int test_min = 13;
        static int test_hour = 16;

        public class SendObj//Класс объекта отправки оповещения.
        {
            public DateTime SendTime;
            public long uid;
            public string name;

            public SendObj(DateTime DT, long id, string namen)
            {
                SendTime = DT;
                uid = id;
                name = namen;
            }
        }
        
        public static void Bot_SendWarn()
        {
            //Dictionary<long, string> NeedSend = new Dictionary<long, string>();
            List<SendObj> NeedSend = new List<SendObj>();
            while (true)
            {
                DateTime now = DateTime.Now;
                //if ((now.Hour == 8 && now.Minute == 0 && now.Second == 0) || (now.Hour == 12 && now.Minute == 0 && now.Second == 0 ) || (now.Hour == 16 && now.Minute == 0 && now.Second == 0 ) || (now.Hour == 20 && now.Minute == 0 && now.Second == 0 ) || (now.Hour == test_hour && now.Minute == test_min && now.Second == 0))
                if (now.Minute % 2 == 0 && now.Second == 0 && now.Millisecond == 0  || (now.Hour == test_hour && now.Minute == test_min && now.Second == 0))
                {
                    NeedSend = CreateNotSendedArr();
                }

                
                Bot_SendWarn_core(now, ref NeedSend);
                
            }
        }



        public static void Bot_SendWarn_core(DateTime now, ref List<SendObj> NeedSend)
        {
            foreach (var i in NeedSend)
            {
                if (now.Hour == i.SendTime.Hour && now.Minute == i.SendTime.Minute && now.Second == i.SendTime.Second && now.Millisecond == i.SendTime.Millisecond)
                {
                    string answer = $"Время подготовки. Сейчас у вас {i.name}.";
                    Student st;
                    if (Program.students.ContainsStudByID(i.uid, out st))
                    {
                        st.prev_loc = st.users_loc;
                        st.users_loc = LevelOfCode.Question_1;
                        answer += " Вы планируете сейчас готовиться?";//да или нет
                    }

                    Program.Bot.SendTextMessageAsync(i.uid, answer);
                    Console.WriteLine($"Оповещение отправлено юзеру {i.uid}");
                }
            }
            
        }

        public static bool MyDateLessNow(DateTime DT)
        {
            bool res = false;
            DateTime Now = DateTime.Now;
            if (DT.Hour < Now.Hour)
                return true;
            else if (DT.Hour == Now.Hour)
                if (DT.Minute < Now.Minute)
                    return true;
                else if (DT.Minute == Now.Minute)
                    if (DT.Second < Now.Second-1)
                        return true;
                    else if (DT.Second == Now.Second)
                        if (DT.Millisecond < Now.Millisecond)
                            return true;

                        return res;
        }

        //tester id 790754149


        /// <summary>
        /// Фильтр сегодняшнего числа
        /// </summary>
        /// <param name="Shed"></param>
        /// <param name="today"></param>
        /// <returns></returns>
        public static bool ContData(DateTime Shed)
        {
            DateTime now = DateTime.Now;
            //return (Shed.Date == now.Date && Shed.Hour == now.Hour && Shed.Minute == now.Minute);
            return (Shed.Date == now.Date);
        }


        public static List<SendObj> CreateNotSendedArr()
        {
            //Dictionary<long, string> res = new Dictionary<long, string>();
            List<SendObj> res = new List<SendObj>();
            foreach (var i in Program.students.list)
            {
                 foreach (var k in i.Shedule.Keys)
                 {
                    foreach (var j in i.Shedule[k])
                    {
                        if (ContData(j) && !MyDateLessNow(j))
                            res.Add(new SendObj(j, i.student_id, k));
                    }
                 }
            }

             return res;
        }

        public static void SetTestShed(ref Students s, long uid)
        {
            Console.WriteLine($"Юзеру {uid} установлено тестовое расписание. Сообщите офицеру безопасности!");
            for (int i = 0; i< s.list.Count;i++)
            {
                DateTime td = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, test_hour, test_min, 0);
                DateTime td2 = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, test_hour, test_min+3, 0);
                DateTime td3 = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, test_hour+1, test_min + 3, 0);
                if (s.list[i].student_id == uid)
                {
                    s.list[i].Shedule.Add("Test shedule", new DateTime[] { td,td2,td3 });
                }

            }
        }

    }
}
