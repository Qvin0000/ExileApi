using System;
using System.Collections.Generic;
using System.Linq;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Interfaces;

namespace ExileCore.PoEMemory.FilesInMemory
{
    public class QuestStates : UniversalFileWrapper<QuestState>
    {
        private Dictionary<string, Dictionary<int, QuestState>> QuestStatesDictionary;

        public QuestStates(IMemory m, Func<long> address) : base(m, address)
        {
        }

        public IList<QuestState> EntriesList => base.EntriesList.ToList();

        public QuestState GetQuestState(string questId, int stateId)
        {
            Dictionary<int, QuestState> dictionary;

            if (QuestStatesDictionary == null)
            {
                CheckCache();
                var qStates = EntriesList;
                QuestStatesDictionary = new Dictionary<string, Dictionary<int, QuestState>>();

                try
                {
                    foreach (var item in qStates)
                    {
                        if (!QuestStatesDictionary.TryGetValue(item.Quest.Id, out dictionary))
                        {
                            dictionary = new Dictionary<int, QuestState>();
                            QuestStatesDictionary.Add(item.Quest.Id.ToLowerInvariant(), dictionary);
                        }

                        dictionary.Add(item.QuestStateId, item);
                    }
                }
                catch (Exception)
                {
                    QuestStatesDictionary = null;
                    throw;
                }
            }

            if (QuestStatesDictionary.TryGetValue(questId.ToLowerInvariant(), out dictionary) &&
                dictionary.TryGetValue(stateId, out var result)) return result;

            return null;
        }

        public QuestState GetByAddress(long address)
        {
            return base.GetByAddress(address);
        }
    }
}
