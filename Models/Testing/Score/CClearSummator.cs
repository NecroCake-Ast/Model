﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Model.Models.Testing.Score
{
    /********************************************************************\
    КЛАСС.....: CClearSummator
    НАСЛЕДУЕТ.: IScoreSummator
    ОПИСАНИЕ..: Вычисляет итоговый балл за блок/тест путём суммирования
                набранных за задания/блоки балов.
    ПОЛЯ......: ESummatorID ID - ID вида сумматора баллов
    МЕТОДЫ....: double Calculate(double UserScore, long UserTime,
                int MinScore, long maxTime) - вычисляет итоговый балл за
                блок/тест по определённой схеме.
                IScoreSummator Copy() - копирует текущую схему оценки.
    ПРИМЕЧАНИЕ: Время не влияет на итоговый балл. При этом настройки теста
                остаются неизменными, т.е. недопуск из-за привышения
                максимального времени выполнения остаётся действительным.
    \********************************************************************/
    public class CClearSummator : IScoreSummator
    {
        public ESummatorID ID { get => ESummatorID.SID_CLEAR; } // ID вида сумматора баллов

        /********************************************************************\
        МЕТОД.....: Calculate
        ОПИСАНИЕ..: Возвращает итоговый балл за блок/тест исходя из
                    набранных пользователем баллов и затраченного и ожидаемого
                    времени.
        ПАРАМЕТРЫ.: List<double> UserScores - набранный пользователем балл.
                    List<long> UserTimes - затраченное пользователем время.
                    long Time - ожидаемое время выполнения.
        ВОЗВРАЩАЕТ: double - итоговый балл при заданных показателях.
        \********************************************************************/
        public double Calculate(List<double> UserScores, List<TimeSpan> UserTimes)
        {
            return UserScores.Sum();
        }

        /********************************************************************\
        МЕТОД.....: Copy
        ОПИСАНИЕ..: Создаёт копию текущего способа оценки.
        ВОЗВРАТ...: IScoreSummator - скопированный способ оценки.
        \********************************************************************/
        public IScoreSummator Copy() { return new CClearSummator(); }
    }
}