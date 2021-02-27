using Microsoft.Extensions.FileProviders;
using Model.Models.Testing.Score;
using Model.Models.Testing.Task;
using System.Collections.Generic;
using System.Xml;

namespace Model.Models.Testing.Load
{
    /// <summary>
    /// Генератор теста, использующий данные из файла
    /// </summary>
    public class CFileTestMaker : ITestMaker
    {
        private readonly string m_storagePath;  //!< Путь до папки с данными
        public CFileTestMaker(string storagePath)
        {
            m_storagePath = storagePath;
        }

        /// <summary>
        /// Сортировка множества по возрастанию номеров правильных ответов
        /// </summary>
        /// <param name="set"></param>
        private void SortSet(List<KeyValuePair<uint, string>> set)
        {
            for (int i = 0; i + 1 < set.Count; i++)
                for (int j = i + 1; j < set.Count; j++)
                    if (set[i].Key > set[j].Key)
                    {
                        KeyValuePair<uint, string> tmp = set[i];
                        set[i] = set[j];
                        set[j] = tmp;
                    }
        }

        /// <summary>
        /// Создаёт класс оценивания по информации, указанной в узле
        /// </summary>
        /// <param name="node">Узел с информацией о способе оценивания</param>
        /// <returns>Способ оценивания</returns>
        private IScoreSummator LoadSummator(XmlNode node)
        {
            switch (node.InnerText)
            {
                case "Простое":
                    return new CClearSummator();
            }
            return new CClearSummator();
        }

        /// <summary>
        /// Загрузка ID задания и ожидаемого времени выполнения
        /// </summary>
        /// <param name="task">Задание</param>
        /// <param name="node">Корневой узел задания</param>
        private void Load_ID_And_Time(ITask task, XmlNode node)
        {
            XmlNode tmpNode = node.Attributes.GetNamedItem("ID");
            if (tmpNode != null)
                task.ID = int.Parse(tmpNode.InnerText);
            tmpNode = node.Attributes.GetNamedItem("Время");
            if (tmpNode != null)
                task.Time = int.Parse(tmpNode.InnerText);
        }

        /// <summary>
        /// Внесение в блок id блоков, завершение которых требуется для допуска
        /// </summary>
        /// <param name="block">Блок заданий</param>
        /// <param name="node">Узел, содержащий id требуемых блоков</param>
        private void LoadRequires(CBlock block, XmlNode node)
        {
            string tmp = node.InnerText;
            List<string> values = new List<string>(tmp.Split(' '));
            foreach (var i in values)
                try { block.Requires.Add(int.Parse(i)); } catch { }
        }

        /// <summary>
        /// Добавление в блок задания с выбором 1 правильного ответа
        /// </summary>
        /// <param name="block">Блок заданий</param>
        /// <param name="node">Корневой узел задания с выбором 1 правильного варианта ответа</param>
        private void LoadSingle(CBlock block, XmlNode node)
        {
            CSingleAnswer task = new CSingleAnswer();
            Load_ID_And_Time(task, node);
            List<string> Variants = new List<string>();
            string Correct = "";
            foreach (XmlNode curNode in node)
                switch (curNode.Name)
                {
                    case "Текст":
                        task.Text = curNode.InnerText;
                        break;
                    case "Ответ":
                        Correct = curNode.InnerText;
                        break;
                    case "Дистрактор":
                        Variants.Add(curNode.InnerText);
                        break;
                }
            task.Correct = Correct;
            if (Variants.Count != 0)
            {
                task.Answer = (uint)Program.Random.Next(0, Variants.Count);
                Variants.Insert((int)task.Answer, Correct);
            }
            else
            {
                Variants.Add(Correct);
                task.Answer = 0;
            }
            task.Variants = Variants;
            block.Tasks.Add(task);
        }

        /// <summary>
        /// Добавление в блок задания с выбором нескольких правильных вариантов ответа
        /// </summary>
        /// <param name="block">Блок заданий</param>
        /// <param name="node">
        /// Корневой узел задания с выбором нескольких правильных варианта ответа
        /// </param>
        private void LoadMulti(CBlock block, XmlNode node)
        {
            CMultiAnswer task = new CMultiAnswer();
            Load_ID_And_Time(task, node);
            List<KeyValuePair<string, bool>> Variants = new List<KeyValuePair<string, bool>>();
            task.Correct = "";
            foreach (XmlNode curNode in node)
                switch (curNode.Name)
                {
                    case "Текст":
                        task.Text = curNode.InnerText;
                        break;
                    case "Ответ":
                        task.Correct += curNode.InnerText + "\n";
                        Variants.Add(new KeyValuePair<string, bool>(curNode.InnerText, true));
                        break;
                    case "Дистрактор":
                        Variants.Add(new KeyValuePair<string, bool>(curNode.InnerText, false));
                        break;
                }
            while (Variants.Count != 0)
            {
                int indx = Program.Random.Next(0, Variants.Count);
                task.Variants.Add(Variants[indx].Key);
                if (Variants[indx].Value)
                    task.Answer.Add(task.Variants.Count - 1);
                Variants.RemoveAt(indx);
            }
            block.Tasks.Add(task);
        }

        /// <summary>
        /// Добавление в блок задания на установление правильной последовательности
        /// </summary>
        /// <param name="block">Блок заданий</param>
        /// <param name="node">
        /// Корневой узел задания на установление правильной последовательности
        /// </param>
        private void LoadSequence(CBlock block, XmlNode node)
        {
            CSequenceAnswer task = new CSequenceAnswer();
            Load_ID_And_Time(task, node);
            List<KeyValuePair<uint, string>> Set = new List<KeyValuePair<uint, string>>();
            foreach (XmlNode curNode in node)
                switch (curNode.Name)
                {
                    case "Текст":
                        task.Text = curNode.InnerText;
                        break;
                    case "Элемент":
                        Set.Add(new KeyValuePair<uint, string>(
                                uint.Parse(curNode.Attributes.GetNamedItem("Номер").InnerText),
                                curNode.InnerText
                            ));
                        break;
                }
            SortSet(Set);
            List<int> FreePositions = new List<int>(new int[Set.Count]);
            task.Set = new List<string>(new string[Set.Count]);
            task.Answer = new List<uint>(new uint[Set.Count]);
            for (int i = 0; i < Set.Count; i++)
                FreePositions[i] = i;
            for (int i = 0; i < Set.Count; i++)
            {
                task.Correct += Set[i].Value + "\n";
                int randPos = Program.Random.Next(0, FreePositions.Count);
                task.Set[FreePositions[randPos]] = Set[i].Value;
                task.Answer[i] = (uint)FreePositions[randPos];
                FreePositions.RemoveAt(randPos);
            }
            block.Tasks.Add(task);
        }

        /// <summary>
        /// Добавление в блок задания на установление соответствия
        /// </summary>
        /// <param name="block">Блок заданий</param>
        /// <param name="node">Корневой узел задания на установление соответствия</param>
        private void LoadCompliance(CBlock block, XmlNode node)
        {

        }

        /// <summary>
        /// Добавление в блок задания открытого типа
        /// </summary>
        /// <param name="block">Блок заданий</param>
        /// <param name="node">Корневой узел задания открытого типа</param>
        private void LoadOpen(CBlock block, XmlNode node)
        {
            COpenAnswer task = new COpenAnswer();
            Load_ID_And_Time(task, node);
            foreach (XmlNode curNode in node)
                switch (curNode.Name)
                {
                    case "Текст":
                        task.Text = curNode.InnerText;
                        break;
                    case "Ответ":
                        task.Answer = curNode.InnerText;
                        break;
                }
            block.Tasks.Add(task);
        }

        /// <summary>
        /// Добавление блока заданий в тест
        /// </summary>
        /// <param name="test">Тест</param>
        /// <param name="node">Корневой узел блока заданий</param>
        private void LoadBlock(CTest test, XmlNode node)
        {
            CBlock block = new CBlock();
            // Название
            XmlNode tmpNode = node.Attributes.GetNamedItem("Название");
            if (tmpNode != null)
                block.Name = tmpNode.InnerText;
            // ID
            tmpNode = node.Attributes.GetNamedItem("ID");
            if (tmpNode != null)
                block.ID = int.Parse(tmpNode.InnerText);
            // Минимальный балл
            tmpNode = node.Attributes.GetNamedItem("Балл");
            if (tmpNode != null)
                block.MinScore = int.Parse(tmpNode.InnerText);
            // Время
            tmpNode = node.Attributes.GetNamedItem("Время");
            if (tmpNode != null)
                block.Time = int.Parse(tmpNode.InnerText);
            // Оценивание, вопросы
            foreach (XmlNode curNode in node)
                switch (curNode.Name)
                {
                    case "Оценивание":
                        block.Summator = LoadSummator(curNode);
                        break;
                    case "Требования":
                        LoadRequires(block, curNode);
                        break;
                    case "Одноответный":
                        LoadSingle(block, curNode);
                        break;
                    case "Многоответный":
                        LoadMulti(block, curNode);
                        break;
                    case "Открытый":
                        LoadOpen(block, curNode);
                        break;
                    case "Последовательность":
                        LoadSequence(block, curNode);
                        break;
                    case "Соответствие":
                        LoadCompliance(block, curNode);
                        break;
                }
            if (block.Tasks.Count > 0)
            {
                for (int i = 0; i < block.Tasks.Count; i++)
                {
                    int FirstIdx = Program.Random.Next(0, block.Tasks.Count);
                    int SecondIdx = Program.Random.Next(0, block.Tasks.Count);
                    ITask tmp = block.Tasks[FirstIdx];
                    block.Tasks[FirstIdx] = block.Tasks[SecondIdx];
                    block.Tasks[SecondIdx] = tmp;
                }
                test.Blocks.Add(block);
            }
        }

        /// <summary>
        /// Выполняет генерацию теста по данным из файла
        /// </summary>
        /// <param name="filePath">Путь до файла с тестом</param>
        /// <param name="id">ID генерируемого теста</param>
        /// <returns>Сгенерированный тест</returns>
        public CTest Make(string filePath, int id)
        {
            CTest test = new CTest();
            test.ID = id;
            PhysicalFileProvider provider = new PhysicalFileProvider(m_storagePath);
            if (provider.GetFileInfo(filePath).Exists)
            {
                XmlDocument document = new XmlDocument();
                document.Load(m_storagePath + "/" + filePath);
                XmlNode root = document.DocumentElement;
                foreach (XmlNode curNode in root)
                {
                    switch (curNode.Name)
                    {
                        // Способ оценивания теста
                        case "Оценивание":
                            test.Summator = LoadSummator(curNode);
                            break;
                        // Загрузка блока заданий
                        case "Блок":
                            LoadBlock(test, curNode);
                            break;
                    }
                }
            }
            return test;
        }
    }
}
