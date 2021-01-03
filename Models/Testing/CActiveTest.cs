using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Model.Models.Testing
{
    /********************************************************************\
    КЛАСС.....: CActiveTest
    ОПИСАНИЕ..: Тест, отслеживающий время выполнения заданий и блоков и
                прочую информацию, применяемую для сбора статистики.
    ПОЛЯ......: Test test - отслеживаемый тест.
                List<List<double>> Scores - баллы за задания.
                List<double> BlocksScores - баллы за блоки.
                List<List<long>> Times - время, затраченное на задания.
    МЕТОДЫ....: bool Reboot() - обнуление собранных данных.
                bool SetActiveTask(int blockIdx, int taskIdx = 0) - 
                смена активного задания.
    \********************************************************************/
    public class CActiveTest
    {   
        public  CTest                Test            { get; set; }         // Отслеживаемый тест
        public  CActiveTestData      TestData        { get; set; }         // Информация, связанная с тестом
        public  DateTime             ActiveStartTime { get; private set; } // Время начала/возврата к активному заданию
        public  List<List<TimeSpan>> Times           { get; private set; } // Время, затраченное на задания
        public  int                  ActiveBlock     { get; private set; } // Индекс активного блока
        public  int                  ActiveTask      { get; private set; } // Индекс активного задания
        
        public CActiveTest(CTest test)
        {
            Test = test;
            Reboot();
        }

        [JsonConstructor]
        public CActiveTest(CTest test, CActiveTestData testdata, DateTime activestarttime,
            List<List<TimeSpan>> times, int activeblock, int activetask)
        {
            Test = test;
            TestData = testdata;
            ActiveStartTime = activestarttime;
            Times = times;
            ActiveBlock = activeblock;
            ActiveTask = activetask;
        }
        /********************************************************************\
        МЕТОД.....: Reboot
        ОПИСАНИЕ..: Перезапуск теста, обнуление информации.
        ВОЗВРАТ...: bool - удалось ли перезапустить.
        ПРИМЕЧАНИЕ: После перезапуска сразу начинается выполнение теста, т.е.
                    активным станет первое задание первого блока и начнётся
                    отсчёт затраченного времени.
        \********************************************************************/
        public bool Reboot()
        {
            if (Test == null || Test.Blocks.Count() == 0)
                return false;
            TestData = new CActiveTestData(Test.Blocks.Count);
            Times = new List<List<TimeSpan>>(new List<TimeSpan>[Test.Blocks.Count]);
            for (int i = 0; i < Test.Blocks.Count; i++)
            {
                TestData.Scores[i] = new List<double>(new double[Test.Blocks[i].Tasks.Count]);
                TestData.Answers[i] = new List<string>(new string[Test.Blocks[i].Tasks.Count]);
                Times[i] = new List<TimeSpan>(new TimeSpan[Test.Blocks[i].Tasks.Count]);
            }
            ActiveBlock = ActiveTask = 0;
            ActiveStartTime = DateTime.Now;
            return true;
        }

        /********************************************************************\
        МЕТОД.....: SetActiveTask
        ОПИСАНИЕ..: Смена активного задания.
        ВОЗВРАТ...: bool - удалось ли сменить.
        \********************************************************************/
        public bool SetActiveTask(int blockIdx, int taskIdx = 0)
        {
            if (blockIdx >= 0 && blockIdx < Test.Blocks.Count && taskIdx >= 0 && taskIdx < Test.Blocks[blockIdx].Tasks.Count)
            {
                Times[ActiveBlock][ActiveTask] += DateTime.Now - ActiveStartTime;
                ActiveBlock = blockIdx;
                ActiveTask  = taskIdx;
                ActiveStartTime = DateTime.Now;
                return true;
            }
            return false;
        }

        public void Finish()
        {
            Times[ActiveBlock][ActiveTask] += DateTime.Now - ActiveStartTime;
            ActiveStartTime = DateTime.Now;
        }
    }
}
