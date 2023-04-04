using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PageReplacementAlgs
{
    public class Algorithms
    {
        public static void FIFO(List<int> pages, int memoryCount, out int interuptions, out string fifoLog)
        {
            fifoLog = "";
            StringBuilder sb = new StringBuilder();
            sb.Append("Ход алгоритма FIFO:\n\n");
            var memory = new List<int>();
            interuptions = 0;
            for (int i = 0; i < pages.Count; i++)
            {
                var calledPage = pages[i];
                if (memory.Contains(calledPage))
                {
                    sb.Append(String.Format("           | Обращение: {0, 3} | Страница: {1, 3} | Память: {2}\n", i + 1, calledPage, string.Join(" ", memory)));
                }
                else
                {
                    interuptions++;
                    if (memory.Count < memoryCount)
                    {
                        memory.Add(calledPage);
                        sb.Append(String.Format("Прерывание | Обращение: {0, 3} | Страница: {1, 3} | Память: {2}\n", i + 1, calledPage, string.Join(" ", memory)));
                    }
                    else
                    {
                        memory.RemoveAt(0);
                        memory.Add(calledPage);
                        sb.Append(String.Format("Прерывание | Обращение: {0, 3} | Страница: {1, 3} | Память: {2}\n", i + 1, calledPage, string.Join(" ", memory)));
                    }
                }
            }
            sb.Append(String.Format("\nКоличество прерываний: {0}", interuptions));
            fifoLog = sb.ToString();
            memory.Clear();
        }

        public static void LRU(List<int> pages, int memoryCount, out int interuptions, out string lruLog)
        {
            lruLog = "";
            StringBuilder sb = new StringBuilder();
            sb.Append("Ход алгоритма LRU:\n\n");
            interuptions = 0;
            var memory = new List<int>();
            var age = new List<int>();
            for (int i = 0; i < memoryCount; i++)
            {
                age.Add(0);
            }
            for (int i = 0; i < pages.Count; i++)
            {
                var calledPage = pages[i];
                if (memory.Contains(calledPage))
                {
                    age = age.Select(a => a + 1).ToList();
                    age[memory.IndexOf(calledPage)] = 0;
                    sb.Append(String.Format("           | Обращение: {0, 3} | Страница: {1, 3} | Память: {2}\n", i + 1, calledPage, string.Join(" ", memory)));
                }
                else
                {
                    interuptions++;
                    if (memory.Count < memoryCount)
                    {
                        memory.Add(calledPage);
                        age = age.Select(a => a + 1).ToList();
                        age[memory.IndexOf(calledPage)] = 0;
                        sb.Append(String.Format("Прерывание | Обращение: {0, 3} | Страница: {1, 3} | Память: {2}\n", i + 1, calledPage, string.Join(" ", memory)));
                    }
                    else
                    {
                        int indexOldest = age.IndexOf(age.Max());
                        age = age.Select(a => a + 1).ToList();
                        memory[indexOldest] = calledPage;
                        age[indexOldest] = 0;
                        sb.Append(String.Format("Прерывание | Обращение: {0, 3} | Страница: {1, 3} | Память: {2}\n", i + 1, calledPage, string.Join(" ", memory)));
                    }
                }
            }
            sb.Append(String.Format("\nКоличество прерываний: {0}", interuptions));
            lruLog = sb.ToString();
            memory.Clear();
        }

        // case 0: not referenced, not modified (0 0). case 1: not referenced, modified (0 1). case 2: referenced, not modified (1 0). case 3: referenced, modified (1 1).
        private struct Bits
        {
            public Bits(int intValue1, int intValue2)
            {
                RBit = intValue1;
                MBit = intValue2;
            }
            public int RBit { get; set; }
            public int MBit { get; set; }
            public int BitCase { get { return (RBit << 1) | MBit; } }
        }

        public static void NRU(List<int> pages, int memoryCount, out int interuptions, out string nruLog)
        {
            nruLog = "";
            StringBuilder sb = new StringBuilder();
            sb.Append("Ход алгоритма NRU:\n\n");
            interuptions = 0;
            var memory = new List<int>();
            var bits = new Dictionary<int, Bits>();
            for (int i = 0; i < pages.Count; i++)
            {
                var calledPage = pages[i];
                if (i % 3 == 0)
                {
                    for (int j = 0; j < bits.Count; j++)
                    {
                        bits[bits.ElementAt(j).Key] = new Bits(0, bits[bits.ElementAt(j).Key].MBit);
                    }
                }
                if (memory.Contains(calledPage))
                {
                    bits[calledPage] = new Bits(1, bits[calledPage].MBit);
                    sb.Append(String.Format("           | Обращение: {0, 3} | Страница: {1, 3} | Память: {2}\n", i + 1, calledPage, string.Join(" ", memory)));
                }
                else
                {
                    interuptions++;
                    if (memory.Count < memoryCount)
                    {
                        memory.Add(calledPage);
                        if (bits.Keys.Contains(calledPage))
                        {
                            bits[calledPage] = new Bits(bits[calledPage].RBit, bits[calledPage].MBit);
                        }
                        else
                            bits[calledPage] = new Bits(0, 1);
                        sb.Append(String.Format("Прерывание | Обращение: {0, 3} | Страница: {1, 3} | Память: {2}\n", i + 1, calledPage, string.Join(" ", memory)));
                    }
                    else
                    {
                        var tempBits = bits.Where(x => memory.Contains(x.Key));
                        var pageForPush = tempBits.Aggregate((l, r) => l.Value.BitCase <= r.Value.BitCase ? l : r).Key;
                        bits[pageForPush] = new Bits(bits[pageForPush].RBit, 0);
                        memory[memory.IndexOf(pageForPush)] = calledPage;
                        if (bits.Keys.Contains(calledPage))
                        {
                            bits[calledPage] = new Bits(bits[calledPage].RBit, bits[calledPage].MBit);
                        }
                        else
                            bits[calledPage] = new Bits(0, 1);
                        sb.Append(String.Format("Прерывание | Обращение: {0, 3} | Страница: {1, 3} | Память: {2}\n", i + 1, calledPage, string.Join(" ", memory)));
                    }
                }
            }
            sb.Append(String.Format("\nКоличество прерываний: {0}", interuptions));
            nruLog = sb.ToString();
            memory.Clear();
        }

        public static void SC(List<int> pages, int memoryCount, out int interuptions, out string scLog)
        {
            scLog = "";
            StringBuilder sb = new StringBuilder();
            sb.Append(@"Ход алгоритма ""Вторая попытка"":" + "\n\n");
            var memory = new List<int>();
            var rBit = new List<int>();
            interuptions = 0;
            int counter = 0;
            for (int i = 0; i < memoryCount; i++)
            {
                rBit.Add(0);
            }
            for (int i = 0; i < pages.Count; i++)
            {
                var calledPage = pages[i];
                if (memory.Contains(calledPage))
                {
                    rBit[memory.IndexOf(calledPage)] = 1;
                    counter++;
                    sb.Append(String.Format("           | Обращение: {0, 3} | Страница: {1, 3} | Память: {2}\n", counter, calledPage, string.Join(" ", memory)));
                }
                else
                {                   
                    if (memory.Count < memoryCount)
                    {
                        memory.Add(calledPage);
                        rBit[memory.IndexOf(calledPage)] = 1;
                        counter++;
                        interuptions++;
                        sb.Append(String.Format("Прерывание | Обращение: {0, 3} | Страница: {1, 3} | Память: {2}\n", counter, calledPage, string.Join(" ", memory)));
                    }
                    else
                    {
                        if (rBit[0] == 0)
                        {
                            memory.RemoveAt(0);
                            memory.Add(calledPage);
                            counter++;
                            interuptions++;
                            sb.Append(String.Format("Прерывание | Обращение: {0, 3} | Страница: {1, 3} | Память: {2}\n", counter, calledPage, string.Join(" ", memory)));
                        }
                        else
                        {
                            rBit.RemoveAt(0);
                            rBit.Add(0);
                            memory.Add(memory[0]);
                            memory.RemoveAt(0);
                            counter++;
                            i--;
                            sb.Append(String.Format("Прерывание | Обращение: {0, 3} | Страница: {1, 3} | Память: {2}\n", counter, calledPage, string.Join(" ", memory)));
                        }
                    }
                }
            }
            sb.Append(String.Format("\nКоличество прерываний: {0}", interuptions));
            scLog = sb.ToString();
            memory.Clear();
        }

        public static void Clock(List<int> pages, int memoryCount, out int interuptions, out string clockLog)
        {
            clockLog = "";
            StringBuilder sb = new StringBuilder();
            sb.Append(@"Ход алгоритма ""Часы"":" + "\n\n");
            var memory = new List<int>();
            var rBit = new List<int>();
            interuptions = 0;
            int clockArrow = 0;
            int counter = 0;
            for (int i = 0; i < memoryCount; i++)
            {
                rBit.Add(0);
            }
            for (int i = 0; i < pages.Count; i++)
            {
                var calledPage = pages[i];
                if (clockArrow == memoryCount)
                {
                    clockArrow = 0;
                }
                if (memory.Contains(calledPage))
                {
                    rBit[memory.IndexOf(calledPage)] = 1;
                    counter++;
                    clockArrow++;
                    sb.Append(String.Format("           | Обращение: {0, 3} | Страница: {1, 3} | Память: {2}\n", counter, calledPage, string.Join(" ", memory)));
                }
                else
                {
                    if (memory.Count < memoryCount)
                    {
                        memory.Add(calledPage);
                        rBit[memory.IndexOf(calledPage)] = 1;
                        counter++;
                        clockArrow++;
                        interuptions++;
                        sb.Append(String.Format("Прерывание | Обращение: {0, 3} | Страница: {1, 3} | Память: {2}\n", counter, calledPage, string.Join(" ", memory)));
                    }
                    else
                    {
                        if (rBit[clockArrow] == 0)
                        {
                            memory[clockArrow] = calledPage;
                            rBit[clockArrow] = 1;
                            counter++;
                            clockArrow++;
                            interuptions++;
                            sb.Append(String.Format("Прерывание | Обращение: {0, 3} | Страница: {1, 3} | Память: {2}\n", counter, calledPage, string.Join(" ", memory)));
                        }
                        else
                        {
                            rBit[clockArrow] = 0;
                            counter++;
                            clockArrow++;
                            i--;
                            sb.Append(String.Format("Прерывание | Обращение: {0, 3} | Страница: {1, 3} | Память: {2}\n", counter, calledPage, string.Join(" ", memory)));
                        }
                    }
                }
            }
            sb.Append(String.Format("\nКоличество прерываний: {0}", interuptions));
            clockLog = sb.ToString();
            memory.Clear();
        }
    }
}
