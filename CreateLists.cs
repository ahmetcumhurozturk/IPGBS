using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPGBS
{
    class CreateLists
    {
        private static Dictionary<string, Dictionary<string, List<int>>> adjacencyList;
        private static Dictionary<string, Dictionary<string, List<int>>> inverseAdjacencyList;
        public Dictionary<string, Dictionary<string, List<int>>> AdjacencyList
        {
            get { return adjacencyList; }
        }
        public Dictionary<string, Dictionary<string, List<int>>> InverseAdjacencyList
        {
            get { return inverseAdjacencyList; }
        }
        public CreateLists()
        {
            adjacencyList = new Dictionary<string, Dictionary<string, List<int>>>();
            inverseAdjacencyList = new Dictionary<string, Dictionary<string, List<int>>>();
        }
        public void CreateAdjacencyList(List<string> transactions, int vertex)
        {
            if (transactions.Count == 1)//create self loop vertex
            {
                if (!adjacencyList.ContainsKey(transactions[0]))
                {
                    adjacencyList[transactions[0]] = new Dictionary<string, List<int>>();
                }
                if (!adjacencyList[transactions[0]].ContainsKey(transactions[0].ToString()))
                {
                    adjacencyList[transactions[0]][transactions[0]] = new List<int>();
                }
                adjacencyList[transactions[0]][transactions[0]].Add(vertex);
                CreateInverseAdjacencyList(transactions, vertex);

            }
            else
            {
                for (int i = 0; i < transactions.Count - 1; i++)
                {
                    if (!adjacencyList.ContainsKey(transactions[i]))
                    {
                        adjacencyList[transactions[i]] = new Dictionary<string, List<int>>();
                    }
                    if (!adjacencyList[transactions[i]].ContainsKey(transactions[i + 1].ToString()))
                    {
                        adjacencyList[transactions[i]][transactions[i + 1]] = new List<int>();
                    }
                    adjacencyList[transactions[i]][transactions[i + 1]].Add(vertex);
                }
                CreateInverseAdjacencyList(transactions, vertex);
            }
        }
        public void CreateInverseAdjacencyList(List<string> transactions, int vertex)
        {
            if (transactions.Count == 1)
            {
                if (!inverseAdjacencyList.ContainsKey(transactions[0].ToString()))
                {
                    inverseAdjacencyList[transactions[0].ToString()] = new Dictionary<string, List<int>>();
                }
                if (!inverseAdjacencyList[transactions[0].ToString()].ContainsKey(transactions[0].ToString()))
                {
                    inverseAdjacencyList[transactions[0].ToString()][transactions[0].ToString()] = new List<int>();
                }
                inverseAdjacencyList[transactions[0].ToString()][transactions[0].ToString()].Add(vertex);

            }
            else
            {
                for (int i = transactions.Count - 1; i > 0; i--)
                {
                    if (!inverseAdjacencyList.ContainsKey(transactions[i].ToString()))
                    {
                        inverseAdjacencyList[transactions[i].ToString()] = new Dictionary<string, List<int>>();
                    }
                    if (!inverseAdjacencyList[transactions[i].ToString()].ContainsKey(transactions[i - 1].ToString()))
                    {
                        inverseAdjacencyList[transactions[i].ToString()][transactions[i - 1].ToString()] = new List<int>();
                    }
                    inverseAdjacencyList[transactions[i].ToString()][transactions[i - 1].ToString()].Add(vertex);
                }
            }
        }
    }
}
