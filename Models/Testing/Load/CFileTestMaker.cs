using Microsoft.Extensions.FileProviders;
using Model.Models.Testing.Score;
using Model.Models.Testing.Task;
using System.Collections.Generic;
using System.Xml;

namespace Model.Models.Testing.Load
{
    public class CFileTestMaker : ITestMaker
    {
        private readonly string m_storagePath;
        public CFileTestMaker(string storagePath)
        {
            m_storagePath = storagePath;
        }

        private void LoadTestScoreSummator(CTest test, XmlNode node)
        {
            switch (node.InnerText)
            {
                case "Простое":
                    test.Summator = new CClearSummator();
                    break;
                default:
                    test.Summator = new CClearSummator();
                    break;
            }
        }

        private IScoreSummator LoadSummator(XmlNode node)
        {
            switch (node.InnerText)
            {
                case "Простое":
                    return new CClearSummator();
            }
            return new CClearSummator();
        }

        private void LoadRequires(CBlock block, XmlNode node)
        {
            string tmp = node.InnerText;
            List<string> values = new List<string>(tmp.Split(' '));
            foreach (var i in values)
                try { block.Requires.Add(int.Parse(i)); } catch { }
        }

        private void LoadSingle(CBlock block, XmlNode node)
        {
            CSingleAnswer task = new CSingleAnswer();
            XmlNode tmpNode = node.Attributes.GetNamedItem("ID");
            if (tmpNode != null)
                task.ID = int.Parse(tmpNode.InnerText);
            tmpNode = node.Attributes.GetNamedItem("Время");
            if (tmpNode != null)
                task.Time = int.Parse(tmpNode.InnerText);
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

        private void LoadMulti(CBlock block, XmlNode node)
        {
            CMultiAnswer task = new CMultiAnswer();
            XmlNode tmpNode = node.Attributes.GetNamedItem("ID");
            if (tmpNode != null)
                task.ID = int.Parse(tmpNode.InnerText);
            tmpNode = node.Attributes.GetNamedItem("Время");
            if (tmpNode != null)
                task.Time = int.Parse(tmpNode.InnerText);
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
        private void LoadSequence(CBlock block, XmlNode node)
        {

        }
        private void LoadCompliance(CBlock block, XmlNode node)
        {

        }
        private void LoadOpen(CBlock block, XmlNode node)
        {
            COpenAnswer task = new COpenAnswer();
            XmlNode tmpNode = node.Attributes.GetNamedItem("ID");
            if (tmpNode != null)
                task.ID = int.Parse(tmpNode.InnerText);
            tmpNode = node.Attributes.GetNamedItem("Время");
            if (tmpNode != null)
                task.Time = int.Parse(tmpNode.InnerText);
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
                            LoadTestScoreSummator(test, curNode);
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
