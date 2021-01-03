namespace Model.Models.User
{
    /********************************************************************\
    КЛАСС.....: CRecordSorter
    ОПИСАНИЕ..: Предоставляет способы сортировки списков записей 
                о пользователях.
    МЕТОДЫ....: int CompareByID(CFindRecord left, CFindRecord right) -
                сравнение по ID
    \********************************************************************/
    public static class CRecordSorter
    {
        /********************************************************************\
        МЕТОД.....: CompareByID
        ОПИСАНИЕ..: Функция сравнения записей по ID
        ПАРАМЕТРЫ.: CFindRecord left - первая запись для сравнения
                    CFindRecord right - вторая запись для сравнения
        ВОЗВРАЩАЕТ: int - результат сравнения
        ПРИМЕЧАНИЕ: При сравнении
                    -1 - left < right или left равен null
                    0  - равны
                    1  - left > right или right равен null
        \********************************************************************/
        public static int CompareByID(CFindRecord left, CFindRecord right)
        {
            if ((left == null || left.user == null) && (right == null || right.user == null))
                return 0;
            if (left == null || left.user == null)
                return -1;
            if (right == null || right.user == null)
                return 1;
            return left.user.ID.CompareTo(right.user.ID);
        }
    }
}
